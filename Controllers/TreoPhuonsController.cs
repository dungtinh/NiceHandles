using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NiceHandles.Models;
using PagedList;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Manager,Member,OutSide")]
    public class TreoPhuonsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: TreoPhuons
        public ActionResult Index(int? Page_No, int? Add, string fromdate, string todate)
        {
            ViewBag.FROMDATE = fromdate;
            ViewBag.TODATE = todate;
            ViewBag.ADD = Add;
            int Size_Of_Page = 50;
            int No_Of_Page = (Page_No ?? 1);
            DateTime FromDate = string.IsNullOrEmpty(fromdate) ? new DateTime() : DateTime.Parse(fromdate);
            DateTime ToDate = string.IsNullOrEmpty(todate) ? DateTime.Now : DateTime.Parse(todate);
            ToDate = ToDate.AddDays(1);
            IQueryable<TreoPhuon> result;
            var lstDVs = db.DvhcC1.Where(x => x.address_id == Add.Value).Select(x => x.id).ToArray();
            result = db.TreoPhuons.Where(x => x.NgayTreo >= FromDate &&
                      x.NgayTreo < ToDate);
            if (Add.HasValue)
                result = result.Where(x => lstDVs.Contains(x.DvhcC1_id));
            ViewBag.TONGSO = result.Count().ToString("N0");
            result = result.OrderByDescending(x => x.NgayTreo);
            return View(result.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        // GET: TreoPhuons/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TreoPhuon treoPhuon = db.TreoPhuons.Find(id);
            if (treoPhuon == null)
            {
                return HttpNotFound();
            }
            return View(treoPhuon);
        }

        // GET: TreoPhuons/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TreoPhuons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TreoPhuon treoPhuon, HttpPostedFileBase[] imgs)
        {
            if (ModelState.IsValid)
            {
                if (imgs.Length > 0)
                {
                    db.TreoPhuons.Add(treoPhuon);
                    var file = imgs[0];
                    if (file != null)
                    {
                        string filename = DateTime.Now.ToString("sshhmmddMMyy") + file.FileName;
                        file.SaveAs(Server.MapPath("~/public/phuon/") + filename);
                        treoPhuon.Images = "/public/phuon/" + filename;
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(treoPhuon);
        }

        // GET: TreoPhuons/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TreoPhuon treoPhuon = db.TreoPhuons.Find(id);
            if (treoPhuon == null)
            {
                return HttpNotFound();
            }
            var dvhc = db.DvhcC1.Find(treoPhuon.DvhcC1_id);
            ViewBag.CAPXA = dvhc.address_id;
            return View(treoPhuon);
        }

        // POST: TreoPhuons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TreoPhuon treoPhuon, HttpPostedFileBase[] imgs)
        {
            if (ModelState.IsValid)
            {
                var oldtreoPhuon = db.TreoPhuons.Find(treoPhuon.id);
                oldtreoPhuon.DvhcC1_id = treoPhuon.DvhcC1_id;
                oldtreoPhuon.GoogleMap = treoPhuon.GoogleMap;
                oldtreoPhuon.note = treoPhuon.note;
                oldtreoPhuon.NgayKiemTra = treoPhuon.NgayKiemTra;
                oldtreoPhuon.NgayTreo = treoPhuon.NgayTreo;

                if (imgs.Length > 0 && imgs[0] != null)
                {
                    var file = imgs[0];
                    if (file != null)
                    {
                        string filename = DateTime.Now.ToString("sshhmmddMMyy") + file.FileName;
                        file.SaveAs(Server.MapPath("~/public/phuon/") + filename);
                        oldtreoPhuon.Images = "/public/phuon/" + filename;
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(treoPhuon);
        }

        // GET: TreoPhuons/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TreoPhuon treoPhuon = db.TreoPhuons.Find(id);
            if (treoPhuon == null)
            {
                return HttpNotFound();
            }
            return View(treoPhuon);
        }

        // POST: TreoPhuons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TreoPhuon treoPhuon = db.TreoPhuons.Find(id);
            db.TreoPhuons.Remove(treoPhuon);
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
