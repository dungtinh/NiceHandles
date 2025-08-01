using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using Aspose.Words;
using Aspose.Words.Rendering;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;
using PagedList;
using PdfSharp.Charting;
using static System.Net.WebRequestMethods;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Manager,Member")]
    public class ContractsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: Contracts        
        [Authorize(Roles = "SuperAdmin,Manager,Accounting")]
        public ActionResult Index(string Search_Data, int? add, int? acc, int? ser, int? par, int? loai, int? hh, string Filter_Value, int? Page_No, string fromdate, string todate)
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
            ViewBag.ADD = add;
            ViewBag.SER = ser;
            ViewBag.LOAI = loai;
            ViewBag.PAR = par;
            ViewBag.ACC = acc;
            ViewBag.HH = hh;
            ViewBag.FROMDATE = fromdate;
            ViewBag.TODATE = todate;
            DateTime FromDate = string.IsNullOrEmpty(fromdate) ? new DateTime() : DateTime.Parse(fromdate);
            DateTime ToDate = string.IsNullOrEmpty(todate) ? DateTime.Now : DateTime.Parse(todate);
            ToDate = ToDate.AddDays(1);
            int[] partners = new int[0];
            if (hh.HasValue)
            {
                partners = db.Partners.Where(x => x.account_id == hh.Value).Select(x => x.id).ToArray();
            }
            var results = from io in db.vContracts
                          where (
                          (io.loai == (loai.HasValue ? loai.Value : io.loai)) &&
                          (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                          (io.account_id == (acc.HasValue ? acc.Value : io.account_id)) &&
                          (io.address_id == (add.HasValue ? add : io.address_id)) &&
                          io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                          (hh.HasValue ? partners.Contains(io.partner) : true) &&
                          io.time >= FromDate &&
                          io.time < ToDate &&
                          io.status < (int)XContract.eStatus.Cancel &&
                          io.name.ToUpper().Contains((!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.name.ToUpper())
                          ))
                          orderby io.time descending
                          select io;
            ViewBag.db = db;
            int Size_Of_Page = 30;
            int No_Of_Page = (Page_No ?? 1);
            //SUM
            //Đóng mở sumery
            //var cateRose = db.Categories.Single(x => x.kind == (int)XCategory.eKind.Rose);
            //var cateDove = db.Categories.Single(x => x.kind == (int)XCategory.eKind.Dove);
            //var cateRemunerate = db.Categories.Single(x => x.kind == (int)XCategory.eKind.Remunerate);
            //var catePartner = db.Categories.Single(x => x.kind == (int)XCategory.eKind.Partner);

            //var summery = results.Select(x => new { x.id, x.amount, x.consuming, x.dove, x.outrose, x.rose }).ToList();
            //var SumGiaTriHopDong = summery.Sum(x => x.amount);
            //var summeryInout = (from s in summery
            //                    join io in db.InOuts on s.id equals io.contract_id
            //                    where io.status == (int)XInOut.eStatus.DaThucHien
            //                    select new { io.amount, io.type, io.category_id }).ToList();
            //var SumDaThu = summeryInout.Where(x => x.type == (int)XCategory.eType.Thu).Sum(x => x.amount);
            //var SumPhaiThu = SumGiaTriHopDong - SumDaThu;
            //var SumDuChi = summery.Sum(x => x.consuming);
            //var SumHoaHongNV = summeryInout.Where(x => x.type == (int)XCategory.eType.Chi && x.category_id == cateRose.id).Sum(x => x.amount);
            //var SumHoaHongDT = summeryInout.Where(x => x.type == (int)XCategory.eType.Chi && x.category_id == catePartner.id).Sum(x => x.amount);
            //var SumDaChi = summeryInout.Where(x => x.type == (int)XCategory.eType.Chi).Sum(x => x.amount);
            //var SumDaThuNet = SumDaThu - SumDaChi;
            //var SumDuThuNet = SumGiaTriHopDong - SumDuChi;

            //ViewBag.SumGiaTriHopDong = SumGiaTriHopDong;
            //ViewBag.SumDaThu = SumDaThu;
            //ViewBag.SumPhaiThu = SumPhaiThu;
            //ViewBag.SumDuChi = SumDuChi;
            //ViewBag.SumHoaHongNV = SumHoaHongNV;
            //ViewBag.SumHoaHongDT = SumHoaHongDT;
            //ViewBag.SumDaChi = SumDaChi;
            //ViewBag.SumDaThuNet = SumDaThuNet;
            //ViewBag.SumDuThuNet = SumDuThuNet;

            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        [Authorize(Roles = "SuperAdmin,Manager,Accounting")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            return View(contract);
        }

        [Authorize(Roles = "SuperAdmin,Manager,Accounting")]
        // GET: Contracts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "SuperAdmin,Accounting")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(XContract obj)
        {
            obj.hoso.name = obj.contract.name;
            obj.hoso.account_id = obj.contract.account_id;
            var contract = obj.contract;
            var hoso = obj.hoso;
            if (ModelState.IsValid)
            {
                var username = User.Identity.GetUserName();
                var acc = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

                contract.name = Utils.RemoveSpecialCharactor(contract.name);
                contract.status = (int)XContract.eStatus.Processing;
                contract.time = DateTime.Now;
                db.Contracts.Add(contract);
                db.SaveChanges();
                wf_contract wf = new wf_contract();

                wf.account_id = acc.id;
                wf.contract_id = contract.id;
                wf.note = "Khởi tạo";
                wf.status = (int)Xwf_contract.eStatus.Processing;
                wf.time = DateTime.Now;
                db.wf_contract.Add(wf);
                // Hồ sơ                                
                hoso.account_id_soanthao = contract.soanthao;
                hoso.contract_id = contract.id;
                //hoso.note = contract.note;
                hoso.priority = contract.Priority;
                hoso.service_id = contract.service_id;
                hoso.status = contract.status;
                hoso.time_created = contract.time;
                hoso.trang4 = contract.type;
                hoso.step_id = db.Steps.OrderBy(x => x.sort).First().id;
                db.HoSoes.Add(hoso);
                db.SaveChanges();
                NHTrans.SaveLog(db, acc.id, "Hệ thống", "Tự động thêm mới hồ sơ " + hoso.name);

                // Tạo lịch nhắc nhớ ////////////////////////////////////////////////////////
                //1, Nhiệm vụ                
                var nhiemvu = XViecPhaiLam.CreateModel(hoso.id, acc.id, (int)XProgress.eType.NhiemVu, 2);
                db.ViecPhaiLams.Add(nhiemvu);
                //2, Điều tra
                var dieutra = XViecPhaiLam.CreateModel(hoso.id, acc.id, (int)XProgress.eType.DieuTra, 7);
                db.ViecPhaiLams.Add(dieutra);
                //3, Gia phả
                var giapha = XViecPhaiLam.CreateModel(hoso.id, acc.id, (int)XProgress.eType.GiaPha, 7);
                db.ViecPhaiLams.Add(giapha);
                //4, Điều tra
                var banve = XViecPhaiLam.CreateModel(hoso.id, acc.id, (int)XProgress.eType.BanVe, 7);
                db.ViecPhaiLams.Add(banve);
                //5, Hộ tịch
                var hotich = XViecPhaiLam.CreateModel(hoso.id, acc.id, (int)XProgress.eType.GiayToHoTich, 7);
                db.ViecPhaiLams.Add(hotich);
                //6, Giấy tờ xã
                var gtxa = XViecPhaiLam.CreateModel(hoso.id, acc.id, (int)XProgress.eType.GiayToXa, 7);
                db.ViecPhaiLams.Add(gtxa);
                ////////////////////////////END////////////////////////////////////////////
                // Hồ sơ                                
                var infomation = new Infomation();
                infomation.hoso_id = hoso.id;
                infomation.contract_id = contract.id;
                db.Infomations.Add(infomation);
                db.SaveChanges();

                // 
                if (obj.HardFileLinks.Count > 0)
                {
                    var file = obj.HardFileLinks[0];
                    if (file != null)
                    {
                        string filename = obj.contract.id + "FC_" + file.FileName;
                        file.SaveAs(Server.MapPath("~/public/FILEHOPDONG/") + filename);
                        obj.contract.HardFileLink = "/public/FILEHOPDONG/" + filename;
                    }
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // GET: Contracts/Edit/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            return View(new XContract() { contract = contract });
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.               

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Contract contract)
        {
            if (ModelState.IsValid)
            {
                contract.name = Utils.RemoveSpecialCharactor(contract.name);
                db.Entry(contract).State = EntityState.Modified;
                // wf
                wf_contract wf = new wf_contract();
                var username = User.Identity.GetUserName();
                var acc = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
                wf.account_id = acc.id;
                wf.contract_id = contract.id;
                wf.note = "Chỉnh sửa";
                wf.status = (int)Xwf_contract.eStatus.Processing;
                wf.time = DateTime.Now;
                db.wf_contract.Add(wf);
                NHTrans.SaveLog(db, acc.id, "HỢP ĐỒNG", "Chỉnh sửa " + contract.name);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contract);
        }

        // GET: Contracts/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contract contract = db.Contracts.Find(id);
            if (contract == null)
            {
                return HttpNotFound();
            }
            return View(contract);
        }

        // POST: Contracts/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contract contract = db.Contracts.Find(id);
            contract.status = (int)XContract.eStatus.Cancel;

            var username = User.Identity.GetUserName();
            var acc = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            NHTrans.SaveLog(db, acc.id, "HỢP ĐỒNG", "Xóa hợp đồng " + contract.name);
            wf_contract wf = new wf_contract();
            wf.account_id = acc.id;
            wf.contract_id = contract.id;
            wf.note = "Xóa hợp đồng " + contract.name;
            wf.status = (int)Xwf_contract.eStatus.Complete;
            wf.time = DateTime.Now;
            db.wf_contract.Add(wf);

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //[Authorize(Roles = "SuperAdmin,Manager,Member")]
        //public ActionResult BanLamViec(string Search_Data, int? add, int? ser, int? ste, string Filter_Value, int? Page_No)
        //{
        //    if (Search_Data != null)
        //    {
        //        Page_No = 1;
        //    }
        //    else
        //    {
        //        Search_Data = Filter_Value;
        //    }
        //    ViewBag.FilterValue = Search_Data;
        //    ViewBag.ADD = add;
        //    ViewBag.SER = ser;
        //    ViewBag.STEP = ste;
        //    ViewBag.Categories = new SelectListItem[0];
        //    ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
        //    ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
        //    ViewBag.STEPS = new SelectList(db.Steps, "id", "name");

        //    var username = User.Identity.GetUserName();
        //    var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
        //    var steps = db.StepAccounts.Where(x => x.account_id == us.id).Select(x => x.step_id).ToArray();

        //    IQueryable<Contract> results = null;

        //    results = (from io in db.Contracts
        //               join cus in db.Customers on io.customer_id equals cus.id
        //               join fk in db.fk_contract_service on io.id equals fk.contract_id
        //               join s in steps on io.step equals s
        //               where (
        //                    (io.step == (ste.HasValue ? ste : io.step)) &&
        //                    (io.address_id == (add.HasValue ? add : io.address_id)) &&
        //                    fk.service_id == (ser.HasValue ? ser.Value : fk.service_id) &&
        //                    cus.name.ToUpper().Contains((!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : cus.name.ToUpper())
        //               ))
        //               select io).Distinct();

        //    List<XContract> lst = new List<XContract>();
        //    int index = 0;
        //    int Size_Of_Page = 50;
        //    int No_Of_Page = (Page_No ?? 1);
        //    foreach (var item in results.OrderByDescending(x => x.Priority).ThenByDescending(x => x.time))
        //    {
        //        XContract con = new XContract();
        //        if (index >= ((No_Of_Page - 1) * Size_Of_Page) && index < (No_Of_Page * Size_Of_Page))
        //        {
        //            var cus = db.Customers.Find(item.customer_id);
        //            con.customer_name = cus != null ? cus.name : null;
        //            var address = db.Addresses.Find(item.address_id);
        //            con.address_name = address != null ? address.name : null;
        //            con.obj = item;

        //            var nguoithuchien = db.Accounts.Find(item.account_id);
        //            con.nguoithuchien = nguoithuchien != null ?
        //                nguoithuchien.fullname + "<a href='/contracts/cancelassign/?id=" + item.id + "' style='color: red; vertical-align: 2px; padding-left: 5px;'>x</a>" :
        //                "<a href='/contracts/assign/?id=" + item.id + "' class='text-success'>Nhận việc</a>";


        //            var services = db.fk_contract_service.Where(x => x.contract_id == item.id).Select(x => x.service_id).ToArray();
        //            con.str_service = String.Join(", ", db.Services.Where(x => services.Contains(x.id)).Select(x => x.name));
        //            con.stepname = db.Steps.Find(item.step).name;
        //            var step = db.Steps.Find(con.obj.step);
        //            con.step = step;
        //            switch (step.code)
        //            {
        //                //case "HOSOMOI":
        //                //    con.href = "<a href='/Contracts/Details/" + con.obj.id + "'>" + con.customer_name + "</a>";
        //                //    break;
        //                //case "NHAPTHONGTIN":
        //                //    con.href = "<a href='/Infomations/Edit?contract_id=" + con.obj.id + "'>" + con.customer_name + "</a>";
        //                //    break;
        //                //case "NOP1CUA":
        //                //    var odi1cua = db.di1cua.Where(x => x.contract_id == item.id).SingleOrDefault();
        //                //    if (odi1cua == null)
        //                //    {
        //                //        odi1cua = new di1cua();
        //                //        odi1cua.contract_id = item.id;
        //                //        odi1cua.ngaynop = DateTime.Now;
        //                //        odi1cua.trangthai = (int)Xdi1cua.eStatus.ChoNop;
        //                //        odi1cua.ngaytra = DateTime.Now.AddDays(30);
        //                //        var canbo = db.CanBoes.Where(x => x.motcua == (int)XModels.eYesNo.Yes).FirstOrDefault();
        //                //        odi1cua.canbo1cua = canbo != null ? canbo.id : db.CanBoes.First().id;
        //                //        db.di1cua.Add(odi1cua);
        //                //    }
        //                //    con.href = "<a href='/di1cua/Edit/" + odi1cua.id + "'>" + con.customer_name + "</a>";
        //                //    break;
        //                default:
        //                    //con.href = con.customer_name;
        //                    con.href = "<a href='/NhiemVus/?id=" + con.obj.id + "&step=" + step.id + "'>" + con.customer_name + "</a>";
        //                    break;
        //            }
        //        }
        //        lst.Add(con);
        //        index++;
        //    }
        //    //Lưu 1 cửa
        //    //db.SaveChanges();
        //    ViewBag.db = db;

        //    // Jobs
        //    DateTime history = DateTime.Now.AddDays(-7);
        //    ViewBag.Duy = db.Jobs.Where(x => x.process_by == 4 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
        //    ViewBag.Tin = db.Jobs.Where(x => x.process_by == 2 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
        //    ViewBag.Giang = db.Jobs.Where(x => x.process_by == 3 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();

        //    return View(lst.ToPagedList(No_Of_Page, Size_Of_Page));
        //}
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult BanLamViec(string Search_Data, int? add, int? uq, int? ser, int? par, int? ste, string Filter_Value, int? Page_No)
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
            ViewBag.ADD = add;
            ViewBag.UQ = uq;
            ViewBag.SER = ser;
            ViewBag.STEP = ste;
            ViewBag.PAR = par;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");
            ViewBag.STEPS = new SelectList(db.Steps, "id", "name");
            ViewBag.UYQUYENS = new SelectList(db.Accounts.Where(x => x.is_uq == (int)XModels.eYesNo.Yes), "id", "fullname");
            //var page4_ids = db.Services.Where(x => x.code.Equals("chuyennhuong")).Select(x => x.id).ToArray();
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var steps = db.StepAccounts.Where(x => x.account_id == us.id).Select(x => x.step_id).ToArray();

            var results = from io in db.DSBanLamViecs
                          join s in steps on io.step equals s
                          where
                               io.status != (int)XContract.eStatus.Complete &&
                               io.status != (int)XContract.eStatus.Cancel &&
                               io.type == (int)XContract.eType.Normal &&
                               (io.account_id == (uq.HasValue ? uq : io.account_id)) &&
                               (io.step == (ste.HasValue ? ste : io.step)) &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.customer_name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                          orderby io.Priority descending, io.time descending
                          select io;
            ViewBag.db = db;
            //ViewBag.Duy = db.Jobs.Where(x => x.process_by == 4 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            //ViewBag.Tin = db.Jobs.Where(x => x.process_by == 2 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            //ViewBag.Giang = db.Jobs.Where(x => x.process_by == 3 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult CancelAssign(int id)
        {
            Contract contract = db.Contracts.Find(id);
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var wf = new wf_contract();
            wf.account_id = us.id;
            wf.time = DateTime.Now;
            wf.status = (int)Xwf_contract.eStatus.Complete;
            wf.contract_id = contract.id;
            wf.note = us.fullname + " hủy nhận thực hiện nhập hồ sơ";

            var backWF = db.wf_contract.Where(x => x.contract_id == id).OrderByDescending(x => x.time).FirstOrDefault();
            wf.step_id = backWF.step_id;
            wf.from_id = backWF.from_id;

            db.wf_contract.Add(wf);
            contract.account_id = 3;
            db.SaveChanges();
            return RedirectToAction("BanLamViec");
        }
        [HttpGet]
        public JsonResult GetContract(string term, int? page, string _type)
        {
            int p = page ?? 0;
            int ps = 10;
            var lstCT = (from ct in db.Contracts
                         join add in db.Addresses on ct.address_id equals add.id
                         where ct.status == (int)XContract.eStatus.Processing && (ct.name.ToUpper().Contains(String.IsNullOrEmpty(term) ? ct.name.ToUpper() : term))
                         select new { id = ct.id, cus = ct.name, add = add.name }).OrderBy(x => x.cus).Skip(p * ps).Take(ps).ToList();
            data d = new data();
            List<item> lst = new List<item>();
            foreach (var item in lstCT)
            {
                item it = new item();
                it.name = item.cus;
                it.text = item.add;
                it.id = item.id;
                lst.Add(it);
            }
            d.total_count = lst.Count;
            d.items = lst;
            return Json(d, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult PhuTrach(string Search_Data, int? add, int? ser, int? par, int? ste, string Filter_Value, int? Page_No)
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
            ViewBag.ADD = add;
            ViewBag.SER = ser;
            ViewBag.STEP = ste;
            ViewBag.PAR = par;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");
            ViewBag.STEPS = new SelectList(db.Steps, "id", "name");

            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            var results = from io in db.DSBanLamViecs
                          where
                               io.status == (int)XContract.eStatus.Processing &&
                               io.account_id == us.id &&
                               (io.step == (ste.HasValue ? ste : io.step)) &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.customer_name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                          orderby io.Priority descending, io.time descending
                          select io;
            ViewBag.db = db;
            //ViewBag.Duy = db.Jobs.Where(x => x.process_by == 4 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            //ViewBag.Tin = db.Jobs.Where(x => x.process_by == 2 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            //ViewBag.Giang = db.Jobs.Where(x => x.process_by == 3 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult MyFile(string Search_Data, int? add, int? par, int? acc, string Filter_Value, int? Page_No)
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
            ViewBag.ADD = add;
            ViewBag.PAR = par;
            ViewBag.ACC = acc;

            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");
            ViewBag.STEPS = new SelectList(db.Steps, "id", "name");
            ViewBag.ACCOUNTS = new SelectList(db.Accounts, "id", "fullname");

            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var lstAcc = new List<int>();
            if (User.IsInRole("SuperAdmin") || User.IsInRole("Manager") || User.IsInRole("Accounting"))
            {
                if (acc.HasValue)
                    lstAcc.Add(acc.Value);
                else
                    lstAcc.AddRange(db.Accounts.Select(x => x.id));
            }
            else lstAcc.Add(us.id);
            var results = from io in db.vMyFiles
                          where
                                io.rose > 0 &&
                               lstAcc.Contains(io.account_id) &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.customer_name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                          orderby io.time descending
                          select io;
            ViewBag.db = db;
            var roses = results.Count() > 0 ? results.Sum(x => x.rose) : 0;
            ViewBag.TONGTHUONG = roses.ToString("N0");

            var claimed = from rs in results
                          join io in db.InOuts on rs.id equals io.contract_id
                          where io.category_id == 1034 && io.status == (int)XInOut.eStatus.DaThucHien
                          select io.amount;
            ViewBag.DANHAN = claimed.Count() > 0 ? claimed.Sum().ToString("N0") : "0";

            int Size_Of_Page = 50;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult TiepNhan(string Search_Data, int? add, int? ser, int? par, int? ste, string Filter_Value, int? Page_No)
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
            ViewBag.ADD = add;
            ViewBag.SER = ser;
            ViewBag.STEP = ste;
            ViewBag.PAR = par;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");
            ViewBag.STEPS = new SelectList(db.Steps, "id", "name");

            var steps = db.Steps.Where(x => x.category_id == (int)XStep.ePhong.TiepNhan).Select(x => x.id).ToArray();

            var results = from io in db.DSBanLamViecs
                          join s in steps on io.step equals s
                          where
                               (io.step == (ste.HasValue ? ste : io.step)) &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.customer_name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                          orderby io.Priority descending, io.time descending
                          select io;
            ViewBag.db = db;
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult ThamDinh(string Search_Data, int? add, int? ser, int? par, int? ste, string Filter_Value, int? Page_No)
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
            ViewBag.ADD = add;
            ViewBag.SER = ser;
            ViewBag.STEP = ste;
            ViewBag.PAR = par;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");
            ViewBag.STEPS = new SelectList(db.Steps, "id", "name");

            var steps = db.Steps.Where(x => x.category_id == (int)XStep.ePhong.ThamDinhTrichDo).Select(x => x.id).ToArray();

            var results = from io in db.DSBanLamViecs
                          join s in steps on io.step equals s
                          where
                               (io.step == (ste.HasValue ? ste : io.step)) &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.customer_name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                          orderby io.Priority descending, io.time descending
                          select io;
            ViewBag.db = db;
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult HoanThien(string Search_Data, int? add, int? ser, int? par, int? ste, string Filter_Value, int? Page_No)
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
            ViewBag.ADD = add;
            ViewBag.SER = ser;
            ViewBag.STEP = ste;
            ViewBag.PAR = par;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");
            ViewBag.STEPS = new SelectList(db.Steps, "id", "name");

            var steps = db.Steps.Where(x => x.category_id == (int)XStep.ePhong.HoanThien).Select(x => x.id).ToArray();

            var results = from io in db.DSBanLamViecs
                          join s in steps on io.step equals s
                          where
                               (io.step == (ste.HasValue ? ste : io.step)) &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.customer_name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                          orderby io.Priority descending, io.time descending
                          select io;
            ViewBag.db = db;
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult ThamDinhX(string Search_Data, int? add, int? ser, int? par, int? ste, string Filter_Value, int? Page_No)
        {
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var steps = db.StepAccounts.Where(x => x.account_id == us.id).Select(x => x.step_id).ToArray();
            var step2s = db.Steps.Where(x => x.category_id == (int)XStep.ePhong.ThamDinhTrichDo).Select(x => x.id).ToArray();
            steps = (from x in steps
                     join y in step2s on x equals y
                     select x).ToArray();
            var results = GetResult(Search_Data, add, null, ser, par, null, Filter_Value, steps);
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            Sigma(results);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult HoanThienX(string Search_Data, int? add, int? ser, int? par, int? ste, string Filter_Value, int? Page_No)
        {
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var steps = db.StepAccounts.Where(x => x.account_id == us.id).Select(x => x.step_id).ToArray();
            var step2s = db.Steps.Where(x => x.category_id == (int)XStep.ePhong.HoanThien).Select(x => x.id).ToArray();
            steps = (from x in steps
                     join y in step2s on x equals y
                     select x).ToArray();
            var results = GetResult(Search_Data, add, null, ser, par, null, Filter_Value, steps);
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            Sigma(results);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult KhoLuu(string Search_Data, int? add, int? ser, int? par, string Filter_Value, int? Page_No)
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
            ViewBag.ADD = add;
            ViewBag.SER = ser;
            ViewBag.PAR = par;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");

            var results = from io in db.vStorages
                          where
                               io.status == (int)XContract.eStatus.Complete &&
                               io.type == (int)XContract.eType.Normal &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.customer_name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                          orderby io.Priority descending, io.time descending
                          select io;
            ViewBag.db = db;
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult Page4(string Search_Data, int? add, int? par, string Filter_Value, int? Page_No)
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
            ViewBag.ADD = add;
            ViewBag.PAR = par;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");

            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var steps = db.StepAccounts.Where(x => x.account_id == us.id).Select(x => x.step_id).ToArray();
            //var page4_ids = db.Services.Where(x => x.code.Equals("chuyennhuong")).Select(x => x.id).ToArray();

            var results = from io in db.DSBanLamViecs
                          join s in steps on io.step equals s
                          where
                               io.status != (int)XContract.eStatus.Complete &&
                               io.status != (int)XContract.eStatus.Cancel &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.type == (int)XContract.eType.P4 &&
                               io.customer_name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                          orderby io.time descending
                          select io;
            ViewBag.db = db;
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult Page4X(string Search_Data, int? add, int? par, string Filter_Value, int? Page_No)
        {
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var steps = db.StepAccounts.Where(x => x.account_id == us.id).Select(x => x.step_id).ToArray();
            var results = GetResult(Search_Data, add, null, null, par, null, Filter_Value, steps);
            results = results.Where(x => x.type == (int)XContract.eType.P4);
            Sigma(results);
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult KhoLuuP4(string Search_Data, int? add, int? ser, int? par, string Filter_Value, int? Page_No)
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
            ViewBag.ADD = add;
            ViewBag.SER = ser;
            ViewBag.PAR = par;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");

            var results = from io in db.vStorages
                          where
                               io.status == (int)XContract.eStatus.Complete &&
                               io.type == (int)XContract.eType.P4 &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.customer_name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                          orderby io.Priority descending, io.time descending
                          select io;
            ViewBag.db = db;
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [Authorize(Roles = "SuperAdmin,Accounting")]
        [HttpGet]
        public JsonResult DuyetChiHoaHong(int id)
        {
            var contract = db.Contracts.Find(id);
            var partner = db.Partners.Find(contract.partner);
            var category = db.Categories.Single(x => x.kind == (int)XCategory.eKind.Rose);
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            InOut inOut = new InOut();
            inOut.account_id = partner.account_id;
            inOut.category_id = category.id;
            inOut.code = Utils.SetCodeInout(category);
            inOut.contract_id = contract.id;
            inOut.created_by = us.id;
            inOut.note = "Trả thưởng hoa hồng hồ sơ (" + contract.name + ")";
            inOut.status = (int)XInOut.eStatus.ChoDuyet;
            inOut.time = DateTime.Now;
            inOut.type = (int)XCategory.eType.Chi;
            inOut.amount = contract.rose;

            db.InOuts.Add(inOut);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult MyReward(string Search_Data, int? acc, string Filter_Value, int? Page_No)
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
            ViewBag.ACC = acc;
            ViewBag.ACCOUNTS = new SelectList(db.Accounts.Where(x => x.sta == (int)XAccount.eStatus.Processing), "id", "fullname");
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var lstAcc = new List<int>();
            if (acc.HasValue)
                lstAcc.Add(acc.Value);
            else
                lstAcc.AddRange(db.Accounts.Select(x => x.id));
            // Thưởng hợp đồng
            //var FromDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);
            //var ToDay = DateTime.Now.Month == 12 ? new DateTime(DateTime.Now.Year + 1, 1, 1) : new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1);

            //var services = db.Services.Where(x => x.reward == (int)XModels.eYesNo.Yes).Select(x => x.id).ToList();

            var results = from io in db.vMyRewards
                          where
                               io.account_id == (acc.HasValue ? acc.Value : io.account_id) &&
                               //services.Contains(io.service_id) &&
                               io.customer_name.ToUpper().Contains((!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())) &&
                               io.amount > 0
                          orderby io.time descending
                          select io;
            ViewBag.db = db;
            int Size_Of_Page = 50;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public JsonResult DuyetChiPhuTrach(int id)
        {
            var hoso = db.HoSoes.Find(id);
            var contract = db.Contracts.Find(hoso.contract_id);
            var category = db.Categories.Single(x => x.kind == (int)XCategory.eKind.Remunerate);
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            InOut inOut = new InOut();
            inOut.amount = 1000000;
            inOut.account_id = hoso.account_id;
            inOut.category_id = category.id;
            inOut.code = Utils.SetCodeInout(category);
            inOut.contract_id = contract.id;
            inOut.created_by = us.id;
            inOut.note = "Trả thưởng phụ trách hồ sơ (" + contract.name + ")";
            inOut.status = (int)XInOut.eStatus.ChoDuyet;
            inOut.time = DateTime.Now;
            inOut.type = (int)XCategory.eType.Chi;
            db.InOuts.Add(inOut);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public JsonResult ThuongThucHien(int contract_id, int account_id, long amount)
        {
            var contract = db.Contracts.Find(contract_id);
            var category = db.Categories.Single(x => x.kind == (int)XCategory.eKind.Remunerate);
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            InOut inOut = new InOut();
            inOut.amount = 1000000;
            inOut.account_id = account_id;
            inOut.category_id = category.id;
            inOut.code = Utils.SetCodeInout(category);
            inOut.contract_id = contract.id;
            inOut.created_by = us.id;
            inOut.note = "Trả thưởng phụ trách hồ sơ (" + contract.name + ")";
            inOut.status = (int)XInOut.eStatus.ChoDuyet;
            inOut.time = DateTime.Now;
            inOut.type = (int)XCategory.eType.Chi;
            db.InOuts.Add(inOut);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public JsonResult ThuongDoVe(int contract_id, int account_id, long amount)
        {
            var contract = db.Contracts.Find(contract_id);
            var category = db.Categories.Single(x => x.kind == (int)XCategory.eKind.Dove);
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            InOut inOut = new InOut();
            inOut.amount = amount;
            inOut.account_id = account_id;
            inOut.category_id = category.id;
            inOut.code = Utils.SetCodeInout(category);
            inOut.contract_id = contract.id;
            inOut.created_by = us.id;
            inOut.note = "Trả thưởng đo vẽ hồ sơ (" + contract.name + ")";
            inOut.status = (int)XInOut.eStatus.ChoDuyet;
            inOut.time = DateTime.Now;
            inOut.type = (int)XCategory.eType.Chi;
            db.InOuts.Add(inOut);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public JsonResult DuyetChiDoVe(int id)
        {
            var hoso = db.HoSoes.Find(id);
            var contract = db.Contracts.Find(hoso.contract_id);
            var category = db.Categories.Single(x => x.kind == (int)XCategory.eKind.Dove);
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            InOut inOut = new InOut();
            inOut.amount = 600000;
            inOut.account_id = hoso.account_id;
            inOut.category_id = category.id;
            inOut.code = Utils.SetCodeInout(category);
            inOut.contract_id = contract.id;
            inOut.created_by = us.id;
            inOut.note = "Trả thưởng đo vẽ hồ sơ (" + contract.name + ")";
            inOut.status = (int)XInOut.eStatus.ChoDuyet;
            inOut.time = DateTime.Now;
            inOut.type = (int)XCategory.eType.Chi;
            db.InOuts.Add(inOut);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult SoanThao(string Search_Data, int? add, int? uq, int? ser, int? par, int? ste, string Filter_Value, int? Page_No)
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
            ViewBag.ADD = add;
            ViewBag.UQ = uq;
            ViewBag.SER = ser;
            ViewBag.STEP = ste;
            ViewBag.PAR = par;

            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var steps = db.StepAccounts.Where(x => x.account_id == us.id).Select(x => x.step_id).ToArray();

            var results = from io in db.vSoanThaos
                          join s in steps on io.step equals s
                          where
                               io.status != (int)XContract.eStatus.Complete &&
                               io.status != (int)XContract.eStatus.Cancel &&
                               io.type == (int)XContract.eType.Normal &&
                               (io.account_id == (uq.HasValue ? uq : io.account_id)) &&
                               io.soanthao == us.id &&
                               (io.step == (ste.HasValue ? ste : io.step)) &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.customer_name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                          orderby io.Priority descending, io.time descending
                          select io;
            //ViewBag.Duy = db.Jobs.Where(x => x.process_by == 4 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            //ViewBag.Tin = db.Jobs.Where(x => x.process_by == 2 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            //ViewBag.Giang = db.Jobs.Where(x => x.process_by == 3 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult BaoCaoTienDo(string Search_Data, int? add, int? uq, int? ser, int? par, int? ste, string Filter_Value, int? Page_No)
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
            ViewBag.ADD = add;
            ViewBag.UQ = uq;
            ViewBag.SER = ser;
            ViewBag.STEP = ste;
            ViewBag.PAR = par;

            var results = from io in db.vTienDoes
                          where
                               io.status != (int)XContract.eStatus.Complete &&
                               io.status != (int)XContract.eStatus.Cancel &&
                               io.type == (int)XContract.eType.Normal &&
                               (io.account_id == (uq.HasValue ? uq : io.account_id)) &&
                               (io.step == (ste.HasValue ? ste : io.step)) &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.customer_name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                          orderby io.Priority descending, io.time descending
                          select io;
            //ViewBag.Duy = db.Jobs.Where(x => x.process_by == 4 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            //ViewBag.Tin = db.Jobs.Where(x => x.process_by == 2 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            //ViewBag.Giang = db.Jobs.Where(x => x.process_by == 3 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [HttpPost]
        public JsonResult PrintProcessing(string Search_Data, int? add, int? uq, int? ser, int? par, int? ste)
        {
            List<Account> P = new List<Account>();

            P = db.Accounts.Where(x => x.sta == (int)XModels.eStatus.Processing).ToList();

            var J = from io in db.vTienDoes
                    where
                         io.status != (int)XContract.eStatus.Complete &&
                         io.status != (int)XContract.eStatus.Cancel &&
                         io.type == (int)XContract.eType.Normal &&
                         (io.account_id == (uq.HasValue ? uq : io.account_id)) &&
                         (io.step == (ste.HasValue ? ste : io.step)) &&
                         (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                         (io.address_id == (add.HasValue ? add : io.address_id)) &&
                         io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                         io.customer_name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                    orderby io.Priority descending, io.time descending
                    select io;

            var T1 = from w in db.wf_contract
                     join c in J on w.contract_id equals c.id
                     select w;
            var T = T1.ToArray().Select(x => new temp() { contract_id = x.contract_id, id = x.id, note = x.note, time = x.time.ToString("dd/MM/yyyy") });

            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/Processing.xls");
            string webpart = "Processing_" + DateTime.Now.ToString("ssmmHH") + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("NGAYTHANG", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
                    flexCelReport.AddTable("P", P);
                    flexCelReport.AddTable("J", J);
                    flexCelReport.AddTable("T", T);
                    flexCelReport.AddRelationship("P", "J", "id", "account_id");
                    flexCelReport.AddRelationship("J", "T", "id", "contract_id");
                    flexCelReport.Run(sr, fs);
                }
            }
            db.SaveChanges();
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DSCongViec()
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            var results = from io in db.DSBanLamViecs
                          where
                               io.status == (int)XContract.eStatus.Processing &&
                               io.account_id == us.id
                          orderby io.Priority descending, io.time descending
                          select io;
            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/BangDSCongViec.xls");
            string webpart = "BangDSCongViec_" + DateTime.Now.ToString("ssmmHH") + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("NGAYTHANG", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
                    flexCelReport.AddTable("T", results);
                    flexCelReport.Run(sr, fs);
                }
            }
            db.SaveChanges();
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DSCongViecSupport()
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            var results = from io in db.vSoanThaos
                          where
                               io.status != (int)XContract.eStatus.Complete &&
                               io.status != (int)XContract.eStatus.Cancel &&
                               io.soanthao == us.id
                          orderby io.Priority descending, io.time descending
                          select io;
            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/BangDSCongViec.xls");
            string webpart = "BangDSCongViec_" + DateTime.Now.ToString("ssmmHH") + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("NGAYTHANG", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
                    flexCelReport.AddTable("T", results);
                    flexCelReport.Run(sr, fs);
                }
            }
            db.SaveChanges();
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin,Manager,Accounting")]
        public ActionResult ViewHardFile(string Search_Data, int? add, int? ser, int? par, string Filter_Value, int? Page_No)
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
            ViewBag.ADD = add;
            ViewBag.SER = ser;
            ViewBag.PAR = par;

            var results = from io in db.vContractHardFiles
                          where
                          (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                           (io.address_id == (add.HasValue ? add : io.address_id)) &&
                           io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                           io.customer_name.ToUpper().Contains((!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                          )
                          orderby io.time descending
                          select io;
            ViewBag.db = db;
            int Size_Of_Page = 50;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [HttpPost]
        public JsonResult UploadHardFile()
        {
            var contract = db.Contracts.Find(Convert.ToInt64(Request["id"]));
            var address = db.Addresses.Find(contract.address_id);

            if (Request.Files.Count > 0)
            {
                var f0 = Request.Files[0];

                string subPath = Server.MapPath("~/public/" + address.code);
                bool exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id);
                exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id + "/ContractHardFile");
                exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                f0.SaveAs(subPath + "/" + f0.FileName);
                contract.HardFileLink = "/public/" + address.code + "/" + contract.id + "/ContractHardFile/" + f0.FileName;
            }
            db.SaveChanges();
            return Json(contract.HardFileLink, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "SuperAdmin,Manager,Accounting")]
        public ActionResult ProgressConfig()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TDTDChangeService(int service_id)
        {
            var kpis = db.service_step_day.Where(x => x.service_id == service_id);
            return Json(kpis, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult TDTDSubmit(int service_id, List<int[]> chitieus)
        {
            foreach (var item in chitieus)
            {
                var step_id = item[0];
                service_step_day ssd = db.service_step_day.SingleOrDefault(x => x.step_id == step_id && x.service_id == service_id);
                if (ssd == null)
                {
                    ssd = new service_step_day();
                    ssd.service_id = service_id;
                    ssd.step_id = item[0];
                    ssd.yellow = item[1];
                    ssd.red = item[2];
                    db.service_step_day.Add(ssd);
                }
                else
                {
                    ssd.yellow = item[1];
                    ssd.red = item[2];
                }
            }
            db.SaveChanges();
            return Json("/contracts/progressconfig", JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult HSCuaToi(string Search_Data, int? add, int? uq, int? ser, int? par, int? ste, string Filter_Value, int? Page_No)
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
            ViewBag.ADD = add;
            ViewBag.UQ = uq;
            ViewBag.SER = ser;
            ViewBag.STEP = ste;
            ViewBag.PAR = par;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");
            ViewBag.STEPS = new SelectList(db.Steps, "id", "name");
            ViewBag.UYQUYENS = new SelectList(db.Accounts.Where(x => x.is_uq == (int)XModels.eYesNo.Yes), "id", "fullname");
            ViewBag.MANAGER = true;
            //var page4_ids = db.Services.Where(x => x.code.Equals("chuyennhuong")).Select(x => x.id).ToArray();
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var steps = db.StepAccounts.Where(x => x.account_id == us.id).Select(x => x.step_id).ToArray();
            var lstUser = new List<int>();
            lstUser.Add(us.id);
            var grp = db.GroupMembers.Where(x => x.account_id == us.id).FirstOrDefault();
            if (User.IsInRole("SuperAdmin"))
            {
                lstUser = Common.cAccounts().Select(x => x.id).ToList();
            }
            else if (grp != null)
            {
                var lstMember = db.GroupMember_Account.Where(x => x.groupmember_id == grp.id).Select(x => x.account_id);
            }
            else
            {
                ViewBag.MANAGER = false;
            }
            var results = from io in db.DSBanLamViecs
                          join s in steps on io.step equals s
                          where
                               io.status != (int)XContract.eStatus.Complete &&
                               io.status != (int)XContract.eStatus.Cancel &&
                               io.type == (int)XContract.eType.Normal &&
                               (io.account_id == (uq.HasValue ? uq : io.account_id)) &&
                               lstUser.Contains(io.account_id.Value) &&
                               (io.step == (ste.HasValue ? ste : io.step)) &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.customer_name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                          orderby io.Priority descending, io.time descending
                          select io;
            ViewBag.db = db;
            //ViewBag.Duy = db.Jobs.Where(x => x.process_by == 4 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            //ViewBag.Tin = db.Jobs.Where(x => x.process_by == 2 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            //ViewBag.Giang = db.Jobs.Where(x => x.process_by == 3 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            Sigma(results);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult PhongChuanBi(string Search_Data, int? add, int? uq, int? ser, int? par, string Filter_Value, int? Page_No)
        {
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var steps = db.StepAccounts.Where(x => x.account_id == us.id).Select(x => x.step_id).ToArray();
            var step2s = db.Steps.Where(x => x.category_id == (int)XStep.ePhong.TiepNhan).Select(x => x.id).ToArray();
            steps = (from x in steps
                     join y in step2s on x equals y
                     select x).ToArray();
            var results = GetResult(Search_Data, add, uq, ser, par, null, Filter_Value, steps);
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            Sigma(results);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult DoVe(string Search_Data, int? add, int? uq, int? ser, int? par, string Filter_Value, int? Page_No)
        {
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var steps = db.StepAccounts.Where(x => x.account_id == us.id).Select(x => x.step_id).ToArray();
            var step2s = db.Steps.Where(x => x.category_id == (int)XStep.ePhong.DoVe).Select(x => x.id).ToArray();
            steps = (from x in steps
                     join y in step2s on x equals y
                     select x).ToArray();
            var results = GetResult(Search_Data, add, uq, ser, par, null, Filter_Value, steps);
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            Sigma(results);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        IQueryable<DSBanLamViec> GetResult(string Search_Data, int? add, int? uq, int? ser, int? par, int? ste, string Filter_Value, int[] stepids)
        {
            if (Search_Data == null)
            {
                Search_Data = Filter_Value;
            }
            ViewBag.FilterValue = Search_Data;
            ViewBag.ADD = add;
            ViewBag.UQ = uq;
            ViewBag.SER = ser;
            ViewBag.PAR = par;
            ViewBag.STEP = ste;
            ViewBag.STEPS = new SelectList(db.Steps, "id", "name");
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");
            ViewBag.UYQUYENS = new SelectList(db.Accounts.Where(x => x.is_uq == (int)XModels.eYesNo.Yes), "id", "fullname");
            ViewBag.MANAGER = true;
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var lstUser = new List<int>();
            lstUser.Add(us.id);
            var grp = db.GroupMembers.Where(x => x.account_id == us.id).FirstOrDefault();
            if (User.IsInRole("SuperAdmin"))
            {
                lstUser = Common.cAccounts().Select(x => x.id).ToList();
            }
            else if (grp != null)
            {
                var lstMember = db.GroupMember_Account.Where(x => x.groupmember_id == grp.id).Select(x => x.account_id);
            }
            else
            {
                ViewBag.MANAGER = false;
            }
            var results = from io in db.DSBanLamViecs
                          where
                                stepids.Contains(io.step) &&
                               io.status == (int)XContract.eStatus.Processing &&
                               (io.account_id == (uq.HasValue ? uq : io.account_id)) &&
                               lstUser.Contains(io.account_id.Value) &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.customer_name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                          orderby io.Priority descending, io.time descending
                          select io;
            Sigma(results);
            return results;
        }
        void Sigma(IQueryable<DSBanLamViec> results)
        {
            var lstSteps = db.Steps.ToArray();
            var shschuanbiids = lstSteps.Where(x => x.code.Equals("HOSOMOI") || x.code.Equals("NHAPTHONGTIN")).Select(x => x.id).ToList();
            ViewBag.chosomoi = results.Count(x => shschuanbiids.Contains(x.step));
            var sbosung = lstSteps.Single(x => x.code.Equals("DOVE")).id;
            ViewBag.cbosung = results.Count(x => x.step == sbosung);
            var shoanthien = lstSteps.Single(x => x.code.Equals("HOANTHIEN")).id;
            ViewBag.choanthien = results.Count(x => x.step == shoanthien);
            var s1cua = lstSteps.Single(x => x.code.Equals("NOP1CUA")).id;
            ViewBag.c1cua = results.Count(x => x.step == s1cua);
            var sdonthu = lstSteps.Single(x => x.code.Equals("DONTHU")).id;
            ViewBag.cdonthu = results.Count(x => x.step == sdonthu);
        }
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult TrinhDuyetChi(int? id)
        {
            var contract = db.Contracts.Find(id);
            return View(contract);
        }

    }

    class temp
    {
        public long id { get; set; }
        public int contract_id { get; set; }
        public string time { get; set; }
        public string note { get; set; }
    }
}