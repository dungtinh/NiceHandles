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
    public class syslogsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: syslogs
        [Authorize(Roles = "SuperAdmin,Manager,Accounting")]
        public ActionResult Index(string Search_Data, int? account_id, string Filter_Value, int? Page_No, string fromdate, string todate)
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
            ViewBag.account_id = account_id;

            ViewBag.FROMDATE = fromdate;
            ViewBag.TODATE = todate;
            DateTime FromDate = string.IsNullOrEmpty(fromdate) ? new DateTime() : DateTime.Parse(fromdate);
            DateTime ToDate = string.IsNullOrEmpty(todate) ? DateTime.Now : DateTime.Parse(todate);
            ToDate = ToDate.AddDays(1);

            var results = from v in db.syslogs
                          where (
                          (string.IsNullOrEmpty(v.message) || v.message.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.message.ToUpper())) &&
                          v.created_time >= FromDate &&
                          v.created_time < ToDate
                          )
                          orderby v.created_time descending
                          select v;

            int Size_Of_Page = 100;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        // GET: syslogs/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            syslog syslog = db.syslogs.Find(id);
            if (syslog == null)
            {
                return HttpNotFound();
            }
            return View(syslog);
        }

        // GET: syslogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: syslogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,account_id,message,created_time,caption")] syslog syslog)
        {
            if (ModelState.IsValid)
            {
                db.syslogs.Add(syslog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(syslog);
        }

        // GET: syslogs/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            syslog syslog = db.syslogs.Find(id);
            if (syslog == null)
            {
                return HttpNotFound();
            }
            return View(syslog);
        }

        // POST: syslogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,account_id,message,created_time,caption")] syslog syslog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(syslog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(syslog);
        }

        // GET: syslogs/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            syslog syslog = db.syslogs.Find(id);
            if (syslog == null)
            {
                return HttpNotFound();
            }
            return View(syslog);
        }

        // POST: syslogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            syslog syslog = db.syslogs.Find(id);
            db.syslogs.Remove(syslog);
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
