using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    public class TachThuasController : Controller
    {
        private NHModel db = new NHModel();

        // GET: TachThuas
        public ActionResult Index()
        {
            return View(db.TachThuas.ToList());
        }

        // GET: TachThuas/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TachThua tachThua = db.TachThuas.Find(id);
            if (tachThua == null)
            {
                return HttpNotFound();
            }
            return View(tachThua);
        }
        [HttpPost]
        public JsonResult DetailsX(int? id)
        {
            TachThua tachThua = db.TachThuas.Find(id);
            XTachThua xtd = new XTachThua();
            xtd.obj = tachThua;
            xtd.b_ngaycap = tachThua.b_ngaycap.HasValue ? tachThua.b_ngaycap.Value.ToString("dd/MM/yyyy") : null;
            xtd.d_ngaycap_gt = tachThua.d_ngaycap_gt.HasValue ? tachThua.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : null;
            xtd.d_ngaycap_gt1 = tachThua.d_ngaycap_gt1.HasValue ? tachThua.d_ngaycap_gt1.Value.ToString("dd/MM/yyyy") : null;

            xtd.d_ngaysinh = tachThua.d_ngaysinh.HasValue ? tachThua.d_ngaysinh.Value.ToString("dd/MM/yyyy") : null;
            xtd.d_ngaysinh1 = tachThua.d_ngaysinh1.HasValue ? tachThua.d_ngaysinh1.Value.ToString("dd/MM/yyyy") : null;
            xtd.d_ngaycongchung = tachThua.d_ngaycongchung.HasValue ? tachThua.d_ngaycongchung.Value.ToString("dd/MM/yyyy") : null;

            return Json(xtd, JsonRequestBehavior.AllowGet);
        }
        // GET: TachThuas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TachThuas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,infomation_id,a_loaigiayto,a_sogiayto,a_hoten,a_ngaysinh,a_gioitinh,a_quoctich,a_nguyenquan,a_hktt,a_thon,a_xa,a_huyen,a_tinh,a_ngaycap_gt,a_noicap_gt,a_masothue,a_loaigiayto1,a_sogiayto1,a_hoten1,a_ngaysinh1,a_gioitinh1,a_quoctich1,a_nguyenquan1,a_hktt1,a_ngaycap_gt1,a_noicap_gt1,b_csh,b_hoten,b_ngaysinh,b_loaigiayto,b_sogiayto,b_diachithuongtru,b_sogiaychungnhan,b_sothua,b_tobando,b_thon,b_xa,b_huyen,b_tinh,b_diachithuadat,b_dientich,b_hinhthucsudung,b_mucdichsudung,b_nguongoc,b_ghichu,b_ngaycap,b_noicap,b_sovaoso,b_loaidat1,b_dientich1,b_loaidat2,b_dientich2,b_loainguongoc,b_chuyenquyen,c_xaphuong,c_xa,c_baocao_veviec,c_dondenghi_noidung,d_sogiayto,d_hoten,d_ngaysinh,d_gioitinh,d_quoctich,d_nguyenquan,d_hktt,d_loaigiayto,d_ngaycap_gt,d_noicap_gt,d_masothue,d_sogiayto1,d_loaigiayto1,d_hoten1,d_ngaysinh1,d_gioitinh1,d_hktt1,d_ngaycap_gt1,d_noicap_gt1,d_loaibiendong,d_noidungbiendong,d_lydobiendong,d_sohopdong,d_noicongchung,d_ngaycongchung,d_tienhopdong,d_vitrithuadat,e_sogiayto,e_hoten,e_ngaysinh,e_gioitinh,e_quoctich,e_nguyenquan,e_hktt,e_ngaycap_gt,e_noicap_gt,e_masothue,e_giayuyquyenso,e_ngayuyquyen,e_doituong")] TachThua tachThua)
        {
            if (ModelState.IsValid)
            {
                db.TachThuas.Add(tachThua);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tachThua);
        }

        [HttpPost]
        public ActionResult CreateX(TachThua tachThua, int? id, string b_ngaycap, string d_ngaycap_gt, string d_ngaycap_gt1)
        {
            string result = string.Empty;
            if (!id.HasValue)
            {
                db.TachThuas.Add(tachThua);
            }
            else
            {
                tachThua.id = id.Value;
                db.Entry(tachThua).State = EntityState.Modified;
            }
            if (!string.IsNullOrEmpty(b_ngaycap)) tachThua.b_ngaycap = Convert.ToDateTime(b_ngaycap);
            if (!string.IsNullOrEmpty(d_ngaycap_gt)) tachThua.d_ngaycap_gt = Convert.ToDateTime(d_ngaycap_gt);
            if (!string.IsNullOrEmpty(d_ngaycap_gt1)) tachThua.d_ngaycap_gt1 = Convert.ToDateTime(d_ngaycap_gt1);
            db.SaveChanges();

            result = "<tr id='" + tachThua.id + "'><td>" + tachThua.b_sothua +
                "</td><td>" + tachThua.b_tobando +
                "</td><td>" + tachThua.b_dientich +
                "</td><td>" + tachThua.d_hoten +
                "</td><td>" + tachThua.d_hoten1 +
                "</td><td>" + tachThua.d_sohopdong +
                "</td><td><a href='#' data-toggle='modal' data-target='#TachThuaModal' onclick='EditTachThua(" + tachThua.id + " )'>Sửa</a> |" +
                "<a href='#' onclick='XoaTachThua(" + tachThua.id + ")'>Xóa</a>" +
                "</td></tr>";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        // GET: TachThuas/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TachThua tachThua = db.TachThuas.Find(id);
            if (tachThua == null)
            {
                return HttpNotFound();
            }
            return View(tachThua);
        }

        // POST: TachThuas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,infomation_id,a_loaigiayto,a_sogiayto,a_hoten,a_ngaysinh,a_gioitinh,a_quoctich,a_nguyenquan,a_hktt,a_thon,a_xa,a_huyen,a_tinh,a_ngaycap_gt,a_noicap_gt,a_masothue,a_loaigiayto1,a_sogiayto1,a_hoten1,a_ngaysinh1,a_gioitinh1,a_quoctich1,a_nguyenquan1,a_hktt1,a_ngaycap_gt1,a_noicap_gt1,b_csh,b_hoten,b_ngaysinh,b_loaigiayto,b_sogiayto,b_diachithuongtru,b_sogiaychungnhan,b_sothua,b_tobando,b_thon,b_xa,b_huyen,b_tinh,b_diachithuadat,b_dientich,b_hinhthucsudung,b_mucdichsudung,b_nguongoc,b_ghichu,b_ngaycap,b_noicap,b_sovaoso,b_loaidat1,b_dientich1,b_loaidat2,b_dientich2,b_loainguongoc,b_chuyenquyen,c_xaphuong,c_xa,c_baocao_veviec,c_dondenghi_noidung,d_sogiayto,d_hoten,d_ngaysinh,d_gioitinh,d_quoctich,d_nguyenquan,d_hktt,d_loaigiayto,d_ngaycap_gt,d_noicap_gt,d_masothue,d_sogiayto1,d_loaigiayto1,d_hoten1,d_ngaysinh1,d_gioitinh1,d_hktt1,d_ngaycap_gt1,d_noicap_gt1,d_loaibiendong,d_noidungbiendong,d_lydobiendong,d_sohopdong,d_noicongchung,d_ngaycongchung,d_tienhopdong,d_vitrithuadat,e_sogiayto,e_hoten,e_ngaysinh,e_gioitinh,e_quoctich,e_nguyenquan,e_hktt,e_ngaycap_gt,e_noicap_gt,e_masothue,e_giayuyquyenso,e_ngayuyquyen,e_doituong")] TachThua tachThua)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tachThua).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tachThua);
        }

        // GET: TachThuas/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TachThua tachThua = db.TachThuas.Find(id);
            if (tachThua == null)
            {
                return HttpNotFound();
            }
            return View(tachThua);
        }

        // POST: TachThuas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            TachThua tachThua = db.TachThuas.Find(id);
            db.TachThuas.Remove(tachThua);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost, ActionName("DeleteX")]
        public ActionResult DeleteX(int id)
        {
            TachThua tachThua = db.TachThuas.Find(id);
            db.TachThuas.Remove(tachThua);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
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
