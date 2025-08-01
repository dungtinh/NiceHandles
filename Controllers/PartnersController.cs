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

namespace NiceHandles.Controllers
{
    public class PartnersController : Controller
    {
        private NHModel db = new NHModel();

        // GET: Partners
        public ActionResult Index()
        {
            return View(db.Partners.Where(x => !x.code.Equals("FREE")).OrderBy(x => x.sothutu).ToList());
        }
        public ActionResult Indexx()
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var rs = db.Partners.Where(x => !x.code.Equals("FREE") && x.account_id == us.id).OrderBy(x => x.sothutu).ToList();
            return View(rs);
        }
        [HttpGet]
        public JsonResult GetPartnerService(string term, int? page, string _type)
        {
            int p = page ?? 0;
            int ps = 20;
            var lstCT = (from par in db.Partners
                         join acc in db.Accounts on par.account_id equals acc.id
                         where par.name.ToUpper().Contains(String.IsNullOrEmpty(term) ? par.name.ToUpper() : term) ||
                         acc.fullname.ToUpper().Contains(String.IsNullOrEmpty(term) ? acc.fullname.ToUpper() : term)
                         select new { id = par.id, name = par.name, acc = acc.fullname }).OrderBy(x => x.name).Skip(p * ps).Take(ps).ToList();
            data d = new data();
            List<item> lst = new List<item>();
            foreach (var item in lstCT)
            {
                item it = new item();
                it.name = item.name;
                it.text = item.acc;
                it.id = item.id;
                lst.Add(it);
            }
            d.total_count = lst.Count;
            d.items = lst;
            return Json(d, JsonRequestBehavior.AllowGet);
        }

        // GET: Partners/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partner partner = db.Partners.Find(id);
            if (partner == null)
            {
                return HttpNotFound();
            }
            return View(partner);
        }

        // GET: Partners/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Partners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Partner partner)
        {
            if (ModelState.IsValid)
            {
                db.Partners.Add(partner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(partner);
        }

        // GET: Partners/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partner partner = db.Partners.Find(id);
            if (partner == null)
            {
                return HttpNotFound();
            }
            return View(partner);
        }

        // POST: Partners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Partner partner)
        {
            if (ModelState.IsValid)
            {
                db.Entry(partner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(partner);
        }

        // GET: Partners/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Partner partner = db.Partners.Find(id);
            if (partner == null)
            {
                return HttpNotFound();
            }
            return View(partner);
        }

        // POST: Partners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Partner partner = db.Partners.Find(id);
            db.Partners.Remove(partner);
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
