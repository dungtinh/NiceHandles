using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class DocumentsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: Documents
        public ActionResult Index()
        {
            return View(db.Documents.ToList());
        }

        // GET: Documents/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // GET: Documents/Create        
        public ActionResult Create()
        {
            ViewBag.lst_services = db.Services;
            return View();
        }

        // POST: Documents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Document document, string str_service)
        {
            if (ModelState.IsValid)
            {
                db.Documents.Add(document);
                db.SaveChanges();
                var lst = str_service.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in lst)
                {
                    var sv = new fk_service_document();
                    sv.document_id = document.id;
                    sv.service_id = Convert.ToInt32(item);
                    db.fk_service_document.Add(sv);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.lst_services = db.Services;
            return View(document);
        }

        // GET: Documents/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            ViewBag.lst_services = db.Services;
            var service_ids = db.fk_service_document.Where(x => x.document_id == id.Value).Select(x => x.service_id).ToArray();
            ViewBag.str_service = string.Join(",", service_ids);
            return View(document);
        }

        // POST: Documents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Document document, string str_service)
        {
            if (ModelState.IsValid)
            {

                var lst = str_service.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                var fk = db.fk_service_document.Where(x => x.document_id == document.id);
                db.fk_service_document.RemoveRange(fk);

                foreach (var item in lst)
                {
                    var sv = new fk_service_document();
                    sv.document_id = document.id;
                    sv.service_id = Convert.ToInt32(item);
                    db.fk_service_document.Add(sv);
                }

                db.Entry(document).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.lst_services = db.Services;
            ViewBag.str_service = str_service;
            return View(document);
        }

        // GET: Documents/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Document document = db.Documents.Find(id);
            if (document == null)
            {
                return HttpNotFound();
            }
            return View(document);
        }

        // POST: Documents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Document document = db.Documents.Find(id);
            db.Documents.Remove(document);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
