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
    public class CanBoesController : Controller
    {
        private NHModel db = new NHModel();

        // GET: CanBoes
        public ActionResult Index()
        {
            ViewBag.db = db;
            return View(db.CanBoes.ToList());
        }

        // GET: CanBoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CanBo canBo = db.CanBoes.Find(id);
            if (canBo == null)
            {
                return HttpNotFound();
            }
            return View(canBo);
        }

        // GET: CanBoes/Create
        public ActionResult Create()
        {
            ViewBag.lst_address = db.Addresses.ToArray();
            return View();
        }

        // POST: CanBoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CanBo canBo, string str_address)
        {
            if (ModelState.IsValid)
            {
                db.CanBoes.Add(canBo);
                db.SaveChanges();

                //dia chỉ
                var lst = str_address.Split(',');
                foreach (var item in lst)
                {
                    var sv = new fk_CanBo_Address();
                    sv.canbo_id = canBo.id;
                    sv.address_id = Convert.ToInt32(item);
                    db.fk_CanBo_Address.Add(sv);
                }
                db.SaveChanges();
                //end
                return RedirectToAction("Index");
            }
            ViewBag.lst_address = db.Addresses.ToArray();
            return View(canBo);
        }

        // GET: CanBoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CanBo canBo = db.CanBoes.Find(id);
            if (canBo == null)
            {
                return HttpNotFound();
            }
            ViewBag.lst_address = db.Addresses.ToArray();

            var str_address = db.fk_CanBo_Address.Where(x => x.canbo_id == id).Select(x => x.address_id).ToArray();
            ViewBag.str_address = string.Join(",", str_address);

            return View(canBo);
        }

        // POST: CanBoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CanBo canBo, string str_address)
        {
            if (ModelState.IsValid)
            {

                var lst = str_address.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                var fk = db.fk_CanBo_Address.Where(x => x.canbo_id == canBo.id);
                db.fk_CanBo_Address.RemoveRange(fk);

                foreach (var item in lst)
                {
                    var sv = new fk_CanBo_Address();
                    sv.canbo_id = canBo.id;
                    sv.address_id = Convert.ToInt32(item);
                    db.fk_CanBo_Address.Add(sv);
                }

                db.Entry(canBo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.lst_address = db.Addresses.ToArray();
            return View(canBo);
        }

        // GET: CanBoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CanBo canBo = db.CanBoes.Find(id);
            if (canBo == null)
            {
                return HttpNotFound();
            }
            return View(canBo);
        }

        // POST: CanBoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CanBo canBo = db.CanBoes.Find(id);
            db.CanBoes.Remove(canBo);
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
