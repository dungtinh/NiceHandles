using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Tables;
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
        void Gen2024MS11DKThongTin(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            builder.MoveToMergeField("chusudung");
            var temp = Utils.ToTTCN1(info, true, true, false, false, true);
            builder.Write(temp);
            builder.MoveToMergeField("GiayToPhapNhan");
            temp = Utils.ToTTCN1(info, true, false, true, false, true);
            builder.Write(temp);
            builder.MoveToMergeField("NoiDungBienDong");
            temp = dict["d_noidungbiendong"];
            builder.Write(temp);
        }
        void Gen2024MS1DKTACHTHUA(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            var lydotachthua = new List<string>();
            var lstTachThua = db.TachThuas.Where(x => x.infomation_id == info.id);

            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            //
            builder.MoveToMergeField("nguoikekhai");
            var temp = Utils.ToTTCN1(info, true, true, false, false, true);
            builder.Write(temp);
            builder.MoveToMergeField("giaytophapnhan");
            if (string.IsNullOrEmpty(info.a_hoten1)) { temp = Utils.ToTTCN1(info, false, false, true, false, true); }
            else { temp = Utils.ToTTCN1(info, true, false, true, false, true); }
            builder.Write(temp);
            builder.MoveToMergeField("diachi");
            if (info.a_hktt.Equals(info.a_hktt1))
                temp = info.a_hktt;
            else
            {
                if (string.IsNullOrEmpty(info.a_hoten1)) { temp = Utils.ToTTCN1(info, false, false, false, true, true); }
                else { temp = Utils.ToTTCN1(info, true, false, false, true, true); }
            }
            builder.Write(temp);
            //tachthua
            builder.MoveToMergeField("tachthua");
            var b_soluongthua = lstTachThua.Count().ToString();
            int index = 0;
            string[] strIndex = new string[30] {
                    "nhất","hai","ba","bốn","năm", "sáu", "bảy", "tám", "chín", "mười", "mười một", "mười hai", "mười ba", "mười bốn", "mười năm", "mười sáu", "mười bảy", "mười tám", "mười chín"
                    , "hai mươi", "hai mươi mốt", "hai mươi hai", "hai mươi ba", "hai mươi tư", "hai mươi năm", "hai mươi sáu", "hai mươi bảy", "hai mươi tám", "hai mươi chín", "ba mươi"};
            var thuasudungchung = new TachThua();
            var flag = lstTachThua.Count(x => !string.IsNullOrEmpty(x.b_dientich_chung)) > 0;
            foreach (var td in lstTachThua)
            {
                if (string.IsNullOrEmpty(thuasudungchung.b_dientich_chung) && !string.IsNullOrEmpty(td.b_dientich_chung))
                { thuasudungchung = td; }
                temp = "Thửa thứ " + strIndex[index] + ": " + td.b_dientich_rieng + " m²; loại đất: " + td.b_mucdichsudung + "; ";
                builder.Writeln(temp);
                index++;
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
            if (!string.IsNullOrEmpty(thuasudungchung.b_dientich_chung))
                builder.Writeln("Thửa thứ " + strIndex[index] + ": " + thuasudungchung.b_dientich_chung + " m²; loại đất: Đất ở tại đô thị; (Sử dụng chung)");
            dict.Add("b_soluongthua", b_soluongthua);
            dict.Add("lydotach", "Tách thửa để " + string.Join(", ", lydotachthua.Distinct()));
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void Gen2024MS11DKTACHTHUA(Infomation info, Aspose.Words.Document doc, string pathTemp, Dictionary<string, string> dict)
        {
            // Đang thực hiện
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
        void GenBIENDONG2024ThongTin(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            var dicttemp = dict.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            doc.RemoveAllChildren();
            // I, TỜ KHAI 09            
            var docTK = db.Documents.First(x => x.code.Equals("2024MS11DKThongTin"));
            string pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            var doctemp = new Aspose.Words.Document(pathTemp);
            Gen2024MS11DKThongTin(info, doctemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            var pagecount = doc.PageCount;
            if (pagecount % 2 == 1)
            {
                doc.AppendDocument(new Aspose.Words.Document(), Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }

            // TỜ KHAI ĐĂNG KÝ THUẾ
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                                 entry => entry.Value);
            docTK = db.Documents.First(x => x.code.Equals("TKDKT"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            GenTKDKT(info, doctemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            // TỜ KHAI LỆ PHÍ TRƯỚC BẠ
            //TKLPTB_BD
            docTK = db.Documents.First(x => x.code.Equals("TKLPTB"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                                 entry => entry.Value);
            GenTKLPTB(info, doctemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            // TỜ KHAI THUẾ PHI NÔNG NGHIỆP
            docTK = db.Documents.First(x => x.code.Equals("TKTSDDPNN"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                                 entry => entry.Value);
            GenTKMS03TNCN(info, doctemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            // END            
        }


        void Gen2024MS02CDonXinChuyenMDSDD(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            builder.MoveToMergeField("thongtin");
            string buff = String.Empty;
            if (!string.IsNullOrEmpty(info.a_hoten1))
            {
                buff = "- " + dict["a_gioitinh"] + " " + dict["a_hoten"] + " " + info.a_loaigiayto + " số " + info.a_sogiayto + " do " + info.a_noicap_gt + " cấp ngày " + dict["a_ngaycap_gt"] + ";";
                builder.Writeln(buff);

                buff = "- " + dict["a_gioitinh1"] + " " + dict["a_hoten1"] + " " + info.a_loaigiayto1 + " số " + info.a_sogiayto1 + " do " + info.a_noicap_gt1 + " cấp ngày " + dict["a_ngaycap_gt1"] + ".";
                builder.Write(buff);
            }
            else
            {
                buff = "- " + info.a_loaigiayto + " số " + info.a_sogiayto + " do " + info.a_noicap_gt + " cấp ngày " + info.a_ngaycap_gt;
                builder.Write(buff);
            }
            var pagecount = doc.PageCount;
            if (pagecount % 2 == 1)
            {
                doc.AppendDocument(new Aspose.Words.Document(), Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void Gen2024DonCamKetChuyenVuon(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            string buff = String.Empty;
            builder.MoveToMergeField("thongtin");
            var nhanxung = string.Empty;
            string rs2 = string.Empty;
            if (!string.IsNullOrEmpty(info.a_hoten1))
            {
                nhanxung = "chúng tôi";
            }
            else
            {
                nhanxung = "tôi";
            }
            builder.Writeln(Utils.ToTTCN2(info, dict, ref rs2));
            builder.Write(rs2);
            dict.Add("nhanxung", nhanxung);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
            var pagecount = doc.PageCount;
            if (pagecount % 2 == 1)
            {
                doc.AppendDocument(new Aspose.Words.Document(), Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
        }
    }
}