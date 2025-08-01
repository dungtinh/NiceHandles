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
using Aspose.Words;
using Aspose.Words.Reporting;

namespace NiceHandles.Controllers
{
    public class ValsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: Vals
        public ActionResult Index(int cid, int did)
        {
            ViewBag.document = db.Documents.Find(did);
            ViewBag.contract = db.Contracts.Find(cid);
            ViewBag.db = db;
            return View();
        }
        [HttpPost]
        public JsonResult Save(Val[] vls)
        {
            foreach (var vl in vls)
            {
                var v = db.Vals.Where(x => x.contract_id == vl.contract_id && x.field_id == vl.field_id).SingleOrDefault();
                if (v == null)
                {
                    db.Vals.Add(vl);
                }
                else
                {
                    v.value = vl.value;
                }
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Download(int hoso_id, int document_id)
        {
            var document = db.Documents.Find(document_id);
            ViewBag.document = document;
            ViewBag.hoso = db.HoSoes.Find(hoso_id);
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            //flexCelReport.Run()
            string path = Server.MapPath("~/public/");

            //string pathTemp = Server.MapPath("~/App_Data/templates/" + "test.docx");

            string pathTemp = Server.MapPath("~/App_Data/templates/" + document.template);
            //string pathTemp = Server.MapPath("~/App_Data/templates/KhachHang.xlsx");

            try
            {

                var kvp = from xgr in db.GroupFields
                          join xfi in db.Fields on xgr.id equals xfi.group_id
                          join xv in db.Vals on xfi.id equals xv.field_id
                          where xv.contract_id == hoso_id && xgr.document_id == document_id
                          select new { xfi.name, xv.value };
                //using (FileStream fs = System.IO.File.Create(path + DateTime.Now.ToString("ssmmHHddMMyy") + ".xlsx"))
                //{
                //    using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                //    {
                //        foreach (var item in kvp)
                //        {
                //            flexCelReport.SetValue(item.name, item.value);
                //        }

                //        flexCelReport.Run(sr, fs);
                //    }
                //}
                Aspose.Words.Document doc = new Aspose.Words.Document(pathTemp);
                //ReportingEngine engine = new ReportingEngine();


                //foreach (var item in kvp)
                //{
                //    switch (item.name)
                //    {
                //        case "MASOTHUE_CD":
                //            for (int i = 0; i < 10; i++)
                //            {
                //                var val = i == item.value.Length - 1 ? "" : item.value.Substring(i, 1);                                
                //            }
                //            break;+

                //            break;
                //    }
                //}
                //
                List<string> keys = new List<string>();
                List<string> vals = new List<string>();
                foreach (var item in kvp)
                {
                    keys.Add(item.name);
                    vals.Add(string.IsNullOrEmpty(item.value) ? "" : item.value);
                    if (item.name.StartsWith("MST"))
                    {
                        for (int i = 1; i < 11; i++)
                        {
                            keys.Add(item.name + i);
                            vals.Add(string.IsNullOrEmpty(item.value) ? "" : item.value.Length > i ? item.value.Substring(i - 1, 1) : "");
                        }
                    }
                }

                //engine.BuildReport(doc, vals.ToArray(), keys.ToArray());
                doc.MailMerge.Execute(keys.ToArray(), vals.ToArray());
                doc.Save(path + "word.docx");
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return RedirectToAction("Index", new { cid = hoso_id, did = document_id });
        }

        // GET: Vals/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Val val = db.Vals.Find(id);
            if (val == null)
            {
                return HttpNotFound();
            }
            return View(val);
        }

        // GET: Vals/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Vals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,contract_id,field_id,value")] Val val)
        {
            if (ModelState.IsValid)
            {
                db.Vals.Add(val);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(val);
        }

        // GET: Vals/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Val val = db.Vals.Find(id);
            if (val == null)
            {
                return HttpNotFound();
            }
            return View(val);
        }

        // POST: Vals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,contract_id,field_id,value")] Val val)
        {
            if (ModelState.IsValid)
            {
                db.Entry(val).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(val);
        }

        // GET: Vals/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Val val = db.Vals.Find(id);
            if (val == null)
            {
                return HttpNotFound();
            }
            return View(val);
        }

        // POST: Vals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Val val = db.Vals.Find(id);
            db.Vals.Remove(val);
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
