using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Aspose.Words.Lists;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;
using PagedList;

namespace NiceHandles.Controllers
{
    public class KPIsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: KPIs
        public ActionResult Index(int? Page_No)
        {
            var result = db.KPIs.OrderByDescending(x => x.ngaythang).ToList();
            int Size_Of_Page = 50;
            int No_Of_Page = (Page_No ?? 1);
            return View(result.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        // GET: KPIs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KPI kPI = db.KPIs.Find(id);
            if (kPI == null)
            {
                return HttpNotFound();
            }
            return View(kPI);
        }

        // GET: KPIs/Create
        public ActionResult Create()
        {
            var now = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            var username = User.Identity.GetUserName();
            var acc = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            KPI kPI = db.KPIs.Where(x => x.ngaythang > now && x.user_id == acc.id).SingleOrDefault();
            if (kPI == null)
            {
                kPI = new KPI();
                kPI.ngaythang = DateTime.Now;
                kPI.user_id = acc.id;
                db.KPIs.Add(kPI);
                db.SaveChanges();
            }
            return View(kPI);
        }

        // POST: KPIs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,ngaythang,user_id,ghichu")] KPI kPI)
        {
            if (ModelState.IsValid)
            {
                db.KPIs.Add(kPI);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(kPI);
        }

        // GET: KPIs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KPI kPI = db.KPIs.Find(id);
            if (kPI == null)
            {
                return HttpNotFound();
            }
            return View(kPI);
        }

        // POST: KPIs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,ngaythang,user_id,ghichu")] KPI kPI)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kPI).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(kPI);
        }

        // GET: KPIs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KPI kPI = db.KPIs.Find(id);
            if (kPI == null)
            {
                return HttpNotFound();
            }
            return View(kPI);
        }

        // POST: KPIs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KPI kPI = db.KPIs.Find(id);
            db.KPIs.Remove(kPI);
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

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Submit(string report, KPILink[] lstFB, KPILink[] lstZL, KPILink[] lstGP, KPILink[] lstFP, KPILink[] lstWB, Social[] lstKBZL, Social[] lstKBFB)
        {
            var now = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            var username = User.Identity.GetUserName();
            var acc = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            KPI kPI = db.KPIs.Where(x => x.ngaythang > now && x.user_id == acc.id).SingleOrDefault();
            if (kPI == null)
            {
                kPI = new KPI();
                kPI.ngaythang = DateTime.Now;
                kPI.user_id = acc.id;
                db.KPIs.Add(kPI);
                db.SaveChanges();
            }
            else
            {
                var links = db.KPILinks.Where(x => x.KPI_id == kPI.id);
                var socials = db.Socials.Where(x => x.kpi_id == kPI.id);
                db.KPILinks.RemoveRange(links);
                db.Socials.RemoveRange(socials);
            }
            kPI.ghichu = report;
            if (lstFB != null)
            {
                foreach (var item in lstFB)
                {
                    item.KPI_id = kPI.id;
                    db.KPILinks.Add(item);
                }
            }
            if (lstZL != null)
            {
                foreach (var item in lstZL)
                {
                    item.KPI_id = kPI.id;
                    db.KPILinks.Add(item);
                }
            }
            if (lstGP != null)
            {
                foreach (var item in lstGP)
                {
                    item.KPI_id = kPI.id;
                    db.KPILinks.Add(item);
                }
            }
            if (lstFP != null)
            {
                foreach (var item in lstFP)
                {
                    item.KPI_id = kPI.id;
                    db.KPILinks.Add(item);
                }
            }
            if (lstWB != null)
            {
                foreach (var item in lstWB)
                {
                    item.KPI_id = kPI.id;
                    db.KPILinks.Add(item);
                }
            }
            // KB
            if (lstKBZL != null)
            {
                foreach (var item in lstKBZL)
                {
                    item.kpi_id = kPI.id;
                    db.Socials.Add(item);
                }
            }
            if (lstKBFB != null)
            {
                foreach (var item in lstKBFB)
                {
                    item.kpi_id = kPI.id;
                    db.Socials.Add(item);
                }
            }
            db.SaveChanges();
            return Json("/kpis", JsonRequestBehavior.AllowGet);
        }
        public ActionResult BaoCaoNgay()
        {
            var now = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            var username = User.Identity.GetUserName();
            var acc = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            KPI kPI = db.KPIs.Where(x => x.ngaythang > now && x.user_id == acc.id).SingleOrDefault();
            if (kPI == null)
            {
                kPI = new KPI();
                kPI.ngaythang = DateTime.Now;
                kPI.user_id = acc.id;
                db.KPIs.Add(kPI);
                db.SaveChanges();
            }
            return View(kPI);
        }
        public ActionResult rptKPI()
        {
            return View();
        }
        public ActionResult GetKPI()
        {
            var data = new List<XChart>();
            bool isFirst = true;
            var tomonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var CurrentMontheKPIs = db.KPIs.Where(x => x.ngaythang > tomonth).OrderBy(x => x.ngaythang).ToArray();
            var lastday = CurrentMontheKPIs.Last().ngaythang.Day;
            foreach (var acc in db.Accounts.Where(x => x.sta == (int)XAccount.eStatus.Processing))
            {
                XChart xChart = new XChart();
                xChart.type = "spline";
                xChart.name = acc.fullname;
                if (isFirst)
                {
                    isFirst = false;
                    xChart.axisYType = "primary";
                }
                else
                    xChart.axisYType = "secondary";
                xChart.showInLegend = true;
                xChart.xValueFormatString = "DD/MM";
                xChart.yValueFormatString = "#,##0";
                var lst = CurrentMontheKPIs.Where(x => x.user_id == acc.id).ToArray();
                if (lst.Length > 0)
                {
                    xChart.dataPoints = new XChartPoint[lastday];

                    for (var i = 1; i <= lastday; i++)
                    {
                        xChart.dataPoints[i - 1] = new XChartPoint();
                        var startTime = tomonth.AddDays(i - 1);
                        var endTime = tomonth.AddDays(i);
                        var item = lst.Where(x => x.ngaythang > startTime && x.ngaythang < endTime).FirstOrDefault();
                        xChart.dataPoints[i - 1].x = startTime.ToString("yyyy-MM-" + i);
                        if (item != null)
                        {
                            var links = db.KPILinks.Count(x => x.KPI_id == item.id);
                            var social = db.Socials.Count(x => x.kpi_id == item.id);
                            xChart.dataPoints[i - 1].y = links + social;
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
    }
}
