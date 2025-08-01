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
    public class SoHoKhausController : Controller
    {
        private NHModel db = new NHModel();

        // GET: SoHoKhaus
        public ActionResult Index()
        {
            return View(db.SoHoKhaus.ToList());
        }

        // GET: SoHoKhaus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SoHoKhau soHoKhau = db.SoHoKhaus.Find(id);
            if (soHoKhau == null)
            {
                return HttpNotFound();
            }
            return View(soHoKhau);
        }
        [HttpPost]
        public JsonResult DetailsX(int? id)
        {
            SoHoKhau shk = db.SoHoKhaus.Find(id);
            return Json(shk, JsonRequestBehavior.AllowGet);
        }
        // GET: SoHoKhaus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SoHoKhaus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,infomation_id,tenchuho,diachi")] SoHoKhau soHoKhau)
        {
            if (ModelState.IsValid)
            {
                db.SoHoKhaus.Add(soHoKhau);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(soHoKhau);
        }
        [HttpPost]
        public ActionResult CreateX(SoHoKhau shk, int? id)
        {
            string result = string.Empty;
            if (!id.HasValue)
            {
                db.SoHoKhaus.Add(shk);
            }
            else
            {
                shk.id = id.Value;
                db.Entry(shk).State = EntityState.Modified;
            }
            db.SaveChanges();

            result =
                "<tr id='" + shk.id + "'>" +
                "   <td>" +
                "       " + shk.so + "" +
                "   </td>" +
                "   <td>" +
                "       <a href='#' data-toggle=\"modal\" data-target=\"#NhanKhauModal\" onclick=\"OpenNhanKhau(" + shk.id + ")\">Nhân khẩu</a> | " +
                "       <a href='#' data-toggle='modal' data-target='#SoHoKhauModal' onclick='EditSoHoKhau(" + shk.id + " )'>Sửa</a> |" +
                "       <a href='#' onclick='XoaSoHoKhau(" + shk.id + ")'>Xóa</a>" +
                "   </td>" +
                "</tr>";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // GET: SoHoKhaus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SoHoKhau soHoKhau = db.SoHoKhaus.Find(id);
            if (soHoKhau == null)
            {
                return HttpNotFound();
            }
            return View(soHoKhau);
        }

        // POST: SoHoKhaus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,infomation_id,tenchuho,diachi")] SoHoKhau soHoKhau)
        {
            if (ModelState.IsValid)
            {
                db.Entry(soHoKhau).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(soHoKhau);
        }

        // GET: SoHoKhaus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SoHoKhau soHoKhau = db.SoHoKhaus.Find(id);
            if (soHoKhau == null)
            {
                return HttpNotFound();
            }
            return View(soHoKhau);
        }

        // POST: SoHoKhaus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SoHoKhau soHoKhau = db.SoHoKhaus.Find(id);
            db.SoHoKhaus.Remove(soHoKhau);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost, ActionName("DeleteX")]
        public ActionResult DeleteX(int id)
        {
            SoHoKhau shk = db.SoHoKhaus.Find(id);
            db.SoHoKhaus.Remove(shk);
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
