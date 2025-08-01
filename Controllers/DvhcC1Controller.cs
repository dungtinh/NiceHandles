using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NiceHandles.Models;
using PagedList;

namespace NiceHandles.Controllers
{
    public class DvhcC1Controller : Controller
    {
        private NHModel db = new NHModel();

        // GET: DvhcC1       
        public ActionResult Index(string Search_Data, int? address_id, string Filter_Value, int? Page_No)
        {
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }
            ViewBag.FilterValue = Search_Data;
            ViewBag.address_id = address_id;

            vDvhcC1[] results = (from vdv in db.vDvhcC1
                                 where (
                                 (vdv.name.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : vdv.name.ToUpper())) &&
                                 vdv.address_id == (address_id.HasValue ? address_id.Value : vdv.address_id)
                                 )
                                 orderby vdv.name
                                 select vdv).ToArray();

            int Size_Of_Page = 20;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        // GET: DvhcC1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DvhcC1 dvhcC1 = db.DvhcC1.Find(id);
            if (dvhcC1 == null)
            {
                return HttpNotFound();
            }
            return View(dvhcC1);
        }

        // GET: DvhcC1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DvhcC1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,address_id,name,code")] DvhcC1 dvhcC1)
        {
            if (ModelState.IsValid)
            {
                db.DvhcC1.Add(dvhcC1);
                db.SaveChanges();
                //return RedirectToAction("Index");
            }

            return View(dvhcC1);
        }

        // GET: DvhcC1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DvhcC1 dvhcC1 = db.DvhcC1.Find(id);
            if (dvhcC1 == null)
            {
                return HttpNotFound();
            }
            return View(dvhcC1);
        }

        // POST: DvhcC1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,address_id,name,code")] DvhcC1 dvhcC1)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dvhcC1).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dvhcC1);
        }

        // GET: DvhcC1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DvhcC1 dvhcC1 = db.DvhcC1.Find(id);
            if (dvhcC1 == null)
            {
                return HttpNotFound();
            }
            return View(dvhcC1);
        }

        // POST: DvhcC1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DvhcC1 dvhcC1 = db.DvhcC1.Find(id);
            db.DvhcC1.Remove(dvhcC1);
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
