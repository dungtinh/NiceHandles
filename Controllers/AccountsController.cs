using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Manager")]
    public class AccountsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: Accounts
        public ActionResult Index()
        {
            return View(db.Accounts.ToList());
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }
        public ActionResult Infomation()
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            return View(us);
        }

        sealed class sAccount
        {
            public Account obj { get; set; }
            public string ngaysinh { get; set; }
            public string ngaycap { get; set; }

        }
        public ActionResult DetailsX(int? id)
        {
            Account obj = db.Accounts.Find(id);
            sAccount acc = new sAccount();
            acc.obj = obj;
            acc.ngaysinh = obj.birthday.HasValue ? obj.birthday.Value.ToString("dd/MM/yyyy") : null;
            acc.ngaycap = obj.ngaycap_gt.HasValue ? obj.ngaycap_gt.Value.ToString("dd/MM/yyyy") : null;
            return Json(acc, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult ChoSo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            string NGAYTHANG = "Từ ngày: " + (account.chotso.HasValue ? account.chotso.Value.ToString("dd-MM-yyyy") : "          ") + " đến ngày: " + DateTime.Now.ToString("dd/MM/yyyy");
            account.chotso = DateTime.Now;

            var rs = db.thuchis.Where(x => x.trangthai == (int)XModels.eStatus.Processing && x.account_id == account.id).OrderBy(x => x.thoigian).ToArray();
            long TONGTIEN = 0;
            foreach (var item in rs)
            {
                item.ngaychotso = DateTime.Now;
                item.trangthai = (int)XModels.eStatus.Complete;
                TONGTIEN += item.sotien;
            }
            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/SoThuChi.xls");
            string webpart = "SoThuChi_" + account.id + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("NGAYTHANG", NGAYTHANG);
                    flexCelReport.SetValue("TONGTIEN", TONGTIEN.ToString("N0"));
                    flexCelReport.AddTable("TB", rs.Select(x => new { THOIGIAN = x.thoigian.ToString("dd/MM/yyyy"), SOTIEN = x.sotien.ToString("N0"), LYDO = x.lydo }));
                    flexCelReport.Run(sr, fs);
                }
            }
            db.SaveChanges();
            ViewBag.URL = "/public/" + webpart;
            ViewBag.GO = Url.Action("Index");
            return View();
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,UserName,fullname")] Account account)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            XAccount model = new XAccount();
            model.obj = account;
            return View(model);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(XAccount account)
        {
            db.Entry(account.obj).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Accounts/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
                      Account   account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
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
