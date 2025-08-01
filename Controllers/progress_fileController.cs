using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NiceHandles.Models;
using PagedList;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Manager")]
    public class progress_fileController : Controller
    {
        private NHModel db = new NHModel();

        // GET: progress_file
        [Authorize(Roles = "SuperAdmin,Manager")]
        public ActionResult Index(string Search_Data, int? hoso_id, int? progress_id, int? type, string Filter_Value, int? Page_No)
        {
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }
            ViewBag.FilterValue = Search_Data;
            ViewBag.hoso_id = hoso_id;
            ViewBag.progress_id = progress_id;
            ViewBag.type = type;

            var results = from v in db.progress_file
                          where (
                          (string.IsNullOrEmpty(v.name) || v.name.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.name.ToUpper())) &&
                          v.hoso_id == (hoso_id.HasValue ? hoso_id.Value : v.hoso_id) &&
                          v.type == (type.HasValue ? type.Value : v.type) &&
                          v.progress_id == (progress_id.HasValue ? progress_id.Value : v.progress_id)
                          )
                          orderby v.id descending
                          select v;

            int Size_Of_Page = 30;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        // GET: progress_file/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            progress_file progress_file = db.progress_file.Find(id);
            if (progress_file == null)
            {
                return HttpNotFound();
            }
            return View(progress_file);
        }

        // GET: progress_file/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: progress_file/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,hoso_id,progress_id,name,url,type,category")] progress_file progress_file)
        {
            if (ModelState.IsValid)
            {
                db.progress_file.Add(progress_file);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(progress_file);
        }

        // GET: progress_file/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            progress_file progress_file = db.progress_file.Find(id);
            if (progress_file == null)
            {
                return HttpNotFound();
            }
            return View(progress_file);
        }

        // POST: progress_file/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,hoso_id,progress_id,name,url,type,category")] progress_file progress_file)
        {
            if (ModelState.IsValid)
            {
                db.Entry(progress_file).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(progress_file);
        }

        // GET: progress_file/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            progress_file progress_file = db.progress_file.Find(id);
            if (progress_file == null)
            {
                return HttpNotFound();
            }
            return View(progress_file);
        }

        // POST: progress_file/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            progress_file progress_file = db.progress_file.Find(id);
            db.progress_file.Remove(progress_file);
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
