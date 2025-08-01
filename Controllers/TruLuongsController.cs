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
    public class TruLuongsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: TruLuongs
        public ActionResult Index()
        {
            return View(db.TruLuongs.ToList());
        }

        // GET: TruLuongs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TruLuong truLuong = db.TruLuongs.Find(id);
            if (truLuong == null)
            {
                return HttpNotFound();
            }
            return View(truLuong);
        }

        // GET: TruLuongs/Create
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: TruLuongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult Create([Bind(Include = "id,account_id,thoigian,sotien,note")] TruLuong truLuong)
        {
            if (ModelState.IsValid)
            {
                db.TruLuongs.Add(truLuong);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(truLuong);
        }

        // GET: TruLuongs/Edit/5
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TruLuong truLuong = db.TruLuongs.Find(id);
            if (truLuong == null)
            {
                return HttpNotFound();
            }
            return View(truLuong);
        }

        // POST: TruLuongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult Edit([Bind(Include = "id,account_id,thoigian,sotien,note")] TruLuong truLuong)
        {
            if (ModelState.IsValid)
            {
                db.Entry(truLuong).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(truLuong);
        }

        // GET: TruLuongs/Delete/5
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TruLuong truLuong = db.TruLuongs.Find(id);
            if (truLuong == null)
            {
                return HttpNotFound();
            }
            return View(truLuong);
        }

        // POST: TruLuongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult DeleteConfirmed(int id)
        {
            TruLuong truLuong = db.TruLuongs.Find(id);
            db.TruLuongs.Remove(truLuong);
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
