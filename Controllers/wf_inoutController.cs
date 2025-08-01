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
    public class wf_inoutController : Controller
    {
        private NHModel db = new NHModel();

        // GET: wf_inout
        public ActionResult Index()
        {
            return View(db.wf_inout.ToList());
        }

        // GET: wf_inout/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            wf_inout wf_inout = db.wf_inout.Find(id);
            if (wf_inout == null)
            {
                return HttpNotFound();
            }
            return View(wf_inout);
        }

        // GET: wf_inout/Create
        public ActionResult Create(long id)
        {
            ViewBag.id = id;
            return View();
        }

        // POST: wf_inout/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(wf_inout wf_inout, long amount)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            wf_inout.account_id = us.id;
            wf_inout.time = DateTime.Now;
            wf_inout.status = (int)XInOut.eStatus.DaThucHien;
            if (ModelState.IsValid)
            {
                if (amount > 0)
                {
                    var thuchicu = db.InOuts.Find(wf_inout.kid);
                    var thuchi = new InOut();
                    int type;
                    if (thuchicu.type == (int)XCategory.eType.Chi)
                        type = (int)XCategory.eType.Thu;
                    else
                        type = (int)XCategory.eType.Chi;
                    var danhmuccu = db.Categories.Find(thuchicu.category_id);
                    var category = db.Categories.Where(x => x.type == type && x.pair == danhmuccu.pair);
                    if (category == null || category.Count() == 0)
                    {
                        ModelState.AddModelError("category_id", "Chưa tạo danh mục cùng cặp thu chi.");
                    }
                    var cate = category.First();
                    thuchi.account_id = thuchicu.account_id;
                    thuchi.category_id = cate.id;
                    thuchi.code = Utils.SetCodeInout(cate);
                    thuchi.created_by = us.id;
                    thuchi.note = wf_inout.note;
                    thuchi.status = (int)XInOut.eStatus.DaThucHien;
                    thuchi.time = wf_inout.time;
                    thuchi.type = type;
                    db.InOuts.Add(thuchi);
                    db.SaveChanges();
                    // Thêm WF sau khi có id
                    if (cate.wf == (int)XCategory.eWf.Co)
                    {
                        var wf = new wf_inout();
                        wf.account_id = us.id;
                        wf.kid = thuchi.id;
                        wf.note = "Tự động sinh";
                        wf.status = (int)XInOut.eStatus.DaThucHien;
                        wf.time = thuchi.time;
                        db.wf_inout.Add(wf);
                    }
                    wf_inout.intout_id = thuchi.id;
                    db.wf_inout.Add(wf_inout);
                    db.SaveChanges();
                    var tongthanhtoan = from xwf in db.wf_inout
                                        join xtc in db.InOuts on xwf.intout_id equals xtc.id
                                        where xwf.kid == wf_inout.kid
                                        select xtc.amount;
                    var tong = tongthanhtoan.Sum();
                    if (tong >= thuchicu.amount)
                    {
                        wf_inout.status = (int)XInOut.eStatus.DaThucHien;
                        thuchicu.status = wf_inout.status;
                    }
                    wf_inout.note += " Tổng: " + tong;
                    db.SaveChanges();
                }
                return RedirectToAction("Details", "InOuts", new { id = wf_inout.kid });
            }

            return View(wf_inout);
        }

        // GET: wf_inout/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            wf_inout wf_inout = db.wf_inout.Find(id);
            if (wf_inout == null)
            {
                return HttpNotFound();
            }
            return View(wf_inout);
        }

        // POST: wf_inout/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,kid,status,time,account_id,note")] wf_inout wf_inout)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wf_inout).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(wf_inout);
        }

        // GET: wf_inout/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            wf_inout wf_inout = db.wf_inout.Find(id);
            if (wf_inout == null)
            {
                return HttpNotFound();
            }
            return View(wf_inout);
        }

        // POST: wf_inout/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            wf_inout wf_inout = db.wf_inout.Find(id);
            db.wf_inout.Remove(wf_inout);
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
