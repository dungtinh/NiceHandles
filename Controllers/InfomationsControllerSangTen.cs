using Aspose.Words;
using Aspose.Words.Drawing;
using NiceHandles.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace NiceHandles.Controllers
{
    public partial class InfomationsController
    {
        void GenSangTen_GUQ(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            builder.MoveToMergeField("benA");
            string bena1 = string.Empty;
            string bena2 = string.Empty;
            string nhanxung = string.Empty;
            if (!string.IsNullOrEmpty(info.d_hoten1))
            {
                nhanxung = "chúng tôi";
                bena1 = "Chúng tôi là: " + info.d_hoten.ToUpper() + ", sinh năm " + info.d_ngaysinh.Value.ToString("yyyy") + ", " +
                       info.d_loaigiayto + " số " + info.d_sogiayto + " do " + info.d_noicap_gt + " cấp ngày " + dict["d_ngaycap_gt"];
                bena2 = dict["d_gioitinh"] + " " + info.d_hoten.ToUpper() + ", sinh năm " + info.d_ngaysinh.Value.ToString("yyyy") + ", " +
                       info.d_loaigiayto + " số " + info.d_sogiayto + " do " + info.d_noicap_gt + " cấp ngày " + dict["d_ngaycap_gt"];
                if (!info.d_hktt.Equals(info.d_hktt1))
                {
                    bena1 += ". Đăng ký thường trú tại: " + info.d_hktt + ". Và " + info.d_hoten1.ToUpper() + ", sinh năm " + info.d_ngaysinh1.Value.ToString("yyyy") + ", " +
                       info.d_loaigiayto1 + " số " + info.d_sogiayto1 + " do " + info.d_noicap_gt1 + " cấp ngày " + dict["d_ngaycap_gt1"] + ". Đăng ký thường trú tại: " + info.d_hktt1 + ".";
                    bena2 += ". Đăng ký thường trú tại: " + info.d_hktt + ". Và " + dict["d_gioitinh1"] + " " + info.d_hoten1.ToUpper() + ", sinh năm " + info.d_ngaysinh1.Value.ToString("yyyy") + ", " +
                       info.d_loaigiayto1 + " số " + info.d_sogiayto1 + " do " + info.d_noicap_gt1 + " cấp ngày " + dict["d_ngaycap_gt1"] + ". Đăng ký thường trú tại: " + info.d_hktt1 + ".";
                }
                else
                {
                    bena1 += ". Và " + info.d_hoten1.ToUpper() + ", sinh năm " + info.d_ngaysinh1.Value.ToString("yyyy") + ", " +
                       info.d_loaigiayto1 + " số " + info.d_sogiayto1 + " do " + info.d_noicap_gt1 + " cấp ngày " + dict["d_ngaycap_gt1"] + ". Cùng đăng ký thường trú tại: " + info.d_hktt1 + ".";
                    bena2 += ". Và " + dict["d_gioitinh1"] + " " + info.d_hoten1.ToUpper() + ", sinh năm " + info.d_ngaysinh1.Value.ToString("yyyy") + ", " +
                       info.d_loaigiayto1 + " số " + info.d_sogiayto1 + " do " + info.d_noicap_gt1 + " cấp ngày " + dict["d_ngaycap_gt1"] + ". Cùng đăng ký thường trú tại: " + info.d_hktt1 + ".";
                }
            }
            else
            {
                nhanxung = "tôi";
                bena1 = "Tôi là: " + info.d_hoten.ToUpper() + ", sinh năm " + info.d_ngaysinh.Value.ToString("yyyy") + ", " +
                       info.d_loaigiayto + " số " + info.d_sogiayto + " do " + info.d_noicap_gt + " cấp ngày " + dict["d_ngaycap_gt"] +
                        ". Đăng ký thường trú tại: " + info.d_hktt + ".";
                bena2 = dict["d_gioitinh"] + " " + info.d_hoten.ToUpper() + ", sinh năm " + info.d_ngaysinh.Value.ToString("yyyy") + ", " +
                       info.d_loaigiayto + " số " + info.d_sogiayto + " do " + info.d_noicap_gt + " cấp ngày " + dict["d_ngaycap_gt"] +
                        ". Đăng ký thường trú tại: " + info.d_hktt + ".";
            }
            builder.Write(bena1);
            builder.MoveToMergeField("benA2");
            builder.Write(bena2);
            var hoso = db.HoSoes.Find(info.hoso_id);
            var service = db.Services.Find(hoso.service_id);
            switch (service.id)
            {
                case 18:
                    // Thừa kế
                    break;
                case 17:
                    // Tặng cho

                    break;
                case 2:
                    // Mua bán

                    break;
                default:
                    break;
            }
            dict["nhanxung"] = nhanxung;
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenSangTen_11DK(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            builder.MoveToMergeField("d_benmua");
            var temp = string.Empty;
            if (string.IsNullOrEmpty(info.d_hoten1))
            {
                builder.Write(dict["d_gioitinh"] + " ");
                builder.Bold = true;
                builder.Write(info.d_hoten.ToUpper());
                builder.Bold = false;
                builder.Write("; Năm sinh: " + dict["d_namsinh"] + ";");
                builder.MoveToMergeField("GiayToPhapNhan");
                builder.Write(info.d_loaigiayto + ": ");
                builder.Bold = true;
                builder.Write(info.d_sogiayto);
                builder.Bold = false;
                builder.Write("; Nơi cấp: " + info.d_noicap_gt + "; Ngày cấp: " + dict["d_ngaycap_gt"] + ";");
            }
            else
            {
                builder.Write(dict["d_gioitinh"] + " ");
                builder.Bold = true;
                builder.Write(info.d_hoten.ToUpper());
                builder.Bold = false;
                builder.Write("; Năm sinh: " + dict["d_namsinh"] + " và " + dict["d_gioitinh1"] + " ");
                builder.Bold = true;
                builder.Write(info.d_hoten1.ToUpper());
                builder.Bold = false;
                builder.Write("; Năm sinh: " + dict["d_namsinh1"] + ".");

                builder.MoveToMergeField("GiayToPhapNhan");
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
                builder.Writeln("- " + dict["d_gioitinh"] + " " + info.d_hoten + "; " + info.d_loaigiayto + ": " + info.d_sogiayto +
                    "; Nơi cấp: " + info.d_noicap_gt + "; Ngày cấp: " + dict["d_ngaycap_gt"] + ";");
                builder.Write("- " + dict["d_gioitinh1"] + " " + info.d_hoten1 + "; " + info.d_loaigiayto1 + ": " + info.d_sogiayto1 +
                    "; Nơi cấp: " + info.d_noicap_gt1 + "; Ngày cấp: " + dict["d_ngaycap_gt1"] + ";");
            }

            var hoso = db.HoSoes.Find(info.hoso_id);
            var service = db.Services.Find(hoso.service_id);
            builder.MoveToMergeField("NoiDungBienDong");
            switch (service.id)
            {
                case 18:
                    // Thừa kế
                    temp = dict["d_lydobiendong"] + " theo " + dict["d_loaihopdong"] + " số " + dict["d_sohopdong"] + " của " + dict["d_noicongchung"];
                    break;
                case 17:
                case 2:
                    // Tặng cho                   
                    // Mua bán
                    // builder.MoveToMergeField("NoiDungBienDong");
                    temp = dict["d_lydobiendong"] + " từ " + dict["a_chusohuu"];
                    break;
                default:
                    break;
            }
            builder.Write(temp);
        }
        void GenSangTen_TongHop(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            var dicttemp = dict.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            doc.RemoveAllChildren();
            // I, TỜ KHAI 09            
            var docTK = db.Documents.First(x => x.code.Equals("SangTen_11DK"));
            string pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            var doctemp = new Aspose.Words.Document(pathTemp);
            GenSangTen_11DK(info, doctemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            var pagecount = doc.PageCount;
            if (pagecount % 2 == 1)
            {
                doc.AppendDocument(new Aspose.Words.Document(), Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }

            // TỜ KHAI ĐĂNG KÝ THUẾ            
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                                 entry => entry.Value);
            docTK = db.Documents.First(x => x.code.Equals("SangTen_DKT"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            var isNull = false;
            GenSangTen_DKT(info, doctemp, dicttemp, out isNull);
            if (!isNull)
                doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);

            // TỜ KHAI LỆ PHÍ TRƯỚC BẠ
            //TKLPTB_BD
            docTK = db.Documents.First(x => x.code.Equals("SangTen_LPTB"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                                 entry => entry.Value);
            GenSangTen_LPTB(info, doctemp, dicttemp);

            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            // TỜ KHAI THUẾ THU NHẬP CÁ NHÂN
            docTK = db.Documents.First(x => x.code.Equals("SangTen_TNCN"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                                 entry => entry.Value);
            GenSangTen_TNCN(info, doctemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            // TỜ KHAI THUẾ PHI NÔNG NGHIỆP
            docTK = db.Documents.First(x => x.code.Equals("SangTen_TPNN"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                                 entry => entry.Value);
            GenSangTen_TPNN(info, doctemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            // END            
        }
        void GenSangTen_DKT(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict, out bool isNull)
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
            isNull = !flag;
        }
        void GenSangTen_TPNN(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            if (string.IsNullOrEmpty(info.d_hoten1))
            {
                dict.Add("d_tyle", "100%");
                dict.Add("d_tyle1", "");
            }
            else
            {
                dict.Add("d_tyle", "50%");
                dict.Add("d_tyle1", "50%");
            }
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenSangTen_TNCN(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            dict["d_tienhopdong"] = info.d_tienhopdong.HasValue ? info.d_tienhopdong.Value.ToString("N0") + " đồng." : "";
            var hoso = db.HoSoes.Find(info.hoso_id);
            var service = db.Services.Find(hoso.service_id);
            dict["a_hoten"] = info.a_hoten + (string.IsNullOrEmpty(info.a_hoten1) ? "" : " + " + info.a_hoten1);
            dict["d_hoten"] = info.d_hoten + (string.IsNullOrEmpty(info.d_hoten1) ? "" : " + " + info.d_hoten1);
            switch (service.id)
            {
                case 18:
                    // Thừa kế
                    dict["a_hoten"] = dict["d_hoten"];
                    dict["a_mst1"] = dict["d_mst1"];
                    dict["a_mst2"] = dict["d_mst2"];
                    dict["a_mst3"] = dict["d_mst3"];
                    dict["a_mst4"] = dict["d_mst4"];
                    dict["a_mst5"] = dict["d_mst5"];
                    dict["a_mst6"] = dict["d_mst6"];
                    dict["a_mst7"] = dict["d_mst7"];
                    dict["a_mst8"] = dict["d_mst8"];
                    dict["a_mst9"] = dict["d_mst9"];
                    dict["a_mst10"] = dict["d_mst10"];
                    dict["a_sogiayto"] = dict["d_sogiayto"];

                    dict["a_ngaycap_gt"] = dict["d_ngaycap_gt"];
                    dict["a_noicap_gt"] = dict["d_noicap_gt"];
                    dict["a_diachi"] = dict["d_diachi"];
                    dict["a_huyen"] = dict["d_huyen"];
                    dict["a_tinh"] = dict["d_tinh"];
                    dict["a_sogiayto"] = dict["d_sogiayto"];
                    dict["a_sogiayto"] = dict["d_sogiayto"];

                    dict.Add("d_sohopdong_tc", info.d_sohopdong);
                    dict.Add("d_noicongchung_tc", info.d_noicongchung);
                    dict.Add("d_ngaycongchung_tc", info.d_ngaycongchung.HasValue ? info.d_ngaycongchung.Value.ToString("dd/MM/yyyy") : "..............");
                    dict.Add("d_sohopdong_cn", "..................");
                    dict.Add("d_noicongchung_cn", "..................");
                    dict.Add("d_ngaycongchung_cn", "..................");
                    dict.Add("d_CN", "");
                    dict.Add("d_TC", "X");
                    break;
                case 17:
                    // Tặng cho
                    dict["a_hoten"] = dict["d_hoten"];
                    dict["a_mst1"] = dict["d_mst1"];
                    dict["a_mst2"] = dict["d_mst2"];
                    dict["a_mst3"] = dict["d_mst3"];
                    dict["a_mst4"] = dict["d_mst4"];
                    dict["a_mst5"] = dict["d_mst5"];
                    dict["a_mst6"] = dict["d_mst6"];
                    dict["a_mst7"] = dict["d_mst7"];
                    dict["a_mst8"] = dict["d_mst8"];
                    dict["a_mst9"] = dict["d_mst9"];
                    dict["a_mst10"] = dict["d_mst10"];
                    dict["a_sogiayto"] = dict["d_sogiayto"];

                    dict["a_ngaycap_gt"] = dict["d_ngaycap_gt"];
                    dict["a_noicap_gt"] = dict["d_noicap_gt"];
                    dict["a_diachi"] = dict["d_diachi"];
                    dict["a_huyen"] = dict["d_huyen"];
                    dict["a_tinh"] = dict["d_tinh"];
                    dict["a_sogiayto"] = dict["d_sogiayto"];
                    dict["a_sogiayto"] = dict["d_sogiayto"];

                    dict.Add("d_sohopdong_tc", info.d_sohopdong);
                    dict.Add("d_noicongchung_tc", info.d_noicongchung);
                    dict.Add("d_ngaycongchung_tc", info.d_ngaycongchung.HasValue ? info.d_ngaycongchung.Value.ToString("dd/MM/yyyy") : "..............");
                    dict.Add("d_sohopdong_cn", "..................");
                    dict.Add("d_noicongchung_cn", "..................");
                    dict.Add("d_ngaycongchung_cn", "..................");
                    dict.Add("d_CN", "");
                    dict.Add("d_TC", "X");
                    break;
                case 2:
                    // Mua bán
                    dict.Add("d_sohopdong_cn", info.d_sohopdong);
                    dict.Add("d_noicongchung_cn", info.d_noicongchung);
                    dict.Add("d_ngaycongchung_cn", info.d_ngaycongchung.HasValue ? info.d_ngaycongchung.Value.ToString("dd/MM/yyyy") : "..............");
                    dict.Add("d_sohopdong_tc", "..................");
                    dict.Add("d_noicongchung_tc", "..................");
                    dict.Add("d_ngaycongchung_tc", "..................");
                    dict.Add("d_CN", "X");
                    dict.Add("d_TC", "");
                    break;
                default:
                    dict.Add("d_sohopdong_cn", "..................");
                    dict.Add("d_noicongchung_cn", "..................");
                    dict.Add("d_ngaycongchung_cn", "..................");
                    dict.Add("d_sohopdong_tc", "..................");
                    dict.Add("d_noicongchung_tc", "..................");
                    dict.Add("d_ngaycongchung_tc", "..................");
                    dict.Add("d_CN", "");
                    dict.Add("d_TC", "");
                    break;
            }
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenSangTen_LPTB(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            DocumentBuilder builder = new DocumentBuilder(doc);
            var hoso = db.HoSoes.Find(info.hoso_id);
            var service = db.Services.Find(hoso.service_id);
            dict = AddToDictAuto(info, dict);

            switch (service.id)
            {
                case 18:
                    // Thừa kế
                    builder.MoveToMergeField("checkedtypetk");
                    builder.PushFont();
                    builder.Font.Name = "Wingdings 2";
                    builder.Write("\x0052");
                    builder.PopFont();
                    //
                    builder.MoveToMergeField("checkedtypetc");
                    builder.PushFont();
                    builder.Font.Name = "Wingdings 2";
                    builder.Write("\x00A3");
                    builder.PopFont();

                    builder.MoveToMergeField("checkedtypecn");
                    builder.PushFont();
                    builder.Font.Name = "Wingdings 2";
                    builder.Write("\x00A3");
                    builder.PopFont();
                    break;
                case 17:
                    // Tặng cho
                    builder.MoveToMergeField("checkedtypetc");
                    builder.PushFont();
                    builder.Font.Name = "Wingdings 2";
                    builder.Write("\x0052");
                    builder.PopFont();

                    builder.MoveToMergeField("checkedtypetk");
                    builder.PushFont();
                    builder.Font.Name = "Wingdings 2";
                    builder.Write("\x00A3");
                    builder.PopFont();

                    builder.MoveToMergeField("checkedtypecn");
                    builder.PushFont();
                    builder.Font.Name = "Wingdings 2";
                    builder.Write("\x00A3");
                    builder.PopFont();
                    break;
                case 2:
                    // Mua bán
                    builder.MoveToMergeField("checkedtypecn");
                    builder.PushFont();
                    builder.Font.Name = "Wingdings 2";
                    builder.Write("\x0052");
                    builder.PopFont();

                    builder.MoveToMergeField("checkedtypetk");
                    builder.PushFont();
                    builder.Font.Name = "Wingdings 2";
                    builder.Write("\x00A3");
                    builder.PopFont();

                    builder.MoveToMergeField("checkedtypetc");
                    builder.PushFont();
                    builder.Font.Name = "Wingdings 2";
                    builder.Write("\x00A3");
                    builder.PopFont();
                    if (string.IsNullOrEmpty(info.d_lydomiengiamthue))
                        dict["d_lydomiengiamthue"] = "..................................................................................................................................................";
                    break;
                default: break;
            }
            dict["d_tienhopdong"] = info.d_tienhopdong.HasValue ? info.d_tienhopdong.Value.ToString("N0") + " đồng." : "";
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
    }
}