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
    public class PayPlansController : Controller
    {
        private NHModel db = new NHModel();

        // GET: PayPlans
        public ActionResult Index(int? contract_id)
        {
            ViewBag.contract_id = contract_id;
            return View(db.vPayPlans.ToList());
        }

        // GET: PayPlans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PayPlan payPlan = db.PayPlans.Find(id);
            if (payPlan == null)
            {
                return HttpNotFound();
            }
            return View(payPlan);
        }

        // GET: PayPlans/Create
        public ActionResult Create(int? contract_id, int? category_id)
        {
            var payPlan = new PayPlan();
            if (contract_id.HasValue && category_id.HasValue)
            {
                payPlan.contract_id = contract_id.Value;
                var contract = db.Contracts.Find(contract_id);
                var category = db.Categories.Find(category_id);
                payPlan.account_id = contract.account_id;
                payPlan.category_id = category_id.Value;
            }
            return View(payPlan);
        }

        // POST: PayPlans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PayPlan payPlan)
        {
            payPlan.type = (int)XCategory.eType.Chi;
            payPlan.status = (int)XPayplan.eStatus.ChoDuyet;
            payPlan.time = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.PayPlans.Add(payPlan);
                db.SaveChanges();
                if (payPlan.contract_id.HasValue)
                {
                    return RedirectToAction("Index", new { contract_id = payPlan.contract_id });
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            return View(payPlan);
        }

        // GET: PayPlans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PayPlan payPlan = db.PayPlans.Find(id);
            if (payPlan == null)
            {
                return HttpNotFound();
            }
            return View(payPlan);
        }

        // POST: PayPlans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,account_id,category_id,type,note,time,contract_id,status")] PayPlan payPlan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payPlan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(payPlan);
        }

        // GET: PayPlans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PayPlan payPlan = db.PayPlans.Find(id);
            if (payPlan == null)
            {
                return HttpNotFound();
            }
            return View(payPlan);
        }

        // POST: PayPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PayPlan payPlan = db.PayPlans.Find(id);
            db.PayPlans.Remove(payPlan);
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
        [Authorize(Roles = "SuperAdmin,Accounting")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateX(PayPlan inOut)
        {
            inOut.status = (int)XPayplan.eStatus.ChoDuyet;
            db.PayPlans.Add(inOut);
            db.SaveChanges();
            return View(new PayPlan());
        }
        [Authorize(Roles = "SuperAdmin,Accounting")]
        public ActionResult CreateX()
        {
            return View();
        }
    }
}
