using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.Mvc;
using Aspose.Words;
using Aspose.Words.Rendering;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;
using PagedList;
using static System.Data.Entity.Infrastructure.Design.Executor;
using static NiceHandles.Controllers.InOutsController;
using static NiceHandles.Models.XModels;

namespace NiceHandles.Controllers
{
    public class InOutChoDuyetsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: InOutChoDuyets
        [Authorize(Roles = "SuperAdmin,Manager,Accounting")]
        public ActionResult Index(string Search_Data, string Filter_Value, int? Page_No, string fromdate, string todate)
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

            ViewBag.FROMDATE = fromdate;
            ViewBag.TODATE = todate;
            DateTime FromDate = string.IsNullOrEmpty(fromdate) ? new DateTime() : DateTime.Parse(fromdate);
            DateTime ToDate = string.IsNullOrEmpty(todate) ? DateTime.Now : DateTime.Parse(todate);
            ToDate = ToDate.AddDays(1);

            var results = from v in db.InOutChoDuyets
                          where (
                          (string.IsNullOrEmpty(v.note) ||
                          v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper()) ||
                          v.code.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.code.ToUpper())
                          ) &&
                          v.time >= FromDate &&
                          v.time < ToDate
                          )
                          orderby v.time descending
                          select v;

            int Size_Of_Page = 30;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        [Authorize(Roles = "SuperAdmin,Manager,Accounting")]
        public ActionResult ChoDuyet(string Search_Data, int? s_account_id, int? s_type, int? s_category_id, int? s_currency, string fromdate, string todate)
        {
            ViewBag.FilterValue = Search_Data;
            ViewBag.category_id = s_category_id;
            ViewBag.account_id = s_account_id;

            ViewBag.FROMDATE = fromdate;
            ViewBag.TODATE = todate;
            DateTime FromDate = string.IsNullOrEmpty(fromdate) ? new DateTime() : DateTime.Parse(fromdate);
            DateTime ToDate = string.IsNullOrEmpty(todate) ? DateTime.Now : DateTime.Parse(todate);
            ToDate = ToDate.AddDays(1);
            ViewBag.type = s_type;

            var results = from v in db.InOuts
                          where (
                          (string.IsNullOrEmpty(v.note) || v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper())) &&
                          v.category_id == (s_category_id.HasValue ? s_category_id.Value : v.category_id) &&
                          v.type == (s_type.HasValue ? s_type.Value : v.type) &&
                          v.account_id == (s_account_id.HasValue ? s_account_id.Value : v.account_id) &&
                          v.currency == (s_currency.HasValue ? s_currency.Value : v.currency) &&
                          v.status < (int)XInOut.eStatus.DaThucHien &&
                          v.time >= FromDate &&
                          v.time < ToDate
                          )
                          orderby v.time descending
                          select v;
            return View(results);
        }
        [Authorize(Roles = "SuperAdmin,Manager,Accounting")]
        public ActionResult DuyetTheoHoSo()
        {
            var results = from v in db.InOuts
                          where (
                          v.status == (int)XInOut.eStatus.ChoDuyet
                          )
                          orderby v.time descending
                          select v;
            return View(results);
        }
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult ChoXemXet(string Search_Data, int? s_account_id, int? s_type, int? s_category_id, int? s_currency, string fromdate, string todate)
        {
            ViewBag.FilterValue = Search_Data;
            ViewBag.category_id = s_category_id;
            ViewBag.account_id = s_account_id;

            ViewBag.FROMDATE = fromdate;
            ViewBag.TODATE = todate;
            DateTime FromDate = string.IsNullOrEmpty(fromdate) ? new DateTime() : DateTime.Parse(fromdate);
            DateTime ToDate = string.IsNullOrEmpty(todate) ? DateTime.Now : DateTime.Parse(todate);
            ToDate = ToDate.AddDays(1);
            ViewBag.type = s_type;

            var results = from v in db.InOuts
                          where (
                          (string.IsNullOrEmpty(v.note) || v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper())) &&
                          v.category_id == (s_category_id.HasValue ? s_category_id.Value : v.category_id) &&
                          v.type == (s_type.HasValue ? s_type.Value : v.type) &&
                          v.account_id == (s_account_id.HasValue ? s_account_id.Value : v.account_id) &&
                          v.currency == (s_currency.HasValue ? s_currency.Value : v.currency) &&
                          v.status < (int)XInOut.eStatus.DaThucHien &&
                          v.unlock == 1 &&
                          v.time >= FromDate &&
                          v.time < ToDate
                          )
                          orderby v.time descending
                          select v;
            return View(results);
        }

        // GET: InOutChoDuyets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InOutChoDuyet inOutChoDuyet = db.InOutChoDuyets.Find(id);
            if (inOutChoDuyet == null)
            {
                return HttpNotFound();
            }
            return View(inOutChoDuyet);
        }

        // GET: InOutChoDuyets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InOutChoDuyets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InOutChoDuyet inOutChoDuyet, string checkedids)
        {
            inOutChoDuyet.time = DateTime.Now;
            inOutChoDuyet.status = (int)XInOutChoDuyet.eStatus.Processing;
            db.InOutChoDuyets.Add(inOutChoDuyet);
            db.SaveChanges();

            var ioids = checkedids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            ioids = ioids.Select(x => x.Replace("io", "")).ToArray();
            foreach (var item in ioids)
            {
                var io = db.InOuts.Find(Convert.ToInt64(item));
                io.inoutchoduyet_id = inOutChoDuyet.id;
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(InOutChoDuyet inOutChoDuyet, string checkedids)
        {
            if (inOutChoDuyet.status == (int)XInOutChoDuyet.eStatus.Processing)
            {
                inOutChoDuyet.time = DateTime.Now;
                inOutChoDuyet.status = (int)XInOutChoDuyet.eStatus.Complete;

                var ioids = checkedids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                ioids = ioids.Select(x => x.Replace("io", "")).ToArray();
                var lstioids = ioids.Select(x => Convert.ToInt64(x)).ToArray();
                var lstInOuts = db.InOuts.Where(x => x.inoutchoduyet_id == inOutChoDuyet.id).ToArray();
                foreach (var io in lstInOuts)
                {
                    if (!lstioids.Contains(io.id))
                    {
                        io.inoutchoduyet_id = null;
                    }
                }
                db.SaveChanges();
            }
            return View(inOutChoDuyet);
        }
        public ActionResult Action(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InOutChoDuyet inOutChoDuyet = db.InOutChoDuyets.Find(id);
            if (inOutChoDuyet == null)
            {
                return HttpNotFound();
            }
            return View(inOutChoDuyet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Action(InOutChoDuyet inOutChoDuyet, string checkedids)
        {
            inOutChoDuyet.time = DateTime.Now;
            var ioids = checkedids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            ioids = ioids.Select(x => x.Replace("io", "")).ToArray();
            var lstioids = ioids.Select(x => Convert.ToInt64(x)).ToArray();
            var lstInOuts = db.InOuts.Where(x => x.inoutchoduyet_id == inOutChoDuyet.id).ToArray();

            if (inOutChoDuyet.status == (int)XInOutChoDuyet.eStatus.Processing)
            {
                foreach (var io in lstInOuts)
                {
                    if (!lstioids.Contains(io.id))
                    {
                        io.inoutchoduyet_id = null;
                    }
                }
            }
            inOutChoDuyet.status = (int)XInOutChoDuyet.eStatus.Complete;

            foreach (var io in lstInOuts)
            {
                if (lstioids.Contains(io.id))
                {
                    io.status = (int)XInOut.eStatus.DaThucHien;
                }
            }

            db.Entry(inOutChoDuyet).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Action", new { id = inOutChoDuyet.id });
        }
        [HttpPost]
        public JsonResult CreatePhieuThuChi(InOutChoDuyet inOutChoDuyet, InOut[] lstInOuts)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            inOutChoDuyet.time = DateTime.Now;
            inOutChoDuyet.code = "PC-" + DateTime.Now.ToString("ddMMyyyy-ssmmHH");
            inOutChoDuyet.status = (int)XInOutChoDuyet.eStatus.Complete;
            db.InOutChoDuyets.Add(inOutChoDuyet);
            db.SaveChanges();
            foreach (var io in lstInOuts)
            {
                var perIo = db.InOuts.Find(io.id);
                perIo.inoutchoduyet_id = inOutChoDuyet.id;
                perIo.currency = io.currency;
                perIo.status = (int)XInOut.eStatus.DaThucHien;
                //Create Nhat ký duyệt
                var cate = db.Categories.Find(perIo.category_id);
                if (cate.wf == (int)XCategory.eWf.Co)
                {
                    var wf = new wf_inout();
                    wf.account_id = us.id;
                    wf.kid = perIo.id;
                    wf.note = "Khởi tạo";
                    wf.status = (int)XInOut.eStatus.DaThucHien;
                    wf.time = DateTime.Now;                    
                    db.wf_inout.Add(wf);
                    db.SaveChanges();
                }
                if (cate.kind == (int)XCategory.eKind.Quy)
                {
                    var quy = new thuchi();
                    quy.account_id = perIo.account_id;
                    quy.thoigian = DateTime.Now;
                    quy.sotien = perIo.amount;
                    quy.loai = (int)XCategory.eType.Thu;
                    quy.lydo = perIo.note;
                    quy.trangthai = (int)XModels.eStatus.Complete;
                    db.thuchis.Add(quy);
                    db.SaveChanges();
                }
                if (cate.kind == (int)XCategory.eKind.Dove)
                {
                    var quy = new QuyDove();
                    quy.account_id = us.id;
                    quy.time = DateTime.Now;
                    quy.amount = perIo.amount;
                    quy.type = (int)XCategory.eType.Thu;
                    if (perIo.contract_id.HasValue)
                    {
                        var con = db.Contracts.Find(perIo.contract_id);
                        quy.note = perIo.note + " (" + con.name + ")";
                    }
                    else
                        quy.note = perIo.note;
                    quy.sta = (int)XModels.eStatus.Complete;
                    quy.state = (int)XModels.eStatus.Processing;
                    db.QuyDoves.Add(quy);
                    db.SaveChanges();
                    var wf = new wf_quydove();
                    wf.account_id = us.id;
                    wf.kid = (int)quy.id;
                    wf.note = "Khởi tạo";
                    wf.time = DateTime.Now;
                    db.wf_quydove.Add(wf);
                    db.SaveChanges();
                }

                if (perIo.contract_id != null)
                {
                    var category = db.Categories.Find(perIo.category_id);
                    if (category.theodoi == (int)XCategory.eTheoDoi.Co)
                    {
                        DuChiNhatKy nk = new DuChiNhatKy();
                        nk.inout_id = perIo.id;
                        nk.status = (int)XDuChiNhatKy.eStatus.LuongGiu;
                        nk.time = DateTime.Now;
                        nk.account_id = perIo.account_id;
                        db.DuChiNhatKies.Add(nk);
                    }
                }
                db.SaveChanges();
            }
            return Json(inOutChoDuyet.id, JsonRequestBehavior.AllowGet);
        }

        // GET: InOutChoDuyets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InOutChoDuyet inOutChoDuyet = db.InOutChoDuyets.Find(id);
            if (inOutChoDuyet == null)
            {
                return HttpNotFound();
            }
            return View(inOutChoDuyet);
        }

        // POST: InOutChoDuyets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,note,time,status")] InOutChoDuyet inOutChoDuyet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inOutChoDuyet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inOutChoDuyet);
        }

        // GET: InOutChoDuyets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InOutChoDuyet inOutChoDuyet = db.InOutChoDuyets.Find(id);
            if (inOutChoDuyet == null)
            {
                return HttpNotFound();
            }
            return View(inOutChoDuyet);
        }

        // POST: InOutChoDuyets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InOutChoDuyet inOutChoDuyet = db.InOutChoDuyets.Find(id);
            inOutChoDuyet.status = (int)XInOutChoDuyet.eStatus.Cancel;
            var lstInOuts = db.InOuts.Where(x => x.inoutchoduyet_id == id);
            foreach (var item in lstInOuts)
            {
                item.inoutchoduyet_id = null;
            }
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

        [Authorize(Roles = "SuperAdmin,Manager,Accounting")]
        public JsonResult InOutChoDuyetPrint(int id)
        {
            var obj = db.InOutChoDuyets.Find(id);
            var lstInOuts = db.InOuts.Where(x => x.inoutchoduyet_id == obj.id).ToArray();

            var lsttongthu = lstInOuts.Where(x => x.type == (int)XCategory.eType.Thu);
            long tongthu = 0;
            if (lsttongthu.Count() > 0)
                tongthu = lsttongthu.Sum(x => x.amount);

            long tongchi = 0;
            var lsttongchi = lstInOuts.Where(x => x.type == (int)XCategory.eType.Chi);
            if (lsttongchi.Count() > 0)
                tongchi = lsttongchi.Sum(x => x.amount);
            var thuchi = tongthu - tongchi;
            string NGAYTHANG = "Ngày " + DateTime.Now.ToString("dd/MM/yyyy");
            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/InOutChoDuyet.xls");
            string webpart = "InOutChoDuyet_" + DateTime.Now.ToString("ssmmHHddMMyy") + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            var rs = new List<InOutChoDuyetPrint>();
            foreach (var item in lstInOuts)
            {
                var print = new InOutChoDuyetPrint();
                if (item.contract_id.HasValue)
                    print.HOPDONG = db.Contracts.Find(item.contract_id).name;
                print.LYDO = item.note;
                print.LOAICHITIET = db.Categories.Find(item.category_id).name;
                print.TAIKHOAN = db.Accounts.Find(item.account_id).fullname;
                print.SOTIEN = item.amount;
                rs.Add(print);
            }
            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("NOTE", obj.note);
                    flexCelReport.SetValue("CODE", obj.code);
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
        [Authorize(Roles = "SuperAdmin,Manager,Accounting")]
        public ActionResult TheoDoiChi(string Search_Data, int? sort, int? account_id, int? status, int? category_id, int? contract_id, string Filter_Value, int? Page_No, string fromdate, string todate)
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
            ViewBag.category_id = category_id;
            ViewBag.account_id = account_id;
            ViewBag.contract_id = contract_id;
            ViewBag.status = status;

            ViewBag.FROMDATE = fromdate;
            ViewBag.TODATE = todate;
            DateTime FromDate = string.IsNullOrEmpty(fromdate) ? new DateTime() : DateTime.Parse(fromdate);
            DateTime ToDate = string.IsNullOrEmpty(todate) ? DateTime.Now : DateTime.Parse(todate);
            ToDate = ToDate.AddDays(1);

            IOrderedQueryable<vTheoDoiChi> results;
            if (sort == null || sort == 0)
            {
                sort = 0;
                ViewBag.SORTSTATUS = true;
                ViewBag.SORTTIME = false;
                ViewBag.SORTNAME = false;
                results = from v in db.vTheoDoiChis
                          where (
                          (string.IsNullOrEmpty(v.note) || v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper())) &&
                          v.category_id == (category_id.HasValue ? category_id.Value : v.category_id) &&
                          v.account_id == (account_id.HasValue ? account_id.Value : v.account_id) &&
                          v.contract_id == (contract_id.HasValue ? contract_id.Value : v.contract_id) &&
                          v.status == (status.HasValue ? status.Value : v.status) &&
                          v.time >= FromDate &&
                          v.time < ToDate
                          )
                          orderby v.status
                          select v;
            }
            else if (sort == 1)
            {
                ViewBag.SORTSTATUS = false;
                ViewBag.SORTTIME = true;
                ViewBag.SORTNAME = false;
                results = from v in db.vTheoDoiChis
                          where (
                          (string.IsNullOrEmpty(v.note) || v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper())) &&
                          v.category_id == (category_id.HasValue ? category_id.Value : v.category_id) &&
                          v.account_id == (account_id.HasValue ? account_id.Value : v.account_id) &&
                          v.contract_id == (contract_id.HasValue ? contract_id.Value : v.contract_id) &&
                          v.status == (status.HasValue ? status.Value : v.status) &&
                          v.time >= FromDate &&
                          v.time < ToDate
                          )
                          orderby v.time
                          select v;
            }
            else
            {
                ViewBag.SORTSTATUS = false;
                ViewBag.SORTTIME = false;
                ViewBag.SORTNAME = true;
                results = from v in db.vTheoDoiChis
                          where (
                          (string.IsNullOrEmpty(v.note) || v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper())) &&
                          v.category_id == (category_id.HasValue ? category_id.Value : v.category_id) &&
                          v.account_id == (account_id.HasValue ? account_id.Value : v.account_id) &&
                          v.contract_id == (contract_id.HasValue ? contract_id.Value : v.contract_id) &&
                          v.status == (status.HasValue ? status.Value : v.status) &&
                          v.time >= FromDate &&
                          v.time < ToDate
                          )
                          orderby v.contract_name
                          select v;
            }
            ViewBag.sort = sort;
            var check = results.Count();

            var SumMoney = check > 0 ? results.Sum(x => x.amount) : 0;

            long SumDaThucHien = 0;
            if (results.Count(x => x.status == (int)XDuChiNhatKy.eStatus.DaDi) > 0)
            {
                SumDaThucHien = results.Where(x => x.status == (int)XDuChiNhatKy.eStatus.DaDi).Sum(x => x.amount);
            }
            long SumChoThuHien = 0;
            if (results.Count(x => x.status == (int)XDuChiNhatKy.eStatus.DiThucHien) > 0)
            {
                SumChoThuHien = results.Where(x => x.status == (int)XDuChiNhatKy.eStatus.DiThucHien).Sum(x => x.amount);
            }
            long SumLuongGiu = 0;
            if (results.Count(x => x.status == (int)XDuChiNhatKy.eStatus.LuongGiu) > 0)
            {
                SumLuongGiu = results.Where(x => x.status == (int)XDuChiNhatKy.eStatus.LuongGiu).Sum(x => x.amount);
            }
            long SumTraLai = 0;
            if (results.Count(x => x.status == (int)XDuChiNhatKy.eStatus.TraLai) > 0)
            {
                SumTraLai = results.Where(x => x.status == (int)XDuChiNhatKy.eStatus.TraLai).Sum(x => x.amount);
            }

            ViewBag.TONG = (SumLuongGiu + SumDaThucHien + SumChoThuHien - SumTraLai).ToString("N0");
            ViewBag.SumLuongGiu = SumLuongGiu.ToString("N0");
            ViewBag.SumDaThucHien = SumDaThucHien.ToString("N0");
            ViewBag.SumChoThuHien = SumChoThuHien.ToString("N0");
            ViewBag.SumTraLai = SumTraLai.ToString("N0");

            int Size_Of_Page = 30;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [Authorize(Roles = "SuperAdmin,Manager,Accounting")]
        public JsonResult BanInTheoDoiChi(string Search_Data, int? account_id, int? status, int? category_id, int? contract_id, string Filter_Value, int? Page_No, string fromdate, string todate)
        {
            DateTime FromDate = string.IsNullOrEmpty(fromdate) ? new DateTime() : DateTime.Parse(fromdate);
            DateTime ToDate = string.IsNullOrEmpty(todate) ? DateTime.Now : DateTime.Parse(todate);
            ToDate = ToDate.AddDays(1);

            var results = from v in db.vTheoDoiChis
                          where (
                          (string.IsNullOrEmpty(v.note) || v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper())) &&
                          v.category_id == (category_id.HasValue ? category_id.Value : v.category_id) &&
                          v.account_id == (account_id.HasValue ? account_id.Value : v.account_id) &&
                          v.contract_id == (contract_id.HasValue ? contract_id.Value : v.contract_id) &&
                          v.status == (status.HasValue ? status.Value : v.status) &&
                          v.time >= FromDate &&
                          v.time < ToDate
                          )
                          orderby v.status, v.time descending
                          select v;

            var check = results.Count();

            var SumMoney = check > 0 ? results.Sum(x => x.amount) : 0;

            long SumDaThucHien = 0;
            if (results.Count(x => x.status == (int)XDuChiNhatKy.eStatus.DaDi) > 0)
            {
                SumDaThucHien = results.Where(x => x.status == (int)XDuChiNhatKy.eStatus.DaDi).Sum(x => x.amount);
            }
            long SumChoThuHien = 0;
            if (results.Count(x => x.status == (int)XDuChiNhatKy.eStatus.DiThucHien) > 0)
            {
                SumChoThuHien = results.Where(x => x.status == (int)XDuChiNhatKy.eStatus.DiThucHien).Sum(x => x.amount);
            }
            long SumLuongGiu = 0;
            if (results.Count(x => x.status == (int)XDuChiNhatKy.eStatus.LuongGiu) > 0)
            {
                SumLuongGiu = results.Where(x => x.status == (int)XDuChiNhatKy.eStatus.LuongGiu).Sum(x => x.amount);
            }
            long SumTraLai = 0;
            if (results.Count(x => x.status == (int)XDuChiNhatKy.eStatus.TraLai) > 0)
            {
                SumTraLai = results.Where(x => x.status == (int)XDuChiNhatKy.eStatus.TraLai).Sum(x => x.amount);
            }

            string NGAYTHANG = "Từ ngày: " + (!string.IsNullOrEmpty(fromdate) ? FromDate.ToString("dd/MM/yyyy") : "          ") +
                " đến ngày: " + (!string.IsNullOrEmpty(todate) ? ToDate.ToString("dd/MM/yyyy") : "              ");
            string path = Server.MapPath("~/public/temp/thuchi/");
            string pathTemp = Server.MapPath("~/App_Data/templates/DSTheoDoiChi.xls");
            string webpart = "TheoDoiChi_" + DateTime.Now.ToString("ddMMyy") + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();

            var rs = results.ToArray().Select(x => new cBanInTheoDoiChi()
            {
                address_name = x.address_name,
                amount = x.amount,
                category_name = x.category_name,
                contract_name = x.contract_name,
                fullname = x.fullname,
                note = x.note,
                service_name = x.service_name,
                status_name = XDuChiNhatKy.sStatus[x.status]
            });

            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("NGAYTHANG", NGAYTHANG);
                    flexCelReport.SetValue("TONG", SumMoney.ToString("N0"));
                    flexCelReport.SetValue("LUONG", SumLuongGiu.ToString("N0"));
                    flexCelReport.SetValue("CHO", SumChoThuHien.ToString("N0"));
                    flexCelReport.SetValue("DA", SumDaThucHien.ToString("N0"));
                    flexCelReport.SetValue("TRA", SumTraLai.ToString("N0"));
                    flexCelReport.AddTable("TB", rs);
                    flexCelReport.Run(sr, fs);
                }
            }
            return Json("/public/temp/thuchi/" + webpart, JsonRequestBehavior.AllowGet);
        }
        public JsonResult InDSChoDuyet(string Search_Data, int? account_id, int? status, int? category_id, int? contract_id, string Filter_Value, int? Page_No, string fromdate, string todate)
        {
            var results = from v in db.InOuts
                          where
                          v.status < (int)XInOut.eStatus.DaThucHien
                          orderby v.time descending
                          select v;

            string path = Server.MapPath("~/public/temp/thuchi/");
            string pathTemp = Server.MapPath("~/App_Data/templates/DSChoDuyet.xls");
            string webpart = "DSChoDuyet_" + DateTime.Now.ToString("ddMMyy") + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            var rs = new List<cBanInTheoDoiChi>();
            foreach (var item in results)
            {
                var contrat = db.Contracts.Find(item.contract_id);
                var address = db.Addresses.Find(contrat.address_id);
                var cate = db.Categories.Find(item.category_id);
                var acc = db.Accounts.Find(item.account_id);
                var ser = db.Services.Find(contrat.service_id);
                var x = new cBanInTheoDoiChi();
                x.address_name = address.name;
                x.contract_name = contrat.name;
                x.category_name = cate.name;
                x.service_name = ser.name;
                x.fullname = acc.fullname;
                x.amount = item.amount;
                x.note = item.note;
                x.status_name = XDuChiNhatKy.sStatus[item.status];
                rs.Add(x);
            }

            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.AddTable("TB", rs);
                    flexCelReport.Run(sr, fs);
                }
            }
            return Json("/public/temp/thuchi/" + webpart, JsonRequestBehavior.AllowGet);
        }
    }
}
