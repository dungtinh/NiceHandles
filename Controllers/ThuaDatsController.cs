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
    public class ThuaDatsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: ThuaDats
        public ActionResult Index()
        {
            return View(db.ThuaDats.ToList());
        }

        // GET: ThuaDats/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThuaDat thuaDat = db.ThuaDats.Find(id);
            if (thuaDat == null)
            {
                return HttpNotFound();
            }
            return View(thuaDat);
        }
        public JsonResult DetailsX(int? id)
        {
            ThuaDat thuaDat = db.ThuaDats.Find(id);
            XThuaDat xtd = new XThuaDat();
            xtd.obj = thuaDat;
            xtd.ngaycap = thuaDat.ngaycap.HasValue ? thuaDat.ngaycap.Value.ToString("dd/MM/yyyy") : null;
            return Json(xtd, JsonRequestBehavior.AllowGet);
        }

        // GET: ThuaDats/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ThuaDats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,infomation_id,type,sogiaychungnhan,sothua,tobando,diachithuadat,dientich,hinhthucsudung,mucdichsudung,nguongoc,ghichu,ngaycap,noicap,sovaoso,loaidat1,dientich1,loaidat2,dientich2")] ThuaDat thuaDat)
        {
            if (ModelState.IsValid)
            {
                db.ThuaDats.Add(thuaDat);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(thuaDat);
        }
        [HttpPost]
        public ActionResult CreateX(ThuaDat thuaDat, int? id, string ngaycap)
        {
            string result = string.Empty;
            if (!id.HasValue)
            {
                db.ThuaDats.Add(thuaDat);
            }
            else
            {
                thuaDat.id = id.Value;
                db.Entry(thuaDat).State = EntityState.Modified;
            }
            if (!string.IsNullOrEmpty(ngaycap)) thuaDat.ngaycap = Convert.ToDateTime(ngaycap);
            db.SaveChanges();
            result = "<tr id='" + thuaDat.id + "'><td>" + thuaDat.sogiaychungnhan +
                "</td><td>" + thuaDat.sovaoso +
                "</td><td>" + thuaDat.sothua +
                "</td><td>" + thuaDat.tobando +
                "</td><td>" + thuaDat.dientich +
                "</td><td>" + (thuaDat.ngaycap.HasValue ? thuaDat.ngaycap.Value.ToString("dd/MM/yyyy") : null) +
                "</td><td>" + thuaDat.noicap +
                "</td><td>" + "<a href='#' data-toggle='modal' data-target='#" + "HopThuaModal" + "' onclick='EditThuaDat(" + thuaDat.id + " )'>Sửa</a> |" +
                "<a href='#' onclick='XoaThuaDat(" + thuaDat.id + ")'>Xóa</a>" +
                "</td></tr>";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: ThuaDats/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThuaDat thuaDat = db.ThuaDats.Find(id);
            if (thuaDat == null)
            {
                return HttpNotFound();
            }
            return View(thuaDat);
        }

        // POST: ThuaDats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,infomation_id,type,sogiaychungnhan,sothua,tobando,diachithuadat,dientich,hinhthucsudung,mucdichsudung,nguongoc,ghichu,ngaycap,noicap,sovaoso,loaidat1,dientich1,loaidat2,dientich2")] ThuaDat thuaDat)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thuaDat).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thuaDat);
        }

        // GET: ThuaDats/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThuaDat thuaDat = db.ThuaDats.Find(id);
            if (thuaDat == null)
            {
                return HttpNotFound();
            }
            return View(thuaDat);
        }

        // POST: ThuaDats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ThuaDat thuaDat = db.ThuaDats.Find(id);
            db.ThuaDats.Remove(thuaDat);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost, ActionName("DeleteX")]
        public ActionResult DeleteX(int id)
        {
            ThuaDat thuaDat = db.ThuaDats.Find(id);
            db.ThuaDats.Remove(thuaDat);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
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
