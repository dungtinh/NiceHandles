using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    public class KPIChiTieuxController : Controller
    {
        private NHModel db = new NHModel();

        // GET: KPIChiTieux
        public ActionResult Index()
        {
            return View(db.Accounts.Where(x => x.sta == (int)XAccount.eStatus.Processing).ToList());
        }

        // GET: KPIChiTieux/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KPIChiTieu kPIChiTieu = db.KPIChiTieux.Find(id);
            if (kPIChiTieu == null)
            {
                return HttpNotFound();
            }
            return View(kPIChiTieu);
        }

        // GET: KPIChiTieux/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KPIChiTieux/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,user_id,type,chitieu")] KPIChiTieu kPIChiTieu)
        {
            if (ModelState.IsValid)
            {
                db.KPIChiTieux.Add(kPIChiTieu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kPIChiTieu);
        }

        // GET: KPIChiTieux/Edit/5
        public ActionResult Edit(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //KPIChiTieu kPIChiTieu = db.KPIChiTieux.Find(id);
            //if (kPIChiTieu == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(kPIChiTieu);
            return View();
        }

        // POST: KPIChiTieux/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,user_id,type,chitieu")] KPIChiTieu kPIChiTieu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kPIChiTieu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kPIChiTieu);
        }
        [HttpPost]
        public ActionResult ChangeUser(int user_id)
        {
            var kpis = db.KPIChiTieux.Where(x => x.user_id == user_id).OrderBy(x => x.type);
            var result = kpis.Select(x => x.chitieu).ToArray();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        // GET: KPIChiTieux/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KPIChiTieu kPIChiTieu = db.KPIChiTieux.Find(id);
            if (kPIChiTieu == null)
            {
                return HttpNotFound();
            }
            return View(kPIChiTieu);
        }

        // POST: KPIChiTieux/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KPIChiTieu kPIChiTieu = db.KPIChiTieux.Find(id);
            db.KPIChiTieux.Remove(kPIChiTieu);
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
        public ActionResult Submit(int user_id, int[] chitieus)
        {
            var kpis = db.KPIChiTieux.Where(x => x.user_id == user_id).OrderBy(x => x.type).ToArray();
            if (kpis.Length == 0)
            {
                for (var item = 0; item < chitieus.Length; item++)
                {
                    KPIChiTieu ct = new KPIChiTieu();
                    ct.chitieu = chitieus[item];
                    ct.type = item;
                    ct.user_id = user_id;
                    db.KPIChiTieux.Add(ct);
                }
            }
            else
            {
                for (var item = 0; item < chitieus.Length; item++)
                {
                    KPIChiTieu ct = kpis.Where(x => x.type == item).Single();
                    ct.chitieu = chitieus[item];
                }
            }
            db.SaveChanges();
            return Json("/kpichitieux", JsonRequestBehavior.AllowGet);
        }
    }
}
