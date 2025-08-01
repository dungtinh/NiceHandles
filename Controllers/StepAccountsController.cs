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
    public class StepAccountsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: StepAccounts
        public ActionResult Index()
        {
            return View(db.StepAccounts.ToList());
        }

        // GET: StepAccounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StepAccount stepAccount = db.StepAccounts.Find(id);
            if (stepAccount == null)
            {
                return HttpNotFound();
            }
            return View(stepAccount);
        }

        // GET: StepAccounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StepAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,account_id,step_id")] StepAccount stepAccount)
        {
            if (ModelState.IsValid)
            {
                db.StepAccounts.Add(stepAccount);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stepAccount);
        }

        // GET: StepAccounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StepAccount stepAccount = db.StepAccounts.Find(id);
            if (stepAccount == null)
            {
                return HttpNotFound();
            }
            return View(stepAccount);
        }

        // POST: StepAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,account_id,step_id")] StepAccount stepAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stepAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stepAccount);
        }

        // GET: StepAccounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            StepAccount stepAccount = db.StepAccounts.Find(id);
            if (stepAccount == null)
            {
                return HttpNotFound();
            }
            return View(stepAccount);
        }

        // POST: StepAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            StepAccount stepAccount = db.StepAccounts.Find(id);
            db.StepAccounts.Remove(stepAccount);
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
