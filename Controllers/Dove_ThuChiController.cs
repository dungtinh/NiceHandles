using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;
using PagedList;
using static NiceHandles.Controllers.InOutsController;

namespace NiceHandles.Controllers
{
    public class Dove_ThuChiController : Controller
    {
        private NHModel db = new NHModel();

        // GET: Dove_ThuChi
        public ActionResult Index(string Search_Data, int? s_account_id, int? s_contract_id, int? s_type, string Filter_Value, int? Page_No, string fromdate, string todate)
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
            ViewBag.account_id = s_account_id;
            ViewBag.contract_id = s_contract_id;
            ViewBag.FROMDATE = fromdate;
            ViewBag.TODATE = todate;
            DateTime FromDate = string.IsNullOrEmpty(fromdate) ? new DateTime() : DateTime.Parse(fromdate);
            DateTime ToDate = string.IsNullOrEmpty(todate) ? DateTime.Now : DateTime.Parse(todate);
            ToDate = ToDate.AddDays(1);
            ViewBag.type = s_type;

            var results = from v in db.Dove_ThuChi
                          where (
                          (string.IsNullOrEmpty(v.note) || v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper())) &&
                          v.type == (s_type.HasValue ? s_type.Value : v.type) &&
                          v.account_id == (s_account_id.HasValue ? s_account_id.Value : v.account_id) &&
                          v.contract_id == (s_contract_id.HasValue ? s_contract_id.Value : v.contract_id) &&
                          v.time >= FromDate &&
                          v.time < ToDate
                          )
                          orderby v.time descending
                          select v;

            var Summery = results.Select(x => new { x.amount, x.type }).ToArray();
            var lsttongthu = Summery.Where(x => x.type == (int)XCategory.eType.Thu);
            long tongthu = 0;
            if (lsttongthu.Count() > 0)
                tongthu = lsttongthu.Sum(x => x.amount);
            ViewBag.TONGTHU = tongthu.ToString("N0");

