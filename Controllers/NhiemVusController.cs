using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Parser.SyntaxTree;
using System.Web.UI.WebControls.WebParts;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    public class NhiemVusController : Controller
    {
        private NHModel db = new NHModel();

        // GET: NhiemVus     
        [ValidateInput(false)]
        public ActionResult SaveNote(NhiemVu obj, string message)
        {
            NhiemVu nhiemVu = db.NhiemVus.Where(x => x.contract_id == obj.contract_id && x.step_id == obj.step_id).FirstOrDefault();
            if (nhiemVu != null)
                nhiemVu.ghichu = obj.ghichu;
            Contract contract = db.Contracts.Find(obj.contract_id);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult Upload(XStep data)
        //{
        //    var nhiemvu = db.NhiemVus.Find(data.id);
        //    var contract = db.Contracts.Find(nhiemvu.contract_id);
        //    var address = db.Addresses.Find(contract.address_id);
        //    var customer = db.Customers.Find(contract.customer_id);

        //    File f = new File();

        //    if (data.f.Count > 0)
        //    {
        //        var file = data.f[0];
        //        if (file != null)
        //        {
        //            string subPath = Server.MapPath("~/public/" + address.name);
        //            bool exists = System.IO.Directory.Exists(subPath);
        //            if (!exists)
        //                System.IO.Directory.CreateDirectory(subPath);
        //            subPath = Server.MapPath("~/public/" + address.name + "/" + customer.name);
        //            exists = System.IO.Directory.Exists(subPath);
        //            if (!exists)
        //                System.IO.Directory.CreateDirectory(subPath);
        //            subPath = Server.MapPath("~/public/" + address.name + "/" + customer.name + "/" + XFile.sType[data.t]);
        //            exists = System.IO.Directory.Exists(subPath);
        //            if (!exists)
        //                System.IO.Directory.CreateDirectory(subPath);
        //            file.SaveAs(subPath + "/" + file.FileName);
        //            f.url = "/public/" + address.name + "/" + customer.name + "/" + XFile.sType[data.t] + "/" + file.FileName;
        //            f.name = file.FileName;
        //        }
        //    }
        //    f.type = data.t;
        //    f.contract_id = nhiemvu.contract_id;
        //    db.Files.Add(f);
        //    db.SaveChanges();
        //    return Json(f.name, JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        public JsonResult Upload()
        {
            var nhiemvu = db.NhiemVus.Find(Convert.ToInt64(Request["id"]));
            var t = Convert.ToInt32(Request["t"]);
            var contract = db.Contracts.Find(nhiemvu.contract_id);
            var address = db.Addresses.Find(contract.address_id);

            File f = new File();

            if (t == (int)XFile.eType.GIAPHA)
            {
                var exited = db.Files.Where(x => x.contract_id == contract.id && x.type == t);
                db.Files.RemoveRange(exited);
            }

            if (Request.Files.Count > 0)
            {
                var f0 = Request.Files[0];

                string subPath = Server.MapPath("~/public/" + address.name);
                bool exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                subPath = Server.MapPath("~/public/" + address.name + "/" + contract.name);
                exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                subPath = Server.MapPath("~/public/" + address.name + "/" + contract.name + "/" + XFile.sType[t]);
                exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                f0.SaveAs(subPath + "/" + f0.FileName);
                f.url = "/public/" + address.name + "/" + contract.name + "/" + XFile.sType[t] + "/" + f0.FileName;
                f.name = f0.FileName;
            }
            f.type = t;
            f.contract_id = nhiemvu.contract_id;
            db.Files.Add(f);
            db.SaveChanges();
            return Json(f.name, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult UploadX(XModels models)
        {
            var nhiemvu = db.NhiemVus.Find(Convert.ToInt32(models.code));
            var t = models.type;
            var contract = db.Contracts.Find(nhiemvu.contract_id);
            var address = db.Addresses.Find(contract.address_id);
            if (models.files.Count > 0)
            {
                var f0 = models.files[0];
                if (f0 != null)
                {
                    File f = new File();
                    string subPath = Server.MapPath("~/public/" + address.name);
                    bool exists = System.IO.Directory.Exists(subPath);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(subPath);
                    subPath = Server.MapPath("~/public/" + address.name + "/" + contract.name);
                    exists = System.IO.Directory.Exists(subPath);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(subPath);
                    subPath = Server.MapPath("~/public/" + address.name + "/" + contract.name + "/" + XFile.sType[t]);
                    exists = System.IO.Directory.Exists(subPath);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(subPath);
                    f0.SaveAs(subPath + "/" + f0.FileName);
                    f.url = "/public/" + address.name + "/" + contract.name + "/" + XFile.sType[t] + "/" + f0.FileName;
                    f.name = f0.FileName;

                    f.type = t;
                    f.contract_id = nhiemvu.contract_id;
                    db.Files.Add(f);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");

        }
        [Authorize(Roles = "SuperAdmin,Manager")]
        [HttpPost]
        public JsonResult DelFile(int id)
        {
            File f = db.Files.Find(id);
            db.Files.Remove(f);
            db.SaveChanges();
            return Json(f.name, JsonRequestBehavior.AllowGet);
        }
        // GET: NhiemVus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhiemVu nhiemVu = db.NhiemVus.Find(id);
            if (nhiemVu == null)
            {
                return HttpNotFound();
            }
            return View(nhiemVu);
        }

        // GET: NhiemVus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NhiemVus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,contract_id,step_id,ghichu")] NhiemVu nhiemVu)
        {
            if (ModelState.IsValid)
            {
                db.NhiemVus.Add(nhiemVu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nhiemVu);
        }

        // GET: NhiemVus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhiemVu nhiemVu = db.NhiemVus.Find(id);
            if (nhiemVu == null)
            {
                return HttpNotFound();
            }
            return View(nhiemVu);
        }

        // POST: NhiemVus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,contract_id,step_id,ghichu")] NhiemVu nhiemVu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nhiemVu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nhiemVu);
        }

        // GET: NhiemVus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhiemVu nhiemVu = db.NhiemVus.Find(id);
            if (nhiemVu == null)
            {
                return HttpNotFound();
            }
            return View(nhiemVu);
        }

        // POST: NhiemVus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NhiemVu nhiemVu = db.NhiemVus.Find(id);
            db.NhiemVus.Remove(nhiemVu);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult InCongViec(int cid, int sid)
        {
            var sv = db.Services.Find(sid);
            var ct = db.Contracts.Find(cid);
            var add = db.Addresses.Find(ct.address_id);
            var lstCVIDS = db.fk_congviec_service.Where(x => x.service_id == sv.id).Select(x => x.congviec_id);
            var lstCVs = lstCVIDS != null ? db.CongViecs.Where(x => lstCVIDS.Contains(x.id)) : null;
            var lstCT = new List<BanInCongViec>();
            var done = db.fk_congviec_hoso.Where(x => x.hoso_id == ct.id && x.status == (int)XModels.eStatus.Complete).Select(x => x.congviec_id);
            foreach (var cate in XCongViec.sCategory)
            {
                foreach (var cv in lstCVs.Where(x => x.category_id == cate.Key))
                {
                    lstCT.Add(new BanInCongViec() { isCheck = done.Contains(cv.id) ? "X" : null, CategoryName = cate.Value, Name = cv.name });
                }
            }
            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/InCongViec.xls");
            string webpart = "InCongViec" + cid + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            using (System.IO.FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (System.IO.FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("HOPDONG", ct.name);
                    flexCelReport.SetValue("DIACHI", add.name);
                    flexCelReport.AddTable("C", XCongViec.sCategory);
                    flexCelReport.AddTable("D", lstCT);
                    flexCelReport.AddRelationship("C", "D", "Value", "CategoryName");
                    flexCelReport.Run(sr, fs);
                }
            }
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
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
        public JsonResult UpdateProcessing(int contract_id, string txt)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            wf_contract wf = new wf_contract();
            wf.contract_id = contract_id;
            wf.account_id = us.id;
            wf.time = DateTime.Now;
            wf.status = (int)XModels.eStatus.Processing;
            wf.note = txt;

            var backWF = db.wf_contract.Where(x => x.contract_id == contract_id).OrderByDescending(x => x.time).FirstOrDefault();
            wf.step_id = backWF.step_id;
            wf.from_id = backWF.from_id;

            db.wf_contract.Add(wf);
            db.SaveChanges();
            string result = string.Empty;
            var processing = db.wf_contract.Where(x => x.contract_id == contract_id);
            foreach (var item in processing)
            {
                var user = db.Accounts.Single(x => x.id == item.account_id);
                result += "<li><span>" + item.time.ToString("dd /MM/yy") + "</span> | <span>" + user.fullname + "</span> | " + item.note + "</li>";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Index(int? id, int? step)
        {
            if (id == null || step == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var nv = db.NhiemVus.Where(x => x.contract_id == id && x.step_id == step).FirstOrDefault();
            if (nv == null)
            {
                nv = new NhiemVu();
                nv.step_id = step.Value;
                nv.contract_id = id.Value;
                db.NhiemVus.Add(nv);
                db.SaveChanges();
            }
            ViewBag.ID = nv.id;
            var contract = db.Contracts.Find(id);
            var ostep = db.Steps.Find(step);
            var address = db.Addresses.Find(contract.address_id);
            ViewBag.Address = address;
            ViewBag.Step = ostep;
            ViewBag.Contract = contract;
            //var sid = db.fk_contract_service.Where(x => x.contract_id == id).First().service_id;
            var service = db.Services.Find(contract.service_id);
            ViewBag.Service = service;
            ViewBag.db = db;

            var fs = db.Files.Where(x => x.contract_id == id);
            string HSTiepNhan = string.Empty;
            Dictionary<int, string> FILES = new Dictionary<int, string>();
            foreach (var per in XFile.sType)
            {
                string temp = string.Empty;
                foreach (var item in fs.Where(x => x.type == per.Key))
                {
                    temp += "<a href='" + item.url + "' target='_blank' >" + item.name + "</a><span id='" + item.id + "' class='del'>x</span> | ";
                }
                FILES.Add(per.Key, temp);
            }
            ViewBag.FILES = FILES;
            ViewBag.ONEDOORFILES = db.OneDoorFiles.Where(x => x.hoso_id == contract.id).ToArray();
            if (ostep.code.Equals("NOP1CUA"))
            {
                var odi1cua = db.di1cua.Where(x => x.hoso_id == id).SingleOrDefault();
                if (odi1cua == null)
                {
                    odi1cua = new di1cua();
                    odi1cua.hoso_id = id.Value;
                    odi1cua.ngaynop = DateTime.Now;
                    odi1cua.trangthai = (int)Xdi1cua.eStatus.ChoNop;
                    odi1cua.ngaytra = DateTime.Now.AddDays(30);
                    var canbo = db.CanBoes.Where(x => x.motcua == (int)XModels.eYesNo.Yes).FirstOrDefault();
                    odi1cua.canbo1cua = canbo != null ? canbo.id : db.CanBoes.First().id;
                    odi1cua.type = contract.type;
                    odi1cua.contract_id = contract.id;
                    db.di1cua.Add(odi1cua);
                    db.SaveChanges();
                }
                ViewBag.NOP1CUA = odi1cua;
                //var lstCT = from ct in db.Contracts
                //            join cus in db.Customers on ct.customer_id equals cus.id
                //            join add in db.Addresses on ct.address_id equals add.id
                //            where (ct.status == (int)XContract.eStatus.Processing && (ct.workflow == null || ct.workflow == (int)XContract.eWorkflow.DiaChinh) || ct.id == id)
                //            select new { id = ct.id.ToString(), name = cus.name + " - " + add.name };
                //ViewBag.lstCT = new SelectList(lstCT, "id", "name", contract.id);
                ViewBag.CanBo = new SelectList(db.CanBoes.ToArray(), "id", "name", odi1cua.canbo1cua);
            }
            return View(nv);
        }
    }
}
