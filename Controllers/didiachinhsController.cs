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
    public class didiachinhsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: didiachinhs
        public ActionResult Index()
        {
            var lst = new List<Xdidiachinh>();
            var lstdiachinh = db.didiachinhs.Where(x => x.trangthai == (int)XModels.eStatus.Processing).ToList();
            foreach (var item in lstdiachinh)
            {
                var per = new Xdidiachinh();
                per.obj = item;
                per.contract = db.Contracts.Find(item.contract_id);
                per.infomation = db.Infomations.Single(x => x.hoso_id == per.obj.contract_id);
                per.infomation = per.infomation == null ? new Infomation() : per.infomation;
                //var sv = db.fk_contract_service.First(x => x.contract_id == per.contract.id).service_id;
                per.service = db.Services.Find(per.contract.id);
                var canbo = db.fk_CanBo_Address.Where(x => x.address_id == per.contract.address_id).FirstOrDefault();
                lst.Add(per);
            }
            return View(lst);
        }

        // GET: didiachinhs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            didiachinh didiachinh = db.didiachinhs.Find(id);
            if (didiachinh == null)
            {
                return HttpNotFound();
            }
            return View(didiachinh);
        }

        // GET: didiachinhs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: didiachinhs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,contract_id,ngaynop,noinop,trangthai")] didiachinh didiachinh)
        {
            if (ModelState.IsValid)
            {
                db.didiachinhs.Add(didiachinh);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(didiachinh);
        }

        // GET: didiachinhs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            didiachinh didiachinh = db.didiachinhs.Find(id);
            if (didiachinh == null)
            {
                return HttpNotFound();
            }
            return View(didiachinh);
        }

        // POST: didiachinhs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,contract_id,ngaynop,noinop,trangthai")] didiachinh didiachinh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(didiachinh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(didiachinh);
        }

        // GET: didiachinhs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            didiachinh didiachinh = db.didiachinhs.Find(id);
            if (didiachinh == null)
            {
                return HttpNotFound();
            }
            return View(didiachinh);
        }

        // POST: didiachinhs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            didiachinh didiachinh = db.didiachinhs.Find(id);
            db.didiachinhs.Remove(didiachinh);
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
