using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;
using PagedList;

namespace NiceHandles.Controllers
{
    public class DiariesController : Controller
    {
        private NHModel db = new NHModel();

        // GET: Diaries
        public ActionResult Index(string Search_Data, string time, string Filter_Value, int? Page_No)
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

            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            Diary[] results = null;
            DateTime? thoigian = null;
            if (!String.IsNullOrEmpty(time))
                thoigian = Convert.ToDateTime(time);
            if (!String.IsNullOrEmpty(Search_Data) || !String.IsNullOrEmpty(time))
            {
                results = (from io in db.Diaries
                           where (
                           (string.IsNullOrEmpty(io.noidung) || io.noidung.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.noidung.ToUpper()))
                           && (io.ngaythang == (thoigian.HasValue ? thoigian.Value : io.ngaythang))
                           && io.account_id.Equals(us.id))
                           orderby io.ngaythang descending
                           select io).ToArray();
            }
            else
            {
                results = (from io in db.Diaries
                           orderby io.ngaythang descending
                           where io.account_id.Equals(us.id)
                           select io).ToArray();
            }
            ViewBag.db = db;
            int Size_Of_Page = 20;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        // GET: Diaries/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Diary diary = db.Diaries.Find(id);
            if (diary == null)
            {
                return HttpNotFound();
            }
            return View(diary);
        }

        // GET: Diaries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Diaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Diary diary)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            diary.account_id = us.id;

            if (ModelState.IsValid)
            {
                db.Diaries.Add(diary);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(diary);
        }

        // GET: Diaries/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Diary diary = db.Diaries.Find(id);
            if (diary == null)
            {
                return HttpNotFound();
            }
            return View(diary);
        }

        // POST: Diaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,ngaythang,noidung,account_id")] Diary diary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(diary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(diary);
        }

        // GET: Diaries/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Diary diary = db.Diaries.Find(id);
            if (diary == null)
            {
                return HttpNotFound();
            }
            return View(diary);
        }

        // POST: Diaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Diary diary = db.Diaries.Find(id);
            db.Diaries.Remove(diary);
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
