using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using Aspose.Words.Rendering;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Manager,Member")]
    public class di1cuaController : Controller
    {
        private NHModel db = new NHModel();
        // GET: di1cua       
        public ActionResult Index(string Search_Data, int? add, int? uq, int? ser, int? par, int? canbo, int? group, int? place, string Filter_Value, int? Page_No)
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
            ViewBag.GROUP = group;
            ViewBag.CANBO = canbo;
            ViewBag.PAR = par;
            ViewBag.PLACE = place;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");
            ViewBag.CANBOES = new SelectList(db.CanBoes, "id", "name");
            ViewBag.UYQUYENS = new SelectList(db.Accounts.Where(x => x.is_uq == (int)XModels.eYesNo.Yes), "id", "fullname");
            ViewBag.PLACE = new SelectList(db.CoQuanGiaiQuyets, "id", "name");
            var step = db.Steps.Single(x => x.code.Equals("NOP1CUA"));
            var stepDonThu = db.Steps.Single(x => x.code.Equals("DONTHU"));
            var lst1cua = from x in db.vDi1Cua
                          where (x.account_id == (uq.HasValue ? uq : x.account_id)) &&
                               (x.address_id == (add.HasValue ? add : x.address_id)) &&
                               x.service_id == (ser.HasValue ? ser.Value : x.service_id) &&
                               x.partner == (par.HasValue ? par.Value : x.partner) &&
                               x.name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : x.name.ToUpper()) &&
                               x.status == (int)XHoSo.eStatus.Processing &&
                               (x.step_id == step.id || x.step_id == stepDonThu.id) &&
                               x.noinhan_id == (place.HasValue ? place.Value : x.noinhan_id) &&
                               x.canbo1cua == (canbo.HasValue ? canbo.Value : x.canbo1cua)
                          orderby x.account_id descending
                          select x;
            return View(lst1cua);
        }
        [HttpGet]
        public JsonResult PrintByFilter(string Search_Data, int? add, int? uq, int? ser, int? par, int? place, int? canbo, int? group)
        {
            var step = db.Steps.Single(x => x.code.Equals("NOP1CUA"));
            var stepDonThu = db.Steps.Single(x => x.code.Equals("DONTHU"));
            var lst1cua = from x in db.vDi1Cua
                          where (x.account_id == (uq.HasValue ? uq : x.account_id)) &&
                               (x.address_id == (add.HasValue ? add : x.address_id)) &&
                               x.service_id == (ser.HasValue ? ser.Value : x.service_id) &&
                               x.partner == (par.HasValue ? par.Value : x.partner) &&
                               x.name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : x.name.ToUpper()) &&
                               x.status == (int)XHoSo.eStatus.Processing &&
                               (x.step_id == step.id || x.step_id == stepDonThu.id) &&
                               x.noinhan_id == (place.HasValue ? place.Value : x.noinhan_id) &&
                               x.canbo1cua == (canbo.HasValue ? canbo.Value : x.canbo1cua)
                          orderby x.account_id descending
                          select x;
            ///END
            var lst = new List<PrintByFilterCls>();
            foreach (var item in lst1cua)
            {
                var hoso = db.HoSoes.Find(item.hoso_id);
                if (hoso == null)
                    continue;
                var contract = db.Contracts.Find(hoso.contract_id);
                if (contract == null)
                    continue;
                var per = new PrintByFilterCls();
                var address = db.Addresses.Find(contract.address_id);
                var infomation = db.Infomations.Single(x => x.hoso_id == item.hoso_id);
                var service = db.Services.Find(hoso.service_id);
                var canbothuly = db.CanBoes.Find(item.canbo1cua);
                var quahan = (DateTime.Now - item.ngaytra).Days;

                per.quahan = quahan > 0 ? quahan.ToString("N0") : "-";
                per.name = hoso.name;
                per.service = service.name;
                per.address = address.name;
                //per.cadres = canbothuly.name;
                per.mahoso = item.maphieuhen;
                per.ngaynop = item.ngaynop.ToString("dd/MM/yyyy");
                per.ngayhan = item.ngaytra.ToString("dd/MM/yyyy");
                per.quahan = quahan > 0 ? quahan.ToString("N0") : "-";
                per.note = NiceHandles.HtmlHelper.StripHtmlAndDecode(item.note + item.ghichu);

                per.ngoaigiao = (hoso.ngoaigiao.HasValue && hoso.ngoaigiao.Value == (int)XHoSo.eNgoaiGiao.NgoaiGiao) ? "Có" : "";
                lst.Add(per);
            }

            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/DS1CuaPrintByFilter.xls"); //document.template

            string webpart = "DS1CuaPrintByFilter";
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
        public ActionResult SoanThao(string Search_Data, int? add, int? uq, int? ser, int? par, int? canbo, int? group, string Filter_Value, int? Page_No)
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
            ViewBag.GROUP = group;
            ViewBag.CANBO = canbo;
            ViewBag.PAR = par;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
            ViewBag.PARTNER = new SelectList(db.Partners.OrderBy(x => x.sothutu), "id", "name");
            ViewBag.CANBOES = new SelectList(db.CanBoes, "id", "name");
            ViewBag.UYQUYENS = new SelectList(db.Accounts.Where(x => x.is_uq == (int)XModels.eYesNo.Yes), "id", "fullname");
            var step = db.Steps.Single(x => x.code.Equals("NOP1CUA"));

            ViewBag.MANAGER = true;
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            var lst1cua = from x in db.vDi1Cua
                          where (x.account_id == (uq.HasValue ? uq : x.account_id)) &&
                               x.account_id_soanthao == us.id &&
                               (x.address_id == (add.HasValue ? add : x.address_id)) &&
                               x.service_id == (ser.HasValue ? ser.Value : x.service_id) &&
                               x.partner == (par.HasValue ? par.Value : x.partner) &&
                               x.name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : x.name.ToUpper()) &&
                               x.status == (int)XHoSo.eStatus.Processing &&
                               x.canbo1cua == (canbo.HasValue ? canbo.Value : x.canbo1cua)
                          orderby x.account_id descending
                          select x;
            return View(lst1cua);
        }
        public ActionResult DSDonThu(string Search_Data, int? add, int? uq, int? type, int? canbo, string Filter_Value, int? Page_No)
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
            ViewBag.TYPE = type;
            ViewBag.CANBO = canbo;
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.CANBOES = new SelectList(db.CanBoes, "id", "name");
            ViewBag.UYQUYENS = new SelectList(db.Accounts.Where(x => x.is_uq == (int)XModels.eYesNo.Yes), "id", "fullname");
            ViewBag.TYPE = new SelectList(XModels.sLoaiDonThu, "Key", "Value");
            var step = db.Steps.Single(x => x.code.Equals("DONTHU"));
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            var donthus = (from don in db.donthus
                           join hoso in db.HoSoes
                           on don.hoso_id equals hoso.id
                           where hoso.status == (int)XHoSo.eStatus.Processing && don.type == (type.HasValue ? type.Value : don.type) && hoso.step_id == step.id
                           select hoso.id).Distinct().ToArray();

            var lst1cua = from x in db.vDi1Cua
                          where (x.account_id == (uq.HasValue ? uq : x.account_id)) &&
                               (x.address_id == (add.HasValue ? add : x.address_id)) &&
                               donthus.Contains(x.hoso_id) &&
                               x.name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : x.name.ToUpper()) &&
                               x.status == (int)XHoSo.eStatus.Processing &&
                               x.canbo1cua == (canbo.HasValue ? canbo.Value : x.canbo1cua)
                          orderby x.account_id descending
                          select x;
            return View(lst1cua);
        }
        // GET: di1cua/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            di1cua di1cua = db.di1cua.Find(id);
            if (di1cua == null)
            {
                return HttpNotFound();
            }
            return View(di1cua);
        }

        // GET: di1cua/Create
        public ActionResult Create()
        {
            var lstCT = from ct in db.Contracts
                        join add in db.Addresses on ct.address_id equals add.id
                        where ct.status == (int)XContract.eStatus.Processing
                        select new { id = ct.id.ToString(), name = ct.name + " - " + add.name };
            ViewBag.lstCT = new SelectList(lstCT, "id", "name");
            ViewBag.CanBo = new SelectList(db.CanBoes.ToArray(), "id", "name", db.CanBoes.Where(x => x.motcua == (int)XModels.eYesNo.Yes).OrderBy(x => x.thutu).FirstOrDefault());
            return View();
        }
        public ActionResult CreateF()
        {
            return View();
        }

        // POST: di1cua/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(di1cua di1cua, bool? ajax)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity.GetUserName();
                var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
                bool exit = db.di1cua.Any(x => x.hoso_id == di1cua.hoso_id);
                if (!exit)
                    db.di1cua.Add(di1cua);
                else
                {
                    di1cua.id = db.di1cua.Single(x => x.hoso_id == di1cua.hoso_id).id;
                    db.Entry(di1cua).State = EntityState.Modified;
                }
                Contract ct = db.Contracts.Find(di1cua.hoso_id);
                wf_contract wf = new wf_contract();
                wf.status = (int)Xwf_contract.eStatus.Processing;
                wf.time = DateTime.Now;
                wf.note = "Đi nộp 1 cửa";
                wf.account_id = us.id;
                wf.contract_id = ct.id;

                var backWF = db.wf_contract.Where(x => x.contract_id == ct.id).OrderByDescending(x => x.time).FirstOrDefault();
                wf.step_id = backWF.step_id;
                wf.from_id = backWF.from_id;

                db.wf_contract.Add(wf);
                di1cua.type = ct.type;
                db.SaveChanges();
                if (ajax.HasValue)
                    return Json(true, JsonRequestBehavior.AllowGet);
                return RedirectToAction("Index");
            }
            var lstCT = from ct in db.Contracts
                        join add in db.Addresses on ct.address_id equals add.id
                        where ct.status == (int)XContract.eStatus.Processing
                        select new { id = ct.id.ToString(), name = ct.name + " - " + add.name };
            ViewBag.lstCT = new SelectList(lstCT, "id", "name");
            ViewBag.CanBo = new SelectList(db.CanBoes.ToArray(), "id", "name", db.CanBoes.Where(x => x.motcua == (int)XModels.eYesNo.Yes).FirstOrDefault());
            return View(di1cua);
        }

        // GET: di1cua/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            di1cua di1cua = db.di1cua.Find(id);
            if (di1cua == null)
            {
                return HttpNotFound();
            }
            var lstCT = from ct in db.HoSoes
                        where (ct.id == di1cua.hoso_id)
                        select new { id = ct.id.ToString(), name = ct.name };
            ViewBag.lstCT = new SelectList(lstCT, "id", "name");
            ViewBag.CanBo = new SelectList(db.CanBoes.ToArray(), "id", "name");
            return View(di1cua);
        }

        // POST: di1cua/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(di1cua di1cua)
        {
            if (ModelState.IsValid)
            {
                db.Entry(di1cua).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(di1cua);
        }
        [HttpPost]
        public JsonResult Save(di1cua di1cua, string ngaynop, string ngaytra)
        {
            if (ModelState.IsValid)
            {
                di1cua.ngaynop = Convert.ToDateTime(ngaynop);
                di1cua.ngaytra = Convert.ToDateTime(ngaytra);
                db.Entry(di1cua).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(di1cua, JsonRequestBehavior.AllowGet);
        }

        // GET: di1cua/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            di1cua di1cua = db.di1cua.Find(id);
            if (di1cua == null)
            {
                return HttpNotFound();
            }
            return View(di1cua);
        }

        // POST: di1cua/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            di1cua di1cua = db.di1cua.Find(id);
            db.di1cua.Remove(di1cua);
            var ct = db.Contracts.Find(di1cua.hoso_id);
            wf_contract wf = new wf_contract();
            wf.contract_id = di1cua.hoso_id;
            wf.status = (int)Xwf_contract.eStatus.Processing;
            wf.time = DateTime.Now;
            wf.account_id = us.id;
            wf.note = "Hủy đi 1 cửa";

            var backWF = db.wf_contract.Where(x => x.contract_id == ct.id).OrderByDescending(x => x.time).FirstOrDefault();
            wf.step_id = backWF.step_id;
            wf.from_id = backWF.from_id;

            db.wf_contract.Add(wf);
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
        public JsonResult UpdateStatus(di1cua di1cua)
        {
            var obj = db.di1cua.Find(di1cua.id);
            obj.trangthai = di1cua.trangthai;
            obj.ghichu = di1cua.ghichu;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ViewDown(int id, int t)
        {
            var files = db.OneDoorFiles.Where(x => x.hoso_id == id && x.type == t).ToArray();
            string result = string.Empty;
            foreach (var file in files)
            {
                result += "<li class='text-info'><a target='_blank' href='" + file.url + "'>" + file.name + "</a><span id='" + file.id + "' class='del' onclick='delFile(this);'>x</span></li>";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin,Manager")]
        [HttpPost]
        public JsonResult DelFile(int id)
        {
            OneDoorFile f = db.OneDoorFiles.Find(id);
            db.OneDoorFiles.Remove(f);
            db.SaveChanges();
            return Json(f.name, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult Upload()
        {
            var contract = db.Contracts.Find(Convert.ToInt64(Request["id"]));
            var t = Convert.ToInt32(Request["t"]);
            var address = db.Addresses.Find(contract.address_id);
            OneDoorFile f = new OneDoorFile();

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
                subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id + "/" + XFile.sType[t]);
                exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                f0.SaveAs(subPath + "/" + f0.FileName);
                f.url = "/public/" + address.code + "/" + contract.id + "/" + XFile.sType[t] + "/" + f0.FileName;
                f.name = f0.FileName;
            }
            f.type = t;
            f.hoso_id = contract.id;
            db.OneDoorFiles.Add(f);
            db.SaveChanges();
            return ViewDown(contract.id, t);
        }
        [HttpGet]
        public JsonResult PrintWarning(int hoso_id)
        {
            Document document = db.Documents.Where(d => d.code.ToUpper().Equals("DPATC")).Single();
            InfomationsController controller = new InfomationsController();
            controller.ControllerContext = this.ControllerContext;
            return controller.Download(document.id, hoso_id);

            //var hoso = db.HoSoes.Find(hoso_id);
            //var contract = db.Contracts.Find(hoso.contract_id);
            //var address = db.Addresses.Find(contract.address_id);

            //var account = db.Accounts.Find(contract.account_id); // 3 là Giang
            //Infomation info = db.Infomations.Single(x => x.hoso_id == hoso_id);
            //var onedoor = db.di1cua.Single(x => x.hoso_id == hoso_id && x.trangthai < (int)Xdi1cua.eStatus.ThanhCong);
            //string path = Server.MapPath("~/public/");
            //string pathTemp = Server.MapPath("~/App_Data/templates/" + document.template);
            //string webpart = Utils.ConvertToUnsign(contract.name) + "_" + Utils.ConvertToUnsign(address.name) + "_" + Utils.ConvertToUnsign(document.name);

            //Aspose.Words.Document doc = new Aspose.Words.Document(pathTemp);
            //webpart += ".doc";
            //path += webpart;
            //var dict = new Dictionary<string, string>();

            //foreach (var item in typeof(Infomation).GetProperties())
            //{
            //    var name = item.Name;
            //    var value = item.GetValue(info);
            //    string val = value == null ? "......................." : !string.IsNullOrEmpty(value.ToString()) ? value.ToString() : ".......................";
            //    dict.Add(item.Name, val);
            //}
            //dict["e_ngaysinh"] = info.e_ngaysinh.HasValue ? info.e_ngaysinh.Value.ToString("dd/MM/yyyy") : "............";
            //dict["e_ngaycap_gt"] = info.e_ngaycap_gt.HasValue ? info.e_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "............";
            //var quahan = (DateTime.Now - onedoor.ngaytra).Days;
            //dict.Add("e_namsinh", info.e_ngaysinh.HasValue ? info.e_ngaysinh.Value.ToString("yyyy") : ".........");
            //dict.Add("e_sdt", account.phoneno);
            //dict.Add("e_email", account.email);
            //dict.Add("maphieuhen", onedoor.maphieuhen);
            //dict.Add("ngaynop", onedoor.ngaynop.ToString("dd/MM/yyyy"));
            //dict.Add("ngayhen", onedoor.ngaytra.ToString("dd/MM/yyyy"));
            //dict.Add("quahan", quahan < 10 ? "0" + quahan.ToString() : quahan.ToString());

            //doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());

            //doc.Save(path);

            //return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult PrintDS(int? account_id)
        {
            var lst = new List<PDi1Cua>();
            var lstNN1 = new List<PDi1Cua>();
            var lstNN2 = new List<PDi1Cua>();
            var lstNN3 = new List<PDi1Cua>();
            var lst1cua = db.di1cua.Where(x => x.trangthai != (int)Xdi1cua.eStatus.ThanhCong).ToList();
            foreach (var item in lst1cua)
            {
                var hoso = db.HoSoes.Find(item.hoso_id);
                if (hoso == null)
                    continue;
                var contract = db.Contracts.Find(hoso.contract_id);
                if (contract == null)
                    continue;
                if (hoso.status == (int)XContract.eStatus.Processing)
                {
                    if (account_id.HasValue && hoso.account_id != account_id.Value)
                        continue;
                    var per = new PDi1Cua();
                    var address = db.Addresses.Find(contract.address_id);
                    var infomation = db.Infomations.Single(x => x.hoso_id == item.hoso_id);
                    var service = db.Services.Find(hoso.service_id);
                    //var canbo = db.fk_CanBo_Address.Where(x => x.address_id == contract.address_id).FirstOrDefault();
                    //var cadres = canbo != null ? db.CanBoes.Find(canbo.canbo_id) : new CanBo();
                    var canbo = db.CanBoes.Find(item.canbo1cua);

                    var quahan = (DateTime.Now - item.ngaytra).Days;
                    var canhbao = string.Empty;
                    if (quahan < 0 - service.nhacnho)
                    {
                        if (item.ngaynop < DateTime.Now.AddDays(-5))
                        {
                            var perNN1 = new PDi1Cua();
                            perNN1.canhbao = canhbao;
                            perNN1.quahan = quahan > 0 ? quahan.ToString("N0") : "-";
                            perNN1.note = item.ghichu;
                            perNN1.name = hoso.name;
                            perNN1.service = service.name;
                            perNN1.address = address.name;
                            perNN1.cadres = canbo.name;
                            perNN1.uyquyen = String.IsNullOrEmpty(infomation.e_hoten) ? "" : infomation.e_hoten.Substring(infomation.e_hoten.LastIndexOf(" "), infomation.e_hoten.Length - infomation.e_hoten.LastIndexOf(" "));
                            perNN1.mahoso = item.maphieuhen;
                            perNN1.snqh = quahan;
                            perNN1.ngaynop = item.ngaynop.ToString("dd/MM/yyyy");

                            lstNN1.Add(perNN1);
                        }
                        canhbao = Xdi1cua.sCanhBao[(int)Xdi1cua.eCanhBao.Khong];
                    }
                    else if (quahan <= 0)
                    {
                        canhbao = Xdi1cua.sCanhBao[(int)Xdi1cua.eCanhBao.NhacNho];
                        var perNN2 = new PDi1Cua();
                        perNN2.canhbao = canhbao;
                        perNN2.quahan = quahan > 0 ? quahan.ToString("N0") : "-";
                        perNN2.note = item.ghichu;
                        perNN2.name = contract.name;
                        perNN2.service = service.name;
                        perNN2.address = address.name;
                        perNN2.cadres = canbo.name;
                        perNN2.uyquyen = String.IsNullOrEmpty(infomation.e_hoten) ? "" : infomation.e_hoten.Substring(infomation.e_hoten.LastIndexOf(" "), infomation.e_hoten.Length - infomation.e_hoten.LastIndexOf(" "));
                        perNN2.mahoso = item.maphieuhen;
                        perNN2.snqh = quahan;
                        perNN2.ngaynop = item.ngaynop.ToString("dd/MM/yyyy");
                        perNN2.ngaytra = item.ngaytra.ToString("dd/MM/yyyy");

                        lstNN2.Add(perNN2);
                    }
                    else if (quahan < service.phananh)
                    {
                        canhbao = Xdi1cua.sCanhBao[(int)Xdi1cua.eCanhBao.NhacNho];
                    }
                    else if (quahan < service.tocao)
                    {
                        canhbao = Xdi1cua.sCanhBao[(int)Xdi1cua.eCanhBao.PhanAnh];
                    }
                    else
                    {
                        canhbao = Xdi1cua.sCanhBao[(int)Xdi1cua.eCanhBao.ToCao];
                    }
                    if (quahan > 0)
                    {
                        var perNN3 = new PDi1Cua();
                        perNN3.canhbao = canhbao;
                        perNN3.quahan = quahan > 0 ? quahan.ToString("N0") : "-";
                        perNN3.note = item.ghichu;
                        perNN3.name = contract.name;
                        perNN3.service = service.name;
                        perNN3.address = address.name;
                        perNN3.cadres = canbo.name;
                        perNN3.uyquyen = String.IsNullOrEmpty(infomation.e_hoten) ? "" : infomation.e_hoten.Substring(infomation.e_hoten.LastIndexOf(" "), infomation.e_hoten.Length - infomation.e_hoten.LastIndexOf(" "));
                        perNN3.mahoso = item.maphieuhen;
                        perNN3.snqh = quahan;
                        perNN3.ngaynop = item.ngaynop.ToString("dd/MM/yyyy");

                        lstNN3.Add(perNN3);
                    }

                    per.quahan = quahan > 0 ? quahan.ToString("N0") : "-";
                    per.canhbao = canhbao;
                    per.note = item.ghichu;
                    per.name = contract.name;
                    per.service = service.name;
                    per.address = address.name;
                    per.cadres = canbo.name;
                    per.uyquyen = String.IsNullOrEmpty(infomation.e_hoten) ? "" : infomation.e_hoten.Substring(infomation.e_hoten.LastIndexOf(" "), infomation.e_hoten.Length - infomation.e_hoten.LastIndexOf(" "));
                    per.snqh = quahan;

                    lst.Add(per);
                }
            }

            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/OneDoors.xlsx"); //document.template

            string webpart = "DS1cua";
            webpart += ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();

            Account account = null;
            string e_hoten = "..............", e_sogiayto = "..............", e_sodienthoai = "..............";
            if (account_id.HasValue)
            {
                account = db.Accounts.Find(account_id.Value);
                e_hoten = account.fullname;
                e_sogiayto = account.sogiayto;
                e_sodienthoai = account.phoneno;
            }
            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("NGAYTHANG", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);

                    flexCelReport.SetValue("e_hoten", e_hoten);
                    flexCelReport.SetValue("e_sogiayto", e_sogiayto);
                    flexCelReport.SetValue("e_sodienthoai", e_sodienthoai);

                    flexCelReport.AddTable("TB", lst.OrderByDescending(x => x.snqh));
                    flexCelReport.AddTable("NN1", lstNN1.OrderByDescending(x => x.snqh));
                    flexCelReport.AddTable("NN2", lstNN2.OrderByDescending(x => x.snqh));
                    flexCelReport.AddTable("NN3", lstNN3.OrderByDescending(x => x.snqh));

                    flexCelReport.Run(sr, fs);
                }
            }
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult PrintDSDonThu(int? account_id)
        {
            // ĐƠN THƯ
            var step = db.Steps.Single(x => x.code.Equals("DONTHU"));
            var donthus = (from don in db.donthus
                           join hoso in db.HoSoes
                           on don.hoso_id equals hoso.id
                           where hoso.status == (int)XHoSo.eStatus.Processing && !don.donthu_id.HasValue && hoso.step_id == step.id
                           select don).ToList();

            var donthuids = donthus.Select(x => x.hoso_id).Distinct().ToList();

            var dshoso = (from x in db.vDi1Cua
                          where
                               donthuids.Contains(x.hoso_id)
                          select x).ToList();

            var hsdonthu = new List<HSDonThu>();
            var dsdonthu = new List<DSDonThu>();
            var dsphanhoi = new List<DSDonThu>();
            int stt = 0;
            foreach (var item in dshoso.OrderByDescending(x => x.address_id))
            {
                stt++;
                HSDonThu per = new HSDonThu();
                per.stt = stt;
                per.name = item.name;
                per.address = db.Addresses.Find(item.address_id).name;
                //per.cadres = db.CanBoes.Find(item.canbo1cua).name;
                per.ngaytra = item.ngaytra.ToString("dd/MM/yyyy");
                per.service = db.Services.Find(item.service_id).name;
                per.hoso_id = item.hoso_id;
                hsdonthu.Add(per);
            }
            foreach (var item in donthus.OrderBy(x => x.time))
            {
                DSDonThu per = new DSDonThu();
                per.id = item.id;
                per.hoso_id = item.hoso_id;
                per.name = item.name;
                per.nguoigui = db.Accounts.Find(item.account_id).disname;
                per.cachgui = XModels.sCachGui[item.cachgui];
                per.lydo = item.lydo;
                per.type = XModels.sLoaiDonThu[item.type];
                per.time = item.time.ToString("dd/MM/yy");
                dsdonthu.Add(per);
                var phanhoi = db.donthus.Where(x => x.donthu_id == item.id);
                foreach (var ph in phanhoi)
                {
                    var perPH = new DSDonThu();
                    perPH.time = ph.time.ToString("dd/MM/yy");
                    perPH.name = ph.name;
                    perPH.donthu_id = ph.donthu_id.Value;
                    perPH.lydo = ph.lydo;
                    dsphanhoi.Add(perPH);
                }
            }
            //END

            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/DSDonThu.xlsx"); //document.template

            string webpart = "DSDonThu";
            webpart += ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();

            Account account = null;
            string e_hoten = "..............", e_sogiayto = "..............", e_sodienthoai = "..............";
            if (account_id.HasValue)
            {
                account = db.Accounts.Find(account_id.Value);
                e_hoten = account.fullname;
                e_sogiayto = account.sogiayto;
                e_sodienthoai = account.phoneno;
            }
            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("NGAYTHANG", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);

                    flexCelReport.SetValue("e_hoten", e_hoten);
                    flexCelReport.SetValue("e_sogiayto", e_sogiayto);
                    flexCelReport.SetValue("e_sodienthoai", e_sodienthoai);

                    // ĐƠn thư
                    flexCelReport.AddTable("HS", hsdonthu);
                    flexCelReport.AddTable("DT", dsdonthu);
                    flexCelReport.AddRelationship("HS", "DT", "hoso_id", "hoso_id");
                    flexCelReport.AddTable("PH", dsphanhoi);
                    flexCelReport.AddRelationship("DT", "PH", "id", "donthu_id");

                    flexCelReport.Run(sr, fs);
                }
            }
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ShowDi1CuaSession(int di1cua_id, int progressfile_type)
        {
            switch (progressfile_type)
            {
                case (int)XProgressFile.eType.PhieuTiepNhan:
                    return PartialView("../Progresses/PhieuHen", di1cua_id);
                case (int)XProgressFile.eType.VanBan1CuaDi:
                case (int)XProgressFile.eType.VanBan1CuaDen:
                    return PartialView("../Progresses/VBSau1cua", di1cua_id);
                case (int)XProgressFile.eType.UnType:
                    return PartialView("../Progresses/LichNhac1Cua", di1cua_id);
                default:
                    return null;
            }
        }
        [HttpGet]
        public ActionResult ShowTienDo(int hoso_id)
        {
            return PartialView("../Progresses/TienDo", hoso_id);
        }
    }
    public sealed class PrintByFilterCls
    {
        public string name { get; set; }
        public string service { get; set; }
        public string address { get; set; }
        public string cadres { get; set; }
        public string ngaynop { get; set; }
        public string ngayhan { get; set; }
        public string quahan { get; set; }
        public string note { get; set; }
        public string mahoso { get; set; }
        public string ngoaigiao { get; set; }
    }
    public sealed class PDi1Cua
    {
        public string name { get; set; }
        public string service { get; set; }
        public string address { get; set; }
        public string cadres { get; set; }
        public string uyquyen { get; set; }
        public string quahan { get; set; }
        public string canhbao { get; set; }
        public string note { get; set; }
        public string mahoso { get; set; }
        public int snqh { get; set; }
        public string ngaynop { get; set; }
        public string ngaytra { get; set; }
    }
    public sealed class HSDonThu
    {
        public int hoso_id { get; set; }
        public int stt { get; set; }
        public string name { get; set; }
        public string service { get; set; }
        public string address { get; set; }
        public string cadres { get; set; }
        public string ngaytra { get; set; }

    }
    public sealed class DSDonThu
    {
        public int id { get; set; }
        public int hoso_id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string time { get; set; }
        public string nguoigui { get; set; }
        public string cachgui { get; set; }
        public string lydo { get; set; }
        public int donthu_id { get; set; }

    }
}
