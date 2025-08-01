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
    public class fk_service_documentController : Controller
    {
        private NHModel db = new NHModel();

        // GET: fk_service_document
        public ActionResult Index()
        {
            return View(db.fk_service_document.ToList());
        }

        // GET: fk_service_document/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            fk_service_document fk_service_document = db.fk_service_document.Find(id);
            if (fk_service_document == null)
            {
                return HttpNotFound();
            }
            return View(fk_service_document);
        }

        // GET: fk_service_document/Create
        public ActionResult Create(int id)
        {
            ViewBag.id = id;
            ViewBag.Documents = db.Documents.Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.name });
            return View();
        }

        // POST: fk_service_document/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,service_id,document_id")] fk_service_document fk_service_document)
        {
            if (ModelState.IsValid)
            {
                db.fk_service_document.Add(fk_service_document);
                db.SaveChanges();
                return RedirectToAction("Details", "Services", new { id = fk_service_document.service_id });
            }

            return View(fk_service_document);
        }

        // GET: fk_service_document/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            fk_service_document fk_service_document = db.fk_service_document.Find(id);
            if (fk_service_document == null)
            {
                return HttpNotFound();
            }
            return View(fk_service_document);
        }

        // POST: fk_service_document/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,service_id,document_id")] fk_service_document fk_service_document)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fk_service_document).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fk_service_document);
        }

        // GET: fk_service_document/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            fk_service_document fk_service_document = db.fk_service_document.Find(id);
            if (fk_service_document == null)
            {
                return HttpNotFound();
            }
            return View(fk_service_document);
        }

        // POST: fk_service_document/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            fk_service_document fk_service_document = db.fk_service_document.Find(id);
            db.fk_service_document.Remove(fk_service_document);
            db.SaveChanges();
            return RedirectToAction("Details", "Services", new { id = fk_service_document.service_id });
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
