using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Aspose.Words.Lists;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.BuilderProperties;
using NiceHandles.Models;
using PagedList;

namespace NiceHandles.Controllers
{
    public class DovesController : Controller
    {
        private NHModel db = new NHModel();

        // GET: Doves
        public ActionResult Index(string Search_Data, int? acc, int? add, int? uq, int? ser, int? sta, string Filter_Value, int? Page_No)
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
            ViewBag.ADD = add;
            ViewBag.STA = sta;
            ViewBag.UQ = uq;
            ViewBag.SER = ser;
            ViewBag.ACC = acc;

            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            ViewBag.USER = us;
            int temp = (int)XDove.eStatus.KetThuc;
            var results = from io in db.vPhongDoVes
                          where
                               (io.dove_account == (acc.HasValue ? acc.Value : io.dove_account)) &&
                               (io.status == (sta.HasValue ? sta.Value : io.status)) &&
                               (io.status < (sta.HasValue ? temp + 1 : temp)) &&
                               (io.account_id == (uq.HasValue ? uq : io.account_id)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.name.ToUpper())
                          orderby io.time descending
                          select io;
            int Size_Of_Page = 20;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var dove = db.vPhongDoVes.Single(x => x.id == id);
            return View(dove);
        }

        // GET: Doves/Create
        public ActionResult Create(int hoso_id)
        {
            return View();
        }

        // POST: Doves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(dovelichsu lichsu, HttpPostedFileBase[] flLink)
        {
            var hoso = db.HoSoes.Find(lichsu.hoso_id);
            var dove = db.Doves.SingleOrDefault(x => x.hoso_id == lichsu.hoso_id);
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            if (dove == null)
            {
                dove = new Dove();
                dove.time = DateTime.Now;
                dove.time_start = DateTime.Now;
                dove.hoso_id = lichsu.hoso_id;
                dove.status = (int)XModels.eStatus.Processing;
                dove.account_id = lichsu.dove_account;
                db.Doves.Add(dove);
                NHTrans.SaveLog(db, us.id, "ĐO VẼ", "Chuyển đo vẽ (" + hoso.name + ")");
                db.SaveChanges();
            }
            lichsu.account_id = us.id;
            lichsu.hoso_id = lichsu.hoso_id;
            lichsu.time = DateTime.Now;
            if (flLink.Length > 0)
            {
                var file = flLink[0];
                if (file != null)
                {
                    string filename = DateTime.Now.ToString("sshhmmddMMyy") + file.FileName;
                    var subPath = Server.MapPath("~/public/dove");
                    var exists = System.IO.Directory.Exists(subPath);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(subPath);
                    file.SaveAs(subPath + "/" + filename);
                    lichsu.url = "/public/dove/" + filename;
                }
            }
            db.dovelichsus.Add(lichsu);
            db.SaveChanges();
            return Json(lichsu, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateNhatKy(dovelichsu lichsu, HttpPostedFileBase[] flLink)
        {
            var hoso = db.HoSoes.Find(lichsu.hoso_id);
            var dove = db.Doves.FirstOrDefault(x => x.hoso_id == hoso.id);
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            dove.account_id = lichsu.dove_account;
            NHTrans.SaveLog(db, us.id, "ĐO VẼ", "Lưu nhật ký đo vẽ (" + hoso.name + ")");
            lichsu.account_id = us.id;
            lichsu.time = DateTime.Now;
            if (flLink.Length > 0)
            {
                var file = flLink[0];
                if (file != null)
                {
                    string filename = DateTime.Now.ToString("sshhmmddMMyy") + file.FileName;
                    var subPath = Server.MapPath("~/public/dove");
                    var exists = System.IO.Directory.Exists(subPath);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(subPath);
                    file.SaveAs(subPath + "/" + filename);
                    lichsu.url = "/public/dove/" + filename;
                }
            }
            db.dovelichsus.Add(lichsu);
            db.SaveChanges();
            return Json(lichsu, JsonRequestBehavior.AllowGet);
        }

        // GET: Doves/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dove dove = db.Doves.Find(id);
            if (dove == null)
            {
                return HttpNotFound();
            }
            return View(dove);
        }

        // POST: Doves/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Dove dove)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dove).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dove);
        }

        // GET: Doves/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dove dove = db.Doves.Find(id);
            if (dove == null)
            {
                return HttpNotFound();
            }
            return View(dove);
        }

        // POST: Doves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Dove dove = db.Doves.Find(id);
            db.Doves.Remove(dove);
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
        [HttpGet]
        public ActionResult ShowNhatKyDoVe(int hoso_id)
        {
            return PartialView("NhatKyDoVe", new dovelichsu() { hoso_id = hoso_id });
        }
        [HttpGet]
        public JsonResult DoVeNextStep(int dove_id, string note)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var dove = db.Doves.Find(dove_id);
            var lastWf = db.dove_wf.Where(x => x.dove_id == dove_id).OrderByDescending(x => x.time).FirstOrDefault();
            if (lastWf.status != (int)XDove.eStatus.KetThuc)
            {
                var wf = new dove_wf();
                wf.account_id = us.id;
                wf.time = DateTime.Now;
                wf.account_id_fk = us.id;
                wf.dove_id = dove_id;
                wf.note = note;
                if (lastWf != null)
                {
                    wf.status = lastWf.status + 1;
                }
                else
                {
                    wf.status = 0;
                }
                dove.status = wf.status;
                db.dove_wf.Add(wf);
                NHTrans.SaveLog(db, us.id, "ĐO VẼ", "Chuyển bước đo vẽ (" + XDove.sStatus[wf.status] + ")");
                db.SaveChanges();
                string result = "";
                result += "<tr>" +
                           "<td>" + wf.time.ToString("dd/MM/yy") + "</td>" +
                           "<td>" + us.disname + "</td>" +
                           "<td>" + wf.note + "</td>" +
                           "<td>" + XDove.sStatus[wf.status] + "</td>" +
                           "</tr>";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult DoVePrevStep(int dove_id, string note)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var dove = db.Doves.Find(dove_id);
            var lastWf = db.dove_wf.Where(x => x.dove_id == dove_id).OrderByDescending(x => x.time).FirstOrDefault();
            if (lastWf.status != 0)
            {
                var wf = new dove_wf();
                wf.account_id = us.id;
                wf.time = DateTime.Now;
                wf.account_id_fk = us.id;
                wf.dove_id = dove_id;
                wf.note = note;
                if (lastWf != null)
                {
                    wf.status = lastWf.status - 1;
                }
                else
                {
                    wf.status = 0;
                }
                dove.status = wf.status;
                db.dove_wf.Add(wf);
                NHTrans.SaveLog(db, us.id, "ĐO VẼ", "Chuyển bước đo vẽ (" + XDove.sStatus[wf.status] + ")");
                db.SaveChanges();
                string result = "";
                result += "<tr>" +
                           "<td>" + wf.time.ToString("dd/MM/yy") + "</td>" +
                           "<td>" + us.disname + "</td>" +
                           "<td>" + wf.note + "</td>" +
                           "<td>" + XDove.sStatus[wf.status] + "</td>" +
                           "</tr>";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
