using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Aspose.Words;
using Aspose.Words.Drawing;
using FlexCel.Core;
using FlexCel.Report;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NiceHandles.Models;
using PdfiumViewer;
using static NiceHandles.Models.XHoSo;
using System.Drawing.Imaging;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Manager,Member")]
    public partial class InfomationsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: Infomations
        [Authorize(Roles = "SuperAdmin,Manager,Member")]
        public ActionResult Infomation(string Search_Data, int? add, int? ser, string Filter_Value, int? Page_No)
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
            ViewBag.Categories = new SelectListItem[0];
            ViewBag.ADDRESS = new SelectList(db.Addresses, "id", "name");
            ViewBag.SERVICES = new SelectList(db.Services, "id", "name");
            IQueryable<Contract> results = null;

            results = (from io in db.Contracts
                       join fk in db.fk_contract_service on io.id equals fk.contract_id
                       where (
                        (io.address_id == (add.HasValue ? add : io.address_id)) &&
                        fk.service_id == (ser.HasValue ? ser.Value : fk.service_id) &&
                        io.name.ToUpper().Contains((!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.name.ToUpper())
                       ))
                       select io).Distinct();

            List<Contract> lst = new List<Contract>();
            foreach (var item in results)
            {
                Contract con = new Contract();
                var address = db.Addresses.Find(item.address_id);
                var temp = (from io in db.InOuts
                            where io.contract_id == item.id && io.type == (int)XCategory.eType.Thu && io.status == (int)XInOut.eStatus.DaThucHien
                            select io.amount).ToArray();
                temp = (from io in db.InOuts
                        where io.contract_id == item.id && io.type == (int)XCategory.eType.Chi
                        select io.amount).ToArray();
                lst.Add(con);
            }
            ViewBag.db = db;
            int Size_Of_Page = 20;
            int No_Of_Page = (Page_No ?? 1);
            return View(lst);
        }

        // GET: Infomations/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Infomation infomation = db.Infomations.Find(id);
            if (infomation == null)
            {
                return HttpNotFound();
            }
            ViewBag.Documents = db.Documents;
            return View(infomation);
        }

        // GET: Infomations/Create
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.contract_id = id;
            return View();
        }

        // POST: Infomations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Infomation infomation)
        {
            if (ModelState.IsValid)
            {
                db.Infomations.Add(infomation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(infomation);
        }
        // GET: Infomations/Edit/5
        public ActionResult Edit(int hoso_id)
        {
            var hoso = db.HoSoes.Find(hoso_id);
            Infomation infomation = db.Infomations.SingleOrDefault(x => x.hoso_id == hoso_id);
            if (infomation == null)
            {
                infomation = new Infomation();
                infomation.hoso_id = hoso_id;
                db.Infomations.Add(infomation);
                infomation.e_account = hoso.account_id;
                db.SaveChanges();
            }
            return View(infomation);
        }
        // POST: Infomations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Infomation infomation)
        {
            var hoso = db.HoSoes.Find(infomation.hoso_id);
            var contract = db.Contracts.Find(hoso.contract_id);
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(infomation.b_diachithuadat) && string.IsNullOrEmpty(infomation.c_xa))
                {
                    var dvhc = infomation.b_diachithuadat.Split(',');
                    foreach (var item in dvhc)
                    {
                        var replace = item.Trim().Split(' ').ToList();
                        replace.RemoveAt(0);
                        if (item.ToUpper().Contains("XÃ"))
                        {
                            infomation.c_xa = String.Join(" ", replace);
                        }
                    }
                }
                db.Entry(infomation).State = EntityState.Modified;
                if (infomation.e_account != contract.account_id)
                {
                    contract.account_id = infomation.e_account.HasValue ? infomation.e_account.Value : 3;
                }
                var username = User.Identity.GetUserName();
                var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
                NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Thực hiện nhập thông tin (" + hoso.name + ")");
                db.SaveChanges();
                //return RedirectToAction("Index");
            }
            var address = db.Addresses.Find(contract.address_id);
            var lstSV = db.Services.Where(x => x.id == contract.service_id).ToArray();
            return View(infomation);
        }
        public JsonResult Download(int document_id, int hoso_id)
        {
            var hoso = db.HoSoes.Find(hoso_id);
            Models.Document document = db.Documents.Find(document_id);
            Contract contract = db.Contracts.Find(hoso.contract_id);
            Address address = db.Addresses.Find(contract.address_id);
            Infomation info = db.Infomations.Single(x => x.hoso_id == hoso_id);
            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/" + document.template);
            string webpart = ConvertToUnsign(hoso.name) + "_" + ConvertToUnsign(address.name) + "_" + ConvertToUnsign(document.name);
            #region Excel
            if (document.template.EndsWith("xls") || document.template.EndsWith("xlsx"))
            {
                webpart += ".xlsx";
                FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
                using (FileStream fs = System.IO.File.Create(path + webpart))
                {
                    using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                    {
                        foreach (var item in typeof(Infomation).GetProperties())
                        {
                            var name = item.Name;
                            var value = item.GetValue(info);
                            flexCelReport.SetValue(item.Name, value);
                        }
                        if (document.code.Equals("CI"))
                        {
                            flexCelReport.SetValue("HOPDONG", contract.name);
                            flexCelReport.SetValue("DIACHI", address.name);
                            flexCelReport = GenCS(info, flexCelReport);
                        }
                        //else if (document.code.Equals("QRC"))
                        //{
                        //    flexCelReport.SetValue("A_GIOITINH", !string.IsNullOrEmpty(info.a_gioitinh) ? info.a_gioitinh.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà");
                        //    flexCelReport.SetValue("NGAYTHANG", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
                        //    flexCelReport.SetValue("TAIKHOAN", contract.name);
                        //    flexCelReport.SetValue("GoogleLink", hoso.link_filecad);
                        //    //flexCelReport.SetValue("Link", "http://dodactracdia.vn/Cad?id=" + contract.id);
                        //    flexCelReport.SetValue("Link", hoso.link_filecad);
                        //    flexCelReport.SetValue("QRC", hoso.link_filecad_qr);
                        //    flexCelReport.GetImageData += new GetImageDataEventHandler(FlexcellReport_GetImageData);
                        //    flexCelReport.SetValue("DiaChi", info.b_diachithuadat);
                        //    flexCelReport.SetValue("DC", hoso.link_gmap_qr);

                        //    flexCelReport = GenCS(info, flexCelReport);
                        //}
                        else
                        {
                            flexCelReport = GenCN(info, flexCelReport);
                        }
                        flexCelReport.Run(sr, fs);
                    }
                }
            }
            #endregion
            else
            {
                Aspose.Words.Document doc = new Aspose.Words.Document(pathTemp);
                webpart += ".docx";
                path += webpart;
                var dict = new Dictionary<string, string>();
                bool temp = false;
                switch (document.code)
                {
                    case "HDUQ_VPL":
                        GenHDUQ_VPL(info, doc, dict);
                        break;
                    #region Sang tên trang 4
                    case "SangTen_11DK":
                        GenSangTen_11DK(info, doc, dict);
                        break;
                    case "SangTen_GUQ":
                        GenSangTen_GUQ(info, doc, dict);
                        break;
                    case "SangTen_TongHop":
                        GenSangTen_TongHop(info, doc, dict);
                        break;
                    case "SangTen_LPTB":
                        GenSangTen_LPTB(info, doc, dict);
                        break;
                    case "SangTen_DKT":
                        GenSangTen_DKT(info, doc, dict, out temp);
                        break;
                    case "SangTen_TPNN":
                        GenSangTen_TPNN(info, doc, dict);
                        break;
                    case "SangTen_TNCN":
                        GenSangTen_TNCN(info, doc, dict);
                        break;
                    #endregion
                    #region Cấp lần đầu                                 
                    case "LanDau_4DK":
                        GenNormal(info, doc, dict);
                        break;
                    case "LanDau_4aDK":
                        LanDau_4aDK(info, doc, dict);
                        break;
                    case "LanDau_TongHop":
                        LanDau_TongHop(info, doc, dict);
                        break;
                    case "LanDau_GUQ":
                        LanDau_GUQ(info, doc, dict);
                        break;
                    case "LanDau_AnhEmKhongTrangChap":
                        LanDau_AnhEmKhongTrangChap(info, doc, dict);
                        break;
                    case "LanDau_TongHopCongChung":
                        LanDau_TongHopCongChung(info, doc, dict);
                        break;
                    case "LanDau_DDN_KiemTra":
                        LanDau_DDN_KiemTra(info, doc, dict);
                        break;
                    case "LanDau_DKT":
                        LanDau_DKT(info, doc, dict);
                        break;
                    case "LanDau_LPTB":
                        LanDau_LPTB(info, doc, dict);
                        break;
                    case "LanDau_TPNN":
                        LanDau_TPNN(info, doc, dict);
                        break;
                    case "LanDau_TNCN":
                        LanDau_TNCN(info, doc, dict);
                        break;
                    case "LanDau_HutDienTich":
                        LanDau_HutDienTich(info, doc, dict);
                        break;
                    case "LanDau_DDN_NopTienVuotHanMuc":
                        LanDau_DDN_NopTienVuotHanMuc(info, doc, dict);
                        break;
                    case "DonToCaoChamMuon":
                        LanDau_DonToCaoChamMuon(info, doc, dict);
                        break;
                    case "LanDau_6VBTuThoaThuan":
                        LanDau_6VBTuThoaThuan(info, doc, dict);
                        break;                        
                    // END lần đầu 
                    #endregion
                    case "HDDD":
                        GenHDDD(info, doc, dict);
                        break;
                    case "HDCLTL":
                        GenHDCLTL(info, doc, dict);
                        break;
                    #region Đính chính/ cấp đổi
                    case "BIENDONG2024ThongTin":
                        GenBIENDONG2024ThongTin(info, doc, dict);
                        break;
                    case "2024MS11DKThongTin":
                        Gen2024MS11DKThongTin(info, doc, dict);
                        break;
                    #endregion
                    #region Tách thửa
                    case "TACHTHUA":
                        GenTACHTHUA(info, doc, dict);
                        break;
                    case "2024MS1DKTACHTHUA":
                        Gen2024MS1DKTACHTHUA(info, doc, dict);
                        break;
                    case "TachThua_CamKetSuDungChung":
                        GenTachThua_CamKetSuDungChung(info, doc, dict);
                        break;
                    #endregion
                    case "CI":
                        GenCI(info, doc, dict, hoso);
                        break;
                    case "TKLPTB":
                        GenTKLPTB(info, doc, dict);
                        break;
                    case "TKMS03TNCN":
                        GenTKMS03TNCN(info, doc, dict);
                        break;
                    case "TKTSDDPNN":
                        GenTKTSDDPNN(info, doc, dict);
                        break;
                    case "TKDKT":
                        GenTKDKT(info, doc, dict);
                        break;
                    case "VBTTPCDS":
                        GenVBTTPCDS(info, doc, dict);
                        break;
                    case "VBTTCNDD":
                        GenVBTTCNDD(info, doc, dict);
                        break;
                    case "GCK-HANMUC":
                        GenGCKHANMUC(info, doc, dict);
                        break;
                    case "CT07":
                        GenCT07(info, doc, pathTemp, dict);
                        break;
                    case "DDNXNNK":
                        GenDDNXNNK(info, doc, dict);
                        break;
                    case "XacNhanNamMat":
                        GenXacNhanNamMat(info, doc, dict);
                        break;
                    case "VanBanCamKetTaiSanChung":
                        GenVanBanCamKetTaiSanChung(info, doc, dict);
                        break;
                    case "HopDongUyQuyenChoNguoiDaiDien":
                        GenHopDongUyQuyenChoNguoiDaiDien(info, doc, pathTemp, dict);
                        break;
                    case "VBCamKetKhongTranhChap":
                        GenVBCamKetKhongTranhChap(info, doc, dict);
                        break;
                    case "BBHopGiaDinh":
                        GenBBHopGiaDinh(info, doc, dict);
                        break;
                    case "BBHopGiaDinhMS01":
                        GenBBHopGiaDinhMS01(info, doc, dict);
                        break;
                    case "VBTTCKUQ":
                        GenVBTTCKUQ(info, doc, dict);
                        break;

                    case "QUYHOACH":
                        GenThonTinQuyHoach(info, hoso, doc, dict);
                        break;
                    case "QRCode":
                        GenQRCode(info, hoso, doc, dict);
                        break;

                    case "DPATC":
                    case "DonPhanAnhChamMuonVAI":
                    case "DonPhanAnhChamMuonUBNDPhuong":
                        GenDonPhanAnhToCao(info, doc, dict);
                        break;
                    case "DPATCPhongTNMT":
                    case "DPATCDeNghiTNMT":
                    case "DPATCChamUBND":
                    case "DPATCChamL3":
                        GenDonPhanAnhToCao(info, doc, dict);
                        break;

                    case "DonRutHS":
                        GenDonRutHS(info, doc, dict);
                        break;
                    case "TrichDo":
                        GenTrichDo(info, doc, dict);
                        break;
                    case "QuyTrinhDoVe":
                        GenQuyTrinhDoVe(info, doc, dict);
                        break;
                    case "2024MS02CDonXinChuyenMDSDD":
                        Gen2024MS02CDonXinChuyenMDSDD(info, doc, dict);
                        break;
                    case "2024DonCamKetChuyenVuon":
                        Gen2024DonCamKetChuyenVuon(info, doc, dict);
                        break;
                    default:
                        GenNormal(info, doc, dict, true);
                        break;
                }
                doc.Save(path);
            }
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }
        void GenNormal(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict, bool pageBreak = false)
        {
            AddToDictAuto(info, dict);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
            if (pageBreak)
            {
                Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
                builder.MoveToDocumentEnd();
                builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
            }
        }
        Dictionary<string, string> AddToDict(Infomation info, Dictionary<string, string> dict)
        {
            dict.Add("a_loaigiayto", !string.IsNullOrEmpty(info.a_loaigiayto) ? info.a_loaigiayto : ".......................");
            dict.Add("a_sogiayto", !string.IsNullOrEmpty(info.a_sogiayto) ? info.a_sogiayto : ".......................");
            dict.Add("a_hoten", !string.IsNullOrEmpty(info.a_hoten) ? info.a_hoten : ".......................");
            dict.Add("a_ngaysinh", info.a_ngaysinh.HasValue ? info.a_ngaysinh.Value.ToString("dd/MM/yyyy") : ".......................");
            dict.Add("a_namsinh", info.a_ngaysinh.HasValue ? info.a_ngaysinh.Value.ToString("yyyy") : ".......................");
            dict.Add("a_gioitinh", !string.IsNullOrEmpty(info.a_gioitinh) ? info.a_gioitinh.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà");
            dict.Add("a_quoctich", !string.IsNullOrEmpty(info.a_quoctich) ? info.a_quoctich : ".......................");
            dict.Add("a_nguyenquan", !string.IsNullOrEmpty(info.a_nguyenquan) ? info.a_nguyenquan : ".......................");
            dict.Add("a_hktt", !string.IsNullOrEmpty(info.a_hktt) ? info.a_hktt : ".......................");
            dict.Add("a_thon", !string.IsNullOrEmpty(info.a_thon) ? info.a_thon : ".......................");
            dict.Add("a_xa", !string.IsNullOrEmpty(info.a_xa) ? info.a_xa : ".......................");
            dict.Add("a_huyen", !string.IsNullOrEmpty(info.a_huyen) ? info.a_huyen : ".......................");
            dict.Add("a_tinh", !string.IsNullOrEmpty(info.a_tinh) ? info.a_tinh : ".......................");
            dict.Add("a_ngaycap_gt", info.a_ngaycap_gt.HasValue ? info.a_ngaycap_gt.Value.ToString("dd/MM/yyyy") : ".......................");
            dict.Add("a_noicap_gt", !string.IsNullOrEmpty(info.a_noicap_gt) ? info.a_noicap_gt : ".......................");
            dict.Add("a_masothue", !string.IsNullOrEmpty(info.a_masothue) ? info.a_masothue : ".......................");
            dict.Add("a_loaigiayto1", !string.IsNullOrEmpty(info.a_loaigiayto1) ? info.a_loaigiayto1 : ".......................");
            dict.Add("a_sogiayto1", !string.IsNullOrEmpty(info.a_sogiayto1) ? info.a_sogiayto1 : ".......................");
            dict.Add("a_hoten1", !string.IsNullOrEmpty(info.a_hoten1) ? info.a_hoten1 : ".......................");
            dict.Add("a_ngaysinh1", info.a_ngaysinh1.HasValue ? info.a_ngaysinh1.Value.ToString("dd/MM/yyyy") : ".......................");
            dict.Add("a_gioitinh1", !string.IsNullOrEmpty(info.a_gioitinh1) ? info.a_gioitinh1 : ".......................");
            dict.Add("a_quoctich1", !string.IsNullOrEmpty(info.a_quoctich1) ? info.a_quoctich1 : ".......................");
            dict.Add("a_nguyenquan1", !string.IsNullOrEmpty(info.a_nguyenquan1) ? info.a_nguyenquan1 : ".......................");
            dict.Add("a_hktt1", !string.IsNullOrEmpty(info.a_hktt1) ? info.a_hktt1 : ".......................");
            dict.Add("a_ngaycap_gt1", info.a_ngaycap_gt1.HasValue ? info.a_ngaycap_gt1.Value.ToString("dd/MM/yyyy") : ".......................");
            dict.Add("a_noicap_gt1", !string.IsNullOrEmpty(info.a_noicap_gt1) ? info.a_noicap_gt1 : ".......................");
            dict.Add("b_hoten", !string.IsNullOrEmpty(info.b_hoten) ? info.b_hoten : ".......................");
            dict.Add("b_ngaysinh", info.b_ngaysinh.HasValue ? info.b_ngaysinh.Value.ToString("dd/MM/yyyy") : ".......................");
            dict.Add("b_loaigiayto", !string.IsNullOrEmpty(info.b_loaigiayto) ? info.b_loaigiayto : ".......................");
            dict.Add("b_sogiayto", !string.IsNullOrEmpty(info.b_sogiayto) ? info.b_sogiayto : ".......................");
            dict.Add("b_diachithuongtru", !string.IsNullOrEmpty(info.b_diachithuongtru) ? info.b_diachithuongtru : ".......................");
            dict.Add("b_sogiaychungnhan", !string.IsNullOrEmpty(info.b_sogiaychungnhan) ? info.b_sogiaychungnhan : ".......................");
            dict.Add("b_sothua", !string.IsNullOrEmpty(info.b_sothua) ? info.b_sothua : ".......................");
            dict.Add("b_tobando", !string.IsNullOrEmpty(info.b_tobando) ? info.b_tobando : ".......................");
            dict.Add("b_thon", !string.IsNullOrEmpty(info.b_thon) ? info.b_thon : ".......................");
            dict.Add("b_xa", !string.IsNullOrEmpty(info.b_xa) ? info.b_xa : ".......................");
            dict.Add("b_huyen", !string.IsNullOrEmpty(info.b_huyen) ? info.b_huyen : ".......................");
            dict.Add("b_tinh", !string.IsNullOrEmpty(info.b_tinh) ? info.b_tinh : ".......................");
            dict.Add("b_diachithuadat", !string.IsNullOrEmpty(info.b_diachithuadat) ? info.b_diachithuadat : ".......................");
            dict.Add("b_dientich", !string.IsNullOrEmpty(info.b_dientich) ? info.b_dientich : ".......................");
            dict.Add("b_hinhthucsudung", !string.IsNullOrEmpty(info.b_hinhthucsudung) ? info.b_hinhthucsudung : ".......................");
            dict.Add("b_mucdichsudung", !string.IsNullOrEmpty(info.b_mucdichsudung) ? info.b_mucdichsudung : ".......................");
            dict.Add("b_nguongoc", !string.IsNullOrEmpty(info.b_nguongoc) ? info.b_nguongoc : ".......................");
            dict.Add("b_ghichu", !string.IsNullOrEmpty(info.b_ghichu) ? info.b_ghichu : ".......................");
            dict.Add("b_ngaycap", info.b_ngaycap.HasValue ? info.b_ngaycap.Value.ToString("dd/MM/yyyy") : ".......................");
            dict.Add("b_noicap", !string.IsNullOrEmpty(info.b_noicap) ? info.b_noicap : ".......................");
            dict.Add("b_sovaoso", !string.IsNullOrEmpty(info.b_sovaoso) ? info.b_sovaoso : ".......................");

            dict.Add("b_loaidat1", !string.IsNullOrEmpty(info.b_loaidat1) ? info.b_loaidat1 : ".......................");
            dict.Add("b_dientich1", !string.IsNullOrEmpty(info.b_dientich1) ? info.b_dientich1 : ".......................");
            dict.Add("b_loaidat2", !string.IsNullOrEmpty(info.b_loaidat2) ? info.b_loaidat2 : ".......................");
            dict.Add("b_dientich2", !string.IsNullOrEmpty(info.b_dientich2) ? info.b_dientich2 : ".......................");

            dict.Add("c_xa", !string.IsNullOrEmpty(info.c_xa) ? info.c_xa : ".......................");
            dict.Add("c_xaphuong", !string.IsNullOrEmpty(info.c_xaphuong) ? info.c_xaphuong : ".......................");
            dict.Add("c_baocao_veviec", !string.IsNullOrEmpty(info.c_baocao_veviec) ? info.c_baocao_veviec : ".......................");
            dict.Add("c_dondenghi_noidung", !string.IsNullOrEmpty(info.c_dondenghi_noidung) ? info.c_dondenghi_noidung : ".......................");
            dict.Add("d_sogiayto", !string.IsNullOrEmpty(info.d_sogiayto) ? info.d_sogiayto : ".......................");
            dict.Add("d_hoten", !string.IsNullOrEmpty(info.d_hoten) ? info.d_hoten : ".......................");
            dict.Add("d_ngaysinh", info.d_ngaysinh.HasValue ? info.d_ngaysinh.Value.ToString("dd/MM/yyyy") : ".......................");
            dict.Add("d_gioitinh", !string.IsNullOrEmpty(info.d_gioitinh) ? info.d_gioitinh : ".......................");
            dict.Add("d_quoctich", !string.IsNullOrEmpty(info.d_quoctich) ? info.d_quoctich : ".......................");
            dict.Add("d_nguyenquan", !string.IsNullOrEmpty(info.d_nguyenquan) ? info.d_nguyenquan : ".......................");
            dict.Add("d_hktt", !string.IsNullOrEmpty(info.d_hktt) ? info.d_hktt : ".......................");
            dict.Add("d_ngaycap_gt", info.d_ngaycap_gt.HasValue ? info.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : ".......................");
            dict.Add("d_noicap_gt", !string.IsNullOrEmpty(info.d_noicap_gt) ? info.d_noicap_gt : ".......................");
            dict.Add("e_sogiayto", !string.IsNullOrEmpty(info.e_sogiayto) ? info.e_sogiayto : ".......................");
            dict.Add("e_hoten", !string.IsNullOrEmpty(info.e_hoten) ? info.e_hoten : ".......................");
            dict.Add("e_ngaysinh", info.e_ngaysinh.HasValue ? info.e_ngaysinh.Value.ToString("dd/MM/yyyy") : ".......................");
            dict.Add("e_gioitinh", !string.IsNullOrEmpty(info.e_gioitinh) ? info.e_gioitinh : ".......................");
            dict.Add("e_quoctich", !string.IsNullOrEmpty(info.e_quoctich) ? info.e_quoctich : ".......................");
            dict.Add("e_nguyenquan", !string.IsNullOrEmpty(info.e_nguyenquan) ? info.e_nguyenquan : ".......................");
            dict.Add("e_hktt", !string.IsNullOrEmpty(info.e_hktt) ? info.e_hktt : ".......................");
            dict.Add("e_ngaycap_gt", info.e_ngaycap_gt.HasValue ? info.e_ngaycap_gt.Value.ToString("dd/MM/yyyy") : ".......................");
            dict.Add("e_noicap_gt", !string.IsNullOrEmpty(info.e_noicap_gt) ? info.e_noicap_gt : ".......................");
            dict.Add("e_masothue", !string.IsNullOrEmpty(info.e_masothue) ? info.e_masothue : ".......................");
            return dict;
        }
        Dictionary<string, string> AddToDictAuto(Infomation info, Dictionary<string, string> dict)
        {
            if (dict.Any()) return dict;
            string uiSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            //
            foreach (var item in typeof(Infomation).GetProperties())
            {
                var name = item.Name;
                var value = item.GetValue(info);
                string val = value == null ? "......................." : !string.IsNullOrEmpty(value.ToString()) ? value.ToString() : ".......................";
                dict.Add(item.Name, val);
            }
            dict["a_ngaysinh"] = info.a_ngaysinh.HasValue ? info.a_ngaysinh.Value.ToString("dd/MM/yyyy") : "...../...../......";
            dict["a_ngaysinh1"] = info.a_ngaysinh1.HasValue ? info.a_ngaysinh1.Value.ToString("dd/MM/yyyy") : "...../...../......";
            dict["a_namsinh"] = info.a_ngaysinh.HasValue ? info.a_ngaysinh.Value.ToString("yyyy") : "...........";
            dict["a_namsinh1"] = info.a_ngaysinh1.HasValue ? info.a_ngaysinh1.Value.ToString("yyyy") : "..........";
            dict["e_namsinh"] = info.e_ngaysinh.HasValue ? info.e_ngaysinh.Value.ToString("yyyy") : ".......................";
            dict["e_ngaysinh"] = info.e_ngaysinh.HasValue ? info.e_ngaysinh.Value.ToString("dd/MM/yyyy") : "....................";
            dict["e_ngaycap_gt"] = info.e_ngaycap_gt.HasValue ? info.e_ngaycap_gt.Value.ToString("dd/MM/yyyy") : ".......................";
            dict["e_ngayuyquyen"] = info.e_ngayuyquyen.HasValue ? info.e_ngayuyquyen.Value.ToString("dd/MM/yyyy") : "...../...../202...";
            dict["e_gioitinh"] = !string.IsNullOrEmpty(info.e_gioitinh) ? info.e_gioitinh.Trim().ToUpper().Equals("NAM") ? "ông" : "bà" : "Ông/Bà";
            dict["a_ngaycap_gt"] = info.a_ngaycap_gt.HasValue ? info.a_ngaycap_gt.Value.ToString("dd/MM/yyyy") : ".......................";
            dict["a_ngaycap_gt1"] = info.a_ngaycap_gt1.HasValue ? info.a_ngaycap_gt1.Value.ToString("dd/MM/yyyy") : ".......................";
            dict["b_ngaycap"] = info.b_ngaycap.HasValue ? info.b_ngaycap.Value.ToString("dd/MM/yyyy") : ".......................";
            dict["a_gioitinh"] = !string.IsNullOrEmpty(info.a_gioitinh) ? info.a_gioitinh.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";
            dict["a_gioitinh1"] = !string.IsNullOrEmpty(info.a_gioitinh1) ? info.a_gioitinh1.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";
            dict["d_gioitinh"] = !string.IsNullOrEmpty(info.d_gioitinh) ? info.d_gioitinh.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";
            dict["d_gioitinh1"] = !string.IsNullOrEmpty(info.d_gioitinh1) ? info.d_gioitinh1.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";
            dict["d_namsinh"] = info.d_ngaysinh.HasValue ? info.d_ngaysinh.Value.ToString("yyyy") : ".......";
            dict["d_namsinh1"] = info.d_ngaysinh1.HasValue ? info.d_ngaysinh1.Value.ToString("yyyy") : ".......";
            dict["d_ngaycap_gt"] = info.d_ngaycap_gt.HasValue ? info.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : ".......................";
            dict["d_ngaycap_gt1"] = info.d_ngaycap_gt1.HasValue ? info.d_ngaycap_gt1.Value.ToString("dd/MM/yyyy") : ".......................";
            dict["d_ngaycongchung"] = info.d_ngaycongchung.HasValue ? info.d_ngaycongchung.Value.ToString("dd/MM/yyyy") : ".......................";
            dict["d_ngaysinh"] = info.d_ngaysinh.HasValue ? info.d_ngaysinh.Value.ToString("dd/MM/yyyy") : "....................";
            dict["d_ngaysinh1"] = info.d_ngaysinh1.HasValue ? info.d_ngaysinh1.Value.ToString("dd/MM/yyyy") : "....................";

            dict.Add("a_gioitinh11", info.a_gioitinh.Trim().ToUpper().Equals("NAM") ? "X" : " ");
            dict.Add("a_gioitinh22", info.a_gioitinh.Trim().ToUpper().Equals("NỮ") ? "X" : " ");
            //
            dict.Add("d_gioitinh11", info.d_gioitinh.Trim().ToUpper().Equals("NAM") ? "X" : " ");
            dict.Add("d_gioitinh22", info.d_gioitinh.Trim().ToUpper().Equals("NỮ") ? "X" : " ");

            dict.Add("a_vochong", dict["a_gioitinh"] + " " + info.a_hoten.ToUpper() + (string.IsNullOrEmpty(info.a_hoten1) ? "" : (info.a_gioitinh1.Trim().ToUpper().Equals("NAM") ? " và chồng là ông " : " và vợ là bà ") + info.a_hoten1.ToUpper()));
            dict.Add("NGAYTHANGNAM", DateTime.Now.ToString("dd/MM/yyyy"));

            string nhanxung = string.Empty;

            if (!string.IsNullOrEmpty(info.a_hoten1))
            {
                dict.Add("a_chusohuu", dict["a_gioitinh"] + " " + info.a_hoten + " và " + dict["a_gioitinh1"] + " " + info.a_hoten1);
                nhanxung = "chúng tôi";
            }
            else
            {
                dict.Add("a_chusohuu", dict["a_gioitinh"] + " " + info.a_hoten);
                nhanxung = "tôi";
            }
            if (!string.IsNullOrEmpty(info.d_hoten1))
            {
                dict.Add("d_chusohuu", dict["d_gioitinh"] + " " + info.d_hoten + " và " + dict["d_gioitinh1"] + " " + info.d_hoten1);
            }
            else
            {
                dict.Add("d_chusohuu", dict["d_gioitinh"] + " " + info.d_hoten);
            }
            dict.Add("a_nhanxung", nhanxung);
            dict.Add("x_CNTC", info.d_lydobiendong.ToUpper().Trim().Equals("NHẬN TẶNG CHO") ? "tặng cho" : "chuyển nhượng");
            dict["d_tienhopdong"] = (info.d_tienhopdong.HasValue ? info.d_tienhopdong.Value : 0).ToString("N0");
            GenMaSoThue(info.d_masothue, "d", dict);
            GenMaSoThue(info.a_masothue, "a", dict);
            var a_ndk = Utils.ToTTCN(info.a_gioitinh, info.a_hoten, info.a_ngaysinh, info.a_loaigiayto, info.a_sogiayto, string.Empty, info.a_ngaycap_gt, false);
            a_ndk += Utils.ToTTCN(info.a_gioitinh1, info.a_hoten1, info.a_ngaysinh1, info.a_loaigiayto1, info.a_sogiayto1, string.Empty, info.a_ngaycap_gt1, true);
            dict.Add("a_ndk", a_ndk);
            var d_ndk = Utils.ToTTCN(info.d_gioitinh, info.d_hoten, info.d_ngaysinh, info.d_loaigiayto, info.d_sogiayto, string.Empty, info.d_ngaycap_gt, false);
            d_ndk += Utils.ToTTCN(info.d_gioitinh1, info.d_hoten1, info.d_ngaysinh1, info.d_loaigiayto1, info.d_sogiayto1, string.Empty, info.d_ngaycap_gt1, true);
            dict.Add("d_ndk", d_ndk);
            if (info.e_account.HasValue)
            {
                var account = db.Accounts.Single(x => x.id == info.e_account.Value);
                dict.Add("e_sdt", account.phoneno);
                dict.Add("e_email", account.email);
            }
            else
            {
                dict.Add("e_email", "..............");
                dict.Add("e_sdt", "..............");
            }
            dict.Add("b_nha", info.b_loainguongoc.Equals(XInfomation.sLoainguongoc[0]) ? "Có" : "Không");
            dict.Add("ngay", DateTime.Now.ToString("dd"));
            dict.Add("thang", DateTime.Now.ToString("MM"));
            dict.Add("nam", DateTime.Now.ToString("yyyy"));

            string thon = string.Empty, xa = string.Empty, huyen = string.Empty, tinh = string.Empty, address = string.Empty;
            bool isCity = false;
            bool isSo = false;
            Utils.SpilitAddress(info.a_hktt, ref thon, ref xa, ref huyen, ref tinh, ref isCity, ref isSo, ref address);
            dict.Add("a_diachi", address);
            dict["a_huyen"] = huyen;
            dict["a_tinh"] = tinh;
            dict["a_thon"] = thon;
            dict["a_xa"] = xa;
            // Thửa đất
            thon = string.Empty;
            xa = string.Empty;
            huyen = string.Empty;
            tinh = string.Empty;
            address = string.Empty;

            isCity = false;
            isSo = false;
            Utils.SpilitAddress(info.b_diachithuadat, ref thon, ref xa, ref huyen, ref tinh, ref isCity, ref isSo, ref address);
            dict.Add("b_diachi", address);
            dict["b_huyen"] = huyen;
            dict["b_tinh"] = tinh;
            dict["b_thon"] = thon;
            dict["b_xa"] = xa;
            // D
            thon = string.Empty;
            xa = string.Empty;
            huyen = string.Empty;
            tinh = string.Empty;
            address = string.Empty;
            isCity = false;
            isSo = false;
            Utils.SpilitAddress(info.d_hktt, ref thon, ref xa, ref huyen, ref tinh, ref isCity, ref isSo, ref address);
            dict.Add("d_diachi", address);
            dict["d_huyen"] = huyen;
            dict["d_tinh"] = tinh;
            dict["d_thon"] = thon;
            dict["d_xa"] = xa;

            if (string.IsNullOrEmpty(info.b_dientich2))
                dict.Add("d_vitrithuadat2", "............");
            else
                dict.Add("d_vitrithuadat2", info.d_vitrithuadat);

            if (string.IsNullOrEmpty(info.b_dientich2) && !string.IsNullOrEmpty(info.b_dientich))
            {
                var temp = info.b_dientich.Replace(".", "").Replace(",", uiSep);
                double area = Convert.ToDouble(temp);
                dict.Add("b_dientichchu", Utils.DecimalToText(area) + " mét vuông");
            }
            else
                dict.Add("b_dientichchu", "");

            return dict;
        }
        void GenBaoCaoXa_ChuyenVuon(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenDonRutHS(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            var onedoor = db.di1cua.Single(x => x.hoso_id == info.hoso_id && x.trangthai < (int)Xdi1cua.eStatus.ThanhCong);
            var quahan = (DateTime.Now - onedoor.ngaytra).Days;
            dict.Add("maphieuhen", onedoor.maphieuhen);
            dict.Add("ngaynop", onedoor.ngaynop.ToString("dd/MM/yyyy"));
            dict.Add("ngayhen", onedoor.ngaytra.ToString("dd/MM/yyyy"));
            dict.Add("quahan", quahan < 10 ? "0" + quahan.ToString() : quahan.ToString());
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenDonPhanAnhToCao(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            var onedoor = db.di1cua.Single(x => x.hoso_id == info.hoso_id && x.trangthai < (int)Xdi1cua.eStatus.ThanhCong);
            var quahan = (DateTime.Now - onedoor.ngaytra).Days;
            dict.Add("maphieuhen", onedoor.maphieuhen);
            dict.Add("ngaynop", onedoor.ngaynop.ToString("dd/MM/yyyy"));
            dict.Add("ngayhen", onedoor.ngaytra.ToString("dd/MM/yyyy"));
            dict.Add("quahan", quahan < 10 ? "0" + quahan.ToString() : quahan.ToString());


            //Ngày 23/04/2024, quá hạn thủ tục hành chính 40 ngày, bà Trịnh Thị Thu Hằng cùng là
            //người được chủ sử dụng đất ủy quyền đã làm đơn phản ánh chậm muộn thủ tục hành
            //chính gửi các cơ quan có thẩm quyền.
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            builder.MoveToMergeField("lichsudon");
            string lichsudon = string.Empty;

            var donthus = db.donthus.Where(x => x.hoso_id == info.hoso_id && !x.donthu_id.HasValue).OrderBy(x => x.time);
            foreach (var item in donthus)
            {
                lichsudon = "Ngày " + item.time.ToString("dd/MM/yyyy") + ", " + item.lydo + ", tôi đã làm đơn " +
                    " " + item.name;
                builder.Writeln(lichsudon);
                var reply = db.donthus.Where(x => x.donthu_id == item.id).OrderBy(x => x.time);
                foreach (var rep in reply)
                {
                    builder.Font.Italic = true;
                    lichsudon = "       - Ngày " + rep.time.ToString("dd/MM/yyyy") + ", tôi đã nhận được " + rep.name + ". Nội dung tóm tắt là \"" + rep.lydo + "\".";
                    builder.Writeln(lichsudon);
                    builder.Font.Italic = false;
                }
            }
            builder.MoveToMergeField("lichsudon");
            foreach (var item in donthus)
            {
                lichsudon = "Ngày " + item.time.ToString("dd/MM/yyyy") + ", " + item.lydo + ", tôi đã làm đơn " +
                    " " + item.name;
                builder.Writeln(lichsudon);
                var reply = db.donthus.Where(x => x.donthu_id == item.id).OrderBy(x => x.time);
                foreach (var rep in reply)
                {
                    builder.Font.Italic = true;
                    lichsudon = "       - Ngày " + rep.time.ToString("dd/MM/yyyy") + ", tôi đã nhận được " + rep.name + ". Nội dung tóm tắt là \"" + rep.lydo + "\".";
                    builder.Writeln(lichsudon);
                    builder.Font.Italic = false;
                }
            }
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenBCX(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenDCD(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenCI(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict, HoSo hoso)
        {
            var contract = db.Contracts.Find(hoso.contract_id);
            Address address = db.Addresses.Find(contract.address_id);
            dict.Add("HOSO", hoso.name);
            dict.Add("DIACHI", address.name);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

            builder.MoveToMergeField("CSH1");

            string buff = string.Empty;
            buff += "Chủ sở hữu 1";
            builder.Writeln(buff);
            buff = "Họ tên: ";
            buff += info.a_hoten;
            builder.Writeln(buff);

            //builder.CurrentParagraph.Remove();

        }
        void GenGCKHANMUC(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            builder.MoveToMergeField("danhsachnguoicamket");
            string danhsachnguoicamket = string.Empty;
            //       Tên tôi là: ĐỖ VĂN NGẠN, sinh năm 1953, CCCD số: 031053001208 do Cục cảnh sát quản lý hành chính về trật tự xã hội cấp ngày 24 / 4 / 2021
            //Và vợ là: NGUYỄN THỊ LIỄU, sinh năm 1958, CCCD số: 031158020774 do Cục cảnh sát quản lý hành chính về trật tự xã hội cấp ngày 30 / 3 / 2023
            //Cùng đăng ký thường trú tại: Thôn Hỗ Đông, xã Hồng Phong, huyện An Dương, thành phố Hải Phòng.
            danhsachnguoicamket = "Tên tôi là: " + info.a_hoten + ", sinh năm " + (info.a_ngaysinh.HasValue ? info.a_ngaysinh.Value.ToString("yyyy") : ".....") +
                    ", số " + (!string.IsNullOrEmpty(info.a_loaigiayto) ? info.a_loaigiayto : string.IsNullOrEmpty(info.a_sogiayto) ? "........." : "định danh") +
                " " + info.a_sogiayto + " do " + info.a_noicap_gt + " cấp ngày " + (info.a_ngaycap_gt.HasValue ? info.a_ngaycap_gt.Value.ToString("dd/MM/yyyy") : ".............") + ".";
            if (!string.IsNullOrEmpty(info.a_hoten1))
            {
                builder.Write(danhsachnguoicamket);
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
                danhsachnguoicamket = "Và vợ là: " + info.a_hoten1 + ", sinh năm " + (info.a_ngaysinh1.HasValue ? info.a_ngaysinh1.Value.ToString("yyyy") : ".....") +
                    ", số " + (!string.IsNullOrEmpty(info.a_loaigiayto1) ? info.a_loaigiayto1 : string.IsNullOrEmpty(info.a_sogiayto1) ? "........." : "định danh") +
                " " + info.a_sogiayto1 + " do " + info.a_noicap_gt1 + " cấp ngày " + (info.a_ngaycap_gt1.HasValue ? info.a_ngaycap_gt1.Value.ToString("dd/MM/yyyy") : ".............");
            }
            builder.Write(danhsachnguoicamket);
            //HKTT
            builder.MoveToMergeField("hktt");
            danhsachnguoicamket = (string.IsNullOrEmpty(info.a_hoten1) ? "Đăng" : "Cùng đăng") + " ký thường trú tại: " + info.a_hktt + ".";
            builder.Write(danhsachnguoicamket);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
            if (doc.PageCount % 2 != 0)
            {
                builder.MoveToDocumentEnd();
                builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
            }
        }
        //void GenGCKHANMUCDATO(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        //{
        //    dict = AddToDictAuto(info, dict);
        //    Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
        //    //var nguoidaidien = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.marker == (int)XModels.eYesNo.Yes).FirstOrDefault();
        //    //dict.Add("hoten", nguoidaidien.hoten);
        //    //dict.Add("namsinh", nguoidaidien.ngaysinh.Value.ToString("yyyy"));
        //    var soho = db.SoHoKhaus.Where(x => x.infomation_id == info.id).ToList();
        //    dict.Add("soho", (soho.Count < 10 ? "0" + soho.Count : soho.Count.ToString()) + " (" + XModels.SoDem[soho.Count] + ")");
        //    var songuoi = 0;

        //    builder.MoveToMergeField("danhsachnguoicamket");
        //    string danhsachnguoicamket = string.Empty;
        //    foreach (var item in soho)
        //    {
        //        var nhankhau = db.ThongTinCaNhans.Where(x => x.sohokhau_id == item.id && x.infomation_id == info.id).ToList();
        //        danhsachnguoicamket = "Hộ gia đình thứ " + XModels.SoThu[soho.IndexOf(item)] + " có " + (nhankhau.Count > 1 ? XModels.SoDem[nhankhau.Count] + " nhân khẩu bao gồm: " : " một nhân khẩu: ");
        //        builder.Write(danhsachnguoicamket);
        //        foreach (var nk in nhankhau)
        //        {
        //            songuoi++;
        //            danhsachnguoicamket = nk.hoten + " (" + nk.ghichuquanhe + ")" + ", sinh năm " + (nk.ngaysinh.HasValue ? nk.ngaysinh.Value.ToString("yyyy") : ".....") + ", số " +
        //                (!string.IsNullOrEmpty(nk.loaigiayto) ? nk.loaigiayto : string.IsNullOrEmpty(nk.sogiayto) ? "........." : "định danh") +
        //                " " + nk.sogiayto + ", do " + nk.noicap_gt + " cấp ngày " + (nk.ngaycap_gt.HasValue ? nk.ngaycap_gt.Value.ToString("dd/MM/yyyy") : ".....") + "; ";
        //            builder.Write(danhsachnguoicamket);
        //        }
        //        if (soho.IndexOf(item) != soho.Count - 1)
        //            builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
        //    }
        //    builder.MoveToMergeField("loichung");
        //    danhsachnguoicamket = string.Empty;
        //    int index = 0;
        //    foreach (var item in soho)
        //    {
        //        var nhankhau = db.ThongTinCaNhans.Where(x => x.sohokhau_id == item.id && x.infomation_id == info.id).ToList();
        //        foreach (var nk in nhankhau)
        //        {
        //            index++;
        //            danhsachnguoicamket = index.ToString() + ", " + nk.hoten + ", sinh năm " + (nk.ngaysinh.HasValue ? nk.ngaysinh.Value.ToString("yyyy") : ".....") + ", số " +
        //                (!string.IsNullOrEmpty(nk.loaigiayto) ? nk.loaigiayto : string.IsNullOrEmpty(nk.sogiayto) ? "........." : "định danh") +
        //                " " + nk.sogiayto + "; ";
        //            builder.Write(danhsachnguoicamket);
        //            builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
        //        }
        //    }
        //    dict.Add("songuoi", songuoi.ToString());
        //    doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        //}        
        void GenXacNhanNamMat(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);

            var ongbanoi = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1 && x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeChong).ToArray();
            var ongnoi = ongbanoi.SingleOrDefault(x => x.gioitinh.Trim().ToUpper().Equals("NAM"));
            var banoi = ongbanoi.SingleOrDefault(x => x.gioitinh.Trim().ToUpper().Equals("NỮ"));
            var ChuDat = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.ChuDat).SingleOrDefault();

            dict.Add("xd_bochudat", ongnoi != null ? ongnoi.hoten : "..........................");
            dict.Add("xd_mechudat", banoi != null ? banoi.hoten : "..........................");
            dict.Add("xd_bochudat_chetnam", ongnoi != null ? ongnoi.ngaychet.HasValue ? ongnoi.ngaychet.Value.ToString("yyyy") : "........." : ".........");
            dict.Add("xd_mechudat_chetnam", banoi != null ? banoi.ngaychet.HasValue ? banoi.ngaychet.Value.ToString("yyyy") : "........." : ".........");
            dict.Add("xd_chudat", ChuDat != null ? ChuDat.hoten : "..........................");

            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenThonTinQuyHoach(Infomation info, HoSo hoso, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            AddToDictAuto(info, dict);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            builder.MoveToMergeField("b_qr");
            builder.InsertImage(Server.MapPath("~/public/" + hoso.link_gmap_qr),
            RelativeHorizontalPosition.Margin,
            0,
            RelativeVerticalPosition.Margin,
            0,
            100,
            100,
            WrapType.Square);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        FlexCel.Report.FlexCelReport GenCS(Infomation info, FlexCel.Report.FlexCelReport flexCelReport)
        {
            flexCelReport.SetValue("a_ngaysinh", info.a_ngaysinh.HasValue ? info.a_ngaysinh.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("a_ngaycap_gt", info.a_ngaycap_gt.HasValue ? info.a_ngaycap_gt.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("a_ngaysinh1", info.a_ngaysinh1.HasValue ? info.a_ngaysinh1.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("a_ngaycap_gt1", info.a_ngaycap_gt1.HasValue ? info.a_ngaycap_gt1.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("b_ngaysinh", info.b_ngaysinh.HasValue ? info.b_ngaysinh.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("b_ngaycap", info.b_ngaycap.HasValue ? info.b_ngaycap.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("d_ngaysinh", info.d_ngaysinh.HasValue ? info.d_ngaysinh.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("d_ngaycap_gt", info.d_ngaycap_gt.HasValue ? info.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("d_ngaysinh1", info.d_ngaysinh1.HasValue ? info.d_ngaysinh1.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("d_ngaycap_gt1", info.d_ngaycap_gt1.HasValue ? info.d_ngaycap_gt1.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("d_ngaycongchung", info.d_ngaycongchung.HasValue ? info.d_ngaycongchung.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("e_ngaysinh", info.e_ngaysinh.HasValue ? info.e_ngaysinh.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("e_ngaycap_gt", info.e_ngaycap_gt.HasValue ? info.e_ngaycap_gt.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("e_ngayuyquyen", info.e_ngayuyquyen.HasValue ? info.e_ngayuyquyen.Value.ToString("dd/MM/yyyy") : null);

            return flexCelReport;
        }
        FlexCel.Report.FlexCelReport GenCN(Infomation info, FlexCel.Report.FlexCelReport flexCelReport)
        {
            flexCelReport.SetValue("a_namsinh", info.a_ngaysinh.HasValue ? info.a_ngaysinh.Value.Year.ToString() : ".....");
            flexCelReport.SetValue("a_namsinh1", info.a_ngaysinh1.HasValue ? info.a_ngaysinh1.Value.Year.ToString() : ".....");
            flexCelReport.SetValue("d_namsinh", info.d_ngaysinh.HasValue ? info.d_ngaysinh.Value.Year.ToString() : ".....");
            flexCelReport.SetValue("d_namsinh1", info.d_ngaysinh1.HasValue ? info.d_ngaysinh1.Value.Year.ToString() : ".....");

            string sgt1 = string.Empty, sgt2 = string.Empty, sgt11 = string.Empty, sgt22 = string.Empty;
            if (info.a_loaigiayto != null && info.a_loaigiayto.ToUpper().Equals("CCCD"))
                sgt1 = info.a_sogiayto;
            else
                sgt2 = info.a_sogiayto;
            if (info.a_loaigiayto1 != null && info.a_loaigiayto1.ToUpper().Equals("CCCD"))
                sgt11 = info.a_sogiayto1;
            else
                sgt22 = info.a_sogiayto1;
            flexCelReport.SetValue("a_sogiayto1", sgt2);
            flexCelReport.SetValue("a_sogiayto2", sgt1);
            flexCelReport.SetValue("a_sogiayto11", sgt22);
            flexCelReport.SetValue("a_sogiayto22", sgt11);
            string dsgt1 = string.Empty, dsgt2 = string.Empty, dsgt11 = string.Empty, dsgt22 = string.Empty;
            //d
            if (info.d_loaigiayto != null && info.d_loaigiayto.ToUpper().Equals("CCCD"))
                dsgt1 = info.d_sogiayto;
            else
                dsgt2 = info.d_sogiayto;
            if (info.d_loaigiayto1 != null && info.d_loaigiayto1.ToUpper().Equals("CCCD"))
                dsgt11 = info.d_sogiayto1;
            else
                dsgt22 = info.d_sogiayto1;
            flexCelReport.SetValue("d_sogiayto1", dsgt2);
            flexCelReport.SetValue("d_sogiayto2", dsgt1);
            flexCelReport.SetValue("d_sogiayto11", dsgt22);
            flexCelReport.SetValue("d_sogiayto22", dsgt11);

            string thon = string.Empty, xa = string.Empty, huyen = string.Empty, tinh = string.Empty, address = string.Empty;
            bool isCity = false, isSo = false;
            Utils.SpilitAddress(info.b_diachithuadat, ref thon, ref xa, ref huyen, ref tinh, ref isCity, ref isSo, ref address);

            flexCelReport.SetValue("b_thon", thon);
            flexCelReport.SetValue("b_xa", xa);
            flexCelReport.SetValue("b_huyen", huyen);
            flexCelReport.SetValue("b_tinh", tinh);
            //d
            string dthon = string.Empty, dxa = string.Empty, dhuyen = string.Empty, dtinh = string.Empty, daddress = string.Empty;
            Utils.SpilitAddress(info.d_hktt, ref thon, ref xa, ref huyen, ref tinh, ref isCity, ref isSo, ref address);
            flexCelReport.SetValue("d_thon", dthon);
            flexCelReport.SetValue("d_xa", dxa);
            flexCelReport.SetValue("d_huyen", dhuyen);
            flexCelReport.SetValue("d_tinh", dtinh);
            // ngay cấp


            flexCelReport.SetValue("a_ngaysinh", info.a_ngaysinh.HasValue ? info.a_ngaysinh.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("a_ngaysinh1", info.a_ngaysinh1.HasValue ? info.a_ngaysinh1.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("a_ngaycap_gt", info.a_ngaycap_gt.HasValue ? info.a_ngaycap_gt.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("a_ngaycap_gt1", info.a_ngaycap_gt1.HasValue ? info.a_ngaycap_gt1.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("b_ngaysinh", info.b_ngaysinh.HasValue ? info.b_ngaysinh.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("b_ngaycap", info.b_ngaycap.HasValue ? info.b_ngaycap.Value.ToString("dd/MM/yyyy") : null);

            flexCelReport.SetValue("d_ngaysinh", info.d_ngaysinh.HasValue ? info.d_ngaysinh.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("d_ngaysinh1", info.d_ngaysinh1.HasValue ? info.d_ngaysinh1.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("d_ngaycap_gt", info.d_ngaycap_gt.HasValue ? info.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("d_ngaycap_gt1", info.d_ngaycap_gt1.HasValue ? info.d_ngaycap_gt1.Value.ToString("dd/MM/yyyy") : null);
            flexCelReport.SetValue("d_ngaycongchung", info.d_ngaycongchung.HasValue ? info.d_ngaycongchung.Value.ToString("dd/MM/yyyy") : null);
            return flexCelReport;
        }

        // GET: Infomations/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Infomation infomation = db.Infomations.Find(id);
            if (infomation == null)
            {
                return HttpNotFound();
            }
            return View(infomation);
        }

        // POST: Infomations/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Infomation infomation = db.Infomations.Find(id);
            db.Infomations.Remove(infomation);
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
        static Regex ConvertToUnsign_rg = null;

        public static string ConvertToUnsign(string strInput)
        {
            if (ReferenceEquals(ConvertToUnsign_rg, null))
            {
                ConvertToUnsign_rg = new Regex("p{IsCombiningDiacriticalMarks}+");
            }
            var temp = strInput.Normalize(NormalizationForm.FormD);
            return ConvertToUnsign_rg.Replace(temp, string.Empty).ToLower();
        }
        public ActionResult ServiceChange(int? service_id, int hoso_id)
        {
            IQueryable<Models.Document> lst;
            if (service_id.HasValue)
            {
                IQueryable<int> doc_id = db.fk_service_document.Where(x => x.service_id == service_id.Value).Select(x => x.document_id);
                lst = db.Documents.Where(x => doc_id.Contains(x.id));
            }
            else
                lst = db.Documents;
            string result = String.Empty;
            foreach (var t in XDocument.sType)
            {
                result +=
                        "<li>" +
                            "<label style=\"color: deeppink; text-transform: uppercase;\">" + t.Value + "</label>" +
                            "<ul>";
                foreach (var item in lst.Where(x => x.type == t.Key))
                {
                    //result += "<li><a href=\"/Infomations/Download?d=" + item.id + "&c=" + contract_id + "\" target=\"_blank\">" + item.name + "</a></li>";
                    result += "<li><a href=\"javascript: Download(" + item.id + ", " + hoso_id + ")\">" + item.name + "</a></li>";
                }
                result += "</ul></li>";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private void FlexcellReport_GetImageData(object sender, FlexCel.Report.GetImageDataEventArgs e)
        {

            Bitmap bmp = new Bitmap(Server.MapPath("~/public/" + e.ImageName));

            bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            using (MemoryStream OutStream = new MemoryStream())
            {
                bmp.Save(OutStream, System.Drawing.Imaging.ImageFormat.Png);
                e.Width = 150; // bmp.Width;
                e.Height = 150; // bmp.Height;
                e.ImageData = OutStream.ToArray();
            }

            //}
        }
        public ActionResult NguonGoc(string p)
        {
            return Json(XInfomation.sNguongoc.Where(x => x.type.Equals(p)).ToArray(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult AccountDetailsX(int? id)
        {
            Account obj = db.Accounts.Find(id);
            sAccount acc = new sAccount();
            acc.obj = obj;
            acc.ngaysinh = obj.birthday.HasValue ? obj.birthday.Value.ToString("dd/MM/yyyy") : null;
            acc.ngaycap = obj.ngaycap_gt.HasValue ? obj.ngaycap_gt.Value.ToString("dd/MM/yyyy") : null;
            return Json(acc, JsonRequestBehavior.AllowGet);
        }

    }
    sealed class sAccount
    {
        public Account obj { get; set; }
        public string ngaysinh { get; set; }
        public string ngaycap { get; set; }
    }
}
