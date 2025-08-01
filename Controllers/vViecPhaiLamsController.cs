using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;
using PagedList;

namespace NiceHandles.Controllers
{
    public class vViecPhaiLamsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: vViecPhaiLams
        public ActionResult Index(string Search_Data, int? account_id, int? hoso_id, string Filter_Value, int? Page_No)
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
            ViewBag.ACC = account_id;
            ViewBag.HS = hoso_id;

            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            var results = from io in db.vViecPhaiLams
                          where
                               io.status == (int)XHoSo.eStatus.Processing &&
                               (io.account_id == (account_id.HasValue ? account_id : io.account_id)) &&
                               (io.hoso_id == (hoso_id.HasValue ? hoso_id : io.hoso_id)) &&
                               io.name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.name.ToUpper())
                          orderby io.time_progress
                          select io;            

            int Size_Of_Page = 30;
            int No_Of_Page = (Page_No ?? 1);            
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
            
        }

        // GET: vViecPhaiLams/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vViecPhaiLam vViecPhaiLam = db.vViecPhaiLams.Find(id);
            if (vViecPhaiLam == null)
            {
                return HttpNotFound();
            }
            return View(vViecPhaiLam);
        }

        // GET: vViecPhaiLams/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: vViecPhaiLams/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "title,id,name,time,time_progress,hoso_id,hoso_name,service_id,service_name,progress_type,account_id,fullname,result")] vViecPhaiLam vViecPhaiLam)
        {
            if (ModelState.IsValid)
            {
                db.vViecPhaiLams.Add(vViecPhaiLam);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(vViecPhaiLam);
        }

        // GET: vViecPhaiLams/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vViecPhaiLam vViecPhaiLam = db.vViecPhaiLams.Find(id);
            if (vViecPhaiLam == null)
            {
                return HttpNotFound();
            }
            return View(vViecPhaiLam);
        }

        // POST: vViecPhaiLams/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "title,id,name,time,time_progress,hoso_id,hoso_name,service_id,service_name,progress_type,account_id,fullname,result")] vViecPhaiLam vViecPhaiLam)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vViecPhaiLam).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vViecPhaiLam);
        }

        // GET: vViecPhaiLams/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            vViecPhaiLam vViecPhaiLam = db.vViecPhaiLams.Find(id);
            if (vViecPhaiLam == null)
            {
                return HttpNotFound();
            }
            return View(vViecPhaiLam);
        }

        // POST: vViecPhaiLams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            vViecPhaiLam vViecPhaiLam = db.vViecPhaiLams.Find(id);
            db.vViecPhaiLams.Remove(vViecPhaiLam);
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
