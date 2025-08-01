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
    [Authorize(Roles = "SuperAdmin,Manager,Member")]
    public class PriceByAreasController : Controller
    {
        private NHModel db = new NHModel();

        // GET: PriceByAreas
        public ActionResult Index()
        {
            return View(db.PriceByAreas.ToList());
        }

        // GET: PriceByAreas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PriceByArea priceByArea = db.PriceByAreas.Find(id);
            if (priceByArea == null)
            {
                return HttpNotFound();
            }
            return View(priceByArea);
        }

        // GET: PriceByAreas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PriceByAreas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,type,area,text,price,note")] PriceByArea priceByArea)
        {
            if (ModelState.IsValid)
            {
                db.PriceByAreas.Add(priceByArea);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(priceByArea);
        }

        // GET: PriceByAreas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PriceByArea priceByArea = db.PriceByAreas.Find(id);
            if (priceByArea == null)
            {
                return HttpNotFound();
            }
            return View(priceByArea);
        }

        // POST: PriceByAreas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,type,area,text,price,note")] PriceByArea priceByArea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(priceByArea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(priceByArea);
        }

        // GET: PriceByAreas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PriceByArea priceByArea = db.PriceByAreas.Find(id);
            if (priceByArea == null)
            {
                return HttpNotFound();
            }
            return View(priceByArea);
        }

        // POST: PriceByAreas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PriceByArea priceByArea = db.PriceByAreas.Find(id);
            db.PriceByAreas.Remove(priceByArea);
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
