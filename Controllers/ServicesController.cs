using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    public class ServicesController : Controller
    {
        private NHModel db = new NHModel();

        // GET: Services
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Index()
        {
            return View(db.Services.ToList());
        }

        // GET: Services/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }

            ViewBag.Documents = from dc in db.Documents
                                join fk in db.fk_service_document on dc.id equals fk.document_id
                                where fk.service_id == id
                                select new ServiceDocument { id = fk.id, name = dc.name };

            return View(service);
        }

        // GET: Services/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Service service)
        {
            if (ModelState.IsValid)
            {
                db.Services.Add(service);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(service);
        }

        // GET: Services/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Service service)
        {
            if (ModelState.IsValid)
            {
                db.Entry(service).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(service);
        }

        // GET: Services/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Service service = db.Services.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }
            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Service service = db.Services.Find(id);
            db.Services.Remove(service);
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
        public JsonResult InCongViec(int hoso_id, int service_id)
        {
            var service = db.Services.Find(service_id);
            var hoso = db.HoSoes.Find(hoso_id);
            var contract = db.Contracts.Find(hoso.contract_id);
            var address = db.Addresses.Find(contract.address_id);
            var lstCVIDS = db.fk_congviec_service.Where(x => x.service_id == service.id).Select(x => x.congviec_id);
            var lstCVs = lstCVIDS != null ? db.CongViecs.Where(x => lstCVIDS.Contains(x.id)) : null;
            var lstCT = new List<BanInCongViec>();
            var done = db.fk_congviec_hoso.Where(x => x.hoso_id == hoso.id && x.status == (int)XModels.eStatus.Complete).Select(x => x.congviec_id);
            foreach (var cate in XCongViec.sCategory)
            {
                foreach (var cv in lstCVs.Where(x => x.category_id == cate.Key))
                {
                    lstCT.Add(new BanInCongViec() { isCheck = done.Contains(cv.id) ? "X" : null, CategoryName = cate.Value, Name = cv.name });
                }
            }
            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/InCongViec.xls");
            string webpart = "InCongViec" + hoso_id + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            using (System.IO.FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (System.IO.FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("HOPDONG", hoso.name);
                    flexCelReport.SetValue("DIACHI", address.name);
                    flexCelReport.AddTable("C", XCongViec.sCategory);
                    flexCelReport.AddTable("D", lstCT);
                    flexCelReport.AddRelationship("C", "D", "Value", "CategoryName");
                    flexCelReport.Run(sr, fs);
                }
            }
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }
    }
    public class BanInCongViec
    {
        public string isCheck { get; set; }
        public string CategoryName { get; set; }
        public string Name { get; set; }
    }
}
