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
    public class StepsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: Steps
        public ActionResult Index()
        {
            return View(db.Steps.ToList());
        }

        // GET: Steps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Step step = db.Steps.Find(id);
            if (step == null)
            {
                return HttpNotFound();
            }
            return View(step);
        }

        // GET: Steps/Create
        public ActionResult Create()
        {
            ViewBag.lst_accounts = db.Accounts;
            var lst_steps = db.Steps.ToList();
            lst_steps.Insert(0, new Step());
            ViewBag.STEP = new SelectList(lst_steps, "id", "name");
            return View();
        }

        // POST: Steps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Step step, string str_account)
        {
            if (ModelState.IsValid)
            {
                db.Steps.Add(step);
                db.SaveChanges();

                var lst = str_account.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in lst)
                {
                    var sv = new StepAccount();
                    sv.step_id = step.id;
                    sv.account_id = Convert.ToInt32(item);
                    db.StepAccounts.Add(sv);
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            ViewBag.lst_accounts = db.Accounts;
            var lst_steps = db.Steps.ToList();
            lst_steps.Insert(0, new Step());
            ViewBag.STEP = new SelectList(lst_steps, "id", "name");
            return View(step);
        }

        // GET: Steps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Step step = db.Steps.Find(id);
            if (step == null)
            {
                return HttpNotFound();
            }
            ViewBag.lst_accounts = db.Accounts;
            var account_ids = db.StepAccounts.Where(x => x.step_id == id.Value).Select(x => x.account_id).ToArray();
            ViewBag.str_account = string.Join(",", account_ids);

            var lst_steps = db.Steps.Where(x => x.id != id).ToList();
            lst_steps.Insert(0, new Step());
            ViewBag.STEP = new SelectList(lst_steps, "id", "name");

            return View(step);
        }

        // POST: Steps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Step step, string str_account)
        {
            if (ModelState.IsValid)
            {
                var lst = str_account.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                var fk = db.StepAccounts.Where(x => x.step_id == step.id);
                db.StepAccounts.RemoveRange(fk);

                foreach (var item in lst)
                {
                    var sv = new StepAccount();
                    sv.step_id = step.id;
                    sv.account_id = Convert.ToInt32(item);
                    db.StepAccounts.Add(sv);
                }

                db.Entry(step).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.lst_accounts = db.Accounts;
            ViewBag.str_account = str_account;

            var lst_steps = db.Steps.Where(x => x.id != step.id).ToList();
            lst_steps.Insert(0, new Step());
            ViewBag.STEP = new SelectList(lst_steps, "id", "name");

            return View(step);
        }

        // GET: Steps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Step step = db.Steps.Find(id);
            if (step == null)
            {
                return HttpNotFound();
            }
            return View(step);
        }

        // POST: Steps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Step step = db.Steps.Find(id);
            db.Steps.Remove(step);
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