            long tongchi = 0;
            var lsttongchi = Summery.Where(x => x.type == (int)XCategory.eType.Chi);
            if (lsttongchi.Count() > 0)
                tongchi = lsttongchi.Sum(x => x.amount);
            ViewBag.TONGCHI = tongchi.ToString("N0");
            var thuchi = tongthu - tongchi;
            ViewBag.THUCHI = thuchi.ToString("N0");
            int Size_Of_Page = 50;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Dove_ThuChi inOut)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            inOut.created_by = us.id;

            inOut.amount = inOut.amount;
            inOut.time = DateTime.Now;
            if (inOut.contract_id.HasValue)
            {
                var con = db.Contracts.Find(inOut.contract_id);
                inOut.note += " (" + con.name + ")";
            }
            var cate = db.Categories.Find(inOut.category_id);
            db.Dove_ThuChi.Add(inOut);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public JsonResult BanInThuChi(string Search_Data, int? account_id, int? type, string fromdate, string todate)
        {
            DateTime FromDate = string.IsNullOrEmpty(fromdate) ? new DateTime() : DateTime.Parse(fromdate);
            DateTime ToDate = string.IsNullOrEmpty(todate) ? DateTime.Now : DateTime.Parse(todate);
            ToDate = ToDate.AddDays(1);
            var lstAccount = db.Accounts.Select(x => new { id = x.id, disname = x.disname }).ToList();
            var results = (from v in db.Dove_ThuChi
                           where (
                           (string.IsNullOrEmpty(v.note) || v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper())) &&
                           v.type == (type.HasValue ? type.Value : v.type) &&
                           v.account_id == (account_id.HasValue ? account_id.Value : v.account_id) &&
                           v.time >= FromDate &&
                           v.time < ToDate
                           )
                           orderby v.time descending
                           select v).ToArray();

            var lsttongthu = results.Where(x => x.type == (int)XCategory.eType.Thu);
            long tongthu = 0;
            if (lsttongthu.Count() > 0)
                tongthu = lsttongthu.Sum(x => x.amount);

            long tongchi = 0;
            var lsttongchi = results.Where(x => x.type == (int)XCategory.eType.Chi);
            if (lsttongchi.Count() > 0)
                tongchi = lsttongchi.Sum(x => x.amount);
            var thuchi = tongthu - tongchi;
            string NGAYTHANG = "Từ ngày: " + (!string.IsNullOrEmpty(fromdate) ? FromDate.ToString("dd/MM/yyyy") : "          ") +
                " đến ngày: " + (!string.IsNullOrEmpty(todate) ? ToDate.ToString("dd/MM/yyyy") : "              ");
            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/BanInThuChi.xls");
            string webpart = "BanInThuChi_" + DateTime.Now.ToString("ddMMyy") + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();

            var rs = results.Select(x => new cBanInThuChi()
            {
                LOAI = XCategory.sType[x.type],
                LYDO = x.note,
                SOTIEN = x.amount.ToString("N0"),
                TAIKHOAN = lstAccount.Find(a => a.id == x.account_id).disname,
                THOIGIAN = x.time.ToString("dd/MM/yyyy")
            });

            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("NGAYTHANG", NGAYTHANG);
                    flexCelReport.SetValue("THU", tongthu.ToString("N0"));
                    flexCelReport.SetValue("CHI", tongchi.ToString("N0"));
                    flexCelReport.SetValue("THUCHI", thuchi.ToString("N0"));
                    flexCelReport.AddTable("TB", rs);
                    flexCelReport.Run(sr, fs);
                }
            }
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }

        // GET: Dove_ThuChi/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dove_ThuChi dove_ThuChi = db.Dove_ThuChi.Find(id);
            if (dove_ThuChi == null)
            {
                return HttpNotFound();
            }
            return View(dove_ThuChi);
        }

        // GET: Dove_ThuChi/Create
        public ActionResult Create(int? id)
        {
            XInOut model = new XInOut();
            if (id.HasValue)
            {
                ViewBag.contract_id = id;
            }
            model.account = db.Accounts.Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.fullname }).ToArray();
            model.types = XCategory.sType.Select(x => new SelectListItem { Value = x.Key.ToString(), Text = x.Value }).ToArray();
            return View(model);
        }
        public ActionResult CreateContract(int contract_id)
        {
            var contract = db.Contracts.Find(contract_id);
            Dove_ThuChi model = new Dove_ThuChi();
            model.contract_id = contract_id;
            model.account_id = contract.account_id;
            return View(model);
        }
        [Authorize(Roles = "SuperAdmin,Accounting")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateContract(Dove_ThuChi inOut)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var contract = db.Contracts.Find(inOut.contract_id);
            inOut.created_by = us.id;
            inOut.time = DateTime.Now;
            inOut.note += " (" + contract.name + ")";
            NHTrans.SaveLog(db, us.id, "THU CHI TỔ ĐO", "Thêm khoản thu chi trong hợp đồng " + contract.name);
            db.Dove_ThuChi.Add(inOut);
            db.SaveChanges();
            ViewBag.CLOSE = 1;
            inOut.amount = 0;
            inOut.note = "";
            return View(inOut);
        }
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dove_ThuChi dove_ThuChi = db.Dove_ThuChi.Find(id);
            if (dove_ThuChi == null)
            {
                return HttpNotFound();
            }
            return View(dove_ThuChi);
        }

        // POST: Dove_ThuChi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,contract_id,category_id,account_id,nguoichi_id,type,note,time,amount")] Dove_ThuChi dove_ThuChi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dove_ThuChi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dove_ThuChi);
        }

        // GET: Dove_ThuChi/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dove_ThuChi dove_ThuChi = db.Dove_ThuChi.Find(id);
            if (dove_ThuChi == null)
            {
                return HttpNotFound();
            }
            return View(dove_ThuChi);
        }

        // POST: Dove_ThuChi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Dove_ThuChi dove_ThuChi = db.Dove_ThuChi.Find(id);
            db.Dove_ThuChi.Remove(dove_ThuChi);
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
        public class cBanInThuChi
        {
            public string THOIGIAN { get; set; }
            public string TAIKHOAN { get; set; }
            public string LOAI { get; set; }
            public string LOAICHITIET { get; set; }
            public string SOTIEN { get; set; }
            public string LYDO { get; set; }
        }
    }
}

