using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Manager,Member")]
    public class CongViecsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: CongViecs
        public ActionResult Index()
        {
            return View(db.CongViecs.ToList());
        }

        // GET: CongViecs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CongViec congViec = db.CongViecs.Find(id);
            if (congViec == null)
            {
                return HttpNotFound();
            }
            return View(congViec);
        }

        // GET: CongViecs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CongViecs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CongViec congViec, string str_service)
        {
            if (ModelState.IsValid)
            {
                db.CongViecs.Add(congViec);
                db.SaveChanges();
                var lst = str_service.Split(',');
                foreach (var item in lst)
                {
                    var sv = new fk_congviec_service();
                    sv.congviec_id = congViec.id;
                    sv.service_id = Convert.ToInt32(item);
                    db.fk_congviec_service.Add(sv);
                }
                db.SaveChanges();
                //return RedirectToAction("Index");
            }
            ViewBag.Done = true;
            return View(congViec);
        }

        // GET: CongViecs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CongViec congViec = db.CongViecs.Find(id);
            if (congViec == null)
            {
                return HttpNotFound();
            }
            var fk = db.fk_congviec_service.Where(x => x.congviec_id == congViec.id).Select(x => x.service_id);
            ViewBag.str_service = string.Join(",", fk);
            return View(congViec);
        }

        // POST: CongViecs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CongViec congViec, string str_service)
        {
            if (ModelState.IsValid)
            {
                db.Entry(congViec).State = EntityState.Modified;
                var lst = str_service.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var fk = db.fk_congviec_service.Where(x => x.congviec_id == congViec.id);
                db.fk_congviec_service.RemoveRange(fk);
                foreach (var item in lst)
                {
                    var sv = new fk_congviec_service();
                    sv.congviec_id = congViec.id;
                    sv.service_id = Convert.ToInt32(item);
                    db.fk_congviec_service.Add(sv);
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(congViec);
        }

        // GET: CongViecs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CongViec congViec = db.CongViecs.Find(id);
            if (congViec == null)
            {
                return HttpNotFound();
            }
            return View(congViec);
        }

        // POST: CongViecs/Delete/5
        [Authorize(Roles = "SuperAdmin,Manager")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CongViec congViec = db.CongViecs.Find(id);
            db.CongViecs.Remove(congViec);
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
        [HttpPost]
        public JsonResult Update(int hoso_id, int congviec_id, bool check)
        {
            var fk = db.fk_congviec_hoso.SingleOrDefault(x => x.hoso_id == hoso_id && x.congviec_id == congviec_id);
            if (fk == null)
            {
                fk = new fk_congviec_hoso();
                fk.congviec_id = congviec_id;
                fk.hoso_id = hoso_id;
                db.fk_congviec_hoso.Add(fk);
            }
            fk.status = check ? (int)XModels.eStatus.Complete : (int)XModels.eStatus.Processing;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
