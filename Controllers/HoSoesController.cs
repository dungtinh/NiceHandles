using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using Aspose.Words.Drawing;
using FlexCel.Report;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;
using PagedList;
using static NiceHandles.Models.XHoSo;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Manager,Member")]
    public class HoSoesController : Controller
    {
        private NHModel db = new NHModel();

        // GET: HoSoes

        public ActionResult Index(string Search_Data, int? add, int? uq, int? ser, int? par, int? ste, int? group, bool? light, string Filter_Value, int? Page_No)
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
            ViewBag.GROUP = group;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");
            ViewBag.STEPS = new SelectList(db.Steps, "id", "name");
            ViewBag.UYQUYENS = new SelectList(db.Accounts.Where(x => x.is_uq == (int)XModels.eYesNo.Yes), "id", "fullname");
            ViewBag.MANAGER = true;
            ViewBag.LIGHT = light.HasValue ? light.Value : false;
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            var steps = db.StepAccounts.Where(x => x.account_id == us.id).Select(x => x.step_id).ToArray();
            var lstUser = new List<int>();
            if (!group.HasValue || group.Value == (int)XHoSo.eView.CuaToi)
            {
                lstUser.Add(us.id);
                ViewBag.MANAGER = false;
            }
            else if (group.Value == (int)XHoSo.eView.Nhom)
            {
                var grp = db.GroupMembers.Where(x => x.account_id == us.id).FirstOrDefault();
                if (grp != null)
                {
                    lstUser = db.GroupMember_Account.Where(x => x.groupmember_id == grp.id).Select(x => x.account_id).ToList(); ;
                }
            }
            else if (group.Value == (int)XHoSo.eView.Manager)
            {
                if (User.IsInRole("SuperAdmin") || User.IsInRole("Manager") || User.IsInRole("Accounting"))
                {
                    lstUser = db.Accounts.Where(x => x.sta == (int)XAccount.eStatus.Processing).Select(x => x.id).ToList();
                }
            }
            var results = from io in db.vHoSoes
                          where
                               io.status == (int)XHoSo.eStatus.Processing &&
                               (io.account_id == (uq.HasValue ? uq : io.account_id)) &&
                               lstUser.Contains(io.account_id) &&
                               (io.step_id == (ste.HasValue ? ste : io.step_id)) &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.light == ((light.HasValue && light.Value == true) ? 1 : io.light) &&
                               io.name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.name.ToUpper())
                          orderby io.priority descending, io.time descending
                          select io;


            int Size_Of_Page = 20;
            int No_Of_Page = (Page_No ?? 1);
            Sigma(results);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        public sealed class DSHoSoPrintByFilterCls
        {
            public string name { get; set; }
            public string service { get; set; }
            public string address { get; set; }
            public string ngayky { get; set; }
            public string note { get; set; }
            public string ngoaigiao { get; set; }
            public string partner { get; set; }
            public string acc { get; set; }
        }
        [HttpGet]
        public JsonResult PrintByFilter(string Search_Data, int? add, int? uq, int? ser, int? par, bool? light, int? ste, int? group)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            if (!group.HasValue || group.Value == (int)XHoSo.eView.CuaToi)
            {
                uq = uq.HasValue ? uq : us.id;
            }
            var results = from io in db.vHoSoes
                          where
                               io.status == (int)XHoSo.eStatus.Processing &&
                               (io.account_id == (uq.HasValue ? uq : io.account_id)) &&
                               (io.step_id == (ste.HasValue ? ste : io.step_id)) &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.light == ((light.HasValue && light.Value == true) ? 1 : io.light) &&
                               io.name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.name.ToUpper())
                          orderby io.priority descending, io.time descending
                          select io;
            ///END
            var lst = new List<DSHoSoPrintByFilterCls>();
            var lstAcc = db.Accounts.Select(x => new Account() { id = x.id, disname = x.disname });
            var lstPart = db.Partners.Select(x => new Partner() { id = x.id, disname = x.disname });
            foreach (var item in results)
            {
                var wfHoso = db.wf_hoso.Where(x => x.hoso_id == item.id).OrderByDescending(x => x.time).FirstOrDefault();
                var contract = db.Contracts.Find(item.contract_id);
                if (contract == null)
                    continue;
                var per = new DSHoSoPrintByFilterCls();
                var address = db.Addresses.Find(contract.address_id);
                var service = db.Services.Find(item.service_id);
                per.name = item.name;
                per.service = service.name;
                per.address = address.name;
                per.ngoaigiao = (item.ngoaigiao.HasValue && item.ngoaigiao.Value == (int)XHoSo.eNgoaiGiao.NgoaiGiao) ? "Có" : "";
                per.acc = item.account_name;
                per.ngayky = contract.time.ToString("dd/MM/yyyy");
                per.partner = item.partner_name;
                per.note = (wfHoso != null ? wfHoso.note : "");
                lst.Add(per);
            }

            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/DSHoSo.xls"); //document.template

            string webpart = "DSHoSo";
            webpart += ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();

            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("NGAYTHANG", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
                    flexCelReport.AddTable("TB", lst);
                    flexCelReport.Run(sr, fs);
                }
            }
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SoanThao(string Search_Data, int? add, int? uq, int? ser, int? par, int? ste, int? group, string Filter_Value, int? Page_No)
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
            ViewBag.GROUP = group;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");
            ViewBag.STEPS = new SelectList(db.Steps, "id", "name");

            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            var results = from io in db.vHoSoes
                          where
                               io.status == (int)XHoSo.eStatus.Processing &&
                               (io.account_id_soanthao == us.id) &&
                               (io.step_id == (ste.HasValue ? ste : io.step_id)) &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.name.ToUpper())
                          orderby io.priority descending, io.time descending
                          select io;
            ViewBag.db = db;

            int Size_Of_Page = 20;
            int No_Of_Page = (Page_No ?? 1);
            Sigma(results);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        public ActionResult KhoLuu(string Search_Data, int? add, int? uq, int? ser, int? par, int? group, string Filter_Value, int? Page_No)
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
            ViewBag.PAR = par;
            ViewBag.GROUP = group;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");
            ViewBag.UYQUYENS = new SelectList(db.Accounts.Where(x => x.is_uq == (int)XModels.eYesNo.Yes), "id", "fullname");

            var results = from io in db.vHoSoes
                          where
                               io.status > (int)XHoSo.eStatus.Processing &&
                               (io.account_id == (uq.HasValue ? uq : io.account_id)) &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.name.ToUpper())
                          orderby io.time_complete descending, io.time descending
                          select io;
            ViewBag.db = db;

            int Size_Of_Page = 100;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        // GET: HoSoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var hoso = db.HoSoes.Find(id);
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            return View(hoso);
        }

        // GET: HoSoes/Create
        public ActionResult Create(int? contract_id)
        {
            if (contract_id.HasValue)
            {
                var hoso = new HoSo();
                var contract = db.Contracts.Find(contract_id);
                hoso.contract_id = contract_id.Value;
                hoso.account_id = contract.account_id;
                hoso.account_id_soanthao = contract.soanthao;
                hoso.service_id = contract.service_id;
                hoso.note = contract.note;
                hoso.priority = contract.Priority;

                return View(hoso);
            }
            return View();
        }

        // POST: HoSoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(HoSo hoSo)
        {
            hoSo.time_created = DateTime.Now;
            hoSo.status = (int)XHoSo.eStatus.Processing;
            hoSo.step_id = db.Steps.OrderBy(x => x.sort).First().id;
            if (ModelState.IsValid)
            {
                db.HoSoes.Add(hoSo);
                var username = User.Identity.GetUserName();
                var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
                NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Thêm mới hồ sơ (" + hoSo.name + ")");
                db.SaveChanges();

                // Tạo lịch nhắc nhớ ////////////////////////////////////////////////////////
                //1, Nhiệm vụ                
                var nhiemvu = XViecPhaiLam.CreateModel(hoSo.id, us.id, (int)XProgress.eType.NhiemVu, 2);
                db.ViecPhaiLams.Add(nhiemvu);
                //2, Điều tra
                var dieutra = XViecPhaiLam.CreateModel(hoSo.id, us.id, (int)XProgress.eType.DieuTra, 7);
                db.ViecPhaiLams.Add(dieutra);
                //3, Gia phả
                var giapha = XViecPhaiLam.CreateModel(hoSo.id, us.id, (int)XProgress.eType.GiaPha, 7);
                db.ViecPhaiLams.Add(giapha);
                //4, Điều tra
                var banve = XViecPhaiLam.CreateModel(hoSo.id, us.id, (int)XProgress.eType.BanVe, 7);
                db.ViecPhaiLams.Add(banve);
                //5, Hộ tịch
                var hotich = XViecPhaiLam.CreateModel(hoSo.id, us.id, (int)XProgress.eType.GiayToHoTich, 7);
                db.ViecPhaiLams.Add(hotich);
                //6, Giấy tờ xã
                var gtxa = XViecPhaiLam.CreateModel(hoSo.id, us.id, (int)XProgress.eType.GiayToXa, 7);
                db.ViecPhaiLams.Add(gtxa);
                ////////////////////////////END////////////////////////////////////////////
                // Hồ sơ                                
                var infomation = new Infomation();
                infomation.hoso_id = hoSo.id;
                infomation.contract_id = hoSo.contract_id;
                db.Infomations.Add(infomation);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(hoSo);
        }
        // GET: HoSoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoSo hoSo = db.HoSoes.Find(id);
            if (hoSo == null)
            {
                return HttpNotFound();
            }
            return View(hoSo);
        }

        // POST: HoSoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(HoSo hoSo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hoSo).State = EntityState.Modified;
                var username = User.Identity.GetUserName();
                var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
                NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Chỉnh sửa hồ sơ (" + hoSo.name + ")");
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hoSo);
        }

        // GET: HoSoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoSo hoSo = db.HoSoes.Find(id);
            if (hoSo == null)
            {
                return HttpNotFound();
            }
            return View(hoSo);
        }

        // POST: HoSoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HoSo hoSo = db.HoSoes.Find(id);
            hoSo.status = (int)XHoSo.eStatus.Cancel;
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Hủy hồ sơ (" + hoSo.name + ")");
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
        void Sigma(IQueryable<vHoSo> results)
        {
            var lstSteps = db.Steps.ToArray();
            var shschuanbiids = lstSteps.Where(x => x.code.Equals("HOSOMOI") || x.code.Equals("NHAPTHONGTIN")).Select(x => x.id).ToList();
            ViewBag.chosomoi = results.Count(x => shschuanbiids.Contains(x.step_id));
            var sbosung = lstSteps.Single(x => x.code.Equals("DOVE")).id;
            ViewBag.cbosung = results.Count(x => x.step_id == sbosung);
            var shoanthien = lstSteps.Single(x => x.code.Equals("HOANTHIEN")).id;
            ViewBag.choanthien = results.Count(x => x.step_id == shoanthien);
            var s1cua = lstSteps.Single(x => x.code.Equals("NOP1CUA")).id;
            ViewBag.c1cua = results.Count(x => x.step_id == s1cua);
            var sdonthu = lstSteps.Single(x => x.code.Equals("DONTHU")).id;
            ViewBag.cdonthu = results.Count(x => x.step_id == sdonthu);
            ViewBag.ctong = results.Count();
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult Next(int hoso_id)
        {
            HoSo hoso = db.HoSoes.Find(hoso_id);
            var contract = db.Contracts.Find(hoso.contract_id);
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var currstep = db.Steps.Find(hoso.step_id);
            if (currstep.next.HasValue && currstep.next != 0)
            {
                var nextstep = db.Steps.Find(currstep.next);
                var wf = new wf_hoso_step();
                wf.account_id = us.id;
                wf.time = DateTime.Now;
                wf.hoso_id = hoso.id;
                wf.note = us.fullname + " thực hiện chuyển bước từ " + currstep.name + " đến " + nextstep.name;
                wf.step_id = nextstep.id;
                db.wf_hoso_step.Add(wf);
                hoso.step_id = nextstep.id;
                if (nextstep.code.ToUpper().Trim().Equals("NOP1CUA"))
                {
                    var odi1cua = db.di1cua.SingleOrDefault(x => x.hoso_id == hoso_id);
                    if (odi1cua == null)
                    {
                        odi1cua = CreateNewDi1Cua(hoso.id, contract.address_id);
                        db.di1cua.Add(odi1cua);
                    }
                    else
                    {
                        odi1cua.trangthai = (int)Xdi1cua.eStatus.ChoNop;
                    }
                }
                else if (nextstep.code.ToUpper().Trim().Equals("HOANTHANH"))
                {
                    hoso.status = (int)XHoSo.eStatus.Complete;
                    hoso.time_complete = DateTime.Now;
                }
            }
            NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Thực hiện hành động bước tiếp (" + hoso.name + ")");
            db.SaveChanges();
            return RedirectToAction("details", new { id = hoso_id });
        }
        private di1cua CreateNewDi1Cua(int hoso_id, int address_id)
        {
            di1cua odi1cua = new di1cua();
            odi1cua.hoso_id = hoso_id;
            odi1cua.ngaynop = DateTime.Now;
            odi1cua.trangthai = (int)Xdi1cua.eStatus.ChoNop;
            odi1cua.ngaytra = DateTime.Now.AddDays(30);
            var canbo = db.CanBoes.Where(x => x.address_id == address_id).FirstOrDefault();
            odi1cua.canbo1cua = canbo != null ? canbo.id : db.CanBoes.First().id;
            return odi1cua;
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult Prev(int? hoso_id)
        {
            var hoso = db.HoSoes.Find(hoso_id);
            var currstep = db.Steps.Find(hoso.step_id);
            var prevstep = db.Steps.Find(currstep.prev);
            Contract contract = db.Contracts.Find(hoso.contract_id);
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            if (currstep.prev.HasValue && currstep.prev > 0)
            {
                var wf = new wf_hoso_step();
                wf.account_id = us.id;
                wf.time = DateTime.Now;
                wf.hoso_id = hoso.id;
                wf.note = us.fullname + " thực hiện lùi bước từ " + currstep.name + " về " + prevstep.name;
                wf.step_id = prevstep.id;
                db.wf_hoso_step.Add(wf);
                hoso.step_id = prevstep.id;
            }
            if (currstep.code.ToUpper().Trim().Equals("HOANTHANH"))
            {
                hoso.status = (int)XHoSo.eStatus.Processing;
                hoso.time_complete = null;
            }
            NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Thực hiện hành động trả về (" + hoso.name + ")");
            db.SaveChanges();
            return RedirectToAction("details", new { id = hoso_id });
        }
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public JsonResult GotoStep(int id, int sp)
        {
            HoSo hoso = db.HoSoes.Find(id);
            if (sp != hoso.step_id)
            {
                var username = User.Identity.GetUserName();
                var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
                var wf = new wf_hoso_step();
                wf.account_id = us.id;
                wf.time = DateTime.Now;
                wf.hoso_id = hoso.id;
                wf.note = us.fullname + " thực hiện chuyển bước tới " + XHoSo.sStep[sp];
                wf.step_id = sp;
                db.wf_hoso_step.Add(wf);
                hoso.step_id = sp;
                // IF 1 cửa            
                if (sp == (int)XHoSo.eStep.Nop1Cua)
                {
                    var odi1cua = db.di1cua.Where(x => x.hoso_id == id).SingleOrDefault();
                    if (odi1cua == null)
                    {
                        odi1cua = new di1cua();
                        odi1cua.hoso_id = id;
                        odi1cua.ngaynop = DateTime.Now;
                        odi1cua.trangthai = (int)Xdi1cua.eStatus.ChoNop;
                        odi1cua.ngaytra = DateTime.Now.AddDays(30);
                        var canbo = db.CanBoes.Where(x => x.motcua == (int)XModels.eYesNo.Yes).FirstOrDefault();
                        odi1cua.canbo1cua = canbo != null ? canbo.id : db.CanBoes.First().id;
                        db.di1cua.Add(odi1cua);
                    }
                }
                db.SaveChanges();
            }
            return Json(sp, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UploadCadAddress()
        {
            var hoso = db.HoSoes.Find(Convert.ToInt64(Request["id"]));
            var nhiemvu = db.NhiemVus.Find(Convert.ToInt64(Request["id"]));
            var contract = db.Contracts.Find(nhiemvu.contract_id);
            var address = db.Addresses.Find(contract.address_id);
            var time = DateTime.Now.ToString("mmHHddMMyy");
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            if (Request.Files.Count > 0)
            {
                var fileCad = Utils.GetSlug(contract.name) + contract.id + "_" + time + ".dwg";
                var f0 = Request.Files[0];
                string subPath = Server.MapPath("~/public/" + address.code);
                bool exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id);
                exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id + "/CAD");
                exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                f0.SaveAs(subPath + "/" + fileCad);
                //contract.GoogleLink = Request.Url.Scheme + "://" + Request.Url.Host + "/public/" + address.code + "/" + contract.id + "/CAD" + "/" + contract.id + "_" + time + ".dwg";
                hoso.link_filecad = address.code + "/" + contract.id + "/CAD" + "/" + fileCad;
                //                
                if (!string.IsNullOrEmpty(hoso.link_filecad))
                {
                    string filename = Utils.GetSlug(contract.name) + contract.id + "_" + time + "_QRCAD.jpg";
                    hoso.link_filecad_qr = address.code + "/" + contract.id + "/CAD" + "/" + filename;

                    //var dove = Request.Url.Scheme + "://dodactracdia.vn/Cad?id=" + contract.id;
                    var dove = hoso.link_filecad_qr;

                    //string url = "https://chart.googleapis.com/chart?cht=qr&chl=" + XModels.LinkDove + "/" + hoso.link_filecad + "&choe=UTF-8&chs=400x400";
                    WebClient webClient = new WebClient();
                    string url = string.Empty;
                    try
                    {
                        url = "https://quickchart.io/qr?text=" + XModels.LinkDove + "/" + hoso.link_filecad + "&size=400";
                        webClient.DownloadFile(url, subPath + "/" + filename);
                    }
                    catch
                    {
                        url = "https://chart.googleapis.com/chart?cht=qr&chl=" + XModels.LinkDove + "/" + hoso.link_filecad + "&choe=UTF-8&chs=400x400";
                        webClient.DownloadFile(url, subPath + "/" + filename);
                    }
                }
                else
                    hoso.link_filecad_qr = string.Empty;
            }
            string gmap = Request["g"].ToString();
            if (!gmap.Equals(hoso.link_gmap))
            {
                hoso.link_gmap = gmap;
                if (!string.IsNullOrEmpty(gmap))
                {
                    string subPath = Server.MapPath("~/public/" + address.code);
                    bool exists = System.IO.Directory.Exists(subPath);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(subPath);
                    subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id);
                    exists = System.IO.Directory.Exists(subPath);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(subPath);
                    subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id + "/CAD");
                    exists = System.IO.Directory.Exists(subPath);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(subPath);

                    string filename = contract.id + "_" + time + "_QRGM.jpg";
                    hoso.link_gmap_qr = "/" + address.code + "/" + contract.id + "/CAD" + "/" + filename;
                    WebClient webClient = new WebClient();
                    string url = "https://chart.googleapis.com/chart?cht=qr&chl=" + hoso.link_gmap + "&choe=UTF-8&chs=400x400";
                    try
                    {
                        url = "https://quickchart.io/qr?text=" + hoso.link_gmap + "&size=400";
                        webClient.DownloadFile(url, subPath + "/" + filename);
                    }
                    catch
                    {
                        url = "https://chart.googleapis.com/chart?cht=qr&chl=" + hoso.link_gmap + "&choe=UTF-8&chs=400x400";
                        webClient.DownloadFile(url, subPath + "/" + filename);
                    }
                }
                else
                    hoso.link_gmap_qr = string.Empty;
            }
            NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Cập nhật file cad, địa chỉ (" + hoso.name + ")");
            db.SaveChanges();
            return Json(hoso.link_gmap, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult LuuDinhViThuaDat(int hoso_id, string googlemap)
        {
            var hoso = db.HoSoes.Find(hoso_id);
            var contract = db.Contracts.Find(hoso.contract_id);
            var address = db.Addresses.Find(contract.address_id);
            var time = DateTime.Now.ToString("mmHHddMMyy");
            if (!googlemap.Equals(hoso.link_gmap))
            {
                hoso.link_gmap = googlemap;
                if (!string.IsNullOrEmpty(googlemap))
                {
                    string subPath = Server.MapPath("~/public/" + address.code);
                    bool exists = System.IO.Directory.Exists(subPath);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(subPath);
                    subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id);
                    exists = System.IO.Directory.Exists(subPath);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(subPath);
                    subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id + "/CAD");
                    exists = System.IO.Directory.Exists(subPath);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(subPath);

                    string filename = contract.id + "_" + time + "_QRGM.jpg";
                    hoso.link_gmap_qr = "/" + address.code + "/" + contract.id + "/CAD" + "/" + filename;
                    WebClient webClient = new WebClient();
                    try
                    {
                        string url = "https://quickchart.io/qr?text=" + googlemap + "&size=400";
                        webClient.DownloadFile(url, subPath + "/" + filename);
                    }
                    catch
                    {
                        string url = "https://chart.googleapis.com/chart?cht=qr&chl=" + googlemap + "&choe=UTF-8&chs=400x400";
                        webClient.DownloadFile(url, subPath + "/" + filename);
                    }
                }
                else
                {
                    hoso.link_gmap = string.Empty;
                    hoso.link_gmap_qr = string.Empty;
                }
            }

            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Lưu định vị thửa đất (" + hoso.name + ")");
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult LuuDoveNote(int dove_id, string note)
        {
            var dove = db.Doves.Find(dove_id);
            dove.note = note;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult LuuFileAutoCad(int hoso_id)
        //{
        //    var hoso = db.HoSoes.Find(hoso_id);
        //    var contract = db.Contracts.Find(hoso.contract_id);
        //    var address = db.Addresses.Find(contract.address_id);
        //    var time = DateTime.Now.ToString("mmHHddMMyy");
        //    if (Request.Files.Count > 0)
        //    {
        //        var fileCad = Utils.GetSlug(contract.name) + contract.id + "_" + time + ".dwg";
        //        var f0 = Request.Files[0];
        //        string subPath = Server.MapPath("~/public/" + address.code);
        //        bool exists = System.IO.Directory.Exists(subPath);
        //        if (!exists)
        //            System.IO.Directory.CreateDirectory(subPath);
        //        subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id);
        //        exists = System.IO.Directory.Exists(subPath);
        //        if (!exists)
        //            System.IO.Directory.CreateDirectory(subPath);
        //        subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id + "/CAD");
        //        exists = System.IO.Directory.Exists(subPath);
        //        if (!exists)
        //            System.IO.Directory.CreateDirectory(subPath);
        //        f0.SaveAs(subPath + "/" + fileCad);                
        //        hoso.link_filecad = address.code + "/" + contract.id + "/CAD" + "/" + fileCad;                
        //        if (!string.IsNullOrEmpty(hoso.link_filecad))
        //        {
        //            string filename = Utils.GetSlug(contract.name) + contract.id + "_" + time + "_QRCAD.jpg";
        //            hoso.link_filecad_qr = address.code + "/" + contract.id + "/CAD" + "/" + filename;                    
        //            var dove = hoso.link_filecad_qr;
        //            string url = "https://chart.googleapis.com/chart?cht=qr&chl=" + XModels.LinkDove + "/" + hoso.link_filecad + "&choe=UTF-8&chs=400x400";
        //            WebClient webClient = new WebClient();
        //            webClient.DownloadFile(url, subPath + "/" + filename);
        //        }
        //        else
        //            hoso.link_filecad_qr = string.Empty;
        //    }
        //    db.SaveChanges();
        //    return Json(hoso.link_filecad, JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        public JsonResult LuuFileAutoCad(int hoso_id, string link_filecad)
        {
            var hoso = db.HoSoes.Find(hoso_id);
            var contract = db.Contracts.Find(hoso.contract_id);
            var address = db.Addresses.Find(contract.address_id);
            var time = DateTime.Now.ToString("mmHHddMMyy");
            if (!link_filecad.Equals(hoso.link_filecad))
            {
                hoso.link_filecad = link_filecad;
                if (!string.IsNullOrEmpty(link_filecad))
                {
                    string subPath = Server.MapPath("~/public/" + address.code);
                    bool exists = System.IO.Directory.Exists(subPath);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(subPath);
                    subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id);
                    exists = System.IO.Directory.Exists(subPath);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(subPath);
                    subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id + "/CAD");
                    exists = System.IO.Directory.Exists(subPath);
                    if (!exists)
                        System.IO.Directory.CreateDirectory(subPath);

                    string filename = contract.id + "_" + time + "_QRCAD.jpg";
                    hoso.link_filecad_qr = "/" + address.code + "/" + contract.id + "/CAD" + "/" + filename;

                    WebClient webClient = new WebClient();
                    try
                    {
                        string url = "https://quickchart.io/qr?text=" + link_filecad + "&size=400";
                        webClient.DownloadFile(url, subPath + "/" + filename);
                    }
                    catch
                    {
                        string url = "https://chart.googleapis.com/chart?cht=qr&chl=" + link_filecad + "&choe=UTF-8&chs=400x400";
                        webClient.DownloadFile(url, subPath + "/" + filename);
                    }
                }
                else
                {
                    hoso.link_filecad = string.Empty;
                    hoso.link_filecad_qr = string.Empty;
                }
            }
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Lưu file CAD (" + hoso.name + ")");
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateProcessing(int hoso_id, string note)
        {
            var username = User.Identity.GetUserName();
            var acc = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var hoso = db.HoSoes.Find(hoso_id);
            var currentstep = db.wf_hoso_step.OrderByDescending(x => x.time).FirstOrDefault(x => x.hoso_id == hoso_id && !x.next_step.HasValue);
            if (currentstep == null)
            {
                currentstep = new wf_hoso_step() { account_id = acc.id, hoso_id = hoso_id, note = "Tự động khởi tạo", step_id = hoso.step_id, time = DateTime.Now };
            }
            db.wf_hoso_step.Add(currentstep);
            db.SaveChanges();
            var wf = new wf_hoso();
            wf.note = note;
            wf.hoso_id = hoso_id;
            wf.account_id = acc.id;
            wf.time = DateTime.Now;
            wf.step_id = currentstep.id;
            db.wf_hoso.Add(wf);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ReloadProcessing(int hoso_id)
        {
            string result = string.Empty;
            var processing = db.wf_hoso.Where(x => x.hoso_id == hoso_id).OrderByDescending(x => x.time);
            foreach (var item in processing)
            {
                var us = db.Accounts.Find(item.account_id);
                result += "<li><span>" + item.time.ToString("dd/MM/yy") + "</span> | <span>" + us.fullname + "</span> | " + item.note + "</li>";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ShowHoSoSession(int hoso_id, int progress_type)
        {
            var hoso = db.HoSoes.Find(hoso_id);
            switch (progress_type)
            {
                case (int)XProgress.eType.NhiemVu:
                    return PartialView("../Progresses/NhiemVu", hoso);
                case (int)XProgress.eType.DieuTra:
                    return PartialView("../Progresses/DieuTra", hoso);
                case (int)XProgress.eType.GiaPha:
                    return PartialView("../Progresses/GiaPha", hoso);
                case (int)XProgress.eType.BanVe:
                    return PartialView("../Progresses/BanVe", hoso);
                case (int)XProgress.eType.GiayToHoTich:
                    return PartialView("../Progresses/HoTich", hoso);
                case (int)XProgress.eType.CongChung:
                    return PartialView("../Progresses/CongChung", hoso);
                case (int)XProgress.eType.GiayToXa:
                    return PartialView("../Progresses/Town", hoso);
                case (int)XProgress.eType.DonThu:
                    return PartialView("../Progresses/DonThu", hoso);
                default:
                    return null;
            }
        }
        [HttpGet]
        public JsonResult HoSoCheckNotice(int hoso_id, int progress_type)
        {
            var progress = db.Progresses.FirstOrDefault(x => x.type == progress_type);
            var option = db.progress_file_option.FirstOrDefault(x => x.hoso_id == hoso_id && x.progress_id == progress.id);
            var job = db.Jobs.Where(x => x.hoso_id == hoso_id && x.progress_type == progress_type).OrderByDescending(x => x.start_date).FirstOrDefault();
            task viecPhaiLam = null;
            if (job != null)
                viecPhaiLam = db.tasks.Where(x => x.job_id == job.id && x.progress_type == progress_type).OrderByDescending(x => x.time).FirstOrDefault();
            string result = string.Empty;
            if (viecPhaiLam != null && viecPhaiLam.time_exp <= DateTime.Now && string.IsNullOrEmpty(viecPhaiLam.note) && (option == null || option.status == (int)XModels.eLevel.Level1))
            {
                result = "<i class=\"fa fa-bell-on text-danger\"></i>";
            }
            else if (option != null && option.status == (int)XModels.eLevel.Level2)
            {
                result = "<i class=\"fa fa-check text-success\"></i>";
            }
            else if (viecPhaiLam != null && viecPhaiLam.time_exp > DateTime.Now && string.IsNullOrEmpty(viecPhaiLam.note))
            {
                var time = (int)((viecPhaiLam.time_exp - DateTime.Now).TotalDays);
                if (time > 0)
                {
                    result = "<i class='text-warning'>" + time + "</i>";
                }
                else
                {
                    result = "<i class=\"fa fa-bell-on text-warning\"></i>";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult HoSoCheckNoticeAll(int hoso_id)
        {
            string result = string.Empty;
            var job = db.Jobs.Where(x => x.hoso_id == hoso_id).OrderByDescending(x => x.start_date).FirstOrDefault();
            task viecPhaiLam = null;
            foreach (var progress in db.Progresses)
            {
                var option = db.progress_file_option.FirstOrDefault(x => x.hoso_id == hoso_id && x.progress_id == progress.id);

                if (job != null)
                    viecPhaiLam = db.tasks.Where(x => x.job_id == job.id && x.progress_type == progress.type).OrderByDescending(x => x.time).FirstOrDefault();
                if (viecPhaiLam != null && viecPhaiLam.time_exp <= DateTime.Now && string.IsNullOrEmpty(viecPhaiLam.note) && (option == null || option.status == (int)XModels.eLevel.Level1))
                {
                    result = "<i class=\"fa fa-bell-on text-danger\"></i>";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TheoDoiTienDo(string Search_Data, int? add, int? uq, int? ser, int? par, int? ste, string Filter_Value, int? Page_No)
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

            var results = from io in db.vTheoDoiTienDoes
                          where (
                             (io.account_id == (uq.HasValue ? uq : io.account_id)) &&
                               (io.step_id == (ste.HasValue ? ste : io.step_id)) &&
                               (io.partner == (par.HasValue ? par.Value : io.partner)) &&
                               (io.address_id == (add.HasValue ? add : io.address_id)) &&
                               io.service_id == (ser.HasValue ? ser.Value : io.service_id) &&
                               io.status == (int)XContract.eStatus.Processing &&
                           io.name.ToUpper().Contains((!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.name.ToUpper())
                          ))
                          orderby io.id descending
                          select io;
            ViewBag.db = db;
            int Size_Of_Page = 30;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        [HttpPost]
        public JsonResult DSCongViec()
        {
            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/DailyPland.docx");
            Aspose.Words.Document doc = new Aspose.Words.Document(pathTemp);
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var results = (from io in db.vHoSoes
                           where
                                io.status == (int)XHoSo.eStatus.Processing &&
                                io.account_id == us.id
                           orderby io.address_name, io.priority descending, io.time descending
                           select io).ToArray();

            string webpart = "DailyPland_" + DateTime.Now.ToString("ssmmHH") + ".docx";
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            builder.MoveToMergeField("ngaythang");
            builder.Writeln("Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
            builder.MoveToMergeField("dscongviec");
            List<Account> P = new List<Account>();
            List<Job> J = new List<Job>();
            List<task> T = new List<task>();
            J = db.Jobs.Where(x => x.process_by == us.id && x.status < (int)XJob.eStatus.Complete).ToList();
            foreach (var item in J)
            {
                builder.ParagraphFormat.LineSpacingRule = Aspose.Words.LineSpacingRule.AtLeast;
                builder.ParagraphFormat.SpaceAfter = 6;
                builder.ParagraphFormat.SpaceBefore = 6;
                builder.Bold = true;
                builder.Writeln(item.name);
                T = db.tasks.Where(x => x.job_id == item.id && x.status == (int)XModels.eStatus.Processing).ToList();
                builder.Bold = false;
                builder.ParagraphFormat.LineSpacingRule = Aspose.Words.LineSpacingRule.AtLeast;
                builder.ParagraphFormat.SpaceAfter = 0;
                builder.ParagraphFormat.SpaceBefore = 0;
                foreach (var t in T)
                {
                    builder.Writeln("       " + t.name);
                }
                Shape line = new Shape(doc, ShapeType.Line);
                line.Width = 500;
                builder.InsertNode(line);
            }
            builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
            builder.MoveToMergeField("dshoso");
            builder.ParagraphFormat.SpaceAfterAuto = false;
            foreach (var item in results)
            {
                var wf = db.wf_hoso.Where(x => x.hoso_id == item.id).OrderByDescending(x => x.time).FirstOrDefault();
                builder.ParagraphFormat.LineSpacingRule = Aspose.Words.LineSpacingRule.AtLeast;
                builder.ParagraphFormat.SpaceAfter = 0;
                builder.Writeln(item.name + " - " + item.service_name + " - " + item.address_name);
                builder.ParagraphFormat.SpaceAfter = 24;
                builder.Italic = true;
                builder.Writeln("       " + (wf != null ? wf.note : ""));
                builder.Italic = false;
                builder.ParagraphFormat.SpaceAfter = 0;
                Shape line = new Shape(doc, ShapeType.Line);
                line.Width = 500;
                builder.InsertNode(line);

                //builder.InsertHtml("<hr />");
            }
            doc.Save(path + webpart);
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }
        #region MyRegion
        //public JsonResult DSCongViec()
        //{
        //    var username = User.Identity.GetUserName();
        //    var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

        //    var results = (from io in db.vHoSoes
        //                   where
        //                        io.status == (int)XHoSo.eStatus.Processing &&
        //                        io.account_id == us.id
        //                   orderby io.address_name, io.priority descending, io.time descending
        //                   select io).ToArray();

        //    var step = db.Steps.Single(x => x.code.Equals("DONTHU"));
        //    var donthus = (from don in results
        //                   where don.step_id == step.id
        //                   select don).ToArray();

        //    string path = Server.MapPath("~/public/");
        //    string pathTemp = Server.MapPath("~/App_Data/templates/BangDSCongViec.xls");
        //    string webpart = "BangDSCongViec_" + DateTime.Now.ToString("ssmmHH") + ".xls";
        //    FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
        //    using (FileStream fs = System.IO.File.Create(path + webpart))
        //    {
        //        using (FileStream sr = System.IO.File.OpenRead(pathTemp))
        //        {
        //            flexCelReport.SetValue("NGAYTHANG", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
        //            flexCelReport.AddTable("T", results);
        //            flexCelReport.AddTable("D", donthus);
        //            flexCelReport.Run(sr, fs);
        //        }
        //    }
        //    db.SaveChanges();
        //    return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        //} 
        #endregion
        [HttpGet]
        public JsonResult GetHosoService(string term, int? page, string _type)
        {
            int p = page ?? 0;
            int ps = 25;
            var lstHosoes = (from ct in db.HoSoes
                             join step in db.Steps on ct.step_id equals step.id
                             where ct.status == (int)XHoSo.eStatus.Processing && (ct.name.ToUpper().Contains(String.IsNullOrEmpty(term) ? ct.name.ToUpper() : term))
                             select new { id = ct.id, cus = ct.name, add = step.name });
            data d = new data();
            d.total_count = lstHosoes.Count();
            var lstProcess = lstHosoes.OrderBy(x => x.cus).Skip(p * ps).Take(ps).ToList();
            List<item> lst = new List<item>();
            foreach (var item in lstProcess)
            {
                item it = new item();
                it.name = item.cus;
                it.text = item.add;
                it.id = item.id;
                lst.Add(it);
            }
            d.items = lst;
            return Json(d, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetHosoDupplicateService(string term, int? page, string _type)
        {
            int p = page ?? 0;
            int ps = 25;
            var lstHosoes = (from ct in db.HoSoes
                             join step in db.Steps on ct.step_id equals step.id
                             join con in db.Contracts on ct.contract_id equals con.id
                             join add in db.Addresses on con.address_id equals add.id

                             where ct.name.ToUpper().Contains(String.IsNullOrEmpty(term) ? ct.name.ToUpper() : term)
                             select new { id = ct.id, cus = ct.name, add = add.name + " - " + step.name });
            data d = new data();
            d.total_count = lstHosoes.Count();
            var lstProcess = lstHosoes.OrderBy(x => x.cus).Skip(p * ps).Take(ps).ToList();
            List<item> lst = new List<item>();
            foreach (var item in lstProcess)
            {
                item it = new item();
                it.name = item.cus;
                it.text = item.add;
                it.id = item.id;
                lst.Add(it);
            }
            d.items = lst;
            return Json(d, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateSoanThao(int hsId, int ddlSoanThao)
        {
            HoSo hoSo = db.HoSoes.Find(hsId);
            hoSo.account_id_soanthao = ddlSoanThao;
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Đổi người soạn thảo hồ sơ (" + hoSo.name + ")");
            db.SaveChanges();
            return RedirectToAction("details", new { id = hsId });
        }
        [HttpPost]
        public ActionResult UpdatePhuTrach(int hsId, int ddlPhuTrach)
        {
            HoSo hoSo = db.HoSoes.Find(hsId);
            hoSo.account_id = ddlPhuTrach;
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Đổi người phụ trách hồ sơ (" + hoSo.name + ")");
            db.SaveChanges();
            return RedirectToAction("details", new { id = hsId });
        }
        [HttpGet]
        public JsonResult CreateModel(int hoso_id)
        {
            var hoso = db.HoSoes.Find(hoso_id);
            return Json(hoso, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult OnChangeLight(int id)
        {
            var hoso = db.HoSoes.Find(id);
            hoso.light = (hoso.light.HasValue && hoso.light == 1) ? 0 : 1;
            db.SaveChanges();
            return Json(hoso.light.Value, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DupplicateInfomation(int id)
        {
            var hoso = db.HoSoes.Find(id);
            var info = db.Infomations.FirstOrDefault(x => x.hoso_id == hoso.id);
            return Json(info, JsonRequestBehavior.AllowGet);
        }
    }
}
