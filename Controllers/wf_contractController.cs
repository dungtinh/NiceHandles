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
    [Authorize(Roles = "SuperAdmin,Manager")]
    public class wf_contractController : Controller
    {
        private NHModel db = new NHModel();

        // GET: wf_contract
        public ActionResult Index()
        {
            return View();
        }

        // GET: wf_contract/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            wf_contract wf_contract = db.wf_contract.Find(id);
            if (wf_contract == null)
            {
                return HttpNotFound();
            }
            Xwf_contract model = new Xwf_contract();
            model.obj = wf_contract;
            model.account = db.Accounts.Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.fullname }).ToArray();
            var contract = db.Contracts.Find(wf_contract.contract_id);
            var account = db.Accounts.Find(wf_contract.account_id);
            model.contract_name = contract.name;
            model.account_name = account.fullname;
            return View(model);
        }

        // GET: wf_contract/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            Xwf_contract model = new Xwf_contract();
            model.obj = new wf_contract();
            model.obj.contract_id = id.Value;
            model.obj.status = (int)Xwf_contract.eStatus.Processing;
            model.account = db.Accounts.Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.fullname }).ToArray();
            return View(model);
        }

        // POST: wf_contract/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Xwf_contract model)
        {
            if (ModelState.IsValid)
            {
                Contract contract = db.Contracts.Find(model.obj.contract_id);
                if (contract == null)
                {
                    return HttpNotFound();
                }
                contract.status = model.obj.status;
                var lastid = db.wf_contract.OrderByDescending(x => x.id).FirstOrDefault();
                if (lastid != null)
                {
                    model.obj.step_id = lastid.step_id;
                    model.obj.from_id = lastid.from_id;
                }
                db.wf_contract.Add(model.obj);
                db.SaveChanges();
                return RedirectToAction("Details", "Contracts", new { id = model.obj.contract_id });
            }
            return View(model);
        }

        // GET: wf_contract/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            wf_contract wf_contract = db.wf_contract.Find(id);
            if (wf_contract == null)
            {
                return HttpNotFound();
            }
            Xwf_contract model = new Xwf_contract();
            model.obj = wf_contract;
            model.account = db.Accounts.Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.fullname }).ToArray();
            var contract = db.Contracts.Find(wf_contract.contract_id);
            return View(model);
        }

        // POST: wf_contract/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Xwf_contract model)
        {
            if (ModelState.IsValid)
            {
                Contract contract = db.Contracts.Find(model.obj.contract_id);
                if (contract == null)
                {
                    return HttpNotFound();
                }
                contract.status = model.obj.status;
                db.Entry(model.obj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Contracts", new { id = model.obj.contract_id });
            }
            return View(model);
        }

        // GET: wf_contract/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            wf_contract wf_contract = db.wf_contract.Find(id);
            if (wf_contract == null)
            {
                return HttpNotFound();
            }
            Xwf_contract model = new Xwf_contract();
            model.obj = wf_contract;
            model.account = db.Accounts.Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.fullname }).ToArray();
            var contract = db.Contracts.Find(wf_contract.contract_id);
            var account = db.Accounts.Find(wf_contract.account_id);
            model.account_name = account.fullname;
            return View(model);
        }

        // POST: wf_contract/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            wf_contract wf_contract = db.wf_contract.Find(id);
            db.wf_contract.Remove(wf_contract);
            db.SaveChanges();
            return RedirectToAction("Details", "Contracts", new { id = wf_contract.contract_id });
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
