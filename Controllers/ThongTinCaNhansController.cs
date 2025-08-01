using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    public class ThongTinCaNhansController : Controller
    {
        private NHModel db = new NHModel();

        // GET: ThongTinCaNhans
        public ActionResult Index()
        {
            return View(db.ThongTinCaNhans.ToList());
        }
        [HttpPost]
        public JsonResult DanhSach(int id)
        {
            string result = string.Empty;
            var nk = db.ThongTinCaNhans.Where(x => x.sohokhau_id == id).ToArray();

            foreach (var item in nk)
            {
                result +=
                      "<tr id='" + item.id + "'>" +
                      "     <td>" + item.hoten +
                      "     </td><td>" + (item.ngaysinh.HasValue ? item.ngaysinh.Value.ToString("yyyy") : null) +
                      "     </td><td>" + item.ghichuquanhe +

                      "     </td><td>" + item.loaigiayto +
                      "     </td><td>" + item.sogiayto +
                      "     </td><td>" + item.noicap_gt +
                      "     </td><td>" + (item.ngaycap_gt.HasValue ? item.ngaycap_gt.Value.ToString("dd/MM/yyyy") : null) +

                      "     </td><td>" +
                      "         <a href='#' onclick='EditNhanKhau(" + item.id + ")'>Sửa</a> |" +
                      "         <a href='#' onclick='XoaNhanKhau(" + item.id + ")'>Xóa</a>" +
                      "     </td>" +
                      "</tr>";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: ThongTinCaNhans/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThongTinCaNhan thongTinCaNhan = db.ThongTinCaNhans.Find(id);
            if (thongTinCaNhan == null)
            {
                return HttpNotFound();
            }
            return View(thongTinCaNhan);
        }
        public ActionResult DetailsX(int? id)
        {
            ThongTinCaNhan thongTinCaNhan = db.ThongTinCaNhans.Find(id);

            XThongTinCaNhan xThongTinCaNhan = new XThongTinCaNhan();
            xThongTinCaNhan.obj = thongTinCaNhan;
            xThongTinCaNhan.ngaysinh = thongTinCaNhan.ngaysinh.HasValue ? thongTinCaNhan.ngaysinh.Value.ToString("dd/MM/yyyy") : null;
            xThongTinCaNhan.ngaycap_gt = thongTinCaNhan.ngaycap_gt.HasValue ? thongTinCaNhan.ngaycap_gt.Value.ToString("dd/MM/yyyy") : null;
            xThongTinCaNhan.ngaysinh1 = thongTinCaNhan.ngaysinh1.HasValue ? thongTinCaNhan.ngaysinh1.Value.ToString("dd/MM/yyyy") : null;
            xThongTinCaNhan.ngaycap_gt1 = thongTinCaNhan.ngaycap_gt1.HasValue ? thongTinCaNhan.ngaycap_gt1.Value.ToString("dd/MM/yyyy") : null;
            xThongTinCaNhan.ngaychet = thongTinCaNhan.ngaychet.HasValue ? thongTinCaNhan.ngaychet.Value.ToString("dd/MM/yyyy") : null;
            xThongTinCaNhan.ngaychet1 = thongTinCaNhan.ngaychet1.HasValue ? thongTinCaNhan.ngaychet1.Value.ToString("dd/MM/yyyy") : null;

            return Json(xThongTinCaNhan, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ActionName("DeleteX")]
        public ActionResult DeleteX(int id)
        {
            ThongTinCaNhan thongTinCaNhan = db.ThongTinCaNhans.Find(id);
            db.ThongTinCaNhans.Remove(thongTinCaNhan);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        // GET: ThongTinCaNhans/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ThongTinCaNhans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,infomation_id,hoten,ngaysinh,loaigiayto,sogiayto,gioitinh,quoctich,hktt,ngaycap_gt,noicap_gt,masothue,marker,note,type")] ThongTinCaNhan thongTinCaNhan)
        {
            if (ModelState.IsValid)
            {
                db.ThongTinCaNhans.Add(thongTinCaNhan);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(thongTinCaNhan);
        }

        [HttpPost]
        public JsonResult CreateX(ThongTinCaNhan thongTinCaNhan, int? id, string ngaycap)
        {
            string result = string.Empty;
            if (!id.HasValue)
            {
                db.ThongTinCaNhans.Add(thongTinCaNhan);
            }
            else
            {
                thongTinCaNhan.id = id.Value;
                db.Entry(thongTinCaNhan).State = EntityState.Modified;
            }

            if (!string.IsNullOrEmpty(ngaycap)) thongTinCaNhan.ngaycap_gt = Convert.ToDateTime(ngaycap);
            db.SaveChanges();
            result = "<tr id='" + thongTinCaNhan.id + "'><td>" + thongTinCaNhan.hoten +
                "</td><td>" + thongTinCaNhan.gioitinh +
                "</td><td>" + (thongTinCaNhan.ngaysinh.HasValue ? thongTinCaNhan.ngaysinh.Value.ToString("dd/MM/yyyy") : null) +
                "</td><td>" + thongTinCaNhan.sogiayto +
                "</td><td>" + (thongTinCaNhan.ngaycap_gt.HasValue ? thongTinCaNhan.ngaycap_gt.Value.ToString("dd/MM/yyyy") : null) +
                "</td><td>" + thongTinCaNhan.noicap_gt +
                "</td><td>" +
                "<a href='#' data-toggle='modal' data-target='#ChuSoHuuModal' onclick='EditChuSoHuu(" + thongTinCaNhan.id + ")'>Sửa</a> |" +
                "<a href='#' onclick='XoaChuSoHuu(" + thongTinCaNhan.id + ")'>Xóa</a>" +
                "</td></tr>";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveChuDat(ThongTinCaNhan ttcn, int? id, string ngaysinh, string ngaycap_gt, string ngaychet, string ngaysinh1, string ngaycap_gt1, string ngaychet1)
        {
            var old = db.ThongTinCaNhans.Where(x => x.infomation_id == ttcn.infomation_id && x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.ChuDat).SingleOrDefault();
            if (old != null)
            {
                old.hoten = ttcn.hoten;
                old.gioitinh = ttcn.gioitinh;
                old.loaigiayto = ttcn.loaigiayto;
                old.sogiayto = ttcn.sogiayto;
                old.noicap_gt = ttcn.noicap_gt;
                old.giaytochet = ttcn.giaytochet;

                old.hoten1 = ttcn.hoten1;
                old.gioitinh1 = ttcn.gioitinh1;
                old.loaigiayto1 = ttcn.loaigiayto1;
                old.sogiayto1 = ttcn.sogiayto1;
                old.noicap_gt1 = ttcn.noicap_gt1;
                old.giaytochet1 = ttcn.giaytochet1;

                if (!string.IsNullOrEmpty(ngaysinh))
                    old.ngaysinh = Convert.ToDateTime(ngaysinh);
                if (!string.IsNullOrEmpty(ngaycap_gt))
                    old.ngaycap_gt = Convert.ToDateTime(ngaycap_gt);
                if (!string.IsNullOrEmpty(ngaychet))
                    old.ngaychet = Convert.ToDateTime(ngaychet);
                if (!string.IsNullOrEmpty(ngaysinh1))
                    old.ngaysinh1 = Convert.ToDateTime(ngaysinh1);
                if (!string.IsNullOrEmpty(ngaycap_gt1))
                    old.ngaycap_gt1 = Convert.ToDateTime(ngaycap_gt1);
                if (!string.IsNullOrEmpty(ngaychet1))
                    old.ngaychet1 = Convert.ToDateTime(ngaychet1);
                db.Entry(old).State = EntityState.Modified;
            }
            else
            {
                ttcn.hangthuake = (int)XThongTinCaNhan.eHangThuaKe.ChuDat;
                if (!string.IsNullOrEmpty(ngaysinh))
                    ttcn.ngaysinh = Convert.ToDateTime(ngaysinh);
                if (!string.IsNullOrEmpty(ngaycap_gt))
                    ttcn.ngaycap_gt = Convert.ToDateTime(ngaycap_gt);
                if (!string.IsNullOrEmpty(ngaychet))
                    ttcn.ngaychet = Convert.ToDateTime(ngaychet);
                if (!string.IsNullOrEmpty(ngaysinh1))
                    ttcn.ngaysinh1 = Convert.ToDateTime(ngaysinh1);
                if (!string.IsNullOrEmpty(ngaycap_gt1))
                    ttcn.ngaycap_gt1 = Convert.ToDateTime(ngaycap_gt1);
                if (!string.IsNullOrEmpty(ngaychet1))
                    ttcn.ngaychet1 = Convert.ToDateTime(ngaychet1);
                db.ThongTinCaNhans.Add(ttcn);
            }
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveHangThuaKe(ThongTinCaNhan ttcn, int? id, string ngaysinh, string ngaycap_gt, string ngaychet, string ngaysinh1, string ngaycap_gt1, string ngaychet1)
        {
            if (!string.IsNullOrEmpty(ngaysinh))
                ttcn.ngaysinh = Convert.ToDateTime(ngaysinh);
            if (!string.IsNullOrEmpty(ngaycap_gt))
                ttcn.ngaycap_gt = Convert.ToDateTime(ngaycap_gt);
            if (!string.IsNullOrEmpty(ngaychet))
                ttcn.ngaychet = Convert.ToDateTime(ngaychet);
            if (!string.IsNullOrEmpty(ngaysinh1))
                ttcn.ngaysinh1 = Convert.ToDateTime(ngaysinh1);
            if (!string.IsNullOrEmpty(ngaycap_gt1))
                ttcn.ngaycap_gt1 = Convert.ToDateTime(ngaycap_gt1);
            if (!string.IsNullOrEmpty(ngaychet1))
                ttcn.ngaychet1 = Convert.ToDateTime(ngaychet1);
            if (id.HasValue)
            {
                ttcn.id = id.Value;
                db.Entry(ttcn).State = EntityState.Modified;
            }
            else
            {
                db.ThongTinCaNhans.Add(ttcn);
            }
            db.SaveChanges();
            string result =
                "<tr id='" + ttcn.id + "'>" +
                "   <td>" + ttcn.hoten +
                "   </td><td>" + (ttcn.ngaysinh.HasValue ? ttcn.ngaysinh.Value.ToString("dd/MM/yyyy") : null) +
                "   </td><td>" + (ttcn.ngaychet.HasValue ? ttcn.ngaychet.Value.ToString("dd/MM/yyyy") : null) +
                "   </td><td>" + (ttcn.marker.HasValue && ttcn.marker.Value == (int)XModels.eYesNo.Yes ? "Đại diện" : "---") +
                "   </td><td>" + (ttcn.songtrendat.HasValue && ttcn.songtrendat.Value == (int)XModels.eYesNo.Yes ? "Ở trên đất" : "Không ở trên đất") +
                "   </td><td>" + (ttcn.quanhe.HasValue ? XThongTinCaNhan.sQuanHe[ttcn.quanhe.Value] : null) +
                "   </td><td>" +
                "   <a href='#' data-toggle='modal' data-target='#HangThuaKe1Modal' onclick='EditHangThuaKe1(" + ttcn.id + ")'>Sửa</a> |" +
                "   <a href='#' onclick='XoaHangThuaKe1(" + ttcn.id + ")'>Xóa</a>" +
                "   </td>" +
                "</tr>";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveHangThuaKe2(ThongTinCaNhan ttcn, int? id, string ngaysinh, string ngaycap_gt, string ngaychet, string ngaysinh1, string ngaycap_gt1, string ngaychet1)
        {
            if (!string.IsNullOrEmpty(ngaysinh))
                ttcn.ngaysinh = Convert.ToDateTime(ngaysinh);
            if (!string.IsNullOrEmpty(ngaycap_gt))
                ttcn.ngaycap_gt = Convert.ToDateTime(ngaycap_gt);
            if (!string.IsNullOrEmpty(ngaychet))
                ttcn.ngaychet = Convert.ToDateTime(ngaychet);
            if (!string.IsNullOrEmpty(ngaysinh1))
                ttcn.ngaysinh1 = Convert.ToDateTime(ngaysinh1);
            if (!string.IsNullOrEmpty(ngaycap_gt1))
                ttcn.ngaycap_gt1 = Convert.ToDateTime(ngaycap_gt1);
            if (!string.IsNullOrEmpty(ngaychet1))
                ttcn.ngaychet1 = Convert.ToDateTime(ngaychet1);
            if (id.HasValue)
            {
                ttcn.id = id.Value;
                db.Entry(ttcn).State = EntityState.Modified;
            }
            else
            {
                db.ThongTinCaNhans.Add(ttcn);
            }
            db.SaveChanges();
            string result =
                "<tr id='" + ttcn.id + "'>" +
                "   <td>" + ttcn.hoten +
                "   </td><td>" + (ttcn.ngaysinh.HasValue ? ttcn.ngaysinh.Value.ToString("dd/MM/yyyy") : null) +
                "   </td><td>" + (ttcn.ngaychet.HasValue ? ttcn.ngaychet.Value.ToString("dd/MM/yyyy") : null) +
                "   </td><td>" + (ttcn.quanhe.HasValue ? XThongTinCaNhan.sQuanHe[ttcn.quanhe.Value] : null) +
                "   </td><td>" + (ttcn.marker.HasValue && ttcn.marker.Value == (int)XModels.eYesNo.Yes ? "Đại diện" : "---") +
                "   </td><td>" + (ttcn.songtrendat.HasValue && ttcn.songtrendat.Value == (int)XModels.eYesNo.Yes ? "Ở trên đât" : "Không ở trên đất") +
                "   </td><td>" + (ttcn.fk_id.HasValue ? db.ThongTinCaNhans.Find(ttcn.fk_id).hoten : null) +
                "   </td><td>" +
                "   <a href='#' data-toggle='modal' data-target='#HangThuaKe2Modal' onclick='EditHangThuaKe2(" + ttcn.id + ")'>Sửa</a> |" +
                "   <a href='#' onclick='XoaHangThuaKe2(" + ttcn.id + ")'>Xóa</a>" +
                "   </td>" +
                "</tr>";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveNhanKhau(ThongTinCaNhan ttcn, int? id, string ngaysinh)
        {
            if (!string.IsNullOrEmpty(ngaysinh))
                ttcn.ngaysinh = Convert.ToDateTime(ngaysinh);
            if (id.HasValue)
            {
                ttcn.id = id.Value;
                db.Entry(ttcn).State = EntityState.Modified;
            }
            else
            {
                db.ThongTinCaNhans.Add(ttcn);
            }
            db.SaveChanges();
            string result =
                "<tr id='" + ttcn.id + "'>" +
                "   <td>" + ttcn.hoten +
                "   </td><td>" + (ttcn.ngaysinh.HasValue ? ttcn.ngaysinh.Value.ToString("dd/MM/yyyy") : null) +
                "   </td><td>" + ttcn.ghichuquanhe +

                "   </td><td>" + ttcn.loaigiayto +
                "   </td><td>" + ttcn.sogiayto +
                "   </td><td>" + ttcn.noicap_gt +
                "   </td><td>" + (ttcn.ngaycap_gt.HasValue ? ttcn.ngaycap_gt.Value.ToString("dd/MM/yyyy") : null) +

                "   </td><td>" +
                "       <a href='#' onclick='EditNhanKhau(" + ttcn.id + ")'>Sửa</a> |" +
                "       <a href='#' onclick='XoaNhanKhau(" + ttcn.id + ")'>Xóa</a>" +
                "   </td>" +
                "</tr>";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // GET: ThongTinCaNhans/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThongTinCaNhan thongTinCaNhan = db.ThongTinCaNhans.Find(id);
            if (thongTinCaNhan == null)
            {
                return HttpNotFound();
            }
            return View(thongTinCaNhan);
        }

        // POST: ThongTinCaNhans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,infomation_id,hoten,ngaysinh,loaigiayto,sogiayto,gioitinh,quoctich,hktt,ngaycap_gt,noicap_gt,masothue,marker,note,type")] ThongTinCaNhan thongTinCaNhan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thongTinCaNhan).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thongTinCaNhan);
        }

        // GET: ThongTinCaNhans/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ThongTinCaNhan thongTinCaNhan = db.ThongTinCaNhans.Find(id);
            if (thongTinCaNhan == null)
            {
                return HttpNotFound();
            }
            return View(thongTinCaNhan);
        }

        // POST: ThongTinCaNhans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ThongTinCaNhan thongTinCaNhan = db.ThongTinCaNhans.Find(id);
            db.ThongTinCaNhans.Remove(thongTinCaNhan);
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
        public JsonResult UpdateX(int id, string val, string prop)
        {
            var ttcn = db.ThongTinCaNhans.Find(id);

            switch (prop)
            {
                case "hoten":
                    ttcn.hoten = val;
                    break;
                case "hoten1":
                    ttcn.hoten1 = val;
                    break;
                case "ngaysinh":
                    ttcn.ngaysinh = Convert.ToDateTime(val);
                    break;
                case "ngaychet":
                    ttcn.ngaychet = Convert.ToDateTime(val);
                    break;
                case "ngaysinh1":
                    ttcn.ngaysinh1 = Convert.ToDateTime(val);
                    break;
                case "ngaychet1":
                    ttcn.ngaychet1 = Convert.ToDateTime(val);
                    break;
                case "quanhe":
                    ttcn.quanhe = Convert.ToInt32(val);
                    break;
                default:
                    break;
            }

            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult InsertX(int id, int quanhe)
        {
            var ttcn = db.ThongTinCaNhans.Find(id);
            var con = new ThongTinCaNhan();
            con.fk_id = ttcn.id;
            con.quanhe = quanhe;
            con.type = (int)XThongTinCaNhan.eType.LanDau;
            if (ttcn.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.ChuDat)
                con.hangthuake = (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1;
            else
                con.hangthuake = (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe2;
            con.infomation_id = ttcn.infomation_id;
            db.ThongTinCaNhans.Add(con);

            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}
