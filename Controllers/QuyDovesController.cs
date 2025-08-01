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
using static NiceHandles.Controllers.StockersController;

namespace NiceHandles.Controllers
{
    public class QuyDovesController : Controller
    {
        private NHModel db = new NHModel();

        // GET: QuyDoves
        public ActionResult Index(string Search_Data, int? type, string Filter_Value, int? Page_No, string fromdate, string todate)
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

            var results = from v in db.QuyDoves
                          where (
                          (string.IsNullOrEmpty(v.note) || v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper())) &&
                          v.type == (type.HasValue ? type.Value : v.type) &&
                          v.time >= FromDate &&
                          v.time < ToDate &&
                          v.sta != (int)XModels.eStatus.Cancel
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

            int Size_Of_Page = 50;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        // GET: QuyDoves/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuyDove quyDove = db.QuyDoves.Find(id);
            if (quyDove == null)
            {
                return HttpNotFound();
            }
            return View(quyDove);
        }

        // GET: QuyDoves/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QuyDoves/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuyDove quyDove)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            quyDove.state = (int)XModels.eStatus.Processing;
            quyDove.sta = (int)XModels.eStatus.Processing;
            quyDove.time = DateTime.Now;
            quyDove.account_id = us.id;
            if (ModelState.IsValid)
            {
                db.QuyDoves.Add(quyDove);
                db.SaveChanges();

                var wf = new wf_quydove();
                wf.account_id = us.id;
                wf.kid = (int)quyDove.id;
                wf.note = "Khởi tạo " + quyDove.amount.ToString("N0") + " " + quyDove.note;
                wf.time = DateTime.Now;
                db.wf_quydove.Add(wf);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(quyDove);
        }

        // GET: QuyDoves/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuyDove quyDove = db.QuyDoves.Find(id);
            if (quyDove == null)
            {
                return HttpNotFound();
            }
            return View(quyDove);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(QuyDove quyDove)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            quyDove.state = (int)XModels.eStatus.Processing;
            quyDove.sta = (int)XModels.eStatus.Processing;
            quyDove.time = DateTime.Now;
            quyDove.account_id = us.id;
            if (ModelState.IsValid)
            {
                db.Entry(quyDove).State = EntityState.Modified;

                var wf = new wf_quydove();
                wf.account_id = us.id;
                wf.kid = (int)quyDove.id;
                wf.note = quyDove.amount.ToString("N0") + " " + quyDove.note;
                wf.time = DateTime.Now;
                db.wf_quydove.Add(wf);

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(quyDove);
        }

        // GET: QuyDoves/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QuyDove quyDove = db.QuyDoves.Find(id);
            if (quyDove == null)
            {
                return HttpNotFound();
            }
            return View(quyDove);
        }

        // POST: QuyDoves/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            QuyDove quyDove = db.QuyDoves.Find(id);

            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var wf = new wf_quydove();
            wf.account_id = us.id;
            wf.kid = (int)quyDove.id;
            wf.note = "Xóa";
            wf.time = DateTime.Now;
            db.wf_quydove.Add(wf);

            quyDove.sta = (int)XModels.eStatus.Cancel;
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
        public JsonResult BanInThuChi(string Search_Data, int? type, string fromdate, string todate)
        {
            DateTime FromDate = string.IsNullOrEmpty(fromdate) ? new DateTime() : DateTime.Parse(fromdate);
            DateTime ToDate = string.IsNullOrEmpty(todate) ? DateTime.Now : DateTime.Parse(todate);
            ToDate = ToDate.AddDays(1);

            var results = (from v in db.QuyDoves
                           where (
                           (string.IsNullOrEmpty(v.note) || v.note.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.note.ToUpper())) &&
                           v.type == (type.HasValue ? type.Value : v.type) &&
                           v.time >= FromDate &&
                           v.time < ToDate &&
                            v.sta != (int)XModels.eStatus.Cancel
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
            string pathTemp = Server.MapPath("~/App_Data/templates/BanInQuyDove.xls");
            string webpart = "BanInQuyDove_" + DateTime.Now.ToString("ddMMyy") + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();

            var rs = results.Select(x => new cBanInStocker()
            {
                LOAI = XCategory.sType[x.type],
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
                    flexCelReport.AddTable("TB", rs);
                    flexCelReport.Run(sr, fs);
                }
            }
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetWF(int id)
        {
            var wfs = db.wf_quydove.Where(x => x.kid == id).ToArray();
            string sb =
                "<table class='table'>";
            foreach (var item in wfs)
            {
                sb +=
                    "   <tr>" +
                    "       <td>" +
                    "           " + item.time.ToString("dd/MM/yyyy") +
                    "       </td>" +
                    "       <td>" +
                    "           " + item.note +
                    "       </td>" +
                    "   </tr>";

            }
            sb += "</table>";
            return Json(sb, JsonRequestBehavior.AllowGet);
        }
    }
}
