using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Aspose.Words;
using System.Web.UI.WebControls.WebParts;
using NiceHandles.Models;
using Microsoft.Owin.BuilderProperties;
using System.Xml.Linq;
using System.Web.WebPages;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Manager")]
    public class QuanTriVanHanhsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: QuanTriVanHanhs
        public ActionResult Index()
        {
            return View(db.QuanTriVanHanhs.OrderBy(x => x.no).ToList());
        }

        // GET: QuanTriVanHanhs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuanTriVanHanh quanTriVanHanh = db.QuanTriVanHanhs.Find(id);
            if (quanTriVanHanh == null)
            {
                return HttpNotFound();
            }
            return View(quanTriVanHanh);
        }

        // GET: QuanTriVanHanhs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QuanTriVanHanhs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.        
        [Authorize(Roles = "SuperAdmin,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(QuanTriVanHanh quanTriVanHanh)
        {
            if (ModelState.IsValid)
            {
                db.QuanTriVanHanhs.Add(quanTriVanHanh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(quanTriVanHanh);
        }

        // GET: QuanTriVanHanhs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuanTriVanHanh quanTriVanHanh = db.QuanTriVanHanhs.Find(id);
            if (quanTriVanHanh == null)
            {
                return HttpNotFound();
            }
            return View(quanTriVanHanh);
        }

        // POST: QuanTriVanHanhs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "SuperAdmin,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(QuanTriVanHanh quanTriVanHanh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quanTriVanHanh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(quanTriVanHanh);
        }

        // GET: QuanTriVanHanhs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuanTriVanHanh quanTriVanHanh = db.QuanTriVanHanhs.Find(id);
            if (quanTriVanHanh == null)
            {
                return HttpNotFound();
            }
            return View(quanTriVanHanh);
        }

        // POST: QuanTriVanHanhs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QuanTriVanHanh quanTriVanHanh = db.QuanTriVanHanhs.Find(id);
            db.QuanTriVanHanhs.Remove(quanTriVanHanh);
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
        public JsonResult Print(int? id)
        {
            QuanTriVanHanh quanTriVanHanh = db.QuanTriVanHanhs.Find(id);
            string path = Server.MapPath("~/public/QuanTriVanHanh" + quanTriVanHanh.id + ".docx");
            string pathTemp = Server.MapPath("~/App_Data/templates/QuanTriVanHanh.docx");
            Aspose.Words.Document doc = new Aspose.Words.Document(pathTemp);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            builder.MoveToMergeField("plans");
            builder.InsertHtml(quanTriVanHanh.plans);
            builder.MoveToMergeField("workflows");
            builder.InsertHtml(quanTriVanHanh.workflows);
            builder.MoveToMergeField("reports");
            builder.InsertHtml(quanTriVanHanh.reports);
            doc.Save(path);
            return Json("/public/QuanTriVanHanh" + quanTriVanHanh.id + ".docx", JsonRequestBehavior.AllowGet);
        }
    }
}
