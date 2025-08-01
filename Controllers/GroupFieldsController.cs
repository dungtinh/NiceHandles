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
    public class GroupFieldsController : Controller
    {
        private NHModel db = new NHModel();
        // GET: GroupFields
        public ActionResult Index(int id)
        {
            ViewBag.id = id;
            var lst = db.GroupFields.Where(x => x.document_id == id).ToList();
            return View(lst);
        }

        // GET: GroupFields/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupField groupField = db.GroupFields.Find(id);
            if (groupField == null)
            {
                return HttpNotFound();
            }
            return View(groupField);
        }

        // GET: GroupFields/Create
        public ActionResult Create(int id)
        {
            ViewBag.id = id;
            return View();
        }

        // POST: GroupFields/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,no,document_id")] GroupField groupField)
        {
            if (ModelState.IsValid)
            {
                db.GroupFields.Add(groupField);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = groupField.document_id });
            }

            return View(groupField);
        }

        // GET: GroupFields/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupField groupField = db.GroupFields.Find(id);
            if (groupField == null)
            {
                return HttpNotFound();
            }
            return View(groupField);
        }

        // POST: GroupFields/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,no,document_id")] GroupField groupField)
        {
            if (ModelState.IsValid)
            {
                db.Entry(groupField).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = groupField.document_id });
            }
            return View(groupField);
        }

        // GET: GroupFields/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupField groupField = db.GroupFields.Find(id);
            if (groupField == null)
            {
                return HttpNotFound();
            }
            return View(groupField);
        }

        // POST: GroupFields/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GroupField groupField = db.GroupFields.Find(id);
            db.GroupFields.Remove(groupField);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = groupField.document_id });
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
