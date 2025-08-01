using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;
using PagedList;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace NiceHandles.Controllers
{
    public class CuocHopsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: CuocHops
        [HttpPost]
        public ActionResult Index(string Search_Data, int? status, int? type, int? Page_No)
        {
            return GetIndex(Search_Data, status, type, string.Empty, Page_No);
        }
        [HttpGet]
        public ActionResult Index(string Search_Data, int? status, int? type, string Filter_Value, int? Page_No)
        {
            //if (!Page_No.HasValue)
            //{
            //    if (!status.HasValue)
            //        status = (int)XCuocHop.eStatus.Cho;
            //    if (!type.HasValue)
            //        type = (int)XCuocHop.eLoai.HopNguonGoc;
            //}
            return GetIndex(Search_Data, status, type, Filter_Value, Page_No);
        }
        ActionResult GetIndex(string Search_Data, int? status, int? type, string Filter_Value, int? Page_No)
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
            ViewBag.status = status;
            ViewBag.type = type;
            var results = from v in db.CuocHops
                          where (
                          (string.IsNullOrEmpty(v.name) || v.name.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : v.name.ToUpper()))
                          && v.status == (status.HasValue ? status.Value : v.status)
                          && v.type == (type.HasValue ? type.Value : v.type)
                          )
                          orderby v.status, v.mucdo descending, v.time
                          select v;
            int Size_Of_Page = 30;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        // GET: CuocHops/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuocHop cuocHop = db.CuocHops.Find(id);
            if (cuocHop == null)
            {
                return HttpNotFound();
            }
            return View(cuocHop);
        }

        // GET: CuocHops/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var cuochop = new CuocHop();
            cuochop.hoso_id = id.Value;
            return View(cuochop);
        }

        // POST: CuocHops/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CuocHop cuocHop, HttpPostedFileBase[] tailieuthamkhao)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var hoso = db.HoSoes.Find(cuocHop.hoso_id);
            cuocHop.name = XCuocHop.sLoai[cuocHop.type] + " " + hoso.name;
            cuocHop.taoboi = us.id;
            cuocHop.time = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (tailieuthamkhao.Length > 0)
                {
                    var file = tailieuthamkhao[0];
                    if (file != null)
                    {
                        string filename = DateTime.Now.ToString("sshhmmddMMyy") + file.FileName;
                        file.SaveAs(Server.MapPath("~/public/cuochop/") + filename);
                        cuocHop.tailieuthamkhao = "/public/cuochop/" + filename;
                    }
                }
                db.CuocHops.Add(cuocHop);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cuocHop);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(CuocHop cuocHop)
        {
            var obj = db.CuocHops.Find(cuocHop.id);
            obj.mucdo = cuocHop.mucdo;
            obj.vande = cuocHop.vande;
            obj.ketqua = cuocHop.ketqua;
            obj.ghichu = cuocHop.ghichu;
            //obj.time_duyet = cuocHop.time_duyet;
            //obj.status = cuocHop.status;
            db.SaveChanges();
            return View(obj);
        }
        [HttpPost]
        public ActionResult UploadTaiLieuThamKhao(int hdid, HttpPostedFileBase[] tailieuthamkhao)
        {
            var cuocHop = db.CuocHops.Find(hdid);
            if (tailieuthamkhao.Length > 0)
            {
                var file = tailieuthamkhao[0];
                if (file != null)
                {
                    string filename = DateTime.Now.ToString("sshhmmddMMyy") + file.FileName;
                    file.SaveAs(Server.MapPath("~/public/cuochop/") + filename);
                    cuocHop.tailieuthamkhao = "/public/cuochop/" + filename;
                }
            }
            db.SaveChanges();
            return RedirectToAction("details", new { id = hdid });
        }
        [HttpPost]
        public ActionResult UploadBienBan(int hdid, HttpPostedFileBase[] bienban)
        {
            var cuocHop = db.CuocHops.Find(hdid);
            if (bienban.Length > 0)
            {
                var file = bienban[0];
                if (file != null)
                {
                    string filename = DateTime.Now.ToString("sshhmmddMMyy") + file.FileName;
                    file.SaveAs(Server.MapPath("~/public/cuochop/") + filename);
                    cuocHop.bienban = "/public/cuochop/" + filename;
                }
            }
            db.SaveChanges();
            return RedirectToAction("details", new { id = hdid });
        }

        // GET: CuocHops/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuocHop cuocHop = db.CuocHops.Find(id);
            if (cuocHop == null)
            {
                return HttpNotFound();
            }
            return View(cuocHop);
        }

        // POST: CuocHops/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CuocHop cuocHop, string str_account)
        {
            if (ModelState.IsValid)
            {
                var lst = str_account.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                var fk = db.NguoiHops.Where(x => x.cuochop_id == cuocHop.id);
                db.NguoiHops.RemoveRange(fk);
                foreach (var item in lst)
                {
                    var sv = new NguoiHop();
                    sv.cuochop_id = cuocHop.id;
                    sv.account_id = Convert.ToInt32(item);
                    db.NguoiHops.Add(sv);
                }

                db.Entry(cuocHop).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cuocHop);
        }

        // GET: CuocHops/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CuocHop cuocHop = db.CuocHops.Find(id);
            if (cuocHop == null)
            {
                return HttpNotFound();
            }
            return View(cuocHop);
        }

        // POST: CuocHops/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CuocHop cuocHop = db.CuocHops.Find(id);
            db.CuocHops.Remove(cuocHop);
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
        public JsonResult GhiBienBanCuocHop(int? id)
        {
            CuocHop cuocHop = db.CuocHops.Find(id);
            HoSo hoSo = db.HoSoes.Find(cuocHop.hoso_id);
            string path = Server.MapPath("~/public/cuochop/" + XCuocHop.sLoai[cuocHop.type] + cuocHop.id + "bienban.docx");
            string pathTemp = Server.MapPath("~/App_Data/templates/BBCuocHop.doc");
            Aspose.Words.Document doc = new Aspose.Words.Document(pathTemp);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            builder.MoveToMergeField("loai");
            builder.InsertHtml(XCuocHop.sLoai[cuocHop.type]);
            builder.MoveToMergeField("hoso_name");
            builder.InsertHtml(string.IsNullOrEmpty(hoSo.name) ? "" : hoSo.name);
            builder.MoveToMergeField("mucdich");
            builder.InsertHtml(string.IsNullOrEmpty(cuocHop.vande) ? "" : cuocHop.vande);
            builder.MoveToMergeField("nguongoc");
            builder.InsertHtml(string.IsNullOrEmpty(cuocHop.ketqua) ? "" : cuocHop.ketqua);
            builder.MoveToMergeField("ghichu");
            builder.InsertHtml(string.IsNullOrEmpty(cuocHop.ghichu) ? "" : cuocHop.ghichu);
            doc.Save(path);
            return Json("/public/cuochop/" + XCuocHop.sLoai[cuocHop.type] + cuocHop.id + "bienban.docx", JsonRequestBehavior.AllowGet);
        }
        public JsonResult StatusChange(int id, int sta)
        {
            CuocHop cuocHop = db.CuocHops.Find(id);
            cuocHop.status = sta;
            if (sta == (int)XCuocHop.eStatus.Duyet)
            {
                cuocHop.time_duyet = DateTime.Now;
            }
            db.SaveChanges();
            return Json(sta, JsonRequestBehavior.AllowGet);
        }

    }
}
