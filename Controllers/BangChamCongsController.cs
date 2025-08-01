using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
using Aspose.Words.Lists;
using FlexCel.Report;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Manager,Accounting,Member")]
    public class BangChamCongsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: BangChamCongs
        [Authorize(Roles = "SuperAdmin,Accounting,Member")]
        public ActionResult Index(int? thang, int? nam)
        {
            thang = !thang.HasValue ? DateTime.Now.Month : thang;
            nam = !nam.HasValue ? DateTime.Now.Year : nam;
            ViewBag.THANG = thang;
            ViewBag.NAM = nam;

            ViewBag.Title = "Bảng chấm công tháng " + thang + " năm " + nam;

            var bcc = db.BangChamCongs.Where(x => x.thoigian.Month == thang && x.thoigian.Year == nam).ToList();
            ViewBag.SoNgayTrongThang = DateTime.DaysInMonth(nam.Value, thang.Value);

            ViewBag.db = db;
            foreach (var acc in db.Accounts.Where(x => x.sta == (int)XAccount.eStatus.Processing))
            {
                if (bcc.Count(x => x.account_id == acc.id) == 0)
                    bcc.Add(new BangChamCong() { account_id = acc.id });
            }
            return View(bcc);
        }
        [Authorize(Roles = "SuperAdmin,Accounting,Member")]
        public ActionResult BangLuong(int thang, int nam)
        {
            thang = thang == 0 ? DateTime.Now.Month : thang;
            nam = nam == 0 ? DateTime.Now.Year : nam;
            int songaylamviec = demsongaylamviec(thang, nam);
            var bcc = db.BangChamCongs.Where(x => x.thoigian.Month == thang && x.thoigian.Year == nam).ToList();
            var NgayThang = "Tháng " + thang + " năm " + nam;
            var tomonth = new DateTime(nam, thang, 1);

            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/BangLuong.xls");
            string webpart = "BangLuong_" + thang + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    List<BangLuong> lstBangLuongs = new List<BangLuong>();
                    List<Alter> lstTLs = new List<Alter>();
                    List<Alter> lstPCs = new List<Alter>();
                    List<Alter> lstTUs = new List<Alter>();

                    foreach (var acc in db.Accounts.Where(x => x.sta == (int)XAccount.eStatus.Processing))
                    {
                        BangLuong obj = new BangLuong();
                        obj.id = acc.id;
                        var TL = db.TruLuongs.Where(x => x.account_id == acc.id && x.thoigian.Month == thang && x.thoigian.Year == nam).ToList();
                        var PC = db.PhuCaps.Where(x => x.account_id == acc.id).ToList();

                        var catetamung = db.Categories.Where(x => x.type == (int)XCategory.eType.Chi && x.pair == (int)XCategory.ePair.TamUng).FirstOrDefault();
                        var ims = (from io in db.InOuts
                                   where io.account_id == acc.id &&
                                         io.category_id == catetamung.id &&
                                         io.status == (int)XInOut.eStatus.DaThucHien &&
                                         io.time >= tomonth
                                   select new InoutMoney() { code = io.code, ngaythang = io.time, amount = io.amount, note = io.note }).ToList();
                        var tamung = ims.Sum(x => x.amount);
                        obj.TamUng = tamung.ToString("N0");
                        var luong = acc.luong + PC.Sum(x => x.sotien);
                        obj.Luong = luong.ToString("N0");
                        var luongngay = luong / songaylamviec;
                        obj.LuongNgay = luongngay.ToString("N0");

                        double cong = 0, cophep = 0, khongphep = 0;
                        var congnhanvien = bcc.Where(x => x.account_id == acc.id);
                        foreach (var item in congnhanvien)
                        {
                            switch (item.cong)
                            {
                                case (int)XBangChamCong.eCong.NuaNgay:
                                    cong += 0.5;
                                    cophep += 0.5;
                                    break;
                                case (int)XBangChamCong.eCong.CaNgay:
                                    cong += 1;
                                    break;
                                case (int)XBangChamCong.eCong.CoPhep:
                                    cophep += 1;
                                    break;
                                case (int)XBangChamCong.eCong.KhongPhep:
                                    khongphep += 1;
                                    break;
                                default:
                                    break;
                            }
                        }
                        var LuongThuc = (acc.luong + PC.Where(x => x.loai == (int)XModels.eLoaiPhuCap.Loai1).Sum(x => x.sotien)) - ((cophep + khongphep) * luongngay);
                        obj.LuongThuc = LuongThuc.ToString();
                        double tong = 0;
                        obj.CoPhep = cophep;
                        obj.KhongPhep = khongphep;
                        obj.NgayNghi = cophep + khongphep;
                        obj.LuongThuc = LuongThuc.ToString("N0");
                        obj.NgayThang = "Tháng " + thang + " năm " + nam;
                        obj.HoTen = acc.fullname;
                        obj.LuongCoBan = acc.luong.ToString("N0");
                        obj.PhuCap = PC.Sum(x => x.sotien).ToString("N0");
                        obj.NgayTrongThang = songaylamviec;
                        obj.NgayLam = cong;
                        if (khongphep > 0)
                            TL.Add(new TruLuong() { account_id = acc.id, sotien = (int)(khongphep * luongngay), note = "Phạt (" + khongphep + ")ngày nghỉ không phép" });
                        if (acc.isNV == (int)XModels.eYesNo.Yes)
                            tong = LuongThuc - TL.Sum(x => x.sotien) - tamung;
                        else
                            tong = (acc.luong + PC.Where(x => x.loai == (int)XModels.eLoaiPhuCap.Loai1).Sum(x => x.sotien)) - TL.Sum(x => x.sotien) - tamung;
                        obj.Tong = tong.ToString("N0");
                        obj.ConLai = (int)tong;
                        obj.TL = TL.Select(x => new Alter() { ten = x.note, sotien = x.sotien.ToString("N0"), account_id = acc.id }).ToList();
                        obj.PC = PC.Select(x => new Alter() { ten = x.ten, sotien = x.sotien.ToString("N0"), account_id = acc.id }).ToList();

                        lstTUs.AddRange(ims.Select(x => new Alter() { ten = x.ngaythang.ToString("dd/MM/yyyy") + " " + x.note, sotien = x.amount.ToString("N0"), account_id = acc.id }).ToList());
                        lstBangLuongs.Add(obj);
                        lstPCs.AddRange(obj.PC);
                        obj.TruLuong = TL.Sum(x => x.sotien).ToString("N0");
                        lstTLs.AddRange(obj.TL);
                    }
                    flexCelReport.SetValue("NgayThang", NgayThang);
                    flexCelReport.AddTable("T", lstBangLuongs);
                    flexCelReport.AddTable("T1", lstBangLuongs);
                    flexCelReport.AddTable("PC", lstPCs);
                    flexCelReport.AddTable("TL", lstTLs);
                    flexCelReport.AddTable("TU", lstTUs);
                    flexCelReport.AddRelationship("T", "PC", "id", "account_id");
                    flexCelReport.AddRelationship("T", "TL", "id", "account_id");
                    flexCelReport.AddRelationship("T", "TU", "id", "account_id");

                    flexCelReport.Run(sr, fs);
                }
            }
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin,Accounting")]
        public ActionResult ChotLuong(int thang, int nam)
        {
            //thang = thang == 0 ? DateTime.Now.Month : thang;
            //nam = nam == 0 ? DateTime.Now.Year : nam;
            //var catetamung = db.Categories.Where(x => x.type == (int)XCategory.eType.Chi && x.pair == (int)XCategory.ePair.TamUng).FirstOrDefault();
            //var tus = db.InOuts.Where(x => x.time.Month == thang && x.time.Year == nam && x.category_id == catetamung.id && x.status != (int)XModels.eStatus.Complete).ToList();
            //foreach (var item in tus)
            //{
            //    var wfs = db.wf_inout.Where(x => x.kid == item.id);
            //    foreach (var per in wfs)
            //    {
            //        per.status = (int)XModels.eStatus.Complete;
            //    }
            //    item.status = (int)XModels.eStatus.Complete;
            //    item.note = "Hoàn tất do thanh toán lương";
            //}
            //db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public int demsongaylamviec(int thang, int nam)
        {
            return 26;
            int dem = 0;
            DateTime f = new DateTime(nam, thang, 01);
            int x = f.Month + 1;
            bool m12 = thang == 12 ? true : false;
            while (f.Month < x && (m12 ? f.Month > 1 : true))
            {
                dem = dem + 1;
                if (f.DayOfWeek == DayOfWeek.Sunday)
                {
                    dem = dem - 1;
                }
                f = f.AddDays(1);
            }
            return dem;
        }
        // GET: BangChamCongs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BangChamCong bangChamCong = db.BangChamCongs.Find(id);
            if (bangChamCong == null)
            {
                return HttpNotFound();
            }
            return View(bangChamCong);
        }

        // GET: BangChamCongs/Create
        public ActionResult Create()
        {
            var username = User.Identity.GetUserName();
            var acc = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var tomonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var lst = db.ChamCongs.Where(x => x.time > tomonth && x.user_id == acc.id).OrderByDescending(x => x.time).ToArray();
            var Model = new List<xChamCong>();
            if (lst.Length > 0)
            {
                var lastday = lst.First().time.Day;
                for (var i = 1; i <= lastday; i++)
                {
                    var startTime = tomonth.AddDays(i - tomonth.Day);
                    var endTime = tomonth.AddDays(i - tomonth.Day + 1);
                    var item = lst.Where(x => x.time > startTime && x.time < endTime).OrderBy(x => x.time);
                    if (item.Count() > 0)
                    {
                        var xchamcong = new xChamCong();
                        var first = item.FirstOrDefault();
                        if (first != null) xchamcong.start = first.time;
                        var last = item.LastOrDefault();
                        if (last != null) xchamcong.end = last.time;
                        Model.Add(xchamcong);
                    }
                }
            }
            return View(Model.OrderByDescending(x => x.start));
        }

        // POST: BangChamCongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChamCong chamCong)
        {
            var username = User.Identity.GetUserName();
            var acc = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var todays = db.ChamCongs.Where(x => x.user_id == acc.id && x.time > DateTime.Today);
            if (todays.Count() <= 1)
            {
                chamCong = new ChamCong();
                chamCong.user_id = acc.id;
                chamCong.time = DateTime.Now;
                db.ChamCongs.Add(chamCong);
            }
            else
            {
                chamCong = todays.OrderByDescending(x => x.time).First();
                chamCong.time = DateTime.Now;
            }
            db.SaveChanges();
            return RedirectToAction("Create");
        }

        // GET: BangChamCongs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BangChamCong bangChamCong = db.BangChamCongs.Find(id);
            if (bangChamCong == null)
            {
                return HttpNotFound();
            }
            return View(bangChamCong);
        }

        // POST: BangChamCongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,account_id,thoigian,note")] BangChamCong bangChamCong)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bangChamCong).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bangChamCong);
        }

        // GET: BangChamCongs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BangChamCong bangChamCong = db.BangChamCongs.Find(id);
            if (bangChamCong == null)
            {
                return HttpNotFound();
            }
            return View(bangChamCong);
        }

        // POST: BangChamCongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BangChamCong bangChamCong = db.BangChamCongs.Find(id);
            db.BangChamCongs.Remove(bangChamCong);
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
        public ActionResult ChamCong(int cong, int nhanvien, int ngay, int thang, int nam)
        {
            var bcc = db.BangChamCongs.Where(c => c.account_id == nhanvien && c.ngay == ngay && c.thoigian.Month == thang && c.thoigian.Year == nam).FirstOrDefault();
            if (bcc != null)
                bcc.cong = cong;
            else
            {
                bcc = new BangChamCong();
                bcc.ngay = ngay;
                bcc.cong = cong;
                //bcc.thoigian = DateTime.Now.AddDays(0 - DateTime.Now.Day).AddDays(ngay);
                bcc.thoigian = new DateTime(nam, thang, ngay);
                bcc.account_id = nhanvien;
                db.BangChamCongs.Add(bcc);
            }
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public ActionResult rptChamCong(int? thang, int? nam)
        {
            thang = !thang.HasValue ? DateTime.Now.Month : thang;
            nam = !nam.HasValue ? DateTime.Now.Year : nam;
            ViewBag.THANG = thang;
            ViewBag.NAM = nam;
            var username = User.Identity.GetUserName();
            var acc = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var tomonth = new DateTime(nam.Value, thang.Value, 1);
            var endmonth = thang.Value == 12 ? new DateTime(nam.Value + 1, 1, 1) : new DateTime(nam.Value, thang.Value + 1, 1);
            var lst = db.ChamCongs.Where(x => x.time > tomonth && x.time < endmonth && x.user_id == acc.id).OrderByDescending(x => x.time).ToArray();
            var Model = new List<xChamCong>();
            if (lst.Length > 0)
            {
                var lastday = lst.First().time.Day;
                for (var i = 1; i <= lastday; i++)
                {
                    var startTime = tomonth.AddDays(i - tomonth.Day);
                    var endTime = tomonth.AddDays(i - tomonth.Day + 1);
                    var item = lst.Where(x => x.time > startTime && x.time < endTime).OrderBy(x => x.time);
                    if (item.Count() > 0)
                    {
                        var xchamcong = new xChamCong();
                        var first = item.FirstOrDefault();
                        if (first != null) xchamcong.start = first.time;
                        var last = item.LastOrDefault();
                        if (last != null) xchamcong.end = last.time;
                        Model.Add(xchamcong);
                    }
                }
            }
            return View(Model);
        }
        public ActionResult rptChamCongGetChart(int? thang, int? nam)
        {
            thang = !thang.HasValue ? DateTime.Now.Month : thang;
            nam = !nam.HasValue ? DateTime.Now.Year : nam;
            ViewBag.THANG = thang;
            ViewBag.NAM = nam;
            var data = new List<XChart>();
            foreach (var acc in db.Accounts.Where(x => x.sta == (int)XAccount.eStatus.Processing))
            {
                XChart xChart = new XChart();
                xChart.type = "spline";
                xChart.name = acc.fullname;
                xChart.axisYType = "primary";
                xChart.showInLegend = true;
                xChart.xValueFormatString = "DD/MM";
                xChart.yValueFormatString = "#,##0 phút";

                var tomonth = new DateTime(nam.Value, thang.Value, 1);
                var endmonth = thang.Value == 12 ? new DateTime(nam.Value + 1, 1, 1) : new DateTime(nam.Value, thang.Value + 1, 1);

                var lst = db.ChamCongs.Where(x => x.time > tomonth && x.time < endmonth && x.user_id == acc.id).OrderBy(x => x.time).ToArray();
                if (lst.Length > 0)
                {
                    var lastday = lst.Last().time.Day;
                    xChart.dataPoints = new XChartPoint[lastday];

                    for (var i = 1; i <= lastday; i++)
                    {
                        xChart.dataPoints[i - 1] = new XChartPoint();
                        var startTime = tomonth.AddDays(i - 1);
                        var endTime = tomonth.AddDays(i);
                        var item = lst.Where(x => x.time > startTime && x.time < endTime).OrderBy(x => x.time);
                        xChart.dataPoints[i - 1].x = startTime.ToString("yyyy-MM-" + i);
                        if (item.Count() > 0)
                        {
                            var first = item.First();
                            var OnTime = Convert.ToDateTime(first.time.ToShortDateString());
                            OnTime = OnTime.AddMinutes(30).AddHours(7);
                            xChart.dataPoints[i - 1].y = first.time.Subtract(OnTime).TotalMinutes;
                        }
                        else
                        {
                            xChart.dataPoints[i - 1].y = 0;
                        }
                    }
                }
                else
                {
                    xChart.dataPoints = new XChartPoint[1] { new XChartPoint() { x = tomonth.ToString("yyyy-MM-" + 1), y = 0 } };
                }
                data.Add(xChart);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DSChamCong(int? thang, int? nam)
        {
            thang = !thang.HasValue ? DateTime.Now.Month : thang;
            nam = !nam.HasValue ? DateTime.Now.Year : nam;
            ViewBag.THANG = thang;
            ViewBag.NAM = nam;
            var tomonth = new DateTime(nam.Value, thang.Value, 1);
            var endmonth = thang.Value == 12 ? new DateTime(nam.Value + 1, 1, 1) : new DateTime(nam.Value, thang.Value + 1, 1);
            var lst = db.ChamCongs.Where(x => x.time > tomonth && x.time < endmonth).OrderByDescending(x => x.time).ToArray();
            var dschamcong = new List<DSChamCong>();
            var NgayThang = "Tháng " + thang + " năm " + nam;
            string path = Server.MapPath("~/public/chamcong/");
            string pathTemp = Server.MapPath("~/App_Data/templates/BangChamCong.xlsx");
            string webpart = "BangChamCong_" + thang + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();

            foreach (var item in db.Accounts.Where(x => x.sta == (int)XAccount.eStatus.Processing))
            {
                var obj = new DSChamCong();
                obj.fullname = item.fullname;
                var lstChamCongAccount = lst.Where(x => x.user_id == item.id);
                var overtime = 0;
                for (int i = 1; i < 32; i++)
                {
                    var lastDayOfMonth = DateTime.DaysInMonth(nam.Value, thang.Value);
                    if (i <= lastDayOfMonth)
                    {
                        var intime = new DateTime(nam.Value, thang.Value, i, 7, 30, 59);
                        var chamcong = lstChamCongAccount.Where(x => x.time.Day == i).OrderBy(x => x.time).FirstOrDefault();
                        if (chamcong == null)
                        {
                            foreach (var ngay in typeof(DSChamCong).GetProperties())
                            {
                                if (ngay.Name == "n" + i.ToString())
                                {
                                    ngay.SetValue(obj, null);
                                }
                            }
                            if (intime.DayOfWeek != DayOfWeek.Sunday)
                            {
                                overtime++;
                            }
                        }
                        else if (chamcong.time > intime)
                        {
                            overtime++;
                            foreach (var ngay in typeof(DSChamCong).GetProperties())
                            {
                                if (ngay.Name == "n" + i.ToString())
                                {
                                    ngay.SetValue(obj, (int)((chamcong.time - intime).TotalMinutes));
                                }
                            }
                        }
                        else
                        {
                            foreach (var ngay in typeof(DSChamCong).GetProperties())
                            {
                                if (ngay.Name == "n" + i.ToString())
                                {
                                    ngay.SetValue(obj, (int)((chamcong.time - intime).TotalMinutes));
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var ngay in typeof(DSChamCong).GetProperties())
                        {
                            if (ngay.Name == "n" + i.ToString())
                            {
                                ngay.SetValue(obj, null);
                            }
                        }
                    }
                }
                obj.count = overtime;
                dschamcong.Add(obj);
            }
            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("NgayThang", NgayThang);
                    flexCelReport.AddTable("T", dschamcong);
                    flexCelReport.Run(sr, fs);
                }
            }
            return Json("/public/chamcong/" + webpart, JsonRequestBehavior.AllowGet);
        }
    }
    public class DSChamCong
    {
        public string fullname { get; set; }
        public int? n1 { get; set; }
        public int? n2 { get; set; }
        public int? n3 { get; set; }
        public int? n4 { get; set; }
        public int? n5 { get; set; }
        public int? n6 { get; set; }
        public int? n7 { get; set; }
        public int? n8 { get; set; }
        public int? n9 { get; set; }
        public int? n10 { get; set; }
        public int? n11 { get; set; }
        public int? n12 { get; set; }
        public int? n13 { get; set; }
        public int? n14 { get; set; }
        public int? n15 { get; set; }
        public int? n16 { get; set; }
        public int? n17 { get; set; }
        public int? n18 { get; set; }
        public int? n19 { get; set; }
        public int? n20 { get; set; }
        public int? n21 { get; set; }
        public int? n22 { get; set; }
        public int? n23 { get; set; }
        public int? n24 { get; set; }
        public int? n25 { get; set; }
        public int? n26 { get; set; }
        public int? n27 { get; set; }
        public int? n28 { get; set; }
        public int? n29 { get; set; }
        public int? n30 { get; set; }
        public int? n31 { get; set; }
        public int count { get; set; }
    }
    public class BangLuong
    {
        public int id { get; set; }
        public string NgayThang { get; set; }
        public string HoTen { get; set; }
        public string Luong { get; set; }
        public string LuongCoBan { get; set; }
        public string PhuCap { get; set; }
        public int NgayTrongThang { get; set; }
        public double NgayLam { get; set; }
        public double NgayNghi { get; set; }
        public double CoPhep { get; set; }
        public double KhongPhep { get; set; }
        public string TruLuong { get; set; }
        public string LuongNgay { get; set; }
        public string LuongThuc { get; set; }
        public string TamUng { get; set; }
        public string Tong { get; set; }
        public int ConLai { get; set; }
        public string Thuong { get; set; }
        public List<Alter> TL { get; set; }
        public List<Alter> PC { get; set; }
        public List<Alter> RW { get; set; }
    }
    public class Alter
    {
        public int account_id { get; set; }
        public string ten { get; set; }
        public string sotien { get; set; }
        public string ngaythang { get; set; }
    }
    public class xChamCong
    {
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }
    }
}