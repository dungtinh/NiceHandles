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
    public class GiaoNhanGiayToesController : Controller
    {
        private NHModel db = new NHModel();

        // GET: GiaoNhanGiayToes        
        public ActionResult Index(search filter)
        {
            if (filter.Search_Data != null)
            {
                filter.Page_No = 1;
            }
            else
            {
                filter.Search_Data = filter.Filter_Value;
            }
            ViewBag.FilterValue = filter.Search_Data;
            ViewBag.contract_id = filter.contract;
            ViewBag.account_id = filter.account;

            IQueryable<GiaoNhanGiayTo> results = null;
            if (!String.IsNullOrEmpty(filter.Search_Data) || filter.contract.HasValue || filter.account.HasValue)
            {
                results = (from r in db.GiaoNhanGiayToes
                           where (
                           (r.nguoinhan.ToUpper().Contains(!String.IsNullOrEmpty(filter.Search_Data) ? filter.Search_Data.ToUpper() : r.nguoinhan.ToUpper()) ||
                           r.giayto.ToUpper().Contains(!String.IsNullOrEmpty(filter.Search_Data) ? filter.Search_Data.ToUpper() : r.giayto.ToUpper())) &&
                           r.contract_id == (filter.contract.HasValue ? filter.contract.Value : r.contract_id) &&
                           r.account_id == (filter.account.HasValue ? filter.account.Value : r.account_id)
                           )
                           orderby r.ngayhan
                           select r);
            }
            else
            {
                results = (from r in db.GiaoNhanGiayToes
                           orderby r.ngayhan
                           select r);
            }
            ViewBag.db = db;
            ViewBag.Accounts = new SelectList(db.Accounts, "id", "fullname", null);

            var lstCT = from ct in db.Contracts
                        join add in db.Addresses on ct.address_id equals add.id
                        where ct.status == (int)XContract.eStatus.Processing
                        select new { id = ct.id.ToString(), name = ct.name + " - " + add.name };
            ViewBag.lstCT = new SelectList(lstCT, "id", "name");

            int Size_Of_Page = 30;
            int No_Of_Page = (filter.Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        // GET: GiaoNhanGiayToes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiaoNhanGiayTo giaoNhanGiayTo = db.GiaoNhanGiayToes.Find(id);
            if (giaoNhanGiayTo == null)
            {
                return HttpNotFound();
            }
            return View(giaoNhanGiayTo);
        }

        // GET: GiaoNhanGiayToes/Create
        public ActionResult Create()
        {
            var lstCT = from ct in db.Contracts
                        join add in db.Addresses on ct.address_id equals add.id
                        where ct.status == (int)XContract.eStatus.Processing
                        select new { id = ct.id.ToString(), name = ct.name + " - " + add.name };
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            ViewBag.lstCT = new SelectList(lstCT, "id", "name");
            ViewBag.Accounts = new SelectList(db.Accounts.ToArray(), "id", "fullname", us);
            return View();
        }

        // POST: GiaoNhanGiayToes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GiaoNhanGiayTo giaoNhanGiayTo)
        {
            if (ModelState.IsValid)
            {
                db.GiaoNhanGiayToes.Add(giaoNhanGiayTo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var lstCT = from ct in db.Contracts
                        join add in db.Addresses on ct.address_id equals add.id
                        where ct.status == (int)XContract.eStatus.Processing
                        select new { id = ct.id.ToString(), name = ct.name + " - " + add.name };
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            ViewBag.lstCT = new SelectList(lstCT, "id", "name");
            ViewBag.Accounts = new SelectList(db.Accounts.ToArray(), "id", "fullname", us);
            return View(giaoNhanGiayTo);
        }

        // GET: GiaoNhanGiayToes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiaoNhanGiayTo giaoNhanGiayTo = db.GiaoNhanGiayToes.Find(id);
            if (giaoNhanGiayTo == null)
            {
                return HttpNotFound();
            }
            var lstCT = from ct in db.Contracts
                        join add in db.Addresses on ct.address_id equals add.id
                        where ct.status == (int)XContract.eStatus.Processing
                        select new { id = ct.id.ToString(), name = ct.name + " - " + add.name };
            ViewBag.lstCT = new SelectList(lstCT, "id", "name");
            ViewBag.Accounts = new SelectList(db.Accounts.ToArray(), "id", "fullname");
            return View(giaoNhanGiayTo);
        }

        // POST: GiaoNhanGiayToes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GiaoNhanGiayTo giaoNhanGiayTo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(giaoNhanGiayTo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var lstCT = from ct in db.Contracts                        
                        join add in db.Addresses on ct.address_id equals add.id
                        where ct.status == (int)XContract.eStatus.Processing
                        select new { id = ct.id.ToString(), name = ct.name + " - " + add.name };
            ViewBag.lstCT = new SelectList(lstCT, "id", "name");
            ViewBag.Accounts = new SelectList(db.Accounts.ToArray(), "id", "fullname");
            return View(giaoNhanGiayTo);
        }

        // GET: GiaoNhanGiayToes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GiaoNhanGiayTo giaoNhanGiayTo = db.GiaoNhanGiayToes.Find(id);
            if (giaoNhanGiayTo == null)
            {
                return HttpNotFound();
            }
            return View(giaoNhanGiayTo);
        }

        // POST: GiaoNhanGiayToes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GiaoNhanGiayTo giaoNhanGiayTo = db.GiaoNhanGiayToes.Find(id);
            db.GiaoNhanGiayToes.Remove(giaoNhanGiayTo);
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
