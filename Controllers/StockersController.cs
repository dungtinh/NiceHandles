using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NiceHandles.Models;
using PagedList;
using static NiceHandles.Controllers.InOutsController;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Stocker")]
    public class StockersController : Controller
    {
        private NHModel db = new NHModel();

        // GET: Stockers
        public ActionResult Index(string Search_Data, int? type, int? currency, string Filter_Value, int? Page_No, string fromdate, string todate)
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
            ViewBag.type = type;
            ViewBag.currency = currency;

            var results = from v in db.Stockers
                          where (
                          (string.IsNullOrEmpty(v.note) || v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper())) &&
                          v.type == (type.HasValue ? type.Value : v.type) &&
                          v.currency == (currency.HasValue ? currency.Value : v.currency) &&
                          v.time >= FromDate &&
                          v.time < ToDate
                          )
                          orderby v.time descending
                          select v;

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

            // Tiền mặt

            var lsttongthuTM = lsttongthu.Where(x => x.currency == (int)XModels.eLoaiTien.TienMat);
            long tongthuTM = 0;
            if (lsttongthuTM.Count() > 0)
                tongthuTM = lsttongthuTM.Sum(x => x.amount);
            ViewBag.TONGTHUTM = tongthuTM.ToString("N0");

            long tongchiTM = 0;
            var lsttongchiTM = lsttongchi.Where(x => x.currency == (int)XModels.eLoaiTien.TienMat);
            if (lsttongchiTM.Count() > 0)
                tongchiTM = lsttongchiTM.Sum(x => x.amount);
            ViewBag.TONGCHITM = tongchiTM.ToString("N0");
            var thuchiTM = tongthuTM - tongchiTM;
            ViewBag.THUCHITM = thuchiTM.ToString("N0");

            // Tiền Tài khoản

            var lsttongthuNH = lsttongthu.Where(x => x.currency == (int)XModels.eLoaiTien.NganHang);
            long tongthuNH = 0;
            if (lsttongthuNH.Count() > 0)
                tongthuNH = lsttongthuNH.Sum(x => x.amount);
            ViewBag.TONGTHUNH = tongthuNH.ToString("N0");

            long tongchiNH = 0;
            var lsttongchiNH = lsttongchi.Where(x => x.currency == (int)XModels.eLoaiTien.NganHang);
            if (lsttongchiNH.Count() > 0)
                tongchiNH = lsttongchiNH.Sum(x => x.amount);
            ViewBag.TONGCHINH = tongchiNH.ToString("N0");
            var thuchiNH = tongthuNH - tongchiNH;
            ViewBag.THUCHINH = thuchiNH.ToString("N0");
            // Công nợ
            var cateVTs = db.Categories.Where(c => c.pair == (int)XCategory.ePair.VayTra).Select(x => x.id).ToArray();
            var lstYIO = (from io in db.InOuts                          
                          where io.status != (int)XInOut.eStatus.Huy
                          select new { T = io.type, AM = io.amount, AC = io.account_id, VT = io.category_id });
            var thu = lstYIO.Where(x => x.T == (int)XCategory.eType.Thu);
            var chi = lstYIO.Where(x => x.T == (int)XCategory.eType.Chi);
            // Giang            
            var thuGiang = thu.Where(x => cateVTs.Contains(x.VT) && x.AC == 3);
            var chiGiang = chi.Where(x => cateVTs.Contains(x.VT) && x.AC == 3);
            long thuNoGiang = thuGiang.Count() > 0 ? thuGiang.Sum(x => x.AM) : 0;
            long chiNoGiang = chiGiang.Count() > 0 ? chiGiang.Sum(x => x.AM) : 0;
            long NoGiang = chiNoGiang - thuNoGiang;
            ViewBag.NoGiang = NoGiang.ToString("N0");
            // Tín
            var thuTin = thu.Where(x => cateVTs.Contains(x.VT) && x.AC == 2);
            var chiTin = chi.Where(x => cateVTs.Contains(x.VT) && x.AC == 2);
            long thuNoTin = thuTin.Count() > 0 ? thuTin.Sum(x => x.AM) : 0;
            long chiNoTin = chiTin.Count() > 0 ? chiTin.Sum(x => x.AM) : 0;
            long NoTin = chiNoTin - thuNoTin;
            ViewBag.NoTin = NoTin.ToString("N0");
            // Duy
            var thuDuy = thu.Where(x => cateVTs.Contains(x.VT) && x.AC == 4);
            var chiDuy = chi.Where(x => cateVTs.Contains(x.VT) && x.AC == 4);
            long thuNoDuy = thuDuy.Count() > 0 ? thuDuy.Sum(x => x.AM) : 0;
            long chiNoDuy = chiDuy.Count() > 0 ? chiDuy.Sum(x => x.AM) : 0;
            long NoDuy = chiNoDuy - thuNoDuy;
            ViewBag.NoDuy = NoDuy.ToString("N0");
            // Cổ tức
            var cateCT = db.Categories.Where(c => c.pair == (int)XCategory.ePair.Stock && c.type == (int)XCategory.eType.Chi).Single();
            var lstCT = (from io in db.InOuts                         
                         where io.status != (int)XInOut.eStatus.Huy && io.category_id == cateCT.id
                         select new { T = io.type, AM = io.amount, AC = io.account_id, VT = io.category_id });
            // Giang                        
            var ctGs = lstCT.Where(x => x.AC == 3);
            long ctG = ctGs.Count() > 0 ? ctGs.Sum(x => x.AM) : 0;
            ViewBag.CTGiang = ctG.ToString("N0");
            // Duy                        
            var ctDs = lstCT.Where(x => x.AC == 4);
            long ctD = ctDs.Count() > 0 ? ctDs.Sum(x => x.AM) : 0;
            ViewBag.CTDuy = ctD.ToString("N0");
            // Tín            
            var ctTs = lstCT.Where(x => x.AC == 2);
            long ctT = ctTs.Count() > 0 ? ctTs.Sum(x => x.AM) : 0;
            ViewBag.CTTin = ctT.ToString("N0");


            int Size_Of_Page = 50;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        public ActionResult IndexX(string Search_Data, int? type, int? currency, string Filter_Value, int? Page_No, string fromdate, string todate)
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
            ViewBag.type = type;
            ViewBag.currency = currency;

            var results = from v in db.Stockers
                          where (
                          (string.IsNullOrEmpty(v.note) || v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper())) &&
                          v.type == (type.HasValue ? type.Value : v.type) &&
                          v.currency == (currency.HasValue ? currency.Value : v.currency) &&
                          v.time >= FromDate &&
                          v.time < ToDate
                          )
                          orderby v.time descending
                          select v;

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

            // Tiền mặt

            var lsttongthuTM = lsttongthu.Where(x => x.currency == (int)XModels.eLoaiTien.TienMat);
            long tongthuTM = 0;
            if (lsttongthuTM.Count() > 0)
                tongthuTM = lsttongthuTM.Sum(x => x.amount);
            ViewBag.TONGTHUTM = tongthuTM.ToString("N0");

            long tongchiTM = 0;
            var lsttongchiTM = lsttongchi.Where(x => x.currency == (int)XModels.eLoaiTien.TienMat);
            if (lsttongchiTM.Count() > 0)
                tongchiTM = lsttongchiTM.Sum(x => x.amount);
            ViewBag.TONGCHITM = tongchiTM.ToString("N0");
            var thuchiTM = tongthuTM - tongchiTM;
            ViewBag.THUCHITM = thuchiTM.ToString("N0");

            // Tiền Tài khoản

            var lsttongthuNH = lsttongthu.Where(x => x.currency == (int)XModels.eLoaiTien.NganHang);
            long tongthuNH = 0;
            if (lsttongthuNH.Count() > 0)
                tongthuNH = lsttongthuNH.Sum(x => x.amount);
            ViewBag.TONGTHUNH = tongthuNH.ToString("N0");

            long tongchiNH = 0;
            var lsttongchiNH = lsttongchi.Where(x => x.currency == (int)XModels.eLoaiTien.NganHang);
            if (lsttongchiNH.Count() > 0)
                tongchiNH = lsttongchiNH.Sum(x => x.amount);
            ViewBag.TONGCHINH = tongchiNH.ToString("N0");
            var thuchiNH = tongthuNH - tongchiNH;
            ViewBag.THUCHINH = thuchiNH.ToString("N0");
            // Công nợ
            var cateVTs = db.Categories.Where(c => c.pair == (int)XCategory.ePair.VayTra).Select(x => x.id).ToArray();
            var lstYIO = (from io in db.InOuts                          
                          where io.status != (int)XInOut.eStatus.Huy
                          select new { T = io.type, AM = io.amount, AC = io.account_id, VT = io.category_id });
            var thu = lstYIO.Where(x => x.T == (int)XCategory.eType.Thu);
            var chi = lstYIO.Where(x => x.T == (int)XCategory.eType.Chi);
            // Giang            
            var thuGiang = thu.Where(x => cateVTs.Contains(x.VT) && x.AC == 3);
            var chiGiang = chi.Where(x => cateVTs.Contains(x.VT) && x.AC == 3);
            long thuNoGiang = thuGiang.Count() > 0 ? thuGiang.Sum(x => x.AM) : 0;
            long chiNoGiang = chiGiang.Count() > 0 ? chiGiang.Sum(x => x.AM) : 0;
            long NoGiang = chiNoGiang - thuNoGiang;
            ViewBag.NoGiang = NoGiang.ToString("N0");
            // Tín
            var thuTin = thu.Where(x => cateVTs.Contains(x.VT) && x.AC == 2);
            var chiTin = chi.Where(x => cateVTs.Contains(x.VT) && x.AC == 2);
            long thuNoTin = thuTin.Count() > 0 ? thuTin.Sum(x => x.AM) : 0;
            long chiNoTin = chiTin.Count() > 0 ? chiTin.Sum(x => x.AM) : 0;
            long NoTin = chiNoTin - thuNoTin;
            ViewBag.NoTin = NoTin.ToString("N0");
            // Duy
            var thuDuy = thu.Where(x => cateVTs.Contains(x.VT) && x.AC == 4);
            var chiDuy = chi.Where(x => cateVTs.Contains(x.VT) && x.AC == 4);
            long thuNoDuy = thuDuy.Count() > 0 ? thuDuy.Sum(x => x.AM) : 0;
            long chiNoDuy = chiDuy.Count() > 0 ? chiDuy.Sum(x => x.AM) : 0;
            long NoDuy = chiNoDuy - thuNoDuy;
            ViewBag.NoDuy = NoDuy.ToString("N0");
            // Cổ tức
            var cateCT = db.Categories.Where(c => c.pair == (int)XCategory.ePair.Stock && c.type == (int)XCategory.eType.Chi).Single();
            var lstCT = (from io in db.InOuts                         
                         where io.status != (int)XInOut.eStatus.Huy && io.category_id == cateCT.id
                         select new { T = io.type, AM = io.amount, AC = io.account_id, VT = io.category_id });
            // Giang                        
            var ctGs = lstCT.Where(x => x.AC == 3);
            long ctG = ctGs.Count() > 0 ? ctGs.Sum(x => x.AM) : 0;
            ViewBag.CTGiang = ctG.ToString("N0");
            // Duy                        
            var ctDs = lstCT.Where(x => x.AC == 4);
            long ctD = ctDs.Count() > 0 ? ctDs.Sum(x => x.AM) : 0;
            ViewBag.CTDuy = ctD.ToString("N0");
            // Tín            
            var ctTs = lstCT.Where(x => x.AC == 2);
            long ctT = ctTs.Count() > 0 ? ctTs.Sum(x => x.AM) : 0;
            ViewBag.CTTin = ctT.ToString("N0");


            int Size_Of_Page = 50;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        public JsonResult BanInThuChi(string Search_Data, int? type, int? currency, string fromdate, string todate)
        {
            DateTime FromDate = string.IsNullOrEmpty(fromdate) ? new DateTime() : DateTime.Parse(fromdate);
            DateTime ToDate = string.IsNullOrEmpty(todate) ? DateTime.Now : DateTime.Parse(todate);
            ToDate = ToDate.AddDays(1);

            var results = (from v in db.Stockers
                           where (
                           (string.IsNullOrEmpty(v.note) || v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper())) &&
                           v.type == (type.HasValue ? type.Value : v.type) &&
                           v.currency == (currency.HasValue ? currency.Value : v.currency) &&
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
            // Tiền mặt
            var lsttongthuTM = lsttongthu.Where(x => x.currency == (int)XModels.eLoaiTien.TienMat);
            long tongthuTM = 0;
            if (lsttongthuTM.Count() > 0)
                tongthuTM = lsttongthuTM.Sum(x => x.amount);
            long tongchiTM = 0;
            var lsttongchiTM = lsttongchi.Where(x => x.currency == (int)XModels.eLoaiTien.TienMat);
            if (lsttongchiTM.Count() > 0)
                tongchiTM = lsttongchiTM.Sum(x => x.amount);
            var thuchiTM = tongthuTM - tongchiTM;
            // Tiền Tài khoản
            var lsttongthuNH = lsttongthu.Where(x => x.currency == (int)XModels.eLoaiTien.NganHang);
            long tongthuNH = 0;
            if (lsttongthuNH.Count() > 0)
                tongthuNH = lsttongthuNH.Sum(x => x.amount);
            long tongchiNH = 0;
            var lsttongchiNH = lsttongchi.Where(x => x.currency == (int)XModels.eLoaiTien.NganHang);
            if (lsttongchiNH.Count() > 0)
                tongchiNH = lsttongchiNH.Sum(x => x.amount);
            var thuchiNH = tongthuNH - tongchiNH;

            string NGAYTHANG = "Từ ngày: " + (!string.IsNullOrEmpty(fromdate) ? FromDate.ToString("dd/MM/yyyy") : "          ") +
                " đến ngày: " + (!string.IsNullOrEmpty(todate) ? ToDate.ToString("dd/MM/yyyy") : "              ");
            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/BanInStocker.xls");
            string webpart = "BanInThuChi_" + DateTime.Now.ToString("ddMMyy") + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();

            var rs = results.Select(x => new cBanInStocker()
            {
                LOAI = XCategory.sType[x.type],
                LOAITIEN = XModels.sLoaiTien[x.currency],
                LYDO = x.note,
                SOTIEN = x.amount.ToString("N0"),
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
                    //
                    flexCelReport.SetValue("THUTM", tongthuTM.ToString("N0"));
                    flexCelReport.SetValue("CHITM", tongchiTM.ToString("N0"));
                    flexCelReport.SetValue("THUCHITM", thuchiTM.ToString("N0"));
                    //
                    flexCelReport.SetValue("THUNH", tongthuNH.ToString("N0"));
                    flexCelReport.SetValue("CHINH", tongchiNH.ToString("N0"));
                    flexCelReport.SetValue("THUCHINH", thuchiNH.ToString("N0"));
                    //
                    flexCelReport.AddTable("TB", rs);
                    flexCelReport.Run(sr, fs);
                }
            }
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }

        // GET: Stockers/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stocker stocker = db.Stockers.Find(id);
            if (stocker == null)
            {
                return HttpNotFound();
            }
            return View(stocker);
        }

        // GET: Stockers/Create
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Asyn(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InOut io = db.InOuts.Find(id);            
            if (io == null)
            {
                return HttpNotFound();
            }
            ViewBag.id = id;
            Stocker stocker = new Stocker();
            stocker.amount = io.amount;
            stocker.note = io.note;
            stocker.time = io.time;
            stocker.type = io.type;
            return View(stocker);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Asyn(Stocker stocker, long inout_id)
        {
            ViewBag.id = inout_id;
            stocker.sta = (int)XModels.eStatus.Processing;
            stocker.state = (int)XModels.eStatus.Processing;
            InOut io = db.InOuts.Find(inout_id);
            io.gostock = (int)XModels.eLevel.Level3;
            if (ModelState.IsValid)
            {
                db.Stockers.Add(stocker);
                db.SaveChanges();
                return RedirectToAction("GoAsyn", "InOuts");
            }

            return View(stocker);
        }
        public ActionResult AllreadyAsyn(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InOut io = db.InOuts.Find(id);
            io.gostock = (int)XModels.eLevel.Level3;
            db.SaveChanges();
            return RedirectToAction("GoAsyn", "InOuts");
        }


        // POST: Stockers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Stocker stocker)
        {
            stocker.sta = (int)XModels.eStatus.Processing;
            stocker.state = (int)XModels.eStatus.Processing;
            stocker.time = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Stockers.Add(stocker);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stocker);
        }

        // GET: Stockers/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stocker stocker = db.Stockers.Find(id);
            if (stocker == null)
            {
                return HttpNotFound();
            }
            return View(stocker);
        }

        // POST: Stockers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,amount,note,currency,type,time,sta,state")] Stocker stocker)
        {
            if (ModelState.IsValid)
            {
                db.Entry(stocker).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stocker);
        }

        // GET: Stockers/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Stocker stocker = db.Stockers.Find(id);
            if (stocker == null)
            {
                return HttpNotFound();
            }
            return View(stocker);
        }

        // POST: Stockers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Stocker stocker = db.Stockers.Find(id);
            db.Stockers.Remove(stocker);
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

        public class cBanInStocker
        {
            public string THOIGIAN { get; set; }
            public string LOAI { get; set; }
            public string LOAITIEN { get; set; }
            public string SOTIEN { get; set; }
            public string LYDO { get; set; }
        }
    }
}
