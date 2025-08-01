using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Aspose.Words;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    public class DuChisController : Controller
    {
        private NHModel db = new NHModel();

        // GET: DuChis
        public ActionResult Index()
        {
            return View(db.vDuChis.OrderBy(x => x.name).ToList());
        }

        // GET: DuChis/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DuChi duChi = db.DuChis.Find(id);
            if (duChi == null)
            {
                return HttpNotFound();
            }
            return View(duChi);
        }

        // GET: DuChis/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DuChis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,category_id,amount")] DuChi duChi)
        {
            if (ModelState.IsValid)
            {
                var count = db.DuChis.Count(x => x.category_id == duChi.category_id);
                if (count == 0)
                {
                    db.DuChis.Add(duChi);
                }
                else
                {
                    var obj = db.DuChis.Single(x => x.category_id == duChi.category_id);
                    obj.amount = duChi.amount;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(duChi);
        }
        [HttpPost]
        public JsonResult ChuyenThucHien(int id, int account_id)
        {
            var nhatky = db.DuChiNhatKies.SingleOrDefault(x => x.inout_id == id);
            if (nhatky == null)
            {
                nhatky = new DuChiNhatKy();
                nhatky.inout_id = id;
                db.DuChiNhatKies.Add(nhatky);
            }
            nhatky.status = (int)XDuChiNhatKy.eStatus.DiThucHien;
            nhatky.time = DateTime.Now;
            nhatky.account_id = account_id;
            db.SaveChanges();

            var wf = new DuChiNhatKyWF();
            wf.duchinhatky_id = nhatky.id;
            wf.account_id = account_id;
            wf.note = "Giao đi thực hiện";
            wf.status = nhatky.status;
            wf.time = DateTime.Now;
            db.DuChiNhatKyWFs.Add(wf);

            db.SaveChanges();
            return Json(nhatky, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DiThucHien(int id)
        {
            var nhatky = db.DuChiNhatKies.SingleOrDefault(x => x.inout_id == id);
            nhatky.status = (int)XDuChiNhatKy.eStatus.DaDi;
            nhatky.time = DateTime.Now;
            db.SaveChanges();

            var wf = new DuChiNhatKyWF();
            wf.duchinhatky_id = nhatky.id;
            wf.account_id = nhatky.account_id;
            wf.note = "Đã thực hiện";
            wf.status = nhatky.status;
            wf.time = DateTime.Now;
            db.DuChiNhatKyWFs.Add(wf);

            db.SaveChanges();
            return Json(nhatky, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult HoanTra(int id)
        {
            var nhatky = db.DuChiNhatKies.SingleOrDefault(x => x.inout_id == id);
            nhatky.status = (int)XDuChiNhatKy.eStatus.LuongGiu;
            nhatky.time = DateTime.Now;
            db.SaveChanges();

            var wf = new DuChiNhatKyWF();
            wf.duchinhatky_id = nhatky.id;
            wf.account_id = nhatky.account_id;
            wf.note = "Chưa thực hiện trả lại quỹ";
            wf.status = nhatky.status;
            wf.time = DateTime.Now;
            db.DuChiNhatKyWFs.Add(wf);

            db.SaveChanges();
            return Json(nhatky, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult LuuGhiChu(int id, string note)
        {
            var nhatky = db.DuChiNhatKies.SingleOrDefault(x => x.inout_id == id);
            if (nhatky == null)
            {
                nhatky = new DuChiNhatKy();
                nhatky.inout_id = id;
                db.DuChiNhatKies.Add(nhatky);
            }
            nhatky.note = note;
            nhatky.time = DateTime.Now;
            db.SaveChanges();

            var wf = new DuChiNhatKyWF();
            wf.duchinhatky_id = nhatky.id;
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            wf.account_id = us.id;
            wf.note = "Ghi chú: " + note;
            wf.status = nhatky.status;
            wf.time = DateTime.Now;
            db.DuChiNhatKyWFs.Add(wf);
            db.SaveChanges();

            return Json(nhatky, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult TraLaiDuChi(int id)
        {
            var nhatky = db.DuChiNhatKies.SingleOrDefault(x => x.inout_id == id);
            if (nhatky == null)
            {
                nhatky = new DuChiNhatKy();
                nhatky.inout_id = id;
                db.DuChiNhatKies.Add(nhatky);
            }
            nhatky.status = (int)XDuChiNhatKy.eStatus.TraLai;
            nhatky.time = DateTime.Now;
            var ioTraLai = db.InOuts.Find(id);
            var cateTraLai = db.Categories.Find(ioTraLai.category_id);
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            // Fix tiền trả lại
            var cate = db.Categories.Find(40);
            var obj = new InOut();
            obj.created_by = us.id;
            obj.amount = ioTraLai.amount;
            obj.code = Utils.SetCodeInout(cate);
            obj.time = DateTime.Now;
            obj.account_id = us.id;
            obj.category_id = cate.id;
            obj.contract_id = ioTraLai.contract_id;
            obj.currency = (int)XModels.eLoaiTien.TienMat;
            obj.note = "Trả lại tiền công thực hiện(" + cateTraLai.name + ")";
            obj.status = (int)XInOut.eStatus.DaThucHien;
            obj.type = (int)XCategory.eType.Thu;
            db.InOuts.Add(obj);
            db.SaveChanges();

            var wf = new DuChiNhatKyWF();
            wf.duchinhatky_id = nhatky.id;
            wf.account_id = nhatky.account_id;
            wf.note = obj.note;
            wf.status = nhatky.status;
            wf.time = DateTime.Now;
            db.DuChiNhatKyWFs.Add(wf);

            db.SaveChanges();

            return Json(nhatky, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ViewHistory(int id)
        {
            var nhatky = db.DuChiNhatKyWFs.Where(x => x.duchinhatky_id == id).ToArray();
            return PartialView("../InOutChoDuyets/DSLichSuChi", nhatky);
        }

        // GET: DuChis/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DuChi duChi = db.DuChis.Find(id);
            if (duChi == null)
            {
                return HttpNotFound();
            }
            return View(duChi);
        }

        // POST: DuChis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,category_id,amount")] DuChi duChi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(duChi).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(duChi);
        }

        // GET: DuChis/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DuChi duChi = db.DuChis.Find(id);
            if (duChi == null)
            {
                return HttpNotFound();
            }
            return View(duChi);
        }

        // POST: DuChis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DuChi duChi = db.DuChis.Find(id);
            db.DuChis.Remove(duChi);
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
