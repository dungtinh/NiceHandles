using Aspose.Words.Drawing;
using NiceHandles.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Web;

namespace NiceHandles.Controllers
{
    public partial class InfomationsController
    {        
        void Gen9DKTACHHOP(Infomation info, Aspose.Words.Document doc, string pathTemp, Dictionary<string, string> dict)
        {
            doc.RemoveAllChildren();
            dict = AddToDictAuto(info, dict);
            var lstThuaDat = db.TachThuas.Where(x => x.infomation_id == info.id).ToArray();
            foreach (var item in lstThuaDat)
            {
                var lstThuaChung = lstThuaDat.Where(x => !string.IsNullOrEmpty(x.b_dientich_chung) && !x.b_dientich_chung.Trim().Equals("0")).Select(x => x.b_sothua.Trim()).ToList();
                var newDictionary = dict.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
                foreach (var per in typeof(TachThua).GetProperties())
                {
                    var name = per.Name;
                    if (!name.Equals("b_sogiaychungnhan") && !name.Equals("b_ngaycap") && !name.Equals("b_noicap") && !name.Equals("b_sovaoso"))
                    {
                        var value = per.GetValue(item);
                        string val = value == null ? "......................." : !string.IsNullOrEmpty(value.ToString()) ? value.ToString() : ".......................";
                        if (newDictionary.ContainsKey(name))
                            newDictionary[name] = val;
                        else
                            newDictionary.Add(name, val);
                    }
                }
                if (!string.IsNullOrEmpty(item.d_hoten) || !string.IsNullOrEmpty(item.d_hoten1))
                {
                    var d_ndk = Utils.ToTTCN(item.d_gioitinh, item.d_hoten, item.d_ngaysinh, item.d_loaigiayto, item.d_sogiayto, string.Empty, item.d_ngaycap_gt, false);
                    d_ndk += Utils.ToTTCN(item.d_gioitinh1, item.d_hoten1, item.d_ngaysinh1, item.d_loaigiayto1, item.d_sogiayto1, string.Empty, item.d_ngaycap_gt1, true);
                    newDictionary.Add("x_ndk", d_ndk);
                    newDictionary.Add("x_hktt", item.d_hktt);
                }
                else
                {
                    newDictionary.Add("x_ndk", newDictionary["a_ndk"]);
                    newDictionary.Add("x_hktt", info.a_hktt);
                }
                Aspose.Words.Document doc1 = new Aspose.Words.Document(pathTemp);
                doc1.MailMerge.Execute(newDictionary.Keys.ToArray(), newDictionary.Values.ToArray());
                Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc1);
                builder.MoveToMergeField("d_noidungtruocbiendong");
                string d_noidungtruocbiendong = String.Empty, d_noidungsaubiendong = string.Empty;
                if (!string.IsNullOrEmpty(item.d_hoten) || !string.IsNullOrEmpty(item.d_hoten1))
                {
                    d_noidungtruocbiendong = "- " + info.a_hoten.ToUpper() + "; năm sinh: " + dict["a_namsinh"] + "; " + info.a_loaigiayto + ": " + info.a_sogiayto + "; địa chỉ: " + info.a_hktt;
                    d_noidungtruocbiendong += "<br />";
                    if (!string.IsNullOrEmpty(info.a_hoten1))
                    {
                        d_noidungtruocbiendong += "- " + info.a_hoten1.ToUpper() + "; năm sinh: " + dict["a_namsinh1"] + "; " + info.a_loaigiayto1 + ": " + info.a_sogiayto1 + "; địa chỉ: " + info.a_hktt1;
                        d_noidungtruocbiendong += "<br />";
                    }
                    if (!string.IsNullOrEmpty(item.d_hoten))
                    {
                        d_noidungsaubiendong += "- " + item.d_hoten.ToUpper() + "; năm sinh: " + (item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("yyyy") : ".....") + "; " + item.d_loaigiayto + ": " + item.d_sogiayto + "; địa chỉ: " + item.d_hktt;
                        d_noidungsaubiendong += "<br />";
                    }
                    if (!string.IsNullOrEmpty(item.d_hoten1))
                    {
                        d_noidungsaubiendong += "- " + item.d_hoten1.ToUpper() + "; năm sinh: " + (item.d_ngaysinh1.HasValue ? item.d_ngaysinh1.Value.ToString("yyyy") : ".....") + "; " + item.d_loaigiayto1 + ": " + item.d_sogiayto1 + "; địa chỉ: " + item.d_hktt1;
                        d_noidungsaubiendong += "<br />";
                    }
                }
                d_noidungtruocbiendong += "- Thửa đất số: " + info.b_sothua + ";";
                d_noidungtruocbiendong += "<br />";
                d_noidungtruocbiendong += "- Tờ bản đồ: " + info.b_tobando + ";";
                d_noidungtruocbiendong += "<br />";
                d_noidungtruocbiendong += "- Diện tích: " + info.b_dientich + " m²;";
                builder.InsertHtml(d_noidungtruocbiendong);

                builder.MoveToMergeField("d_noidungsaubiendong");

                d_noidungsaubiendong += "- Thửa đất số: " + item.b_sothua + ";";
                d_noidungsaubiendong += "<br />";
                d_noidungsaubiendong += "- Tờ bản đồ: " + item.b_tobando + ";";
                d_noidungsaubiendong += "<br />";
                d_noidungsaubiendong += "- Diện tích: " + item.b_dientich + " m²;";
                if (lstThuaChung.Count > 0 && lstThuaChung.Contains(item.b_sothua.Trim()))
                {
                    lstThuaChung.Remove(item.b_sothua.Trim());
                    var strChung = string.Join(", ", lstThuaChung);
                    d_noidungsaubiendong += "; Trong đó:";
                    d_noidungsaubiendong += "<br />";
                    d_noidungsaubiendong += "&nbsp;&nbsp;&nbsp;+ Diện tích sử dụng riêng: " + item.b_dientich_rieng + " m²";
                    d_noidungsaubiendong += "<br />";
                    d_noidungsaubiendong += "&nbsp;&nbsp;&nbsp;+ Diện tích sử dụng chung: " + item.b_dientich_chung + " m² (Sử dụng chung với " + (lstThuaChung.Count == 1 ? "" : "các") + " thửa số " + strChung + ")";
                }
                builder.InsertHtml(d_noidungsaubiendong);
                doc.AppendDocument(doc1, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
        }
        void GenTACHHOPAUTO(Infomation info, Aspose.Words.Document doc, string pathTemp, Dictionary<string, string> dict)
        {
            doc.RemoveAllChildren();
            dict = AddToDictAuto(info, dict);

            var lstThuaDat = db.TachThuas.Where(x => x.infomation_id == info.id);

            foreach (var item in lstThuaDat)
            {
                var newDictionary = dict.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
                foreach (var per in typeof(TachThua).GetProperties())
                {
                    var name = "X" + per.Name;
                    var value = per.GetValue(item);
                    string val = value == null ? "......................." : !string.IsNullOrEmpty(value.ToString()) ? value.ToString() : ".......................";
                    if (newDictionary.ContainsKey(name))
                        newDictionary[name] = val;
                    else
                        newDictionary.Add(name, val);
                }

                if (!string.IsNullOrEmpty(item.b_dientich_chung))
                {
                    var dientichchung = Convert.ToDecimal(item.b_dientich_chung);
                    if (dientichchung > 0)
                    {
                        newDictionary["Xb_dientich"] += "m² ( Diện tích sử dụng riêng: " + item.b_dientich_rieng +
                            "m²; Diện tích sử dụng chung: " + item.b_dientich_chung + "m² )";
                    }
                }
                else
                {
                    newDictionary["Xb_dientich"] += "m²";
                }
                if (!string.IsNullOrEmpty(item.d_hoten) || !string.IsNullOrEmpty(item.d_hoten1))
                {
                    newDictionary["Xd_ngaysinh"] = item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["Xd_namsinh"] = item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("yyyy") : ".....";
                    newDictionary["Xd_gioitinh"] = !string.IsNullOrEmpty(item.d_gioitinh) ? item.d_gioitinh.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";
                    newDictionary["Xd_ngaycap_gt"] = item.d_ngaycap_gt.HasValue ? item.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "...../...../......";

                    newDictionary["Xd_ngaysinh1"] = item.d_ngaysinh1.HasValue ? item.d_ngaysinh1.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["Xd_namsinh1"] = item.d_ngaysinh1.HasValue ? item.d_ngaysinh1.Value.ToString("yyyy") : ".....";
                    newDictionary["Xd_gioitinh1"] = !string.IsNullOrEmpty(item.d_gioitinh1) ? item.d_gioitinh1.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";
                    newDictionary["Xd_ngaycap_gt1"] = item.d_ngaycap_gt1.HasValue ? item.d_ngaycap_gt1.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["Xd_ngaycongchung"] = item.d_ngaycongchung.HasValue ? item.d_ngaycongchung.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["Xd_tienhopdong"] = item.d_tienhopdong.HasValue ? item.d_tienhopdong.Value.ToString("N0") : "0";

                    string thon = string.Empty, xa = string.Empty, huyen = string.Empty, tinh = string.Empty, address = string.Empty;
                    bool isCity = false;
                    bool isSo = false;
                    // D                   
                    isCity = false;
                    isSo = false;
                    Utils.SpilitAddress(item.d_hktt, ref thon, ref xa, ref huyen, ref tinh, ref isCity, ref isSo, ref address);
                    newDictionary.Add("Xd_diachi", address);
                    newDictionary["Xd_huyen"] = huyen;
                    newDictionary["Xd_tinh"] = tinh;
                    newDictionary["Xd_thon"] = thon;
                    newDictionary["Xd_xa"] = xa;
                }
                else
                {
                    newDictionary["Xd_hoten"] = newDictionary["a_hoten"];
                    newDictionary["Xd_gioitinh"] = newDictionary["a_gioitinh"];
                    newDictionary["Xd_namsinh"] = newDictionary["a_namsinh"];
                    newDictionary["Xd_loaigiayto"] = newDictionary["a_loaigiayto"];
                    newDictionary["Xd_sogiayto"] = newDictionary["a_sogiayto"];

                    newDictionary["Xd_ngaycap_gt"] = newDictionary["a_ngaycap_gt"];
                    newDictionary["Xd_noicap_gt"] = newDictionary["a_noicap_gt"];
                    newDictionary["Xd_ngaycap_gt1"] = newDictionary["a_ngaycap_gt1"];
                    newDictionary["Xd_noicap_gt1"] = newDictionary["a_noicap_gt1"];

                    newDictionary["Xd_hktt"] = newDictionary["a_hktt"];
                    newDictionary["Xd_hoten1"] = newDictionary["a_hoten1"];
                    newDictionary["Xd_gioitinh1"] = newDictionary["a_gioitinh1"];
                    newDictionary["Xd_namsinh1"] = newDictionary["a_namsinh1"];
                    newDictionary["Xd_loaigiayto1"] = newDictionary["a_loaigiayto1"];
                    newDictionary["Xd_sogiayto1"] = newDictionary["a_sogiayto1"];
                    newDictionary["Xd_hktt1"] = newDictionary["a_hktt1"];

                    newDictionary["Xd_ngaysinh"] = newDictionary["a_ngaysinh"];
                    newDictionary["Xd_ngaysinh1"] = newDictionary["a_ngaysinh1"];

                    newDictionary.Add("Xd_diachi", newDictionary["a_diachi"]);
                    newDictionary["Xd_huyen"] = newDictionary["a_huyen"];
                    newDictionary["Xd_tinh"] = newDictionary["a_tinh"];
                    newDictionary["Xd_thon"] = newDictionary["a_thon"];
                    newDictionary["Xd_xa"] = newDictionary["a_xa"];
                }
                GenMaSoThue(item.d_masothue, "d", newDictionary);
                GenMaSoThue(item.a_masothue, "a", newDictionary);

                Aspose.Words.Document doc1 = new Aspose.Words.Document(pathTemp);
                doc1.MailMerge.Execute(newDictionary.Keys.ToArray(), newDictionary.Values.ToArray());

                doc.AppendDocument(doc1, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
        }
        void GenTACHHOPAUTONONDUPPLICATE(Infomation info, Aspose.Words.Document doc, string pathTemp, Dictionary<string, string> dict)
        {
            doc.RemoveAllChildren();
            dict = AddToDictAuto(info, dict);

            var lstThuaDat = db.TachThuas.Where(x => x.infomation_id == info.id);

            foreach (var item in lstThuaDat)
            {
                if (!string.IsNullOrEmpty(item.d_hoten) || !string.IsNullOrEmpty(item.d_hoten1))
                {
                    var newDictionary = dict.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
                    foreach (var per in typeof(TachThua).GetProperties())
                    {
                        var name = "X" + per.Name;
                        var value = per.GetValue(item);
                        string val = value == null ? "......................." : !string.IsNullOrEmpty(value.ToString()) ? value.ToString() : ".......................";
                        if (newDictionary.ContainsKey(name))
                            newDictionary[name] = val;
                        else
                            newDictionary.Add(name, val);
                    }

                    newDictionary["Xd_ngaysinh"] = item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["Xd_namsinh"] = item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("yyyy") : ".....";
                    newDictionary["Xd_gioitinh"] = !string.IsNullOrEmpty(item.d_gioitinh) ? item.d_gioitinh.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";
                    newDictionary["Xd_ngaycap_gt"] = item.d_ngaycap_gt.HasValue ? item.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "...../...../......";

                    newDictionary["Xd_ngaysinh1"] = item.d_ngaysinh1.HasValue ? item.d_ngaysinh1.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["Xd_namsinh1"] = item.d_ngaysinh1.HasValue ? item.d_ngaysinh1.Value.ToString("yyyy") : ".....";
                    newDictionary["Xd_gioitinh1"] = !string.IsNullOrEmpty(item.d_gioitinh1) ? item.d_gioitinh1.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";
                    newDictionary["Xd_ngaycap_gt1"] = item.d_ngaycap_gt1.HasValue ? item.d_ngaycap_gt1.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["Xd_ngaycongchung"] = item.d_ngaycongchung.HasValue ? item.d_ngaycongchung.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["Xd_tienhopdong"] = item.d_tienhopdong.HasValue ? item.d_tienhopdong.Value.ToString("N0") : "0";

                    newDictionary.Add("Xd_gioitinh11", item.d_gioitinh.Trim().ToUpper().Equals("NAM") ? "X" : " ");
                    newDictionary.Add("Xd_gioitinh22", item.d_gioitinh.Trim().ToUpper().Equals("NỮ") ? "X" : " ");
                    newDictionary.Add("Xd_sogiayto_cm", item.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "...................." : item.d_sogiayto);
                    newDictionary.Add("Xd_ngaycap_gt_cm", item.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..............." : item.d_ngaycap_gt.HasValue ? item.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : ".................");
                    newDictionary.Add("Xd_noicap_gt_cm", item.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "................." : item.d_noicap_gt);
                    newDictionary.Add("Xd_sogiayto_cc", !item.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "...................." : item.d_sogiayto);
                    newDictionary.Add("Xd_ngaycap_gt_cc", !item.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "................." : item.d_ngaycap_gt.HasValue ? item.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "...............");
                    newDictionary.Add("Xd_noicap_gt_cc", !item.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "................." : item.d_noicap_gt);

                    string thon = string.Empty, xa = string.Empty, huyen = string.Empty, tinh = string.Empty, address = string.Empty;
                    bool isCity = false;
                    bool isSo = false;
                    // D                   
                    isCity = false;
                    isSo = false;

                    Utils.SpilitAddress(item.d_hktt, ref thon, ref xa, ref huyen, ref tinh, ref isCity, ref isSo, ref address);
                    newDictionary.Add("Xd_diachi", address);
                    newDictionary["Xd_huyen"] = huyen;
                    newDictionary["Xd_tinh"] = tinh;
                    newDictionary["Xd_thon"] = thon;
                    newDictionary["Xd_xa"] = xa;
                    Aspose.Words.Document doc1 = new Aspose.Words.Document(pathTemp);
                    doc1.MailMerge.Execute(newDictionary.Keys.ToArray(), newDictionary.Values.ToArray());
                    doc.AppendDocument(doc1, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
                }
            }
            var newDictionary1 = dict.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            newDictionary1["Xd_hoten"] = newDictionary1["a_hoten"];
            newDictionary1["Xd_gioitinh"] = newDictionary1["a_gioitinh"];
            newDictionary1["Xd_namsinh"] = newDictionary1["a_namsinh"];
            newDictionary1["Xd_loaigiayto"] = newDictionary1["a_loaigiayto"];
            newDictionary1["Xd_sogiayto"] = newDictionary1["a_sogiayto"];

            newDictionary1["Xd_ngaycap_gt"] = newDictionary1["a_ngaycap_gt"];
            newDictionary1["Xd_noicap_gt"] = newDictionary1["a_noicap_gt"];
            newDictionary1["Xd_ngaycap_gt1"] = newDictionary1["a_ngaycap_gt1"];
            newDictionary1["Xd_noicap_gt1"] = newDictionary1["a_noicap_gt1"];

            newDictionary1["Xd_hktt"] = newDictionary1["a_hktt"];
            newDictionary1["Xd_hoten1"] = newDictionary1["a_hoten1"];
            newDictionary1["Xd_gioitinh1"] = newDictionary1["a_gioitinh1"];
            newDictionary1["Xd_namsinh1"] = newDictionary1["a_namsinh1"];
            newDictionary1["Xd_loaigiayto1"] = newDictionary1["a_loaigiayto1"];
            newDictionary1["Xd_sogiayto1"] = newDictionary1["a_sogiayto1"];
            newDictionary1["Xd_hktt1"] = newDictionary1["a_hktt1"];

            newDictionary1["Xd_ngaysinh"] = newDictionary1["a_ngaysinh"];
            newDictionary1["Xd_ngaysinh1"] = newDictionary1["a_ngaysinh1"];

            newDictionary1.Add("Xd_diachi", newDictionary1["a_diachi"]);
            newDictionary1["Xd_huyen"] = newDictionary1["a_huyen"];
            newDictionary1["Xd_tinh"] = newDictionary1["a_tinh"];
            newDictionary1["Xd_thon"] = newDictionary1["a_thon"];
            newDictionary1["Xd_xa"] = newDictionary1["a_xa"];

            newDictionary1.Add("Xd_gioitinh11", info.a_gioitinh.Trim().ToUpper().Equals("NAM") ? "X" : " ");
            newDictionary1.Add("Xd_gioitinh22", info.a_gioitinh.Trim().ToUpper().Equals("NỮ") ? "X" : " ");
            newDictionary1.Add("Xd_sogiayto_cm", info.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "......................" : info.d_sogiayto);
            newDictionary1.Add("Xd_ngaycap_gt_cm", info.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..............." : info.d_ngaycap_gt.HasValue ? info.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : ".................");
            newDictionary1.Add("Xd_noicap_gt_cm", info.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..................." : info.d_noicap_gt);
            newDictionary1.Add("Xd_sogiayto_cc", !info.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "......................" : info.d_sogiayto);
            newDictionary1.Add("Xd_ngaycap_gt_cc", !info.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "................." : info.d_ngaycap_gt.HasValue ? info.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "...............");
            newDictionary1.Add("Xd_noicap_gt_cc", !info.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? ".................." : info.a_noicap_gt);

            Aspose.Words.Document doc2 = new Aspose.Words.Document(pathTemp);
            doc2.MailMerge.Execute(newDictionary1.Keys.ToArray(), newDictionary1.Values.ToArray());
            doc.AppendDocument(doc2, Aspose.Words.ImportFormatMode.KeepSourceFormatting);

        }        
        void Gen11DK(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            if (!string.IsNullOrEmpty(info.a_hoten1))
            {
                string a_tt1 = string.Empty;
                a_tt1 = info.a_hoten1.ToUpper() + "; Năm sinh: " + (info.a_ngaysinh1.HasValue ? info.a_ngaysinh1.Value.ToString("yyyy") : ".....") + "; " + info.a_loaigiayto1 + ": " + info.a_sogiayto1 + "; nơi cấp: " + info.a_noicap_gt1;
                dict.Add("a_tt1", a_tt1);
            }
            var b_dientichsaukhitach = "Thửa thứ nhất: … m²; Thửa thứ hai: … m²;";
            var b_danhsachthuatachhop = string.IsNullOrEmpty(info.b_sothua) ? null : info.b_sothua + "; ";
            var b_danhsachgcn = string.IsNullOrEmpty(info.b_sogiaychungnhan) ? null : info.b_sogiaychungnhan + "; ";
            var b_soluongthua = " … ";

            var lstTachThua = db.TachThuas.Where(x => x.infomation_id == info.id);
            if (lstTachThua.Count() > 0)
            {
                b_dientichsaukhitach = String.Empty;
                b_soluongthua = lstTachThua.Count().ToString();
                int index = 0;
                string[] strIndex = new string[30] {
                    "nhất","hai","ba","bốn","năm", "sáu", "bảy", "tám", "chín", "mười", "mười một", "mười hai", "mười ba", "mười bốn", "mười năm", "mười sáu", "mười bảy", "mười tám", "mười chín"
                    , "hai mươi", "hai mươi mốt", "hai mươi hai", "hai mươi ba", "hai mươi tư", "hai mươi năm", "hai mươi sáu", "hai mươi bảy", "hai mươi tám", "hai mươi chín", "ba mươi"};
                foreach (var td in lstTachThua)
                {
                    b_dientichsaukhitach += "Thửa thứ " + strIndex[index] + ": " + td.b_dientich + " m²; ";
                    index++;
                    b_danhsachthuatachhop += td.b_sothua + "; ";
                }
            }
            var lstHopThua = db.ThuaDats.Where(x => x.infomation_id == info.id && x.type == (int)XInfomation.eService.HopThua).ToList();
            foreach (var ht in lstHopThua)
            {
                b_danhsachthuatachhop += ht.sothua + "; ";
                b_danhsachgcn += ht.sogiaychungnhan + "; ";
            }
            dict.Add("b_dientichsaukhitach", b_dientichsaukhitach);
            dict.Add("b_soluongthua", b_soluongthua);
            dict.Add("b_danhsachthuatachhop", b_danhsachthuatachhop);
            dict.Add("b_danhsachgcn", b_danhsachgcn);

            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());

            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            if (string.IsNullOrEmpty(info.a_hoten1))
            {
                builder.MoveToMergeField("a_tt1");
                builder.CurrentParagraph.Remove();
            }
            if (lstHopThua.Count == 0)
                lstHopThua.Add(new ThuaDat() { sothua = "", tobando = "", diachithuadat = "", sogiaychungnhan = "", sovaoso = "" });
            builder.MoveToMergeField("b_hopthua");
            Aspose.Words.Tables.Table table = builder.StartTable();
            builder.InsertCell();
            //table.AutoFit(Aspose.Words.Tables.AutoFitBehavior.FixedColumnWidths);
            builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
            builder.ParagraphFormat.Alignment = Aspose.Words.ParagraphAlignment.Center;
            builder.Write("Thửa đất số");
            builder.InsertCell();
            builder.Write("Tờ bản đồ số");
            builder.InsertCell();
            builder.Write("Địa chỉ thửa đất");
            builder.InsertCell();
            builder.Write("Số phát hành Giấy chứng nhận");
            builder.InsertCell();
            builder.Write("Số vào sổ cấp giấy chứng nhận");
            builder.EndRow();

            foreach (var ht in lstHopThua)
            {
                builder.InsertCell();
                builder.Write(ht.sothua);
                builder.InsertCell();
                builder.Write(ht.tobando);
                builder.InsertCell();
                builder.Write(ht.diachithuadat);
                builder.InsertCell();
                builder.Write(ht.sogiaychungnhan);
                builder.InsertCell();
                builder.Write(ht.sovaoso);

                builder.EndRow();
            }
            table.PreferredWidth = Aspose.Words.Tables.PreferredWidth.FromPercent(100);
            table.SetBorder(Aspose.Words.BorderType.Left, Aspose.Words.LineStyle.None, 0, Color.Transparent, true);
            table.SetBorder(Aspose.Words.BorderType.Right, Aspose.Words.LineStyle.None, 0, Color.Transparent, true);
            table.SetBorder(Aspose.Words.BorderType.Top, Aspose.Words.LineStyle.None, 0, Color.Transparent, true);
            table.SetBorder(Aspose.Words.BorderType.Bottom, Aspose.Words.LineStyle.None, 0, Color.Transparent, true);

            builder.EndTable();
        }
        void Gen11DKTACHTHUA(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            var b_dientichsaukhitach = "Thửa thứ nhất: … m²; Thửa thứ hai: … m²;";
            var b_danhsachthuatachhop = string.IsNullOrEmpty(info.b_sothua) ? null : info.b_sothua + "; ";
            var b_soluongthua = " … ";
            var lydotachthua = new List<string>();
            var lstTachThua = db.TachThuas.Where(x => x.infomation_id == info.id);
            if (lstTachThua.Count() > 0)
            {
                b_dientichsaukhitach = String.Empty;
                b_soluongthua = lstTachThua.Count().ToString();
                int index = 0;
                string[] strIndex = new string[30] {
                    "nhất","hai","ba","bốn","năm", "sáu", "bảy", "tám", "chín", "mười", "mười một", "mười hai", "mười ba", "mười bốn", "mười năm", "mười sáu", "mười bảy", "mười tám", "mười chín"
                    , "hai mươi", "hai mươi mốt", "hai mươi hai", "hai mươi ba", "hai mươi tư", "hai mươi năm", "hai mươi sáu", "hai mươi bảy", "hai mươi tám", "hai mươi chín", "ba mươi"};
                foreach (var td in lstTachThua)
                {
                    b_dientichsaukhitach += "Thửa thứ " + strIndex[index] + ": " + td.b_dientich + " m²; ";
                    index++;
                    b_danhsachthuatachhop += td.b_sothua + "; ";
                    switch (td.d_lydobiendong)
                    {
                        case "Tách thửa để sử dụng":
                            lydotachthua.Add("sử dụng");
                            break;
                        case "Tách thửa để chuyển nhượng":
                        case "Nhận chuyển nhượng":
                            lydotachthua.Add("chuyển nhượng");
                            break;
                        case "Tách thửa để tặng cho":
                        case "Nhận tặng cho":
                            lydotachthua.Add("tặng cho");
                            break;
                        default:
                            break;
                    }

                }
            }
            dict.Add("b_dientichsaukhitach", b_dientichsaukhitach);
            dict.Add("b_soluongthua", b_soluongthua);
            dict.Add("b_danhsachthuatachhop", b_danhsachthuatachhop);
            dict.Add("lydotach", "Tách thửa để " + string.Join(", ", lydotachthua.Distinct()));

            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }                
        void GenTACHTHUA(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            var dicttemp = dict.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            doc.RemoveAllChildren();
            // I, TỜ KHAI 11            
            var docTK = db.Documents.First(x => x.code.Equals("11DKTACHTHUA"));
            string pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            var doctemp = new Aspose.Words.Document(pathTemp);
            Gen11DKTACHTHUA(info, doctemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);

            //9DK Gen9DKTACHHOP
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                             entry => entry.Value);
            docTK = db.Documents.First(x => x.code.Equals("9DKTACHHOP"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            Gen9DKTACHHOP(info, doctemp, pathTemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            // TỜ KHAI ĐĂNG KÝ THUẾ
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                             entry => entry.Value);
            docTK = db.Documents.First(x => x.code.Equals("TKDKTTACHTHUA"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            GenTKDKTTACHTHUA(info, doctemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            // TỜ KHAI LỆ PHÍ TRƯỚC BẠ            
            docTK = db.Documents.First(x => x.code.Equals("TKLPTBTACHTHUA"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                                 entry => entry.Value);
            GenTKLPTBTACHTHUA(info, doctemp, pathTemp, dicttemp);

            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);

            // TỜ KHAI THUẾ PHI NÔNG NGHIỆP
            docTK = db.Documents.First(x => x.code.Equals("TKTPNNTACHTHUA"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                                 entry => entry.Value);
            GenTKPNNTACHTHUA(info, doctemp, pathTemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            // TỜ KHAI THUẾ TNCN
            docTK = db.Documents.First(x => x.code.Equals("TKTTNCNTACHTHUA"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                                 entry => entry.Value);
            GenTKTTNCNTACHHOP(info, doctemp, pathTemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            // END            
        }        
        void GenMS01PYCQuyHoach(Infomation info, HoSo hoso, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            AddToDictAuto(info, dict);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            builder.MoveToMergeField("b_qr");
            builder.InsertImage(Server.MapPath("~/public/" + hoso.link_gmap_qr),
            RelativeHorizontalPosition.Margin,
            0,
            RelativeVerticalPosition.Margin,
            0,
            80,
            80,
            WrapType.Square);
            builder.MoveToMergeField("c_qr");
            builder.InsertImage(Server.MapPath("~/public/" + hoso.link_filecad_qr),
            RelativeHorizontalPosition.Margin,
            0,
            RelativeVerticalPosition.Margin,
            0,
            80,
            80,
            WrapType.Square);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
    }
}