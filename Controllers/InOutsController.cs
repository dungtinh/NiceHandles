using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NiceHandles.Models;
using PagedList;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Web.Helpers;
using System.Security.Principal;
using static System.Data.Entity.Infrastructure.Design.Executor;
using Aspose.Words.Lists;
using Newtonsoft.Json;
using Microsoft.Ajax.Utilities;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Policy;
using System.Web.UI.WebControls.WebParts;
using System.Diagnostics.Contracts;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Manager,Accounting,Stocker")]
    public class InOutsController : Controller
    {
        private NHModel db = new NHModel();

        [Authorize(Roles = "SuperAdmin,Manager,Accounting")]
        public ActionResult Index(string Search_Data, int? s_account_id, int? s_type, int? s_category_id, int? s_currency, string Filter_Value, int? Page_No, string fromdate, string todate)
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
            ViewBag.category_id = s_category_id;
            ViewBag.account_id = s_account_id;

            ViewBag.FROMDATE = fromdate;
            ViewBag.TODATE = todate;
            DateTime FromDate = string.IsNullOrEmpty(fromdate) ? new DateTime() : DateTime.Parse(fromdate);
            DateTime ToDate = string.IsNullOrEmpty(todate) ? DateTime.Now : DateTime.Parse(todate);
            ToDate = ToDate.AddDays(1);
            ViewBag.type = s_type;

            var results = from v in db.vInOuts
                          where (
                          (string.IsNullOrEmpty(v.note) || v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper())) &&
                          v.category_id == (s_category_id.HasValue ? s_category_id.Value : v.category_id) &&
                          v.type == (s_type.HasValue ? s_type.Value : v.type) &&
                          v.account_id == (s_account_id.HasValue ? s_account_id.Value : v.account_id) &&
                          v.currency == (s_currency.HasValue ? s_currency.Value : v.currency) &&
                          v.status != (int)XInOut.eStatus.Huy &&
                          v.time >= FromDate &&
                          v.time < ToDate
                          )
                          orderby v.status, v.time descending
                          select v;

            var Summery = results.Where(x => x.status == (int)XInOut.eStatus.DaThucHien).Select(x => new { x.amount, x.type, x.currency, x.category_id }).ToArray();
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
            var ChiNganHang = Summery.Where(x => x.type == (int)XCategory.eType.Chi && x.currency == (int)XModels.eLoaiTien.NganHang).Sum(x => x.amount);
            var ThuNganHang = Summery.Where(x => x.type == (int)XCategory.eType.Thu && x.currency == (int)XModels.eLoaiTien.NganHang).Sum(x => x.amount);
            var NganHang = ThuNganHang - ChiNganHang;
            ViewBag.NGANHANG = NganHang.ToString("N0");
            ViewBag.TIENMAT = (thuchi - NganHang).ToString("N0");

            // Biểu đồ
            //doughnut[] chart = new doughnut[XCategory.sNhom.Count()];
            var lstCategories = db.Categories.Where(x=>x.status == (int)XModels.eStatus.Processing).ToArray();
            var chart = "[";
            foreach (var item in XCategory.sNhom.Where(x => x.Key != (int)XCategory.eNhom.Thu))
            {
                var lstCateIDs = lstCategories.Where(x => x.nhom == item.Key).Select(x => x.id).ToArray();
                var per = Summery.Where(x => lstCateIDs.Contains(x.category_id)).Sum(x => x.amount);
                chart += "{y:" + per + ",label:'" + item.Value + "'},";
            }
            chart += "]";
            //ViewBag.CHART = new HtmlString(JsonConvert.SerializeObject(chart, Formatting.None));
            ViewBag.CHART = new HtmlString(chart);
            int Size_Of_Page = 30;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        public JsonResult GetDoughnutChart(string chart)
        {
            return Json(JsonConvert.DeserializeObject(chart), JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin,Manager,Accounting")]
        public JsonResult BanInThuChi(string Search_Data, int? account_id, int? type, int? category_id, int? currency, string fromdate, string todate)
        {
            DateTime FromDate = string.IsNullOrEmpty(fromdate) ? new DateTime() : DateTime.Parse(fromdate);
            DateTime ToDate = string.IsNullOrEmpty(todate) ? DateTime.Now : DateTime.Parse(todate);
            ToDate = ToDate.AddDays(1);
            var results = (from v in db.vInOuts
                           where (
                           (string.IsNullOrEmpty(v.note) || v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper())) &&
                           v.category_id == (category_id.HasValue ? category_id.Value : v.category_id) &&
                           v.type == (type.HasValue ? type.Value : v.type) &&
                           v.account_id == (account_id.HasValue ? account_id.Value : v.account_id) &&
                           v.currency == (currency.HasValue ? currency.Value : v.currency) &&
                           v.status != (int)XInOut.eStatus.Huy &&
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
                LOAICHITIET = x.name,
                LYDO = x.note,
                SOTIEN = x.amount.ToString("N0"),
                TAIKHOAN = x.disname,
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

        // GET: InOuts/Details/5
        [Authorize(Roles = "SuperAdmin,Manager,Accounting")]
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InOut inOut = db.InOuts.Find(id);
            if (inOut == null)
            {
                return HttpNotFound();
            }
            XInOut model = new XInOut();
            model.account = db.Accounts.Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.fullname }).ToArray();
            model.types = XCategory.sType.Select(x => new SelectListItem { Value = x.Key.ToString(), Text = x.Value }).ToArray();
            model.categories = db.Categories.Where(x => x.type == inOut.type).Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.name }).ToArray();
            model.amount = inOut.amount;
            model.obj = inOut;
            var lst = db.wf_inout.Where(x => x.kid == inOut.id).ToArray();
            ViewBag.lst = lst;
            ViewBag.db = db;
            return View(model);
        }

        // GET: InOuts/Create
        [Authorize(Roles = "SuperAdmin,Accounting")]
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
        public ActionResult CreateX()
        {
            return View();
        }
        [Authorize(Roles = "SuperAdmin,Accounting")]
        public ActionResult CreateContract(int contract_id, int? category_id, bool? layout)
        {
            var contract = db.Contracts.Find(contract_id);
            InOut model = new InOut();
            model.contract_id = contract_id;
            model.account_id = contract.account_id;

            if (category_id.HasValue)
            {
                var category = db.Categories.Find(category_id);
                model.category_id = category_id.Value;
                model.type = category.type;
                switch (category.kind)
                {
                    case (int)XCategory.eKind.Partner:
                        model.amount = contract.outrose;
                        break;
                    case (int)XCategory.eKind.Rose:
                        model.amount = contract.rose;
                        break;
                    case (int)XCategory.eKind.Dove:
                        model.amount = contract.dove;
                        break;
                    case (int)XCategory.eKind.Remunerate:
                        model.amount = 1000000;
                        break;
                    default: break;
                }
            }
            return View(model);
        }
        [Authorize(Roles = "SuperAdmin,Accounting")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateContract(InOut inOut)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var cate = db.Categories.Find(inOut.category_id);
            var contract = db.Contracts.Find(inOut.contract_id);

            inOut.created_by = us.id;
            inOut.code = Utils.SetCodeInout(cate);
            inOut.time = DateTime.Now;

            inOut.note += " (" + contract.name + ")";
            if (cate.type == (int)XCategory.eType.Thu)
            {
                inOut.status = (int)XInOut.eStatus.DaDuyet;
            }
            else if (cate.duyet == (int)XCategory.eDuyet.Co)
            {
                inOut.status = (int)XInOut.eStatus.ChoDuyet;
            }
            else
            {
                inOut.status = (int)XInOut.eStatus.DaDuyet;
            }
            NHTrans.SaveLog(db, us.id, "THU CHI", "Thêm khoản thu chi trong hợp đồng " + contract.name);
            db.InOuts.Add(inOut);
            db.SaveChanges();
            if (cate.wf == (int)XCategory.eWf.Co)
            {
                var wf = new wf_inout();
                wf.account_id = us.id;
                wf.kid = inOut.id;
                wf.note = "Khởi tạo";
                wf.status = (int)XInOut.eStatus.DaThucHien;
                wf.time = DateTime.Now;
                db.wf_inout.Add(wf);
                db.SaveChanges();
            }
            if (inOut.status == (int)XInOut.eStatus.DaDuyet)
            {
                if (cate.kind == (int)XCategory.eKind.Dove)
                {
                    var thuchi = new Dove_ThuChi();
                    thuchi.account_id = us.id;
                    thuchi.created_by = us.id;
                    thuchi.time = DateTime.Now;
                    thuchi.amount = inOut.amount;
                    thuchi.type = (int)XCategory.eType.Thu;
                    thuchi.contract_id = inOut.contract_id;
                    thuchi.note = inOut.note + "(Tự động)";
                    db.Dove_ThuChi.Add(thuchi);
                    db.SaveChanges();

                    var quy = new QuyDove();
                    quy.account_id = us.id;
                    quy.time = DateTime.Now;
                    quy.amount = inOut.amount;
                    quy.type = (int)XCategory.eType.Thu;
                    if (inOut.contract_id.HasValue)
                    {
                        quy.note = inOut.note + " (" + contract.name + ")";
                    }
                    else
                        quy.note = inOut.note;
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
            }
            ViewBag.CLOSE = 1;
            inOut.amount = 0;
            inOut.note = "";
            return View(inOut);
        }
        [Authorize(Roles = "SuperAdmin,Accounting")]
        public JsonResult TrinhDuyet(int contract_id, int category_id)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var contract = db.Contracts.Find(contract_id);
            var cate = db.Categories.Find(category_id);
            var inOut = new InOut();
            inOut.contract_id = contract_id;
            inOut.category_id = category_id;
            inOut.type = cate.type;
            inOut.created_by = us.id;
            inOut.code = Utils.SetCodeInout(cate);
            inOut.time = DateTime.Now;
            inOut.note = cate.name + " (" + contract.name + ")";
            if (cate.duyet == (int)XCategory.eDuyet.Co)
            {
                inOut.status = (int)XInOut.eStatus.ChoDuyet;
            }
            else
            {
                inOut.status = (int)XInOut.eStatus.DaDuyet;
            }
            var partner = db.Partners.Find(contract.partner);
            switch (cate.kind)
            {
                case (int)XCategory.eKind.Partner:
                    inOut.account_id = partner.account_id;
                    var catePartner = db.Categories.Single(x => x.kind == (int)XCategory.eKind.Partner);
                    var dachi = db.InOuts.Where(x => x.contract_id == contract_id && x.category_id == catePartner.id && x.status < (int)XInOut.eStatus.Huy);
                    if (dachi.Count() > 0)
                    {
                        var sum = dachi.Sum(x => x.amount);
                        inOut.amount = contract.outrose - sum;
                    }
                    else
                    {
                        inOut.amount = contract.outrose;
                    }
                    break;
                case (int)XCategory.eKind.Rose:
                    inOut.account_id = partner.account_id;
                    inOut.amount = contract.rose;
                    break;
                case (int)XCategory.eKind.Dove:
                    inOut.account_id = 14; // Chi chỉ định anh Kiên
                    inOut.amount = contract.dove;
                    break;
                case (int)XCategory.eKind.Remunerate:
                    var hoso = db.HoSoes.Where(x => x.contract_id == contract_id && x.service_id == contract.service_id).FirstOrDefault();
                    if (hoso != null)
                    {
                        inOut.account_id = hoso.account_id;
                    }
                    inOut.amount = 1000000;
                    break;
                default:
                    inOut.account_id = us.id;
                    break;
            }
            NHTrans.SaveLog(db, us.id, "THU CHI", "Thêm khoản thu chi trong hợp đồng " + contract.name);
            db.InOuts.Add(inOut);
            db.SaveChanges();
            if (cate.wf == (int)XCategory.eWf.Co)
            {
                var wf = new wf_inout();
                wf.account_id = us.id;
                wf.kid = inOut.id;
                wf.note = "Khởi tạo";
                wf.status = (int)XInOut.eStatus.DaThucHien;
                wf.time = DateTime.Now;
                db.wf_inout.Add(wf);
                db.SaveChanges();
            }
            return Json(inOut.id, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin,Accounting")]
        public ActionResult TrinhDuyetChi(xTemp temp)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            //var temp = new xTemp() { int1 = item.category_id, int2 = Model.id, long1 = item.amount, str1 = item.name, int3 = item.id };
            var contract = db.Contracts.Find(temp.int2);
            var cate = db.Categories.Find(temp.int1);
            var inOut = new InOut();
            inOut.contract_id = contract.id;
            inOut.category_id = cate.id;
            inOut.type = cate.type;
            inOut.created_by = us.id;
            inOut.code = Utils.SetCodeInout(cate);
            inOut.time = DateTime.Now;
            inOut.note = cate.name + " (" + contract.name + ")";
            inOut.amount = temp.long1;
            if (cate.duyet == (int)XCategory.eDuyet.Co)
            {
                inOut.status = (int)XInOut.eStatus.ChoDuyet;
            }
            else
            {
                inOut.status = (int)XInOut.eStatus.DaDuyet;
            }
            inOut.account_id = temp.int4;
            NHTrans.SaveLog(db, us.id, "THU CHI", "Thêm khoản thu chi trong hợp đồng " + contract.name);
            db.InOuts.Add(inOut);
            db.SaveChanges();
            if (cate.wf == (int)XCategory.eWf.Co)
            {
                var wf = new wf_inout();
                wf.account_id = us.id;
                wf.kid = inOut.id;
                wf.note = "Khởi tạo";
                wf.status = (int)XInOut.eStatus.DaThucHien;
                wf.time = DateTime.Now;
                db.wf_inout.Add(wf);
                db.SaveChanges();
            }
            return RedirectToAction("trinhduyetchi", "contracts", new { id = temp.int2 });
        }
        // POST: InOuts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "SuperAdmin,Accounting")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(XInOut inOut)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var cate = db.Categories.Find(inOut.obj.category_id);

            inOut.obj.created_by = us.id;
            inOut.obj.amount = inOut.amount;
            inOut.obj.code = Utils.SetCodeInout(cate);
            if (ModelState.IsValid)
            {
                if (inOut.obj.contract_id.HasValue)
                {
                    var con = db.Contracts.Find(inOut.obj.contract_id);
                    inOut.obj.note += " (" + con.name + ")";
                }
                NHTrans.SaveLog(db, us.id, "THU CHI", "Thêm khoản thu chi ngoài hợp đồng");
                db.InOuts.Add(inOut.obj);
                db.SaveChanges();
                if (cate.wf == (int)XCategory.eWf.Co)
                {
                    var wf = new wf_inout();
                    wf.account_id = us.id;
                    wf.kid = inOut.obj.id;
                    wf.note = "Khởi tạo";
                    wf.status = (int)XInOut.eStatus.DaThucHien;
                    wf.time = DateTime.Now;
                    inOut.obj.status = (int)XInOut.eStatus.DaThucHien;
                    db.wf_inout.Add(wf);
                    db.SaveChanges();
                }
                if (cate.kind == (int)XCategory.eKind.Quy)
                {
                    var quy = new thuchi();
                    quy.account_id = inOut.obj.account_id;
                    quy.thoigian = DateTime.Now;
                    quy.sotien = inOut.amount;
                    quy.loai = (int)XCategory.eType.Thu;
                    quy.lydo = inOut.obj.note;
                    quy.trangthai = (int)XModels.eStatus.Complete;
                    db.thuchis.Add(quy);
                    db.SaveChanges();
                }
                if (cate.kind == (int)XCategory.eKind.Dove)
                {
                    var quy = new QuyDove();
                    quy.account_id = us.id;
                    quy.time = DateTime.Now;
                    quy.amount = inOut.amount;
                    quy.type = (int)XCategory.eType.Thu;
                    if (inOut.obj.contract_id.HasValue)
                    {
                        var con = db.Contracts.Find(inOut.obj.contract_id);
                        quy.note = inOut.obj.note + " (" + con.name + ")";
                    }
                    else
                        quy.note = inOut.obj.note;
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
                if (inOut.obj.contract_id.HasValue)
                    return RedirectToAction("Index", "Contracts");
                else
                    return RedirectToAction("Create");
            }
            return View(inOut);
        }
        [Authorize(Roles = "SuperAdmin,Accounting")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(InOut inOut)
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
            inOut.code = Utils.SetCodeInout(cate);

            if (cate.type == (int)XCategory.eType.Thu)
            {
                inOut.status = (int)XInOut.eStatus.DaDuyet;
            }
            else if (cate.duyet == (int)XCategory.eDuyet.Co)
            {
                inOut.status = (int)XInOut.eStatus.ChoDuyet;
            }
            else
            {
                inOut.status = (int)XInOut.eStatus.DaDuyet;
            }
            db.InOuts.Add(inOut);
            db.SaveChanges();
            if (cate.wf == (int)XCategory.eWf.Co)
            {
                var wf = new wf_inout();
                wf.account_id = us.id;
                wf.kid = inOut.id;
                wf.note = "Khởi tạo";
                wf.status = (int)XInOut.eStatus.DaThucHien;
                wf.time = DateTime.Now;
                db.wf_inout.Add(wf);
                db.SaveChanges();
            }
            if (cate.duyet == (int)XCategory.eDuyet.Khong)
            {
                if (cate.kind == (int)XCategory.eKind.Quy)
                {
                    var quy = new thuchi();
                    quy.account_id = inOut.account_id;
                    quy.thoigian = DateTime.Now;
                    quy.sotien = inOut.amount;
                    quy.loai = (int)XCategory.eType.Thu;
                    quy.lydo = inOut.note;
                    quy.trangthai = (int)XModels.eStatus.Complete;
                    db.thuchis.Add(quy);
                    db.SaveChanges();
                }
            }
            if (inOut.status == (int)XInOut.eStatus.DaDuyet)
            {
                if (cate.kind == (int)XCategory.eKind.Dove)
                {
                    var thuchi = new Dove_ThuChi();
                    thuchi.account_id = us.id;
                    thuchi.created_by = us.id;
                    thuchi.time = DateTime.Now;
                    thuchi.amount = inOut.amount;
                    thuchi.type = (int)XCategory.eType.Thu;
                    thuchi.contract_id = inOut.contract_id;
                    thuchi.note = inOut.note + "(Tự động)";
                    db.Dove_ThuChi.Add(thuchi);
                    db.SaveChanges();

                    var quy = new QuyDove();
                    quy.account_id = us.id;
                    quy.time = DateTime.Now;
                    quy.amount = inOut.amount;
                    quy.type = (int)XCategory.eType.Thu;
                    if (inOut.contract_id.HasValue)
                    {
                        quy.note = inOut.note;
                    }
                    else
                        quy.note = inOut.note;
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
            }
            // Loại chuyển đổi tiền
            if (cate.pair == (int)XCategory.ePair.Exchange)
            {
                var pairCate = db.Categories.Where(x => x.pair == (int)XCategory.ePair.Exchange && x.type != cate.type).Single();
                var currency = inOut.currency == (int)XModels.eLoaiTien.TienMat ? (int)XModels.eLoaiTien.NganHang : (int)XModels.eLoaiTien.TienMat;
                var type = inOut.type == (int)XCategory.eType.Thu ? (int)XCategory.eType.Chi : (int)XCategory.eType.Thu;
                var thuchi = new InOut();
                thuchi.account_id = inOut.account_id;
                thuchi.amount = inOut.amount;
                thuchi.category_id = pairCate.id;
                thuchi.code = inOut.code;
                thuchi.contract_id = inOut.contract_id;
                thuchi.created_by = inOut.created_by;
                thuchi.currency = currency;
                thuchi.gostock = inOut.gostock;
                thuchi.inoutchoduyet_id = inOut.inoutchoduyet_id;
                thuchi.note = inOut.note;
                thuchi.state = inOut.state;
                thuchi.status = inOut.status;
                thuchi.time = inOut.time;
                thuchi.type = type;
                thuchi.unlock = inOut.unlock;
                thuchi.xclass = inOut.xclass;
                db.InOuts.Add(thuchi);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        // GET: InOuts/Edit/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InOut inOut = db.InOuts.Find(id);
            if (inOut == null || inOut.state.HasValue)
            {
                return HttpNotFound();
            }
            var wf = db.wf_inout.Where(x => x.kid == id).OrderByDescending(x => x.time).FirstOrDefault();
            if (wf != null && wf.status != (int)XInOut.eStatus.DaThucHien)
            {
                ModelState.AddModelError("Error", "Bản ghi đã hoàn tất không được phép sửa, xóa!");
            }
            XInOut model = new XInOut();
            model.account = db.Accounts.Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.fullname }).ToArray();
            model.types = XCategory.sType.Select(x => new SelectListItem { Value = x.Key.ToString(), Text = x.Value }).ToArray();
            model.categories = db.Categories.Where(x => x.type == inOut.type && x.status == (int)XModels.eStatus.Processing).Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.name }).ToArray();
            model.amount = inOut.amount;
            model.obj = inOut;
            return View(model);
        }
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Duyet(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InOut inOut = db.InOuts.Find(id);
            if (inOut == null)
            {
                return HttpNotFound();
            }
            inOut.status = (int)XInOut.eStatus.DaDuyet;

            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            wf_inout wf = new wf_inout();
            wf.account_id = us.id;
            wf.intout_id = inOut.id;
            wf.kid = inOut.id;
            wf.note = "Duyệt khoản chi";
            wf.status = (int)XModels.eStatus.Complete;
            wf.time = DateTime.Now;
            db.wf_inout.Add(wf);
            NHTrans.SaveLog(db, us.id, "THU CHI", "Duyệt thu chi (" + inOut.note + ")");
            var cate = db.Categories.Find(inOut.category_id);
            if (cate.kind == (int)XCategory.eKind.Dove)
            {
                var thuchi = new Dove_ThuChi();
                thuchi.account_id = us.id;
                thuchi.created_by = us.id;
                thuchi.time = DateTime.Now;
                thuchi.amount = inOut.amount;
                thuchi.type = (int)XCategory.eType.Thu;
                thuchi.contract_id = inOut.contract_id;
                thuchi.note = inOut.note + "(Tự động)";
                db.Dove_ThuChi.Add(thuchi);
            }
            db.SaveChanges();
            return RedirectToAction("ChoDuyet", "InOutChoDuyets");
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public JsonResult DuyetAll(long[] ids)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            foreach (var id in ids)
            {
                InOut inOut = db.InOuts.Find(id);
                if (inOut == null || inOut.status != (int)XInOut.eStatus.ChoDuyet)
                {
                    continue;
                }
                inOut.status = (int)XInOut.eStatus.DaDuyet;
                wf_inout wf = new wf_inout();
                wf.account_id = us.id;
                wf.intout_id = inOut.id;
                wf.kid = inOut.id;
                wf.note = "Duyệt khoản chi";
                wf.status = (int)XModels.eStatus.Complete;
                wf.time = DateTime.Now;
                db.wf_inout.Add(wf);
                var cate = db.Categories.Find(inOut.category_id);
                if (cate.kind == (int)XCategory.eKind.Dove)
                {
                    var thuchi = new Dove_ThuChi();
                    thuchi.account_id = us.id;
                    thuchi.created_by = us.id;
                    thuchi.time = DateTime.Now;
                    thuchi.amount = inOut.amount;
                    thuchi.type = (int)XCategory.eType.Thu;
                    thuchi.contract_id = inOut.contract_id;
                    thuchi.note = inOut.note + "(Tự động)";
                    db.Dove_ThuChi.Add(thuchi);
                }
            }
            NHTrans.SaveLog(db, us.id, "THU CHI", "Duyệt nhanh thu chi");
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult ChuyenXemXet(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InOut inOut = db.InOuts.Find(id);
            if (inOut == null)
            {
                return HttpNotFound();
            }
            inOut.unlock = 1;

            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            wf_inout wf = new wf_inout();
            wf.account_id = us.id;
            wf.intout_id = inOut.id;
            wf.kid = inOut.id;
            wf.note = "Chuyển xem xét khoản chi";
            wf.status = (int)XModels.eStatus.Complete;
            wf.time = DateTime.Now;
            db.wf_inout.Add(wf);
            db.SaveChanges();
            NHTrans.SaveLog(db, us.id, "THU CHI", "Chuyển xem xét khoản thu chi (" + inOut.note + ")");
            return RedirectToAction("ChoDuyet", "InOutChoDuyets");
        }
        // POST: InOuts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(XInOut inOut)
        {
            if (ModelState.IsValid)
            {
                inOut.obj.amount = inOut.amount;
                db.Entry(inOut.obj).State = EntityState.Modified;
                var username = User.Identity.GetUserName();
                var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
                var cate = db.Categories.Find(inOut.obj.category_id);
                if (cate.wf == (int)XCategory.eWf.Co)
                {
                    var wf = new wf_inout();
                    wf.account_id = us.id;
                    wf.kid = inOut.obj.id;
                    wf.note = "Chỉnh sửa";
                    wf.status = (int)XInOut.eStatus.DaThucHien;
                    wf.time = DateTime.Now;
                    db.wf_inout.Add(wf);
                }
                NHTrans.SaveLog(db, us.id, "THU CHI", "Chỉnh sửa thu chi (" + inOut.obj.note + ")");
                db.SaveChanges();
                if (inOut.obj.contract_id.HasValue)
                    return RedirectToAction("Details", "Contracts", new { id = inOut.obj.contract_id });
                else
                    return RedirectToAction("Index");
            }
            return View(inOut);
        }
        // GET: InOuts/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InOut inOut = db.InOuts.Find(id);
            if (inOut == null)
            {
                return HttpNotFound();
            }
            var wf = db.wf_inout.Where(x => x.kid == id).OrderByDescending(x => x.time).FirstOrDefault();
            if (wf != null && wf.status != (int)XInOut.eStatus.DaThucHien)
            {
                throw new Exception("Bản ghi đã hoàn tất không được phép sửa, xóa!");
            }
            XInOut model = new XInOut();
            model.account = db.Accounts.Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.fullname }).ToArray();
            model.types = XCategory.sType.Select(x => new SelectListItem { Value = x.Key.ToString(), Text = x.Value }).ToArray();
            model.categories = db.Categories.Where(x => x.type == inOut.type).Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.name }).ToArray();
            model.amount = inOut.amount;
            model.obj = inOut;
            return View(model);
        }
        // POST: InOuts/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            InOut inOut = db.InOuts.Find(id);
            //Money money = db.Moneys.Where(x => x.code.Equals(inOut.money_code)).SingleOrDefault();
            //var wfs = db.wf_inout.Where(x => x.kid == inOut.id);
            //db.Moneys.Remove(money);
            //db.InOuts.Remove(inOut);
            inOut.status = (int)XInOut.eStatus.Huy;
            wf_inout wf = new wf_inout();
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            wf.account_id = us.id;
            wf.kid = id;
            wf.time = DateTime.Now;
            wf.status = (int)XModels.eStatus.Complete;
            wf.intout_id = id;
            wf.note = "Hủy do bị xóa";
            db.wf_inout.Add(wf);
            //db.wf_inout.RemoveRange(wfs);
            NHTrans.SaveLog(db, us.id, "THU CHI", "Hủy khoản thu chi (" + inOut.note + ")");
            db.SaveChanges();
            if (inOut.contract_id.HasValue)
                return RedirectToAction("Details", "Contracts", new { id = inOut.contract_id });
            else
                return RedirectToAction("Index");
        }
        [Authorize(Roles = "SuperAdmin")]
        public JsonResult JDeleteConfirmed(long id)
        {
            InOut inOut = db.InOuts.Find(id);
            inOut.status = (int)XInOut.eStatus.Huy;
            wf_inout wf = new wf_inout();
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            wf.account_id = us.id;
            wf.kid = id;
            wf.time = DateTime.Now;
            wf.status = (int)XModels.eStatus.Complete;
            wf.intout_id = id;
            wf.note = "Hủy do bị xóa";
            db.wf_inout.Add(wf);
            NHTrans.SaveLog(db, us.id, "THU CHI", "Hủy khoản thu chi (" + inOut.note + ")");
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [Authorize(Roles = "SuperAdmin,Manager")]
        public ActionResult CongNo(string Search_Data, string account_id, string type, string Filter_Value, string txtIsIn, string fromdate, string todate)
        {
            if (Search_Data == null)
            {
                Search_Data = Filter_Value;
            }
            ViewBag.FilterValue = Search_Data;
            ViewBag.account_id = account_id;
            ViewBag.type = type;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.FROMDATE = fromdate;
            ViewBag.TODATE = todate;
            DateTime FromDate = string.IsNullOrEmpty(fromdate) ? new DateTime() : DateTime.Parse(fromdate);
            DateTime ToDate = string.IsNullOrEmpty(todate) ? DateTime.Now : DateTime.Parse(todate);
            ToDate = ToDate.AddDays(1);

            IQueryable<InOut> results;
            int? it = null, ic = null, acc = null;
            if (!string.IsNullOrEmpty(type))
            {
                it = int.Parse(type);
                ViewBag.Categories = db.Categories.Where(x => x.type == it.Value && x.status == (int)XModels.eStatus.Processing).Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.name }).ToArray();
            }
            if (!string.IsNullOrEmpty(account_id))
            {
                acc = int.Parse(account_id);
            }
            results = from io in db.InOuts
                      join c in db.Categories on io.category_id equals c.id
                      where (
                      c.pair == (int)XCategory.ePair.VayTra &&
                      ((string.IsNullOrEmpty(io.note) || io.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.note.ToUpper())) &&
                      io.category_id == (ic.HasValue ? ic.Value : io.category_id) &&
                      io.type == (it.HasValue ? it.Value : io.type) &&
                      io.account_id == (acc.HasValue ? acc.Value : io.account_id) &&
                      io.time >= FromDate &&
                      io.time < ToDate
                      ))
                      orderby io.time descending
                      select io;

            ViewBag.db = db;
            ViewBag.Accounts = new SelectList(db.Accounts, "id", "fullname");

            var sum = (from rs in results
                       where rs.status == (int)XInOut.eStatus.DaThucHien
                       select new { rs.amount, rs.type, rs.time, rs.note }).ToArray();
            var thu = sum.Where(x => x.type == (int)XCategory.eType.Thu).Sum(x => x.amount);
            var chi = sum.Where(x => x.type == (int)XCategory.eType.Chi).Sum(x => x.amount);
            ViewBag.THU = thu;
            ViewBag.CHI = chi;

            if (!string.IsNullOrEmpty(txtIsIn))
            {
                ViewBag.Print = 1;
                string TAIKHOAN = "TÀI KHOẢN: ";
                if (!string.IsNullOrEmpty(account_id))
                {
                    Account account = db.Accounts.Find(Convert.ToInt32(account_id));
                    TAIKHOAN += account.fullname;
                }
                string NGAYTHANG = DateTime.Now.ToString("dd/MM/yyyy");

                string path = Server.MapPath("~/public/");
                string pathTemp = Server.MapPath("~/App_Data/templates/CongNo.xls");
                string webpart = "CongNo" + ".xls";
                FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
                using (FileStream fs = System.IO.File.Create(path + webpart))
                {
                    using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                    {
                        flexCelReport.SetValue("NGAYTHANG", NGAYTHANG);
                        flexCelReport.SetValue("TAIKHOAN", TAIKHOAN);

                        flexCelReport.SetValue("THU", thu.ToString("N0"));
                        flexCelReport.SetValue("CHI", chi.ToString("N0"));

                        flexCelReport.SetValue("TONGTIEN", (thu - chi).ToString("N0"));
                        flexCelReport.AddTable("TB", sum.Select(x => new { THOIGIAN = x.time.ToString("dd/MM/yyyy"), SOTIEN = x.amount.ToString("N0"), LYDO = x.note, LOAI = XCategory.sType[x.type] }));
                        flexCelReport.Run(sr, fs);
                    }
                }
                ViewBag.URL = "/public/" + webpart;
            }
            else
                ViewBag.Print = 0;
            return View(results.ToList());
        }
        [Authorize(Roles = "SuperAdmin,Accounting")]
        public ActionResult GoStock()
        {
            var results = db.vInOuts.Where(x => x.gostock == (int)XModels.eLevel.Level1 && x.status != (int)XInOut.eStatus.Huy);
            var lsttongthu = results.Where(x => x.type == (int)XCategory.eType.Thu);
            long tongthu = 0;
            if (lsttongthu.Count() > 0)
                tongthu = lsttongthu.Sum(x => x.amount);
            ViewBag.TONGTHU = tongthu.ToString("N0");

            long tongchi = 0;
            var lsttongchi = results.Where(x => x.type == (int)XCategory.eType.Chi);
            if (lsttongchi.Count() > 0)
                tongchi = lsttongchi.Sum(x => x.amount);
            ViewBag.TONGCHI = tongchi.ToString("N0");
            var thuchi = tongthu - tongchi;
            ViewBag.THUCHI = thuchi.ToString("N0");
            return View(results.ToList());
        }
        [Authorize(Roles = "SuperAdmin,Accounting,Stocker")]
        public ActionResult GoAsyn()
        {
            var results = db.vInOuts.Where(x => x.gostock == (int)XModels.eLevel.Level2 && x.status != (int)XInOut.eStatus.Huy);
            var lsttongthu = results.Where(x => x.type == (int)XCategory.eType.Thu);
            long tongthu = 0;
            if (lsttongthu.Count() > 0)
                tongthu = lsttongthu.Sum(x => x.amount);
            ViewBag.TONGTHU = tongthu.ToString("N0");

            long tongchi = 0;
            var lsttongchi = results.Where(x => x.type == (int)XCategory.eType.Chi);
            if (lsttongchi.Count() > 0)
                tongchi = lsttongchi.Sum(x => x.amount);
            ViewBag.TONGCHI = tongchi.ToString("N0");
            var thuchi = tongthu - tongchi;
            ViewBag.THUCHI = thuchi.ToString("N0");
            return View(results.ToList());
        }
        public JsonResult GoStockPrint()
        {
            var results = db.vInOuts.Where(x => x.gostock == (int)XModels.eLevel.Level1 && x.status != (int)XInOut.eStatus.Huy);
            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/BanInGoStock.xls");
            string webpart = "BanInGoStock_" + DateTime.Now.ToString("ddMMyy") + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            var rs = new List<cBanInThuChi>();
            foreach (var x in results)
            {
                var item = new cBanInThuChi()
                {
                    LOAI = XCategory.sType[x.type],
                    LOAICHITIET = x.name,
                    LYDO = x.note,
                    SOTIEN = x.amount.ToString("N0"),
                    TAIKHOAN = x.disname,
                    THOIGIAN = x.time.ToString("dd/MM/yyyy")
                };
                rs.Add(item);
                var inout = db.InOuts.Find(x.id);
                inout.gostock = (int)XModels.eLevel.Level2;
            }
            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.AddTable("TB", rs);
                    flexCelReport.Run(sr, fs);
                }
            }
            db.SaveChanges();

            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GoAsynPrint()
        {
            var results = db.vInOuts.Where(x => x.gostock == (int)XModels.eLevel.Level2 && x.status != (int)XInOut.eStatus.Huy);
            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/BanInGoStock.xls");
            string webpart = "BanInGoAsyn_" + DateTime.Now.ToString("ddMMyy") + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            var rs = new List<cBanInThuChi>();
            foreach (var x in results)
            {
                var item = new cBanInThuChi()
                {
                    LOAI = XCategory.sType[x.type],
                    LOAICHITIET = x.name,
                    LYDO = x.note,
                    SOTIEN = x.amount.ToString("N0"),
                    TAIKHOAN = x.disname,
                    THOIGIAN = x.time.ToString("dd/MM/yyyy")
                };
                rs.Add(item);
            }
            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.AddTable("TB", rs);
                    flexCelReport.Run(sr, fs);
                }
            }
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin,Accounting,Stocker")]
        public ActionResult InOutDetail(int contract_id, int type)
        {
            var results = db.vInOuts.Where(x => x.contract_id == contract_id && x.type == type);
            return View(results.ToList());
        }
        [Authorize(Roles = "SuperAdmin,Accounting,Stocker")]
        public ActionResult Cancel(int id, string ac, string con)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var inout = db.InOuts.Find(id);
            var cate = db.Categories.Find(inout.category_id);
            if (inout.status == (int)XInOut.eStatus.ChoDuyet)
            {
                inout.status = (int)XInOut.eStatus.Huy;
            }
            else if (inout.status == (int)XInOut.eStatus.DaDuyet)
            {
                if (cate.duyet == (int)XCategory.eDuyet.Co)
                {
                    inout.status = (int)XInOut.eStatus.ChoDuyet;
                }
                else
                {
                    inout.status = (int)XInOut.eStatus.Huy;
                }
            }
            db.SaveChanges();
            NHTrans.SaveLog(db, us.id, "THU CHI", "Từ chối khoản thu chi (" + inout.note + ")");
            return RedirectToAction(ac, con);
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
        [Authorize(Roles = "SuperAdmin,Manager")]
        public ActionResult BieuDoThuChi()
        {
            return View();
        }
        public ActionResult GetChartData(string fromDate, string toDate)
        {
            var data = new List<XChart>();
            bool isMonth = false;
            DateTime fdate = !string.IsNullOrEmpty(fromDate) ? Convert.ToDateTime(fromDate) : DateTime.Now.AddMonths(-10);
            DateTime tdate = !string.IsNullOrEmpty(toDate) ? Convert.ToDateTime(toDate) : DateTime.Now;
            var lstInOut = db.vChartInOuts.Where(x => x.time >= fdate && x.time <= tdate && x.type == (int)XCategory.eType.Chi && x.status != (int)XInOut.eStatus.Huy).ToArray();
            var days = (int)tdate.Subtract(fdate).TotalDays + 1;
            if (days > 60)
            {
                isMonth = true;
                fdate = fdate.AddDays(1 - fdate.Day);
                tdate = tdate.AddDays(1 - tdate.Day).AddMonths(1);
                days = (tdate.Year - fdate.Year) * 12 + (tdate.Month - fdate.Month);
            }
            foreach (var cate in db.Categories.Where(x => x.type == (int)XCategory.eType.Chi))
            {
                XChart xChart = new XChart();
                xChart.type = "spline";
                xChart.name = cate.name;
                xChart.axisYType = "primary";
                xChart.showInLegend = true;
                xChart.xValueFormatString = "DD/MM/YYYY";
                xChart.yValueFormatString = "#,##0 đ";
                var lst = lstInOut.Where(x => x.category_id == cate.id).ToArray();
                xChart.dataPoints = new XChartPoint[days];
                for (var i = 1; i <= days; i++)
                {
                    xChart.dataPoints[i - 1] = new XChartPoint();
                    var startTime = !isMonth ? fdate.AddDays(i - 1) : fdate.AddMonths(i - 1);
                    var endTime = !isMonth ? fdate.AddDays(i) : fdate.AddMonths(i);
                    var item = lst.Where(x => x.time >= startTime && x.time < endTime);
                    xChart.dataPoints[i - 1].x = startTime.ToString("yyyy-MM-" + i);
                    if (item.Count() > 0)
                    {
                        xChart.dataPoints[i - 1].y = item.Sum(x => x.amount);
                    }
                    else
                    {
                        xChart.dataPoints[i - 1].y = 0;
                    }
                }
                data.Add(xChart);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin,Accounting")]
        public ActionResult CreateCongNo()
        {
            return View();
        }
        [Authorize(Roles = "SuperAdmin,Accounting")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCongNo(InOut inOut)
        {
            var cate = db.Categories.Find(inOut.category_id);
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            inOut.created_by = us.id;
            inOut.type = cate.type;
            inOut.time = DateTime.Now;

            if (cate.duyet == (int)XCategory.eDuyet.Co)
            {
                inOut.status = (int)XInOut.eStatus.ChoDuyet;
            }
            else
            {
                inOut.status = (int)XInOut.eStatus.DaDuyet;
            }
            NHTrans.SaveLog(db, us.id, "THU CHI", "Thực hiện khoản cho vay (" + inOut.note + ")");
            db.InOuts.Add(inOut);
            db.SaveChanges();

            if (cate.wf == (int)XCategory.eWf.Co)
            {
                var wf = new wf_inout();
                wf.account_id = us.id;
                wf.kid = inOut.id;
                wf.note = XInOut.sStatus[inOut.status];
                wf.status = (int)XModels.eStatus.Processing;
                wf.time = DateTime.Now;
                db.wf_inout.Add(wf);
                db.SaveChanges();
            }
            return RedirectToAction("CongNo");
        }
        // Thêm action để lấy thông tin thu chi theo hợp đồng
        [HttpGet]
        public JsonResult GetContractFinancialInfo(int contractId)
        {
            try
            {
                var contract = db.Contracts.Find(contractId);
                if (contract == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy hợp đồng" }, JsonRequestBehavior.AllowGet);
                }

                // Lấy tổng thu từ hợp đồng
                var totalThu = db.vInOuts.Where(x => x.contract_id == contractId && x.type == (int)XCategory.eType.Thu)
                                         .Sum(x => (long?)x.amount) ?? 0;

                // Lấy tổng chi từ hợp đồng
                var totalChi = db.vInOuts.Where(x => x.contract_id == contractId && x.type == (int)XCategory.eType.Chi)
                                         .Sum(x => (long?)x.amount) ?? 0;

                // Lấy chi tiết các khoản chi theo loại
                var chiHoaHongDoiTac = db.vInOuts.Where(x => x.contract_id == contractId &&
                                                           x.type == (int)XCategory.eType.Chi &&
                                                           x.name.Contains("hoa hồng đối tác"))
                                                .Sum(x => (long?)x.amount) ?? 0;

                var chiHoaHongNhanVien = db.vInOuts.Where(x => x.contract_id == contractId &&
                                                             x.type == (int)XCategory.eType.Chi &&
                                                             x.name.Contains("hoa hồng nhân viên"))
                                                  .Sum(x => (long?)x.amount) ?? 0;

                var dmThucHien = db.vInOuts.Where(x => x.contract_id == contractId &&
                                                     x.type == (int)XCategory.eType.Chi &&
                                                     x.name.Contains("ĐM thực hiện"))
                                          .Sum(x => (long?)x.amount) ?? 0;

                var tienDoVe = db.vInOuts.Where(x => x.contract_id == contractId &&
                                                   x.type == (int)XCategory.eType.Chi &&
                                                   x.name.Contains("đo vẽ"))
                                         .Sum(x => (long?)x.amount) ?? 0;

                // Lấy danh sách thu chi trong hợp đồng (10 giao dịch gần nhất)
                var transactions = db.vInOuts.Where(x => x.contract_id == contractId)
                                            .OrderByDescending(x => x.time)
                                            .Take(10)
                                            .Select(x => new {
                                                id = x.id,
                                                time = x.time,
                                                type = x.type,
                                                typeName = x.type == (int)XCategory.eType.Thu ? "Thu" : "Chi",
                                                amount = x.amount,
                                                note = x.note,
                                                categoryName = x.name,
                                                status = x.status,
                                                statusName = x.status == (int)XInOut.eStatus.ChoDuyet ? "Chờ duyệt" :
                                                           x.status == (int)XInOut.eStatus.DaDuyet ? "Đã duyệt" : "Hoàn thành"
                                            }).ToList();

                var result = new
                {
                    success = true,
                    contractInfo = new
                    {
                        id = contract.id,
                        name = contract.name,
                        amount = contract.amount,
                        consuming = contract.consuming,
                        outrose = contract.outrose,
                        rose = contract.rose,
                        dove = contract.dove,
                        remunerate = contract.remunerate
                    },
                    summary = new
                    {
                        totalThu = totalThu,
                        totalChi = totalChi,
                        conLai = totalThu - totalChi,
                        chiHoaHongDoiTac = chiHoaHongDoiTac,
                        chiHoaHongNhanVien = chiHoaHongNhanVien,
                        dmThucHien = dmThucHien,
                        tienDoVe = tienDoVe
                    },
                    transactions = transactions
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
