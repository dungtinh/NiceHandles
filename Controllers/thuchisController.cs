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
    [Authorize(Roles = "SuperAdmin,Manager,Member")]
    public class thuchisController : Controller
    {
        private NHModel db = new NHModel();

        // GET: thuchis
        public ActionResult Index(string Search_Data, string Filter_Value, int? Page_No)
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

            thuchi[] results = null;
            if (!String.IsNullOrEmpty(Search_Data))
            {
                results = (from io in db.thuchis
                           where (
                           string.IsNullOrEmpty(io.lydo) || io.lydo.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.lydo.ToUpper()) && io.account_id.Equals(us.id))
                           orderby io.thoigian descending
                           select io).ToArray();
            }
            else
            {
                results = (from io in db.thuchis
                           orderby io.thoigian descending
                           where io.account_id.Equals(us.id)
                           select io).ToArray();
            }
            ViewBag.db = db;
            int Size_Of_Page = 20;
            int No_Of_Page = (Page_No ?? 1);
            var temp = db.thuchis.Count(x => x.account_id.Equals(us.id) && x.loai == (int)XCategory.eType.Thu);
            var tongthu = temp > 0 ? db.thuchis.Where(x => x.account_id.Equals(us.id) && x.loai == (int)XCategory.eType.Thu).Sum(x => x.sotien) : 0;
            temp = db.thuchis.Count(x => x.account_id.Equals(us.id) && x.loai == (int)XCategory.eType.Chi);
            var tongchi = temp > 0 ? db.thuchis.Where(x => x.account_id.Equals(us.id) && x.loai == (int)XCategory.eType.Chi).Sum(x => x.sotien) : 0;
            ViewBag.TongThu = tongthu.ToString("N0");
            ViewBag.TongChi = tongchi.ToString("N0");
            ViewBag.ThuChi = (tongthu - tongchi).ToString("N0");
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        // GET: thuchis/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            thuchi thuchi = db.thuchis.Find(id);
            if (thuchi == null)
            {
                return HttpNotFound();
            }
            return View(thuchi);
        }

        // GET: thuchis/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: thuchis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(thuchi thuchi)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            thuchi.account_id = us.id;
            thuchi.loai = (int)XCategory.eType.Chi;
            thuchi.trangthai = (int)XModels.eStatus.Processing;
            if (ModelState.IsValid)
            {
                db.thuchis.Add(thuchi);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thuchi);
        }

        // GET: thuchis/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            thuchi thuchi = db.thuchis.Find(id);
            if (thuchi == null)
            {
                return HttpNotFound();
            }
            return View(thuchi);
        }

        // POST: thuchis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(thuchi thuchi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thuchi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thuchi);
        }

        // GET: thuchis/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            thuchi thuchi = db.thuchis.Find(id);
            if (thuchi == null)
            {
                return HttpNotFound();
            }
            return View(thuchi);
        }

        // POST: thuchis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            thuchi thuchi = db.thuchis.Find(id);
            db.thuchis.Remove(thuchi);
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
