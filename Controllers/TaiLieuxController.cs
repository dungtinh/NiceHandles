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
    public class TaiLieuxController : Controller
    {
        private NHModel db = new NHModel();

        // GET: TaiLieux
        public ActionResult Index()
        {
            var result = db.TaiLieux.Where(x => x.type == (int)XTaiLieu.eType.TaiLieu).OrderByDescending(x => x.id).ToList();
            return View(result);
        }
        public ActionResult DaoTao()
        {
            var result = db.TaiLieux.Where(x => x.type == (int)XTaiLieu.eType.DaoTao).ToList();
            return View(result);
        }
        public ActionResult QuyChe()
        {
            var result = db.TaiLieux.Where(x => x.type == (int)XTaiLieu.eType.QuyChe).ToList();
            return View(result);
        }
        public ActionResult Template()
        {
            var result = db.TaiLieux.Where(x => x.type == (int)XTaiLieu.eType.MauTrang).ToList();
            return View(result);
        }
        // GET: TaiLieux/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiLieu taiLieu = db.TaiLieux.Find(id);
            if (taiLieu == null)
            {
                return HttpNotFound();
            }
            return View(taiLieu);
        }

        // GET: TaiLieux/Create
        public ActionResult Create(int? type)
        {
            if (type == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiLieu taiLieu = new TaiLieu();
            taiLieu.type = type.Value;
            return View(taiLieu);
        }

        // POST: TaiLieux/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaiLieu taiLieu, HttpPostedFileBase[] flLink)
        {
            if (ModelState.IsValid)
            {
                if (flLink.Length > 0)
                {
                    var file = flLink[0];
                    if (file != null)
                    {
                        string filename = DateTime.Now.ToString("sshhmmddMMyy") + file.FileName;
                        file.SaveAs(Server.MapPath("~/public/tailieu/") + filename);
                        taiLieu.link = "/public/tailieu/" + filename;
                    }
                    db.SaveChanges();
                }
                db.TaiLieux.Add(taiLieu);
                db.SaveChanges();
                if (taiLieu.type == (int)XTaiLieu.eType.QuyChe)
                    return RedirectToAction("QuyChe");
                else if (taiLieu.type == (int)XTaiLieu.eType.DaoTao)
                    return RedirectToAction("DaoTao");
                else if (taiLieu.type == (int)XTaiLieu.eType.MauTrang)
                    return RedirectToAction("Template");
                else
                    return RedirectToAction("Index");
            }

            return View(taiLieu);
        }

        // GET: TaiLieux/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiLieu taiLieu = db.TaiLieux.Find(id);
            if (taiLieu == null)
            {
                return HttpNotFound();
            }
            return View(taiLieu);
        }

        // POST: TaiLieux/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TaiLieu taiLieu, HttpPostedFileBase[] flLink)
        {
            if (ModelState.IsValid)
            {
                if (flLink.Length > 0)
                {
                    var file = flLink[0];
                    if (file != null)
                    {
                        string filename = DateTime.Now.ToString("sshhmmddMMyy") + file.FileName;
                        file.SaveAs(Server.MapPath("~/public/tailieu/") + filename);
                        taiLieu.link = "/public/tailieu/" + filename;
                    }
                }
                db.Entry(taiLieu).State = EntityState.Modified;
                db.SaveChanges();
                if (taiLieu.type == (int)XTaiLieu.eType.QuyChe)
                    return RedirectToAction("QuyChe");
                else if (taiLieu.type == (int)XTaiLieu.eType.DaoTao)
                    return RedirectToAction("DaoTao");
                else if (taiLieu.type == (int)XTaiLieu.eType.MauTrang)
                    return RedirectToAction("Template");
                else
                    return RedirectToAction("Index");
            }
            return View(taiLieu);
        }

        // GET: TaiLieux/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TaiLieu taiLieu = db.TaiLieux.Find(id);
            if (taiLieu == null)
            {
                return HttpNotFound();
            }
            return View(taiLieu);
        }

        // POST: TaiLieux/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TaiLieu taiLieu = db.TaiLieux.Find(id);
            db.TaiLieux.Remove(taiLieu);
            db.SaveChanges();
            if (taiLieu.type == (int)XTaiLieu.eType.QuyChe)
                return RedirectToAction("QuyChe");
            else if (taiLieu.type == (int)XTaiLieu.eType.DaoTao)
                return RedirectToAction("DaoTao");
            else if (taiLieu.type == (int)XTaiLieu.eType.MauTrang)
                return RedirectToAction("Template");
            else
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
