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
    public class PhuCapsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: PhuCaps
        public ActionResult Index()
        {
            return View(db.PhuCaps.ToList());
        }

        // GET: PhuCaps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhuCap phuCap = db.PhuCaps.Find(id);
            if (phuCap == null)
            {
                return HttpNotFound();
            }
            return View(phuCap);
        }

        // GET: PhuCaps/Create
        [Authorize(Roles = "SuperAdmin,Manager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: PhuCaps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Manager")]
        public ActionResult Create([Bind(Include = "id,account_id,ten,sotien,loai,ngaythang,trangthai")] PhuCap phuCap)
        {
            if (ModelState.IsValid)
            {
                db.PhuCaps.Add(phuCap);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(phuCap);
        }

        // GET: PhuCaps/Edit/5
        [Authorize(Roles = "SuperAdmin,Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhuCap phuCap = db.PhuCaps.Find(id);
            if (phuCap == null)
            {
                return HttpNotFound();
            }
            return View(phuCap);
        }

        // POST: PhuCaps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,Manager")]
        public ActionResult Edit([Bind(Include = "id,account_id,ten,sotien,loai,ngaythang,trangthai")] PhuCap phuCap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phuCap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(phuCap);
        }

        // GET: PhuCaps/Delete/5
        [Authorize(Roles = "SuperAdmin,Manager")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhuCap phuCap = db.PhuCaps.Find(id);
            if (phuCap == null)
            {
                return HttpNotFound();
            }
            return View(phuCap);
        }

        // POST: PhuCaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "SuperAdmin,Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PhuCap phuCap = db.PhuCaps.Find(id);
            db.PhuCaps.Remove(phuCap);
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
