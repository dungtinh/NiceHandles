using Aspose.Words;
using Aspose.Words.Drawing;
using NiceHandles.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;

namespace NiceHandles.Controllers
{
    public partial class InfomationsController
    {
        void GenTKLPTBTACHHOP(Infomation info, Aspose.Words.Document doc, string pathTemp, Dictionary<string, string> dict)
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
                        newDictionary["Xb_dientich"] += " ( Diện tích sử dụng riêng: " + item.b_dientich_rieng +
                            "m²; Diện tích sử dụng chung: " + item.b_dientich_chung + "m² )";
                    }
                }
                if (!string.IsNullOrEmpty(item.d_hoten) || !string.IsNullOrEmpty(item.d_hoten1))
                {
                    newDictionary["Xd_ngaysinh"] = item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["Xd_namsinh"] = item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("yyyy") : ".....";
                    newDictionary["Xd_gioitinh"] = !string.IsNullOrEmpty(item.d_gioitinh) ? item.d_gioitinh.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";

                    newDictionary["Xd_ngaysinh1"] = item.d_ngaysinh1.HasValue ? item.d_ngaysinh1.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["Xd_namsinh1"] = item.d_ngaysinh1.HasValue ? item.d_ngaysinh1.Value.ToString("yyyy") : ".....";
                    newDictionary["Xd_gioitinh1"] = !string.IsNullOrEmpty(item.d_gioitinh1) ? item.d_gioitinh1.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";
                    newDictionary["Xd_ngaycongchung"] = item.d_ngaycongchung.HasValue ? item.d_ngaycongchung.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["Xd_tienhopdong"] = (item.d_tienhopdong.HasValue ? item.d_tienhopdong.Value.ToString("N0") : "0") + " đồng";

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
                    GenMaSoThue(item.d_masothue, "d", newDictionary);
                }
                else
                {
                    newDictionary["Xd_hoten"] = newDictionary["a_hoten"];
                    newDictionary["Xd_gioitinh"] = newDictionary["a_gioitinh"];
                    newDictionary["Xd_namsinh"] = newDictionary["a_namsinh"];
                    newDictionary["Xd_loaigiayto"] = newDictionary["a_loaigiayto"];
                    newDictionary["Xd_sogiayto"] = newDictionary["a_sogiayto"];
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
                    GenMaSoThue(item.a_masothue, "d", newDictionary);
                }
                Aspose.Words.Document doc1 = new Aspose.Words.Document(pathTemp);
                doc1.MailMerge.Execute(newDictionary.Keys.ToArray(), newDictionary.Values.ToArray());

                doc.AppendDocument(doc1, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
        }
        void GenTKLPTBTACHTHUA(Infomation info, Aspose.Words.Document doc, string pathTemp, Dictionary<string, string> dict)
        {
            bool flag = false;
            doc.RemoveAllChildren();
            dict = AddToDictAuto(info, dict);

            var lstThuaDat = db.TachThuas.Where(x => x.infomation_id == info.id);

            foreach (var item in lstThuaDat)
            {
                if (!string.IsNullOrEmpty(item.d_hoten))
                {
                    flag = true;
                    var newDictionary = dict.ToDictionary(entry => entry.Key,
                                                   entry => entry.Value);
                    foreach (var per in typeof(TachThua).GetProperties())
                    {
                        var name = per.Name;
                        if (name.StartsWith("d_") || name.StartsWith("b_"))
                        {
                            var value = per.GetValue(item);
                            string val = value == null ? "......................." : !string.IsNullOrEmpty(value.ToString()) ? value.ToString() : ".......................";
                            if (newDictionary.ContainsKey(name))
                                newDictionary[name] = val;
                            else
                                newDictionary.Add(name, val);
                        }
                    }
                    if (!string.IsNullOrEmpty(item.b_dientich_chung))
                    {
                        var dientichchung = Convert.ToDecimal(item.b_dientich_chung);
                        if (dientichchung > 0)
                        {
                            newDictionary["b_dientich"] += " ( Diện tích sử dụng riêng: " + item.b_dientich_rieng +
                                "m²; Diện tích sử dụng chung: " + item.b_dientich_chung + "m² )";
                        }
                    }
                    newDictionary["d_ngaysinh"] = item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["d_namsinh"] = item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("yyyy") : ".....";
                    newDictionary["d_gioitinh"] = !string.IsNullOrEmpty(item.d_gioitinh) ? item.d_gioitinh.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";

                    newDictionary["d_ngaycongchung"] = item.d_ngaycongchung.HasValue ? item.d_ngaycongchung.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["d_tienhopdong"] = (item.d_tienhopdong.HasValue ? item.d_tienhopdong.Value.ToString("N0") : "0") + " đồng";

                    string thon = string.Empty, xa = string.Empty, huyen = string.Empty, tinh = string.Empty, address = string.Empty;
                    bool isCity = false;
                    bool isSo = false;
                    Utils.SpilitAddress(item.d_hktt, ref thon, ref xa, ref huyen, ref tinh, ref isCity, ref isSo, ref address);
                    if (newDictionary.ContainsKey("d_diachi"))
                        newDictionary["d_diachi"] = address;
                    else
                        newDictionary.Add("d_diachi", address);
                    newDictionary["d_huyen"] = huyen;
                    newDictionary["d_tinh"] = tinh;
                    newDictionary["d_thon"] = thon;
                    newDictionary["d_xa"] = xa;
                    isCity = false;
                    isSo = false;
                    Utils.SpilitAddress(item.b_diachithuadat, ref thon, ref xa, ref huyen, ref tinh, ref isCity, ref isSo, ref address);
                    if (newDictionary.ContainsKey("b_diachi"))
                        newDictionary["b_diachi"] = address;
                    else
                        newDictionary.Add("b_diachi", address);
                    newDictionary["b_huyen"] = huyen;
                    newDictionary["b_tinh"] = tinh;
                    newDictionary["b_thon"] = thon;
                    newDictionary["b_xa"] = xa;
                    GenMaSoThue(item.d_masothue, "d", newDictionary);

                    Aspose.Words.Document doc1 = new Aspose.Words.Document(pathTemp);
                    doc1.MailMerge.Execute(newDictionary.Keys.ToArray(), newDictionary.Values.ToArray());
                    doc.AppendDocument(doc1, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
                }
            }
            if (!flag)
            {
                Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
                builder.MoveToDocumentEnd();
                builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
            }
        }
        void GenTKDKTTACHTHUA(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            bool flag = false;
            var docx = doc.Clone();
            doc.RemoveAllChildren();
            dict = AddToDictAuto(info, dict);
            if (string.IsNullOrEmpty(info.a_masothue))
            {
                flag = true;
                var dicta = dict.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
                var doca = docx.Clone();
                dicta.Add("x_hoten", info.a_hoten);
                dicta.Add("x_ngaysinh", dicta["a_ngaysinh"]);
                dicta.Add("x_gioitinh11", dicta["a_gioitinh11"]);
                dicta.Add("x_gioitinh22", dicta["a_gioitinh22"]);

                dicta.Add("x_loaigiayto", dicta["a_loaigiayto"]);

                dicta.Add("x_sogiayto_cm", info.a_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.a_sogiayto);
                dicta.Add("x_ngaycap_gt_cm", info.a_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.a_ngaycap_gt.HasValue ? info.a_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "............");
                dicta.Add("x_noicap_gt_cm", info.a_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.a_noicap_gt);

                dicta.Add("x_sogiayto_cc", !info.a_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.a_sogiayto);
                dicta.Add("x_ngaycap_gt_cc", !info.a_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.a_ngaycap_gt.HasValue ? info.a_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "............");
                dicta.Add("x_noicap_gt_cc", !info.a_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.a_noicap_gt);
                //
                dicta.Add("x_thon", dicta["a_thon"]);
                dicta.Add("x_xa", dicta["a_xa"]);
                dicta.Add("x_huyen", dicta["a_huyen"]);
                dicta.Add("x_tinh", dicta["a_tinh"]);
                doca.MailMerge.Execute(dicta.Keys.ToArray(), dicta.Values.ToArray());
                Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doca);
                builder.MoveToDocumentEnd();
                builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
                doc.AppendDocument(doca, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
            var lstThuaDat = db.TachThuas.Where(x => x.infomation_id == info.id);
            foreach (var item in lstThuaDat)
            {
                if (string.IsNullOrEmpty(item.d_masothue) && !string.IsNullOrEmpty(item.d_hoten))
                {
                    flag = true;
                    var docd = docx.Clone();
                    var dictd = dict.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
                    dictd.Add("x_hoten", item.d_hoten);
                    dictd.Add("x_ngaysinh", item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("dd/MM/yyyy") : "...../...../......");
                    dictd.Add("x_gioitinh11", item.d_gioitinh.Trim().ToUpper().Equals("NAM") ? "X" : " ");
                    dictd.Add("x_gioitinh22", item.d_gioitinh.Trim().ToUpper().Equals("NỮ") ? "X" : " ");

                    dictd.Add("x_loaigiayto", dictd["d_loaigiayto"]);

                    dictd.Add("x_sogiayto_cm", item.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : item.d_sogiayto);
                    dictd.Add("x_ngaycap_gt_cm", item.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : item.d_ngaycap_gt.HasValue ? item.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "............");
                    dictd.Add("x_noicap_gt_cm", item.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : item.d_noicap_gt);

                    dictd.Add("x_sogiayto_cc", !item.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : item.d_sogiayto);
                    dictd.Add("x_ngaycap_gt_cc", !item.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : item.d_ngaycap_gt.HasValue ? item.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "............");
                    dictd.Add("x_noicap_gt_cc", !item.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : item.d_noicap_gt);

                    string thon = string.Empty, xa = string.Empty, huyen = string.Empty, tinh = string.Empty, address = string.Empty;
                    bool isCity = false;
                    bool isSo = false;
                    Utils.SpilitAddress(item.d_hktt, ref thon, ref xa, ref huyen, ref tinh, ref isCity, ref isSo, ref address);
                    //
                    dictd.Add("x_thon", thon);
                    dictd.Add("x_xa", xa);
                    dictd.Add("x_huyen", huyen);
                    dictd.Add("x_tinh", tinh);
                    docd.MailMerge.Execute(dictd.Keys.ToArray(), dictd.Values.ToArray());
                    Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(docd);
                    builder.MoveToDocumentEnd();
                    builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
                    doc.AppendDocument(docd, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
                }
            }
            if (!flag)
            {
                Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
                builder.MoveToDocumentEnd();
                builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
            }
        }
        void GenTKTTNCNTACHHOP(Infomation info, Aspose.Words.Document doc, string pathTemp, Dictionary<string, string> dict)
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

                    newDictionary["Xd_ngaycongchung"] = item.d_ngaycongchung.HasValue ? item.d_ngaycongchung.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["Xd_tienhopdong"] = item.d_tienhopdong.HasValue ? item.d_tienhopdong.Value.ToString("N0") : "0";

                    if (item.d_lydobiendong.ToUpper().Trim().Equals("NHẬN TẶNG CHO"))
                    {
                        newDictionary.Add("Xd_sohopdong1", newDictionary["Xd_sohopdong"]);
                        newDictionary.Add("Xd_noicongchung1", newDictionary["Xd_noicongchung"]);
                        newDictionary.Add("Xd_ngaycongchung1", newDictionary["Xd_ngaycongchung"]);

                        newDictionary["Xd_sohopdong"] = "...............";
                        newDictionary["Xd_noicongchung"] = "...........................";
                        newDictionary["Xd_ngaycongchung"] = "............";
                        newDictionary.Add("xd_CN", "");
                        newDictionary.Add("xd_TC", "X");
                    }
                    else
                    {
                        newDictionary.Add("Xd_sohopdong1", "...............");
                        newDictionary.Add("Xd_noicongchung1", "...........................");
                        newDictionary.Add("Xd_ngaycongchung1", "............");
                        newDictionary.Add("xd_CN", "X");
                        newDictionary.Add("xd_TC", "");
                    }

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
                    GenMaSoThue(info.a_masothue, "a", newDictionary);
                    GenMaSoThue(item.d_masothue, "xd", newDictionary);
                    Aspose.Words.Document doc1 = new Aspose.Words.Document(pathTemp);
                    doc1.MailMerge.Execute(newDictionary.Keys.ToArray(), newDictionary.Values.ToArray());

                    doc.AppendDocument(doc1, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
                }
            }
        }
        void GenTKPNNTACHTHUA(Infomation info, Aspose.Words.Document doc, string pathTemp, Dictionary<string, string> dict)
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
                        var name = per.Name;
                        if (name.StartsWith("d_") || name.StartsWith("b_"))
                        {
                            var value = per.GetValue(item);
                            string val = value == null ? "......................." : !string.IsNullOrEmpty(value.ToString()) ? value.ToString() : ".......................";
                            if (newDictionary.ContainsKey(name))
                                newDictionary[name] = val;
                            else
                                newDictionary.Add(name, val);
                        }
                    }
                    if (!string.IsNullOrEmpty(item.b_dientich_chung))
                    {
                        var dientichchung = Convert.ToDecimal(item.b_dientich_chung);
                        if (dientichchung > 0)
                        {
                            newDictionary["b_dientich"] += "m² ( Diện tích sử dụng riêng: " + item.b_dientich_rieng +
                                "m²; Diện tích sử dụng chung: " + item.b_dientich_chung + "m² )";
                        }
                    }
                    else
                    {
                        newDictionary["b_dientich"] += "m²";
                    }
                    newDictionary["d_ngaysinh"] = item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["d_namsinh"] = item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("yyyy") : ".....";
                    newDictionary["d_gioitinh"] = !string.IsNullOrEmpty(item.d_gioitinh) ? item.d_gioitinh.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";
                    newDictionary["d_ngaycap_gt"] = item.d_ngaycap_gt.HasValue ? item.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "...../...../......";

                    newDictionary["d_ngaycongchung"] = item.d_ngaycongchung.HasValue ? item.d_ngaycongchung.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    newDictionary["d_tienhopdong"] = item.d_tienhopdong.HasValue ? item.d_tienhopdong.Value.ToString("N0") : "0";

                    string thon = string.Empty, xa = string.Empty, huyen = string.Empty, tinh = string.Empty, address = string.Empty;
                    bool isCity = false;
                    bool isSo = false;

                    Utils.SpilitAddress(item.d_hktt, ref thon, ref xa, ref huyen, ref tinh, ref isCity, ref isSo, ref address);
                    if (newDictionary.ContainsKey("d_diachi"))
                        newDictionary["d_diachi"] = address;
                    else
                        newDictionary.Add("d_diachi", address);
                    newDictionary["d_huyen"] = huyen;
                    newDictionary["d_tinh"] = tinh;
                    newDictionary["d_thon"] = thon;
                    newDictionary["d_xa"] = xa;

                    isCity = false;
                    isSo = false;
                    Utils.SpilitAddress(item.b_diachithuadat, ref thon, ref xa, ref huyen, ref tinh, ref isCity, ref isSo, ref address);
                    if (newDictionary.ContainsKey("b_diachi"))
                        newDictionary["b_diachi"] = address;
                    else
                        newDictionary.Add("b_diachi", address);
                    newDictionary["b_huyen"] = huyen;
                    newDictionary["b_tinh"] = tinh;
                    newDictionary["b_thon"] = thon;
                    newDictionary["b_xa"] = xa;

                    GenMaSoThue(item.d_masothue, "d", newDictionary);

                    Aspose.Words.Document doc1 = new Aspose.Words.Document(pathTemp);
                    doc1.MailMerge.Execute(newDictionary.Keys.ToArray(), newDictionary.Values.ToArray());

                    doc.AppendDocument(doc1, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
                }
            }
        }
        void GenMaSoThue(string masothue, string pre, Dictionary<string, string> dict)
        {
            char d_mst1 = ' ', d_mst2 = ' ', d_mst3 = ' ', d_mst4 = ' ', d_mst5 = ' ',
                d_mst6 = ' ', d_mst7 = ' ', d_mst8 = ' ', d_mst9 = ' ', d_mst10 = ' ';
            if (!string.IsNullOrEmpty(masothue))
            {
                var chars = masothue.ToCharArray();
                if (chars.Length >= 10)
                {
                    d_mst1 = chars[0];
                    d_mst2 = chars[1];
                    d_mst3 = chars[2];
                    d_mst4 = chars[3];
                    d_mst5 = chars[4];
                    d_mst6 = chars[5];
                    d_mst7 = chars[6];
                    d_mst8 = chars[7];
                    d_mst9 = chars[8];
                    d_mst10 = chars[9];
                }
            }
            if (dict.ContainsKey(pre + "_mst1"))
                dict[pre + "_mst1"] = d_mst1.ToString();
            else
                dict.Add(pre + "_mst1", d_mst1.ToString());

            if (dict.ContainsKey(pre + "_mst2"))
                dict[pre + "_mst2"] = d_mst2.ToString();
            else
                dict.Add(pre + "_mst2", d_mst2.ToString());
            if (dict.ContainsKey(pre + "_mst3"))
                dict[pre + "_mst3"] = d_mst3.ToString();
            else
                dict.Add(pre + "_mst3", d_mst3.ToString());
            if (dict.ContainsKey(pre + "_mst4"))
                dict[pre + "_mst4"] = d_mst4.ToString();
            else
                dict.Add(pre + "_mst4", d_mst4.ToString());
            if (dict.ContainsKey(pre + "_mst5"))
                dict[pre + "_mst5"] = d_mst5.ToString();
            else
                dict.Add(pre + "_mst5", d_mst5.ToString());
            if (dict.ContainsKey(pre + "_mst6"))
                dict[pre + "_mst6"] = d_mst6.ToString();
            else
                dict.Add(pre + "_mst6", d_mst6.ToString());
            if (dict.ContainsKey(pre + "_mst7"))
                dict[pre + "_mst7"] = d_mst7.ToString();
            else
                dict.Add(pre + "_mst7", d_mst7.ToString());
            if (dict.ContainsKey(pre + "_mst8"))
                dict[pre + "_mst8"] = d_mst8.ToString();
            else
                dict.Add(pre + "_mst8", d_mst8.ToString());
            if (dict.ContainsKey(pre + "_mst9"))
                dict[pre + "_mst9"] = d_mst9.ToString();
            else
                dict.Add(pre + "_mst9", d_mst9.ToString());
            if (dict.ContainsKey(pre + "_mst10"))
                dict[pre + "_mst10"] = d_mst10.ToString();
            else
                dict.Add(pre + "_mst10", d_mst10.ToString());
            //dict.Add(pre + "_mst2", d_mst2.ToString());
            //dict.Add(pre + "_mst3", d_mst3.ToString());
            //dict.Add(pre + "_mst4", d_mst4.ToString());
            //dict.Add(pre + "_mst5", d_mst5.ToString());
            //dict.Add(pre + "_mst6", d_mst6.ToString());
            //dict.Add(pre + "_mst7", d_mst7.ToString());
            //dict.Add(pre + "_mst8", d_mst8.ToString());
            //dict.Add(pre + "_mst9", d_mst9.ToString());
            //dict.Add(pre + "_mst10", d_mst10.ToString());
        }
        void GenTKLPTB(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            if (info.b_loainguongoc.Equals(XInfomation.sLoainguongoc[0]))
            {
                dict["b_chuyenquyen"] = "";
            }
            else
                dict["b_chuyenquyen"] += " được";
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        
        void GenTKMS03TNCN(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        
        
        void GenTKTSDD(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenTKTSDDPNN(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        
        void GenTKDKT(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            bool flag = false;
            var docx = doc.Clone();
            doc.RemoveAllChildren();
            dict = AddToDictAuto(info, dict);
            if (string.IsNullOrEmpty(info.a_masothue))
            {
                flag = true;
                var dicta = dict.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
                var doca = docx.Clone();
                dicta.Add("x_hoten", info.a_hoten);
                dicta.Add("x_ngaysinh", dicta["a_ngaysinh"]);
                dicta.Add("x_gioitinh11", dicta["a_gioitinh11"]);
                dicta.Add("x_gioitinh22", dicta["a_gioitinh22"]);

                dicta.Add("x_loaigiayto", dicta["a_loaigiayto"]);

                dicta.Add("x_sogiayto_cm", info.a_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.a_sogiayto);
                dicta.Add("x_ngaycap_gt_cm", info.a_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.a_ngaycap_gt.HasValue ? info.a_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "............");
                dicta.Add("x_noicap_gt_cm", info.a_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.a_noicap_gt);

                dicta.Add("x_sogiayto_cc", !info.a_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.a_sogiayto);
                dicta.Add("x_ngaycap_gt_cc", !info.a_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.a_ngaycap_gt.HasValue ? info.a_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "............");
                dicta.Add("x_noicap_gt_cc", !info.a_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.a_noicap_gt);
                //
                dicta.Add("x_thon", dicta["a_thon"]);
                dicta.Add("x_xa", dicta["a_xa"]);
                dicta.Add("x_huyen", dicta["a_huyen"]);
                dicta.Add("x_tinh", dicta["a_tinh"]);
                doca.MailMerge.Execute(dicta.Keys.ToArray(), dicta.Values.ToArray());
                Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doca);
                builder.MoveToDocumentEnd();
                builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
                doc.AppendDocument(doca, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
            if (string.IsNullOrEmpty(info.d_masothue) && !string.IsNullOrEmpty(info.d_hoten))
            {
                flag = true;
                var docd = docx.Clone();
                var dictd = dict.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
                dictd.Add("x_hoten", info.d_hoten);
                dictd.Add("x_ngaysinh", dictd["d_ngaysinh"]);
                dictd.Add("x_gioitinh11", dictd["d_gioitinh11"]);
                dictd.Add("x_gioitinh22", dictd["d_gioitinh22"]);

                dictd.Add("x_loaigiayto", dictd["d_loaigiayto"]);

                dictd.Add("x_sogiayto_cm", info.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.d_sogiayto);
                dictd.Add("x_ngaycap_gt_cm", info.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.d_ngaycap_gt.HasValue ? info.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "............");
                dictd.Add("x_noicap_gt_cm", info.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.d_noicap_gt);

                dictd.Add("x_sogiayto_cc", !info.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.d_sogiayto);
                dictd.Add("x_ngaycap_gt_cc", !info.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.d_ngaycap_gt.HasValue ? info.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "............");
                dictd.Add("x_noicap_gt_cc", !info.d_loaigiayto.Trim().ToUpper().Equals("CCCD") ? "..........." : info.d_noicap_gt);
                //
                dictd.Add("x_thon", dictd["d_thon"]);
                dictd.Add("x_xa", dictd["d_xa"]);
                dictd.Add("x_huyen", dictd["d_huyen"]);
                dictd.Add("x_tinh", dictd["d_tinh"]);
                docd.MailMerge.Execute(dictd.Keys.ToArray(), dictd.Values.ToArray());
                Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(docd);
                builder.MoveToDocumentEnd();
                builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
                doc.AppendDocument(docd, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
            if (!flag)
            {
                Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
                builder.MoveToDocumentEnd();
                builder.InsertBreak(Aspose.Words.BreakType.PageBreak);
            }
        }
    }
}