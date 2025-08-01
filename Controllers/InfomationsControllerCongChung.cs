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
        void GenGiayUyQuyenVPCC(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            Aspose.Words.Document docbase = doc.Clone();
            Aspose.Words.Document docx = doc.Clone();
            doc.RemoveAllChildren();
            dict = AddToDictAuto(info, dict);
            //«A_gioitinh» «A_HOTEN», sinh năm «a_namsinh», «a_loaigiayto» số: «a_sogiayto» do «a_noicap_gt» cấp ngày «a_ngaycap_gt».
            //Lấy thông tin a;
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(docx);
            //var dict1 = dict.ToDictionary(entry => entry.Key,
            //                                   entry => entry.Value);            
            builder.MoveToMergeField("benA");
            var nguoiuyquyen = string.Empty;
            string giaytodacap = string.Empty;
            var benA = string.Empty;
            var chong = string.Empty;
            var vo = string.Empty;
            string hktt = string.Empty;
            string nhanxung = string.Empty;

            chong = dict["a_gioitinh"] + " " + info.a_hoten.ToUpper() + ", sinh ngày " + dict["a_ngaysinh"] + ", " +
                        info.a_loaigiayto + " số: " + info.a_sogiayto + " do " + info.a_noicap_gt + " cấp ngày " + dict["a_ngaycap_gt"];
            if (!string.IsNullOrEmpty(info.a_hoten1))
            {
                vo = dict["a_gioitinh1"] + " " + info.a_hoten1.ToUpper() + ", sinh ngày " + dict["a_ngaysinh1"] + ", " +
                            info.a_loaigiayto1 + " số: " + info.a_sogiayto1 + " do " + info.a_noicap_gt1 + " cấp ngày " + dict["a_ngaycap_gt1"];
                benA = chong + " và vợ là " + vo + ", cùng đăng ký thường trú tại: " + info.a_hktt + ".";
                nguoiuyquyen = dict["a_gioitinh"] + " " + info.a_hoten + " và " + dict["a_gioitinh1"] + " " + info.a_hoten1;
                hktt = info.a_hktt + ", chúng tôi gồm:";
                nhanxung = "chúng tôi";
            }
            else
            {
                vo = string.Empty;
                benA = chong + ", đăng ký thường trú tại: " + info.a_hktt + ".";
                nguoiuyquyen = dict["a_gioitinh"] + " " + info.a_hoten;
                hktt = info.a_hktt + ", tôi là:";
                nhanxung = "tôi";
            }
            builder.Write(benA);

            if (string.IsNullOrEmpty(info.b_sogiaychungnhan))
            {
                giaytodacap = "theo “Mảnh trích đo địa chính” số: ........... do Văn phòng đăng ký đất đai thành phố Hải Phòng, Chi nhánh huyện An Dương lập ngày ............";
            }
            else
            {
                giaytodacap = " đã được cấp giấy chứng nhận số: " + info.b_sogiaychungnhan + "; số vào sổ: " + info.b_sovaoso + "; nơi cấp: " + info.b_noicap + "; ngày cấp: " + dict["b_ngaycap"] + ".";
            }

            dict.Add("hktt", hktt);
            dict.Add("chong", chong);
            dict.Add("vo", vo);
            dict.Add("nguoiuyquyen", nguoiuyquyen);
            dict.Add("giaytodacap", giaytodacap);
            dict.Add("nhanxung", nhanxung);

            docx.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
            doc.AppendDocument(docx, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            docx = docbase.Clone();

            // Trường hợp D
            if (!string.IsNullOrEmpty(info.d_hoten))
            {
                builder = new Aspose.Words.DocumentBuilder(docx);
                builder.MoveToMergeField("benA");
                chong = dict["d_gioitinh"] + " " + info.d_hoten.ToUpper() + ", sinh ngày " + dict["d_ngaysinh"] + ", " +
                       info.d_loaigiayto + " số: " + info.d_sogiayto + " do " + info.d_noicap_gt + " cấp ngày " + dict["d_ngaycap_gt"];
                if (!string.IsNullOrEmpty(info.d_hoten1))
                {
                    vo = dict["d_gioitinh1"] + " " + info.d_hoten1.ToUpper() + ", sinh ngày " + dict["d_ngaysinh1"] + ", " +
                                info.d_loaigiayto1 + " số: " + info.d_sogiayto1 + " do " + info.d_noicap_gt1 + " cấp ngày " + dict["d_ngaycap_gt1"];
                    benA = chong + " và vợ là " + vo + ", cùng đăng ký thường trú tại: " + info.d_hktt + ".";
                    nguoiuyquyen = dict["d_gioitinh"] + " " + info.d_hoten + " và " + dict["d_gioitinh1"] + " " + info.d_hoten1;
                    nhanxung = "chúng tôi";
                    hktt = info.d_hktt + ", chúng tôi gồm:";
                }
                else
                {
                    vo = string.Empty;
                    benA = chong + ", đăng ký thường trú tại: " + info.d_hktt + ".";
                    nguoiuyquyen = dict["d_gioitinh"] + " " + info.d_hoten;
                    nhanxung = "tôi";
                    hktt = info.d_hktt + ", tôi là:";
                }
                builder.Write(benA);

                if (string.IsNullOrEmpty(info.b_sogiaychungnhan))
                {
                    giaytodacap = "theo “Mảnh trích đo địa chính” số: ........... do Văn phòng đăng ký đất đai thành phố Hải Phòng, Chi nhánh huyện An Dương lập ngày ............";
                }
                else
                {
                    giaytodacap = " đã được cấp giấy chứng nhận số: " + info.b_sogiaychungnhan + "; số vào sổ: " + info.b_sovaoso + "; nơi cấp: " + info.b_noicap + "; ngày cấp: " + dict["b_ngaycap"] + ".";
                }

                dict["hktt"] = hktt;
                dict["chong"] = chong;
                dict["vo"] = vo;
                dict["nguoiuyquyen"] = nguoiuyquyen;
                dict["giaytodacap"] = giaytodacap;
                dict["nhanxung"] = nhanxung;
                docx.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
                doc.AppendDocument(docx, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
            // Trường hợp tách thửa
            var lstThuaDat = db.TachThuas.Where(x => x.infomation_id == info.id);
            foreach (var item in lstThuaDat)
            {
                if (!string.IsNullOrEmpty(item.d_hoten) || !string.IsNullOrEmpty(item.d_hoten1))
                {
                    var dictx = dict.ToDictionary(entry => entry.Key,
                                              entry => entry.Value);
                    foreach (var per in typeof(TachThua).GetProperties())
                    {
                        var name = "X" + per.Name;
                        var value = per.GetValue(item);
                        string val = value == null ? "......................." : !string.IsNullOrEmpty(value.ToString()) ? value.ToString() : ".......................";
                        if (dictx.ContainsKey(name))
                            dictx[name] = val;
                        else
                            dictx.Add(name, val);
                    }

                    dictx["Xd_ngaysinh"] = item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    dictx["Xd_namsinh"] = item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("yyyy") : ".....";
                    dictx["Xd_gioitinh"] = !string.IsNullOrEmpty(item.d_gioitinh) ? item.d_gioitinh.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";
                    dictx["Xd_ngaycap_gt"] = item.d_ngaycap_gt.HasValue ? item.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    dictx["Xd_ngaysinh1"] = item.d_ngaysinh1.HasValue ? item.d_ngaysinh1.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    dictx["Xd_namsinh1"] = item.d_ngaysinh1.HasValue ? item.d_ngaysinh1.Value.ToString("yyyy") : ".....";
                    dictx["Xd_gioitinh1"] = !string.IsNullOrEmpty(item.d_gioitinh1) ? item.d_gioitinh1.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";
                    dictx["Xd_ngaycap_gt1"] = item.d_ngaycap_gt1.HasValue ? item.d_ngaycap_gt1.Value.ToString("dd/MM/yyyy") : "...../...../......";

                    dictx["b_sothua"] = item.b_sothua;
                    dictx["b_tobando"] = item.b_tobando;
                    dictx["b_dientich"] = item.b_dientich;

                    docx = docbase.Clone();
                    builder = new Aspose.Words.DocumentBuilder(docx);
                    builder.MoveToMergeField("benA");
                    chong = dictx["Xd_gioitinh"] + " " + dictx["Xd_hoten"].ToUpper() + ", sinh ngày " + dictx["Xd_ngaysinh"] + ", " +
                           item.d_loaigiayto + " số: " + item.d_sogiayto + " do " + item.d_noicap_gt + " cấp ngày " + dictx["Xd_ngaycap_gt"];
                    if (!string.IsNullOrEmpty(item.d_hoten1))
                    {
                        vo = dictx["Xd_gioitinh1"] + " " + item.d_hoten1.ToUpper() + ", sinh ngày " + dictx["Xd_ngaysinh1"] + ", " +
                                    item.d_loaigiayto1 + " số: " + item.d_sogiayto1 + " do " + item.d_noicap_gt1 + " cấp ngày " + dictx["Xd_ngaycap_gt1"];
                        benA = chong + " và vợ là " + vo + ", cùng đăng ký thường trú tại: " + item.d_hktt + ".";
                        nguoiuyquyen = dictx["Xd_gioitinh"] + " " + item.d_hoten + " và " + dictx["Xd_gioitinh1"] + " " + item.d_hoten1;
                        nhanxung = "chúng tôi";
                        hktt = item.d_hktt + ", chúng tôi gồm:";
                    }
                    else
                    {
                        vo = string.Empty;
                        benA = chong + ", đăng ký thường trú tại: " + item.d_hktt + ".";
                        nguoiuyquyen = dictx["Xd_gioitinh"] + " " + item.d_hoten;
                        nhanxung = "tôi";
                        hktt = item.d_hktt + ", tôi là:";
                    }
                    builder.Write(benA);
                    giaytodacap = "theo “Mảnh trích đo địa chính” số: ........... do Văn phòng đăng ký đất đai thành phố Hải Phòng, Chi nhánh huyện An Dương lập ngày ............";
                    dictx["hktt"] = hktt;
                    dictx["chong"] = chong;
                    dictx["vo"] = vo;
                    dictx["nguoiuyquyen"] = nguoiuyquyen;
                    dictx["giaytodacap"] = giaytodacap;
                    dictx["nhanxung"] = nhanxung;



                    docx.MailMerge.Execute(dictx.Keys.ToArray(), dictx.Values.ToArray());
                    doc.AppendDocument(docx, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
                }
            }

        }

        void GenHopDongChuyenNhuongMotPhanThuaDat(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            Aspose.Words.Document docbase = doc.Clone();
            Aspose.Words.Document docx = doc.Clone();
            doc.RemoveAllChildren();
            dict = AddToDictAuto(info, dict);
            //Lấy thông tin a;

            var nguoichuyennhuong = string.Empty;
            var benA = string.Empty;
            var benB = string.Empty;
            var chong = string.Empty;
            var vo = string.Empty;

            chong = dict["a_gioitinh"] + " " + info.a_hoten.ToUpper() + ", sinh ngày " + dict["a_ngaysinh"] + ", " +
                        info.a_loaigiayto + " số: " + info.a_sogiayto + " do " + info.a_noicap_gt + " cấp ngày " + dict["a_ngaycap_gt"];
            nguoichuyennhuong = dict["a_gioitinh"] + " " + info.a_hoten;
            if (!string.IsNullOrEmpty(info.a_hoten1))
            {
                vo = dict["a_gioitinh1"] + " " + info.a_hoten1.ToUpper() + ", sinh ngày " + dict["a_ngaysinh1"] + ", " +
                            info.a_loaigiayto1 + " số: " + info.a_sogiayto1 + " do " + info.a_noicap_gt1 + " cấp ngày " + dict["a_ngaycap_gt1"];
                benA = chong + " và vợ là " + vo + ", cùng đăng ký thường trú tại: " + info.a_hktt + ".";
                nguoichuyennhuong += " và " + dict["a_gioitinh1"] + " " + info.a_hoten1;
            }
            else
            {
                vo = string.Empty;
                benA = chong + ", đăng ký thường trú tại: " + info.a_hktt + ".";
            }
            dict.Add("nguoichuyennhuong", nguoichuyennhuong);
            // Trường hợp tách thửa
            var lstThuaDat = db.TachThuas.Where(x => x.infomation_id == info.id);
            string uiSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            foreach (var item in lstThuaDat)
            {
                if (!string.IsNullOrEmpty(item.d_hoten) || !string.IsNullOrEmpty(item.d_hoten1))
                {
                    var dictx = dict.ToDictionary(entry => entry.Key,
                                              entry => entry.Value);
                    foreach (var per in typeof(TachThua).GetProperties())
                    {
                        var name = "X" + per.Name;
                        var value = per.GetValue(item);
                        string val = value == null ? "......................." : !string.IsNullOrEmpty(value.ToString()) ? value.ToString() : ".......................";
                        if (dictx.ContainsKey(name))
                            dictx[name] = val;
                        else
                            dictx.Add(name, val);
                    }
                    dictx["Xd_ngaysinh"] = item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    dictx["Xd_namsinh"] = item.d_ngaysinh.HasValue ? item.d_ngaysinh.Value.ToString("yyyy") : ".....";
                    dictx["Xd_gioitinh"] = !string.IsNullOrEmpty(item.d_gioitinh) ? item.d_gioitinh.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";
                    dictx["Xd_ngaycap_gt"] = item.d_ngaycap_gt.HasValue ? item.d_ngaycap_gt.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    dictx["Xd_ngaysinh1"] = item.d_ngaysinh1.HasValue ? item.d_ngaysinh1.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    dictx["Xd_namsinh1"] = item.d_ngaysinh1.HasValue ? item.d_ngaysinh1.Value.ToString("yyyy") : ".....";
                    dictx["Xd_gioitinh1"] = !string.IsNullOrEmpty(item.d_gioitinh1) ? item.d_gioitinh1.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";
                    dictx["Xd_ngaycap_gt1"] = item.d_ngaycap_gt1.HasValue ? item.d_ngaycap_gt1.Value.ToString("dd/MM/yyyy") : "...../...../......";
                    dictx["Xb_ngaycap"] = item.b_ngaycap.HasValue ? item.b_ngaycap.Value.ToString("dd/MM/yyyy") : "...../...../......";

                    var temp = info.b_dientich.Replace(".", "").Replace(",", uiSep);
                    double area = Convert.ToDouble(temp);
                    dictx.Add("b_dientichbangchu", Utils.DecimalToText(area) + " mét vuông");
                    temp = item.b_dientich.Replace(".", "").Replace(",", uiSep);
                    area = Convert.ToDouble(temp);
                    dictx.Add("Xb_dientichbangchu", Utils.DecimalToText(area) + " mét vuông");

                    docx = docbase.Clone();
                    Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(docx);
                    builder.MoveToMergeField("benA");
                    builder.Write(benA);
                    string chusohuu = string.Empty;
                    builder.MoveToMergeField("benB");
                    chusohuu = dictx["Xd_gioitinh"] + " " + dictx["Xd_hoten"];
                    chong = dictx["Xd_gioitinh"] + " " + dictx["Xd_hoten"].ToUpper() + ", sinh ngày " + dictx["Xd_ngaysinh"] + ", " +
                           item.d_loaigiayto + " số: " + item.d_sogiayto + " do " + item.d_noicap_gt + " cấp ngày " + dictx["Xd_ngaycap_gt"];
                    if (!string.IsNullOrEmpty(item.d_hoten1))
                    {
                        vo = dictx["Xd_gioitinh1"] + " " + item.d_hoten1.ToUpper() + ", sinh ngày " + dictx["Xd_ngaysinh1"] + ", " +
                                    item.d_loaigiayto1 + " số: " + item.d_sogiayto1 + " do " + item.d_noicap_gt1 + " cấp ngày " + dictx["Xd_ngaycap_gt1"];
                        benB = chong + " và vợ là " + vo + ", cùng đăng ký thường trú tại: " + item.d_hktt + ".";
                        chusohuu += " và " + dictx["Xd_gioitinh1"] + " " + item.d_hoten1;
                    }
                    else
                    {
                        vo = string.Empty;
                        benB = chong + ", đăng ký thường trú tại: " + item.d_hktt + ".";
                    }
                    builder.Write(benB);
                    dictx.Add("d_chusohuu", chusohuu);
                    docx.MailMerge.Execute(dictx.Keys.ToArray(), dictx.Values.ToArray());
                    doc.AppendDocument(docx, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
                }
            }
        }
        void GenM37HDCN_UBND(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            //Lấy thông tin a;
            var d_chusohuu = string.Empty;
            var temp = string.Empty;
            var b_dientichchu = string.Empty;
            var d_tienhopdongchu = string.Empty;

            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            builder.MoveToMergeField("BenA");

            temp = dict["a_gioitinh"] + " " + info.a_hoten.ToUpper() + ", sinh ngày " + dict["a_ngaysinh"] + ", " +
                        info.a_loaigiayto + " số: " + info.a_sogiayto + " do " + info.a_noicap_gt + " cấp ngày " + dict["a_ngaycap_gt"];
            builder.Write(temp);
            builder.InsertParagraph();
            if (!string.IsNullOrEmpty(info.a_hoten1))
            {
                temp = "Vợ là " + dict["a_gioitinh1"] + " " + info.a_hoten1.ToUpper() + ", sinh ngày " + dict["a_ngaysinh1"] + ", " +
                            info.a_loaigiayto1 + " số: " + info.a_sogiayto1 + " do " + info.a_noicap_gt1 + " cấp ngày " + dict["a_ngaycap_gt1"];
                builder.Write(temp);
                builder.InsertParagraph();
                temp = "Cùng đăng ký thường trú tại: " + info.a_hktt + ".";
                builder.Write(temp);
            }
            else
            {
                temp = "Đăng ký thường trú tại: " + info.a_hktt + ".";
                builder.Write(temp);
            }

            builder.MoveToMergeField("BenB");

            temp = dict["d_gioitinh"] + " " + info.d_hoten.ToUpper() + ", sinh ngày " + dict["d_ngaysinh"] + ", " +
                        info.d_loaigiayto + " số: " + info.d_sogiayto + " do " + info.d_noicap_gt + " cấp ngày " + dict["d_ngaycap_gt"];
            builder.Write(temp);
            builder.InsertParagraph();
            if (!string.IsNullOrEmpty(info.d_hoten1))
            {
                temp = "Vợ là " + dict["d_gioitinh1"] + " " + info.d_hoten1.ToUpper() + ", sinh ngày " + dict["d_ngaysinh1"] + ", " +
                            info.d_loaigiayto1 + " số: " + info.d_sogiayto1 + " do " + info.d_noicap_gt1 + " cấp ngày " + dict["d_ngaycap_gt1"];
                builder.Write(temp);
                builder.InsertParagraph();
                temp = "Cùng đăng ký thường trú tại: " + info.d_hktt + ".";
                builder.Write(temp);
                d_chusohuu = dict["d_gioitinh"] + " " + info.d_hoten + " và " + dict["d_gioitinh1"] + " " + info.d_hoten1;
            }
            else
            {
                temp = "Đăng ký thường trú tại: " + info.d_hktt + ".";
                builder.Write(temp);
                d_chusohuu = dict["d_gioitinh"] + " " + info.d_hoten;
            }

            builder.MoveToMergeField("BenA1");

            temp = dict["a_gioitinh"] + " " + info.a_hoten.ToUpper() + ", sinh ngày " + dict["a_ngaysinh"] + ", " +
                        info.a_loaigiayto + " số: " + info.a_sogiayto + " do " + info.a_noicap_gt + " cấp ngày " + dict["a_ngaycap_gt"];
            builder.Write(temp);
            if (!string.IsNullOrEmpty(info.a_hoten1))
            {
                builder.InsertParagraph();
                temp = "Vợ là " + dict["a_gioitinh1"] + " " + info.a_hoten1.ToUpper() + ", sinh ngày " + dict["a_ngaysinh1"] + ", " +
                            info.a_loaigiayto1 + " số: " + info.a_sogiayto1 + " do " + info.a_noicap_gt1 + " cấp ngày " + dict["a_ngaycap_gt1"];
                builder.Write(temp);
            }
            builder.MoveToMergeField("BenB1");
            temp = dict["d_gioitinh"] + " " + info.d_hoten.ToUpper() + ", sinh ngày " + dict["d_ngaysinh"] + ", " +
                        info.d_loaigiayto + " số: " + info.d_sogiayto + " do " + info.d_noicap_gt + " cấp ngày " + dict["d_ngaycap_gt"];
            builder.Write(temp);
            if (!string.IsNullOrEmpty(info.d_hoten1))
            {
                builder.InsertParagraph();
                temp = "Vợ là " + dict["d_gioitinh1"] + " " + info.d_hoten1.ToUpper() + ", sinh ngày " + dict["d_ngaysinh1"] + ", " +
                            info.d_loaigiayto1 + " số: " + info.d_sogiayto1 + " do " + info.d_noicap_gt1 + " cấp ngày " + dict["d_ngaycap_gt1"];
                builder.Write(temp);
            }

            string uiSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            temp = info.b_dientich.Replace(".", "").Replace(",", uiSep);
            double area = Convert.ToDouble(temp);
            dict.Add("b_dientichchu", Utils.DecimalToText(area) + " mét vuông");

            area = Convert.ToDouble(info.d_tienhopdong.HasValue ? info.d_tienhopdong.Value : 0);
            dict.Add("d_tienhopdongchu", Utils.DecimalToText(area));

            dict.Add("d_chusohuu", d_chusohuu);

            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenCT07(Infomation info, Aspose.Words.Document doc, string pathTemp, Dictionary<string, string> dict)
        {
            doc.RemoveAllChildren();
            dict = AddToDictAuto(info, dict);
            var soho = db.SoHoKhaus.Where(x => x.infomation_id == info.id).ToList();
            foreach (var item in soho)
            {
                var newDictionary = dict.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
                Aspose.Words.Document doc1 = new Aspose.Words.Document(pathTemp);
                Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc1);

                var nhankhau = db.ThongTinCaNhans.Where(x => x.sohokhau_id == item.id && x.infomation_id == info.id).ToList();
                var chuho = nhankhau.FirstOrDefault();
                nhankhau.Remove(chuho);

                newDictionary.Add("x_hoten", chuho.hoten);
                newDictionary.Add("x_namsinh", chuho.ngaysinh.HasValue ? chuho.ngaysinh.Value.ToString("dd/MM/yyyy") : "................");
                newDictionary.Add("x_gioitinh", chuho.gioitinh);
                if (!string.IsNullOrEmpty(chuho.sogiayto) && chuho.sogiayto.Trim().Length == 12)
                {
                    newDictionary.Add("x_sdd1", chuho.sogiayto.Trim().Substring(0, 1));
                    newDictionary.Add("x_sdd2", chuho.sogiayto.Trim().Substring(1, 1));
                    newDictionary.Add("x_sdd3", chuho.sogiayto.Trim().Substring(2, 1));
                    newDictionary.Add("x_sdd4", chuho.sogiayto.Trim().Substring(3, 1));
                    newDictionary.Add("x_sdd5", chuho.sogiayto.Trim().Substring(4, 1));
                    newDictionary.Add("x_sdd6", chuho.sogiayto.Trim().Substring(5, 1));
                    newDictionary.Add("x_sdd7", chuho.sogiayto.Trim().Substring(6, 1));
                    newDictionary.Add("x_sdd8", chuho.sogiayto.Trim().Substring(7, 1));
                    newDictionary.Add("x_sdd9", chuho.sogiayto.Trim().Substring(8, 1));
                    newDictionary.Add("x_sdd10", chuho.sogiayto.Trim().Substring(9, 1));
                    newDictionary.Add("x_sdd11", chuho.sogiayto.Trim().Substring(10, 1));
                    newDictionary.Add("x_sdd12", chuho.sogiayto.Trim().Substring(11, 1));
                }
                else
                {
                    newDictionary.Add("x_sdd1", null);
                    newDictionary.Add("x_sdd2", null);
                    newDictionary.Add("x_sdd3", null);
                    newDictionary.Add("x_sdd4", null);
                    newDictionary.Add("x_sdd5", null);
                    newDictionary.Add("x_sdd6", null);
                    newDictionary.Add("x_sdd7", null);
                    newDictionary.Add("x_sdd8", null);
                    newDictionary.Add("x_sdd9", null);
                    newDictionary.Add("x_sdd10", null);
                    newDictionary.Add("x_sdd11", null);
                    newDictionary.Add("x_sdd12", null);
                }

                builder.MoveToMergeField("NhanKhau");

                Aspose.Words.Tables.Table table = builder.StartTable();
                builder.InsertCell();
                builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                builder.ParagraphFormat.Alignment = Aspose.Words.ParagraphAlignment.Center;
                builder.Write("TT");
                builder.InsertCell();
                builder.Write("Họ, chữ đệm và tên");
                builder.InsertCell();
                builder.Write("Ngày, tháng, năm sinh");
                builder.InsertCell();
                builder.Write("Giới tính");
                builder.InsertCell();
                builder.Write("Số định danh cá nhân/CMND");
                builder.InsertCell();
                builder.Write("Quan hệ với  chủ hộ");
                builder.EndRow();

                foreach (var nk in nhankhau)
                {
                    builder.InsertCell();
                    builder.Write((nhankhau.IndexOf(nk) + 1).ToString());
                    builder.InsertCell();
                    builder.Write(string.IsNullOrEmpty(nk.hoten) ? "" : nk.hoten);
                    builder.InsertCell();
                    builder.Write(nk.ngaysinh.HasValue ? nk.ngaysinh.Value.ToString("dd/MM/yyyy") : "");
                    builder.InsertCell();
                    builder.Write(string.IsNullOrEmpty(nk.gioitinh) ? "" : nk.gioitinh);
                    builder.InsertCell();
                    builder.Write(string.IsNullOrEmpty(nk.sogiayto) ? "" : nk.sogiayto);
                    builder.InsertCell();
                    builder.Write(string.IsNullOrEmpty(nk.ghichuquanhe) ? "" : nk.ghichuquanhe);
                    builder.EndRow();
                }
                if (nhankhau.Count == 0)
                {
                    builder.InsertCell();
                    builder.InsertCell();
                    builder.InsertCell();
                    builder.InsertCell();
                    builder.InsertCell();
                    builder.InsertCell();
                    builder.EndRow();
                }
                doc1.MailMerge.Execute(newDictionary.Keys.ToArray(), newDictionary.Values.ToArray());
                doc.AppendDocument(doc1, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
        }
        void GenVBTTPCDS(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);

            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

            Aspose.Words.Style style = doc.Styles.Add(Aspose.Words.StyleType.Paragraph, "MyStyle1");
            style.Font.Size = 13;
            style.Font.Name = "Times New Roman";
            // Style.ParagraphFormat.SpaceAfter = 12;            
            builder.ParagraphFormat.Style = style;
            builder.ParagraphFormat.Style = doc.Styles["Normal"];

            builder.MoveToMergeField("ThanhPhan");
            var nguoihuongthuakes = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake.HasValue).ToList();
            var hangthuake1 = nguoihuongthuakes.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1);
            var caccon = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.Con);
            var bochong = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeChong && x.gioitinh.ToUpper().Trim().Equals("NAM")).SingleOrDefault();
            var mechong = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeChong && x.gioitinh.ToUpper().Trim().Equals("NỮ")).SingleOrDefault();
            var bovo = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeVo && x.gioitinh.ToUpper().Trim().Equals("NAM")).SingleOrDefault();
            var mevo = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeVo && x.gioitinh.ToUpper().Trim().Equals("NỮ")).SingleOrDefault();
            var lstTemp = new List<string>();
            var socon = caccon.Count();


            var thanhphan = nguoihuongthuakes.Where(x => x.hangthuake > 0 && !x.ngaychet.HasValue).OrderBy(x => x.hangthuake).ThenBy(x => x.fk_id);
            string buff = String.Empty;
            foreach (var per in thanhphan)
            {
                builder.Bold = true;
                builder.Write("- " + (per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten);
                builder.Bold = false;
                builder.Write(", sinh năm " +
                    (per.ngaysinh.HasValue ? per.ngaysinh.Value.ToString("yyyy") : ".......") + ", " + per.loaigiayto + " số " + per.sogiayto + " do " + per.noicap_gt +
                    " cấp ngày " + (per.ngaycap_gt.HasValue ? per.ngaycap_gt.Value.ToString("dd/MM/yyyy") : "..............") + ". Đăng ký thường trú tại " +
                    per.hktt + ".");
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            }
            // Lời khai
            builder.MoveToMergeField("LoiKhai");
            var nguoitaolap = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake.HasValue && x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.ChuDat).SingleOrDefault();
            string hotenchung = (nguoitaolap.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + nguoitaolap.hoten +
                (!string.IsNullOrEmpty(nguoitaolap.hoten1) ? " và vợ là bà " + nguoitaolap.hoten1 : null);
            string hoten = (nguoitaolap.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + nguoitaolap.hoten;
            string hoten1 = (nguoitaolap.gioitinh1.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + nguoitaolap.hoten1;

            builder.Write(hotenchung);


            builder.Write(". Trong quá trình chung sống, ông bà sinh được " + (socon < 10 ? "0" + socon : socon.ToString()) + "(" + XModels.SoDem[socon] + ") ");
            builder.Write("người con chung là: ");
            lstTemp = new List<string>();
            foreach (var item in caccon)
            {
                lstTemp.Add((item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten);
            }
            builder.Write(String.Join(", ", lstTemp) + ".");
            builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            builder.Write((nguoitaolap.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + nguoitaolap.hoten);
            builder.Write(" chỉ có một vợ là bà " + nguoitaolap.hoten1 + ". Bà " + nguoitaolap.hoten1 + " chỉ có một chồng là ông " + nguoitaolap.hoten + ". Ngoài ");
            builder.Write((socon < 10 ? "0" + socon : socon.ToString()) + "(" + XModels.SoDem[socon] + ") người con đẻ nói trên, " +
                (nguoitaolap.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + nguoitaolap.hoten + (!string.IsNullOrEmpty(nguoitaolap.hoten1) ? " và bà " + nguoitaolap.hoten1 : null));
            builder.Write(" không có người con đẻ, con riêng, con nuôi nào khác; không có bố nuôi, mẹ nuôi. ");
            builder.Write(hoten + " có bố đẻ là ông " + (bochong != null ? bochong.hoten + (bochong.ngaychet.HasValue ? "(chết năm " + bochong.ngaychet.Value.ToString("yyyy") + ")" : null) : null));
            builder.Write(" và mẹ đẻ là bà " + (mechong != null ? mechong.hoten + (mechong.ngaychet.HasValue ? "(chết năm " + mechong.ngaychet.Value.ToString("yyyy") + ")" : null) : null) + ". ");
            builder.Write(hoten1 + " có bố đẻ là ông " + (bovo != null ? bovo.hoten + (bovo.ngaychet.HasValue ? "(chết năm " + bovo.ngaychet.Value.ToString("yyyy") + ")" : null) : null));
            builder.Write(" và mẹ đẻ là bà " + (mevo != null ? mevo.hoten + (mevo.ngaychet.HasValue ? "(chết năm " + mevo.ngaychet.Value.ToString("yyyy") + ")" : null) : null) + ". ");
            //Người tạo lập
            builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            builder.MoveToMergeField("NguoiTaoLap");

            builder.Write(hoten + ", sinh năm " + (nguoitaolap.ngaysinh.HasValue ? nguoitaolap.ngaysinh.Value.ToString("yyyy") : null) +
                ", đã chết ngày " + (!nguoitaolap.ngaychet.Value.ToString("dd/MM").Equals("01/01") ? nguoitaolap.ngaychet.Value.ToString("dd/MM/yyyy") : nguoitaolap.ngaychet.Value.ToString("yyyy")));
            builder.Write(" theo ");
            builder.Write(nguoitaolap.giaytochet + ".");
            builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            builder.Write(hoten1 + ", sinh năm " + (nguoitaolap.ngaysinh1.HasValue ? nguoitaolap.ngaysinh1.Value.ToString("yyyy") : null) +
                ", đã chết ngày " + (!nguoitaolap.ngaychet1.Value.ToString("dd/MM").Equals("01/01") ? nguoitaolap.ngaychet1.Value.ToString("dd/MM/yyyy") : nguoitaolap.ngaychet1.Value.ToString("yyyy")));
            builder.Write(" theo ");
            builder.Write(nguoitaolap.giaytochet1 + ".");
            //Hàng thừa kế 1            
            builder.MoveToMergeField("hangthuake1");
            var hangthuake1x = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.Con || !x.ngaychet.HasValue);
            lstTemp = new List<string>();
            foreach (var item in hangthuake1x)
            {
                lstTemp.Add((item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                    (item.ngaychet.HasValue ? "(chết năm " + item.ngaychet.Value.ToString("yyyy") + ")" : null));
            }
            builder.Write(String.Join(", ", lstTemp) + ".");
            //Phần di sản
            builder.MoveToMergeField("PhanDiSan");
            var hangthuake1lacondachet = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.Con && x.ngaychet.HasValue);
            foreach (var item in hangthuake1lacondachet)
            {
                var hangthuake2 = nguoihuongthuakes.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe2 && x.fk_id == item.id);
                var con2 = hangthuake2.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.Con);
                var socon2 = con2.Count();
                builder.Bold = true;
                builder.Write("* Phần di sản thừa kế của " + (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten + ":");
                builder.Bold = false;
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
                builder.Write((item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten + ", sinh năm " + (item.ngaysinh.HasValue ? item.ngaysinh.Value.ToString("yyyy") : "......") +
                    ", đã chết ngày " + item.ngaychet.Value.ToString("dd/MM/yyyy") + " theo " + item.giaytochet + ".");
                var vochong = hangthuake2.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.VoChong).FirstOrDefault();
                if (hangthuake2.Count() == 0)
                {
                    builder.Writeln((item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten + " không có người vợ hoặc con đẻ, con nuôi, bố nuôi nào.");
                }
                else
                {
                    builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
                    builder.Write((item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                        " chỉ có một " + (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "vợ" : "chồng") + " là " +
                        (vochong.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + vochong.hoten +
                        "; Trong quá trình chung sống, ông bà sinh được " + (socon2 < 10 ? "0" + socon2 : socon2.ToString()) + "(" + XModels.SoDem[socon2] + ") người con chung là: "
                        );
                    lstTemp = new List<string>();
                    foreach (var per in con2)
                    {
                        lstTemp.Add((per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten +
                            (per.ngaychet.HasValue ? "(chết năm " + per.ngaychet.Value.ToString("yyyy") + ")" : null));
                    }
                    builder.Write(String.Join(", ", lstTemp));
                    builder.Write(". Ngoài " + (socon2 < 10 ? "0" + socon2 : socon2.ToString()) + "(" + XModels.SoDem[socon2] + ")" + " người con đẻ nói trên " +
                        (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                        " không có người con đẻ, con riêng, con nuôi nào khác.");
                }
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
                builder.Write((item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                    " chết " + (nguoitaolap.ngaychet < item.ngaychet ? "sau" : "trước") + " bố đẻ và chết " +
                    (nguoitaolap.ngaychet1 < item.ngaychet ? "sau" : "trước") + " mẹ đẻ(người để lại di sản thừa kế), trước khi chết " +
                    (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                    " không để lại di chúc liên quan đến phần tài sản thừa kế mà mình được hưởng tại thửa đất nêu tại văn bản này. Theo quy định pháp luật thì " +
                    (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "vợ" : "chồng") +
                    " các con đẻ của " + (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                    " gồm: "
                    );
                lstTemp = new List<string>();
                foreach (var per in hangthuake2)
                {
                    lstTemp.Add((per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten +
                        (per.ngaychet.HasValue ? "(chết năm " + per.ngaychet.Value.ToString("yyyy") + ")" : null));
                }
                builder.Write(String.Join(", ", lstTemp));
                builder.Write(" - Là những người có quyền hưởng thừa kế theo luật, thuộc hàng thừa kế thứ nhất vào phần quyền sử dụng đất của " +
                    (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                    " được hưởng nếu còn sống.");
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            }
            builder.MoveToMergeField("ThoaThuan");
            lstTemp = new List<string>();
            foreach (var per in thanhphan)
            {
                lstTemp.Add((per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten);
            }
            builder.Write(String.Join(", ", lstTemp));
            builder.Write(" - mỗi người được hưởng một phần bằng nhau, tương đương với tỉ lệ 1 / " + thanhphan.Count());
            builder.Write(" trong toàn bộ bất động sản là quyền sử dụng đất mà ");
            builder.Write(hotenchung + " để lại.");
            builder.MoveToMergeField("NguoiThuaKe");
            builder.Write(String.Join(", ", lstTemp));

            bool bomeconsong = hangthuake1x.Any(x => x.quanhe != (int)XThongTinCaNhan.eQuanHe.Con);
            lstTemp = new List<string>();
            if (bomeconsong)
            {
                var bome = thanhphan.Where(x => x.quanhe != (int)XThongTinCaNhan.eQuanHe.Con).ToArray();
                foreach (var item in bome)
                {
                    lstTemp.Add(XThongTinCaNhan.sQuanHe[item.quanhe.Value]);
                }
                dict.Add("quanhe", string.Join(", ", lstTemp) + ", con đẻ");
            }
            else
                dict.Add("quanhe", "con đẻ");
            dict.Add("hotenchung", hotenchung);
            dict.Add("hoten", hoten);
            dict.Add("hoten1", hoten1);

            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenVBTTCNDD(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);

            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

            builder.MoveToMergeField("ThanhPhan");
            var nguoihuongthuakes = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake.HasValue).ToList();
            var hangthuake1 = nguoihuongthuakes.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1);
            var caccon = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.Con);
            var bochong = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeChong && x.gioitinh.ToUpper().Trim().Equals("NAM")).SingleOrDefault();
            var mechong = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeChong && x.gioitinh.ToUpper().Trim().Equals("NỮ")).SingleOrDefault();
            var bovo = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeVo && x.gioitinh.ToUpper().Trim().Equals("NAM")).SingleOrDefault();
            var mevo = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeVo && x.gioitinh.ToUpper().Trim().Equals("NỮ")).SingleOrDefault();
            var lstTemp = new List<string>();
            var socon = caccon.Count();


            var thanhphan = nguoihuongthuakes.Where(x => x.hangthuake > 0 && !x.ngaychet.HasValue).OrderBy(x => x.hangthuake).ThenBy(x => x.fk_id).ToList();
            string buff = String.Empty;
            foreach (var per in thanhphan)
            {
                builder.Bold = true;
                builder.Write("- " + (per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten);
                builder.Bold = false;
                builder.Write(", sinh năm " +
                    (per.ngaysinh.HasValue ? per.ngaysinh.Value.ToString("yyyy") : ".......") + ", " + per.loaigiayto + " số " + per.sogiayto + " do " + per.noicap_gt +
                    " cấp ngày " + (per.ngaycap_gt.HasValue ? per.ngaycap_gt.Value.ToString("dd/MM/yyyy") : "..............") + ". Đăng ký thường trú tại " +
                    per.hktt + ".");
                if (thanhphan.IndexOf(per) < thanhphan.Count() - 1)
                    builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            }
            // Lời khai
            builder.MoveToMergeField("LoiKhai");
            var nguoitaolap = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake.HasValue && x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.ChuDat).SingleOrDefault();
            string hotenchung = (nguoitaolap.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + nguoitaolap.hoten +
                (!string.IsNullOrEmpty(nguoitaolap.hoten1) ? " và vợ là bà " + nguoitaolap.hoten1 : null);
            string nguoidelaidisan = String.Empty;
            if (nguoitaolap.ngaychet.HasValue && nguoitaolap.ngaychet1.HasValue)
                nguoidelaidisan = hotenchung;
            else if (nguoitaolap.ngaychet.HasValue)
                nguoidelaidisan = (nguoitaolap.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + nguoitaolap.hoten;
            else
                nguoidelaidisan = (nguoitaolap.gioitinh1.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + nguoitaolap.hoten1;

            string hoten = (nguoitaolap.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + nguoitaolap.hoten;
            string hoten1 = (nguoitaolap.gioitinh1.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + nguoitaolap.hoten1;

            builder.Write(hotenchung);

            builder.Write(". Trong quá trình chung sống, ông bà sinh được " + (socon < 10 ? "0" + socon : socon.ToString()) + " (" + XModels.SoDem[socon] + ") ");
            builder.Write("người con chung là: ");
            lstTemp = new List<string>();
            foreach (var item in caccon)
            {
                lstTemp.Add((item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten);
            }
            builder.Write(String.Join(", ", lstTemp) + ".");
            builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            builder.Write((nguoitaolap.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + nguoitaolap.hoten);
            builder.Write(" chỉ có một vợ là bà " + nguoitaolap.hoten1 + ". Bà " + nguoitaolap.hoten1 + " chỉ có một chồng là ông " + nguoitaolap.hoten + ". Ngoài ");
            builder.Write((socon < 10 ? "0" + socon : socon.ToString()) + " (" + XModels.SoDem[socon] + ") người con đẻ nói trên, " +
                (nguoitaolap.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + nguoitaolap.hoten + (!string.IsNullOrEmpty(nguoitaolap.hoten1) ? " và bà " + nguoitaolap.hoten1 : null));
            builder.Write(" không có người con đẻ, con riêng, con nuôi nào khác; không có bố nuôi, mẹ nuôi. ");

            if (nguoitaolap.ngaychet.HasValue)
            {
                builder.Write(hoten + " có bố đẻ là ông " + (bochong != null ? bochong.hoten + (bochong.ngaychet.HasValue ? "(chết năm " + bochong.ngaychet.Value.ToString("yyyy") + ")" : null) : null));
                builder.Write(" và mẹ đẻ là bà " + (mechong != null ? mechong.hoten + (mechong.ngaychet.HasValue ? "(chết năm " + mechong.ngaychet.Value.ToString("yyyy") + ")" : null) : null) + ". ");
            }
            if (nguoitaolap.ngaychet1.HasValue)
            {
                builder.Write(hoten1 + " có bố đẻ là ông " + (bovo != null ? bovo.hoten + (bovo.ngaychet.HasValue ? "(chết năm " + bovo.ngaychet.Value.ToString("yyyy") + ")" : null) : null));
                builder.Write(" và mẹ đẻ là bà " + (mevo != null ? mevo.hoten + (mevo.ngaychet.HasValue ? "(chết năm " + mevo.ngaychet.Value.ToString("yyyy") + ")" : null) : null) + ". ");
            }
            //Người tạo lập            
            builder.MoveToMergeField("NguoiTaoLap");
            if (nguoitaolap.ngaychet.HasValue)
            {
                builder.Write(hoten + ", sinh năm " + (nguoitaolap.ngaysinh.HasValue ? nguoitaolap.ngaysinh.Value.ToString("yyyy") : null) +
                ", đã chết ngày " + (nguoitaolap.ngaychet.HasValue ? (!nguoitaolap.ngaychet.Value.ToString("dd/MM").Equals("01/01") ? nguoitaolap.ngaychet.Value.ToString("dd/MM/yyyy") : nguoitaolap.ngaychet.Value.ToString("yyyy")) : "..........."));
                builder.Write(" theo ");
                builder.Write(nguoitaolap.giaytochet + ".");
            }
            if (nguoitaolap.ngaychet1.HasValue)
            {
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
                builder.Write(hoten1 + ", sinh năm " + (nguoitaolap.ngaysinh1.HasValue ? nguoitaolap.ngaysinh1.Value.ToString("yyyy") : null) +
                ", đã chết ngày " + (nguoitaolap.ngaychet1.HasValue ? (!nguoitaolap.ngaychet1.Value.ToString("dd/MM").Equals("01/01") ? nguoitaolap.ngaychet1.Value.ToString("dd/MM/yyyy") : nguoitaolap.ngaychet1.Value.ToString("yyyy")) : ".........."));
                builder.Write(" theo ");
                builder.Write(nguoitaolap.giaytochet1 + ".");
            }
            //Phần di sản
            builder.MoveToMergeField("PhanDiSan");
            var hangthuake1lacondachet = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.Con && x.ngaychet.HasValue);
            if (hangthuake1lacondachet.Count() == 0)
                builder.CurrentParagraph.Remove();
            foreach (var item in hangthuake1lacondachet)
            {
                var hangthuake2 = nguoihuongthuakes.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe2 && x.fk_id == item.id);
                var con2 = hangthuake2.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.Con);
                var socon2 = con2.Count();
                builder.Bold = true;
                builder.Write("* Phần di sản thừa kế của " + (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten + ":");
                builder.Bold = false;
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
                builder.Write((item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten + ", sinh năm " + (item.ngaysinh.HasValue ? item.ngaysinh.Value.ToString("yyyy") : "......") +
                    ", đã chết ngày " + item.ngaychet.Value.ToString("dd/MM/yyyy") + " theo " + item.giaytochet + ".");
                var vochong = hangthuake2.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.VoChong).FirstOrDefault();
                if (hangthuake2.Count() == 0)
                {
                    builder.Writeln((item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten + " không có người vợ hoặc con đẻ, con nuôi, bố nuôi nào.");
                }
                else
                {
                    builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
                    if (vochong != null)
                    {
                        builder.Write((item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                            " chỉ có một " + (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "vợ" : "chồng") + " là " +
                            (vochong.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + vochong.hoten +
                            "; Trong quá trình chung sống, ông bà sinh được " + (socon2 < 10 ? "0" + socon2 : socon2.ToString()) + " (" + XModels.SoDem[socon2] + ") người con chung là: "
                            );
                        lstTemp = new List<string>();
                        foreach (var per in con2)
                        {
                            lstTemp.Add((per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten +
                                (per.ngaychet.HasValue ? "(chết năm " + per.ngaychet.Value.ToString("yyyy") + ")" : null));
                        }
                        builder.Write(String.Join(", ", lstTemp));
                        builder.Write(". Ngoài " + (socon2 < 10 ? "0" + socon2 : socon2.ToString()) + " (" + XModels.SoDem[socon2] + ")" + " người con đẻ nói trên " +
                            (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                            " không có người con đẻ, con riêng, con nuôi nào khác.");
                    }
                    else if (con2.Count() > 0)
                    {
                        builder.Write((item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                           " chưa kết hôn nhưng có con là: "
                           );
                        lstTemp = new List<string>();
                        foreach (var per in con2)
                        {
                            lstTemp.Add((per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten +
                                (per.ngaychet.HasValue ? "(chết năm " + per.ngaychet.Value.ToString("yyyy") + ")" : null));
                        }
                        builder.Write(String.Join(", ", lstTemp));
                        builder.Write(". Ngoài " + (socon2 < 10 ? "0" + socon2 : socon2.ToString()) + " (" + XModels.SoDem[socon2] + ")" + " người con đẻ nói trên " +
                            (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                            " không có người con đẻ, con nuôi nào khác.");
                    }
                    else
                    {
                        builder.Write((item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                            " chưa kết hôn và không có người con đẻ, con nuôi nào.");
                    }
                }
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
                builder.Write((item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                    " chết " + (nguoitaolap.ngaychet < item.ngaychet ? "sau" : "trước") + " bố đẻ và chết " +
                    (nguoitaolap.ngaychet1 < item.ngaychet ? "sau" : "trước") + " mẹ đẻ(người để lại di sản thừa kế), trước khi chết " +
                    (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                    " không để lại di chúc liên quan đến phần tài sản thừa kế mà mình được hưởng tại thửa đất nêu tại văn bản này. Theo quy định pháp luật thì " +
                    (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "vợ" : "chồng") +
                    " các con đẻ của " + (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                    " gồm: "
                    );
                lstTemp = new List<string>();
                foreach (var per in hangthuake2)
                {
                    lstTemp.Add((per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten +
                        (per.ngaychet.HasValue ? "(chết năm " + per.ngaychet.Value.ToString("yyyy") + ")" : null));
                }
                builder.Write(String.Join(", ", lstTemp));
                builder.Write(" - Là những người có quyền hưởng thừa kế theo luật, thuộc hàng thừa kế thứ nhất vào phần quyền sử dụng đất của " +
                    (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten +
                    " được hưởng nếu còn sống.");
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            }
            lstTemp = new List<string>();
            foreach (var per in thanhphan)
            {
                lstTemp.Add((per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten);
            }
            builder.MoveToMergeField("NguoiThuaKe");
            builder.Write(String.Join(", ", lstTemp));
            var nguoidaidien = thanhphan.Where(x => x.marker == (int)XModels.eYesNo.Yes).FirstOrDefault();
            dict.Add("NguoiDaiDien", (nguoidaidien.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + nguoidaidien.hoten);
            dict.Add("hotenchung", hotenchung);
            dict.Add("nguoidelaidisan", nguoidelaidisan);
            dict.Add("hoten", hoten);
            dict.Add("hoten1", hoten1);

            dict.Add("songuoiduocthuake", thanhphan.Count() > 10 ? thanhphan.Count().ToString() : "0" + thanhphan.Count());
            dict.Add("sobanin", (thanhphan.Count() + 1) > 10 ? (thanhphan.Count() + 1).ToString() : "0" + (thanhphan.Count() + 1).ToString());


            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenDDNXNNK(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            //var nguoidaidien = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.marker == (int)XModels.eYesNo.Yes).FirstOrDefault();
            //dict.Add("hoten", nguoidaidien.hoten);
            //dict.Add("namsinh", nguoidaidien.ngaysinh.Value.ToString("yyyy"));
            var soho = db.SoHoKhaus.Where(x => x.infomation_id == info.id).ToList();
            dict.Add("soho", (soho.Count < 10 ? "0" + soho.Count : soho.Count.ToString()) + "(" + XModels.SoDem[soho.Count] + ")");
            var songuoi = 0;

            builder.MoveToMergeField("DanhSach");
            Aspose.Words.Tables.Table table = builder.StartTable();
            builder.InsertCell();
            //table.AutoFit(Aspose.Words.Tables.AutoFitBehavior.FixedColumnWidths);
            builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
            builder.ParagraphFormat.Alignment = Aspose.Words.ParagraphAlignment.Center;
            builder.Write("STT");
            builder.InsertCell();
            builder.Write("Họ và tên");
            builder.InsertCell();
            builder.Write("Năm sinh");
            builder.InsertCell();
            builder.Write("Ghi chú");
            builder.EndRow();

            foreach (var item in soho)
            {
                builder.InsertCell();
                builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                builder.ParagraphFormat.Alignment = Aspose.Words.ParagraphAlignment.Center;
                builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.First;
                builder.Write("HỘ THỨ " + (soho.IndexOf(item) + 1) + ": SHK số " + item.so);
                builder.InsertCell();
                builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.Previous;
                builder.InsertCell();
                builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.Previous;
                builder.InsertCell();
                builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.Previous;
                builder.EndRow();

                var nhankhau = db.ThongTinCaNhans.Where(x => x.sohokhau_id == item.id && x.infomation_id == info.id).ToList();
                foreach (var nk in nhankhau)
                {
                    songuoi++;
                    builder.InsertCell();
                    builder.CellFormat.HorizontalMerge = Aspose.Words.Tables.CellMerge.None;
                    builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                    builder.ParagraphFormat.Alignment = Aspose.Words.ParagraphAlignment.Center;
                    builder.Write((nhankhau.IndexOf(nk) + 1).ToString());
                    builder.InsertCell();
                    builder.ParagraphFormat.Alignment = Aspose.Words.ParagraphAlignment.Left;
                    builder.Write(nk.hoten);

                    builder.InsertCell();
                    builder.ParagraphFormat.Alignment = Aspose.Words.ParagraphAlignment.Center;
                    builder.Write(nk.ngaysinh.Value.ToString("yyyy"));

                    builder.InsertCell();
                    builder.ParagraphFormat.Alignment = Aspose.Words.ParagraphAlignment.Left;
                    builder.Write(nk.ghichuquanhe);

                    builder.EndRow();
                }
            }
            table.PreferredWidth = Aspose.Words.Tables.PreferredWidth.FromPercent(100);
            builder.EndTable();

            dict.Add("songuoi", songuoi.ToString());
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenVanBanCamKetTaiSanChung(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            string sArea = ".........";
            if (!string.IsNullOrEmpty(info.b_dientich))
            {
                string uiSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                var temp = info.b_dientich.Replace(".", "").Replace(",", uiSep);
                double area = Convert.ToDouble(temp);
                sArea = Utils.DecimalToText(area);
            }
            dict.Add("b_dientichchu", sArea);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenHopDongUyQuyenChoNguoiDaiDien(Infomation info, Aspose.Words.Document doc, string pathTemp, Dictionary<string, string> dict)
        {
            doc.RemoveAllChildren();
            string sArea = ".........";
            if (!string.IsNullOrEmpty(info.b_dientich))
            {
                string uiSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                var temp = info.b_dientich.Replace(".", "").Replace(",", uiSep);
                double area = Convert.ToDouble(temp);
                sArea = Utils.DecimalToText(area);
            }
            dict.Add("b_dientich_chu", sArea);
            var chudat = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.ChuDat).SingleOrDefault();
            string t_chudat = !string.IsNullOrEmpty(chudat.gioitinh) ? chudat.gioitinh.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";
            t_chudat += " " + (!string.IsNullOrEmpty(chudat.hoten) ? chudat.hoten : "..........................");
            t_chudat += (!string.IsNullOrEmpty(chudat.hoten1) ? " và vợ là bà " + chudat.hoten1 : null);
            dict.Add("t_chudat", t_chudat);
            dict.Add("t_hoten", !string.IsNullOrEmpty(chudat.hoten) ? chudat.hoten : "....................");
            dict.Add("t_hoten1", !string.IsNullOrEmpty(chudat.hoten1) ? chudat.hoten1 : "....................");
            dict = AddToDictAuto(info, dict);

            var hangthuake1 = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && (x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1 || x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe2) && !x.ngaychet.HasValue && (!x.marker.HasValue || x.marker.Value == (int)XModels.eYesNo.No)).ToArray();

            foreach (var item in hangthuake1)
            {
                var newDictionary = dict.ToDictionary(entry => entry.Key, entry => entry.Value);
                foreach (var per in typeof(ThongTinCaNhan).GetProperties())
                {
                    var name = "X" + per.Name;
                    var value = per.GetValue(item);
                    string val = value == null ? "......................." : !string.IsNullOrEmpty(value.ToString()) ? value.ToString() : ".......................";
                    if (newDictionary.ContainsKey(name))
                        newDictionary[name] = val;
                    else
                        newDictionary.Add(name, val);
                }
                newDictionary.Add("Xnamsinh", item.ngaysinh.HasValue ? item.ngaysinh.Value.ToString("yyyy") : "....................");
                newDictionary["Xngaycap_gt"] = item.ngaycap_gt.HasValue ? item.ngaycap_gt.Value.ToString("dd/MM/yyyy") : "....................";
                newDictionary["Xgioitinh"] = !string.IsNullOrEmpty(item.gioitinh) ? item.gioitinh.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà";

                Aspose.Words.Document doc1 = new Aspose.Words.Document(pathTemp);
                doc1.MailMerge.Execute(newDictionary.Keys.ToArray(), newDictionary.Values.ToArray());
                doc.AppendDocument(doc1, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
        }
        void GenVBCamKetKhongTranhChap(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);

            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

            builder.MoveToMergeField("b_chungtoibaogom");

            var nguoihuongthuakes = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake.HasValue).ToList();
            var chudat = nguoihuongthuakes.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.ChuDat).SingleOrDefault();
            var hangthuake1 = nguoihuongthuakes.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1);
            var caccon = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.Con);
            var bochong = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeChong && x.gioitinh.ToUpper().Trim().Equals("NAM")).SingleOrDefault();
            var mechong = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeChong && x.gioitinh.ToUpper().Trim().Equals("NỮ")).SingleOrDefault();
            var bovo = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeVo && x.gioitinh.ToUpper().Trim().Equals("NAM")).SingleOrDefault();
            var mevo = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeVo && x.gioitinh.ToUpper().Trim().Equals("NỮ")).SingleOrDefault();
            var lstTemp = new List<string>();
            var socon = caccon.Count();

            var thanhphan = nguoihuongthuakes.Where(x => !x.ngaychet.HasValue).OrderBy(x => x.hangthuake).ThenBy(x => x.fk_id);
            string buff = String.Empty;
            int index = 0;
            foreach (var per in thanhphan)
            {
                index++;
                builder.Bold = true;
                builder.Write(index.ToString() + ". " + (per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten);
                builder.Bold = false;
                builder.Write(", sinh năm " +
                    (per.ngaysinh.HasValue ? per.ngaysinh.Value.ToString("yyyy") : ".......") + ", " + per.loaigiayto + " số " + per.sogiayto + " do " + per.noicap_gt +
                    " cấp ngày " + (per.ngaycap_gt.HasValue ? per.ngaycap_gt.Value.ToString("dd/MM/yyyy") : "..............") + "; Đăng ký thường trú tại " +
                    per.hktt + ".");
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            }
            string chudatchung = string.Empty, chudat1 = string.Empty, chudat2 = string.Empty;
            chudat1 = (chudat.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + chudat.hoten;
            chudat2 = !string.IsNullOrEmpty(chudat.hoten1) ? (chudat.gioitinh1.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + chudat.hoten1 : null;
            chudatchung = chudat1 + (!string.IsNullOrEmpty(chudat2) ? " và vợ là " + chudat2 : null);
            dict.Add("x_chudat", chudatchung);
            // Lời khai
            builder.MoveToMergeField("x_ttchudat");
            buff = chudat.ngaychet.HasValue ? (chudat1 + " sinh năm " + (chudat.ngaysinh.HasValue ? chudat.ngaysinh.Value.ToString("yyyy") : "......") + (chudat.ngaychet.HasValue ? ", đã chết năm " +
                chudat.ngaychet.Value.ToString("yyyy") + " theo " + chudat.giaytochet : null)) : null;
            buff += chudat.ngaychet1.HasValue ? (!String.IsNullOrEmpty(chudat2) ? "; " + chudat2 + " " + " sinh năm " + (chudat.ngaysinh1.HasValue ? chudat.ngaysinh1.Value.ToString("yyyy") : "......") +
                (chudat.ngaychet1.HasValue ? ", đã chết năm " + chudat.ngaychet1.Value.ToString("yyyy") + " theo " + chudat.giaytochet1 : null) : null) : null;
            builder.Write(buff);
            builder.MoveToMergeField("x_bomechudat");
            buff = chudat.ngaychet.HasValue ? "1.2 Cha đẻ của " + chudat1 + " là cụ ông " + bochong.hoten + ", đã chết năm " +
                (bochong.ngaychet.HasValue ? bochong.ngaychet.Value.ToString("yyyy") : ".....") + ". Mẹ đẻ của " + chudat1 + " là cụ bà " + mechong.hoten + ", đã chết năm " +
                (mechong.ngaychet.HasValue ? mechong.ngaychet.Value.ToString("yyyy") : ".....") + ". " + chudat1 + " không có người cha nuôi, mẹ nuôi nào khác." : null;
            buff += chudat.ngaychet1.HasValue ? (chudat.ngaychet.HasValue ? "1.3" : "1.2") + " Cha đẻ của " + chudat2 + " là cụ ông " + bovo.hoten + ", đã chết năm " +
                (bovo.ngaychet.HasValue ? bovo.ngaychet.Value.ToString("yyyy") : ".....") + ". Mẹ đẻ của " + chudat2 + " là cụ bà" + mevo.hoten + ", đã chết năm " +
                (mevo.ngaychet.HasValue ? mevo.ngaychet.Value.ToString("yyyy") : ".....") + ". " + chudat2 + " không có người cha nuôi, mẹ nuôi nào khác." : null;
            builder.Write(buff);
            builder.MoveToMergeField("x_sinhthoi");
            buff = chudat.ngaychet.HasValue ? (chudat1 + " chỉ có 01(một) người vợ hợp pháp là " + chudat2 + " sinh năm " + (chudat.ngaysinh1.HasValue ? chudat.ngaysinh1.Value.ToString("yyyy") : null)
            + ". Ngoài ra, " + chudat1 + " không có người vợ nào khác; ") : null;
            buff += chudat.ngaychet1.HasValue ? (chudat2 + " chỉ có 01(một) người chồng hợp pháp là " + chudat1 + " sinh năm " + (chudat.ngaysinh.HasValue ? chudat.ngaysinh.Value.ToString("yyyy") : null)
            + ". Ngoài ra, " + chudat2 + " không có người chồng nào khác.") : null;
            builder.Write(buff);
            builder.MoveToMergeField("x_conchudat");
            buff = " sinh được " + (socon > 10 ? socon.ToString() : "0" + socon) + "(" + XModels.SoDem[socon] + ") người con đẻ là: ";
            lstTemp = new List<string>();
            foreach (var item in caccon)
            {
                lstTemp.Add((item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten);
            }
            buff += String.Join(", ", lstTemp);
            buff += ". Ngoài " + (socon > 10 ? socon.ToString() : "0" + socon) + "(" + XModels.SoDem[socon] + ") người con đẻ nói trên,";
            builder.Write(buff);
            var nguoidaidien = nguoihuongthuakes.Where(x => x.marker == (int)XModels.eYesNo.Yes).FirstOrDefault();
            dict.Add("x_daidien", (nguoidaidien.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + nguoidaidien.hoten);

            lstTemp = new List<string>();
            foreach (var item in thanhphan.Where(x => !x.marker.HasValue || x.marker.Value == (int)XModels.eYesNo.No))
            {
                lstTemp.Add((item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten);
            }
            dict.Add("x_conchudat2", String.Join(", ", lstTemp));

            dict.Add("x_soban", XModels.SoDem[thanhphan.Count()]);

            builder.MoveToMergeField("x_thanhphan");
            index = 0;
            foreach (var per in thanhphan)
            {
                index++;
                builder.Bold = true;
                builder.Write(index.ToString() + ". " + (per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten);
                builder.Bold = false;
                builder.Write(", sinh năm " +
                    (per.ngaysinh.HasValue ? per.ngaysinh.Value.ToString("yyyy") : ".......") + ", " + per.loaigiayto + " số " + per.sogiayto + " do " + per.noicap_gt +
                    " cấp ngày " + (per.ngaycap_gt.HasValue ? per.ngaycap_gt.Value.ToString("dd/MM/yyyy") : "..............") + "; Đăng ký thường trú tại " +
                    per.hktt + ".");
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            }
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenBBHopGiaDinh(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);

            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

            builder.MoveToMergeField("ThanhPhan");

            var nguoihuongthuakes = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake.HasValue).ToList();
            var chudat = nguoihuongthuakes.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.ChuDat).SingleOrDefault();
            var hangthuake1 = nguoihuongthuakes.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1);
            var caccon = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.Con);
            var bochong = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeChong && x.gioitinh.ToUpper().Trim().Equals("NAM")).SingleOrDefault();
            var mechong = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeChong && x.gioitinh.ToUpper().Trim().Equals("NỮ")).SingleOrDefault();
            var bovo = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeVo && x.gioitinh.ToUpper().Trim().Equals("NAM")).SingleOrDefault();
            var mevo = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeVo && x.gioitinh.ToUpper().Trim().Equals("NỮ")).SingleOrDefault();
            var lstTemp = new List<string>();
            var socon = caccon.Count();
            var nguoidaidien = nguoihuongthuakes.Where(x => x.marker == (int)XModels.eYesNo.Yes).FirstOrDefault();
            dict.Add("ChuSuDung", (nguoidaidien.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + nguoidaidien.hoten);

            var thanhphan = nguoihuongthuakes.Where(x => !x.ngaychet.HasValue).OrderBy(x => x.hangthuake).ThenBy(x => x.fk_id).ToList();
            string buff = String.Empty;
            int index = 0;
            foreach (var per in thanhphan)
            {
                index++;
                builder.Bold = true;
                builder.Write(index.ToString() + ". " + (per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten);
                builder.Bold = false;
                builder.Write(", sinh năm " +
                    (per.ngaysinh.HasValue ? per.ngaysinh.Value.ToString("yyyy") : ".......") + ", " + per.loaigiayto + " số " + per.sogiayto + " do " + per.noicap_gt +
                    " cấp ngày " + (per.ngaycap_gt.HasValue ? per.ngaycap_gt.Value.ToString("dd/MM/yyyy") : "..............") + "; Đăng ký thường trú tại " +
                    per.hktt + ".");
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            }

            string chudatchung = string.Empty, chudat1 = string.Empty, chudat2 = string.Empty;
            chudat1 = (chudat.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + chudat.hoten;
            chudat2 = !string.IsNullOrEmpty(chudat.hoten1) ? (chudat.gioitinh1.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + chudat.hoten1 : null;
            chudatchung = chudat1 + (!string.IsNullOrEmpty(chudat2) ? " và " + chudat2 : null);
            dict.Add("TenChuNguonGoc", chudatchung);
            // Lời khai
            builder.MoveToMergeField("ChuNguonGoc");
            buff = chudat.ngaychet.HasValue ? (chudat1 + " sinh năm " + (chudat.ngaysinh.HasValue ? chudat.ngaysinh.Value.ToString("yyyy") : "......") + (chudat.ngaychet.HasValue ? ", đã chết năm " +
                chudat.ngaychet.Value.ToString("yyyy") + " theo " + chudat.giaytochet : null)) :
            chudat1 + " sinh năm " + (chudat.ngaysinh.HasValue ? chudat.ngaysinh.Value.ToString("yyyy") : "......" + ", " + chudat.loaigiayto + " số " + chudat.sogiayto + " do " + chudat.noicap_gt +
            " cấp ngày " + (chudat.ngaycap_gt.HasValue ? chudat.ngaycap_gt.Value.ToString("dd/MM/yyyy") : "..............") + "; Đăng ký thường trú tại " +
                    chudat.hktt + ".");
            builder.Write(buff);
            builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            buff = chudat.ngaychet1.HasValue ? (!String.IsNullOrEmpty(chudat2) ? "; " + chudat2 + " " + " sinh năm " + (chudat.ngaysinh1.HasValue ? chudat.ngaysinh1.Value.ToString("yyyy") : "......") +
                (chudat.ngaychet1.HasValue ? ", đã chết năm " + chudat.ngaychet1.Value.ToString("yyyy") + " theo " + chudat.giaytochet1 : null) : null) :
                chudat1 + " sinh năm " + (chudat.ngaysinh.HasValue ? chudat.ngaysinh.Value.ToString("yyyy") : "......" + ", " + chudat.loaigiayto + " số " + chudat.sogiayto + " do " + chudat.noicap_gt +
            " cấp ngày " + (chudat.ngaycap_gt.HasValue ? chudat.ngaycap_gt.Value.ToString("dd/MM/yyyy") : "..............") + "; Đăng ký thường trú tại " +
                    chudat.hktt + ".");
            builder.Write(buff);
            buff = (info.a_gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + info.a_hoten;
            buff += !string.IsNullOrEmpty(info.a_hoten1) ? (info.a_gioitinh1.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + info.a_hoten1 : null;
            dict.Add("TenChuSuDung", buff);
            builder.MoveToMergeField("NguoiTuChoi");
            thanhphan = thanhphan.Where(x => !x.hoten.Equals(info.a_hoten) && !x.hoten.Equals(info.a_hoten1)).ToList();
            int count = thanhphan.Count();
            buff = string.Empty;
            index = 0;
            foreach (var per in thanhphan)
            {
                index++;
                if (thanhphan.IndexOf(per) < count - 1)
                {
                    buff += (per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten + " ";
                }
                else
                {
                    buff += "và " + (per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten;
                }
                builder.Bold = true;
                builder.Write(index.ToString() + ". " + (per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten);
                builder.Bold = false;
                builder.Write(", sinh năm " +
                    (per.ngaysinh.HasValue ? per.ngaysinh.Value.ToString("yyyy") : ".......") + ", Đăng ký thường trú tại " +
                    per.hktt + " (thửa đất khác);");
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            }
            dict.Add("TenNguoiTuChoi", buff);

            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenBBHopGiaDinhMS01(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);

            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

            builder.MoveToMergeField("ThanhPhan");

            var nguoihuongthuakes = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake.HasValue).ToList();
            var chudat = nguoihuongthuakes.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.ChuDat).SingleOrDefault();
            var hangthuake1 = nguoihuongthuakes.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1);
            var caccon = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.Con);
            //var bochong = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeChong && x.gioitinh.ToUpper().Trim().Equals("NAM")).SingleOrDefault();
            //var mechong = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeChong && x.gioitinh.ToUpper().Trim().Equals("NỮ")).SingleOrDefault();
            //var bovo = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeVo && x.gioitinh.ToUpper().Trim().Equals("NAM")).SingleOrDefault();
            //var mevo = hangthuake1.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeVo && x.gioitinh.ToUpper().Trim().Equals("NỮ")).SingleOrDefault();
            var lstTemp = new List<string>();
            var socon = caccon.Count();

            var thanhphan = nguoihuongthuakes.Where(x => !x.ngaychet.HasValue).OrderBy(x => x.hangthuake).ThenBy(x => x.fk_id).ToList();
            string buff = String.Empty;
            int index = 0;
            foreach (var per in thanhphan)
            {
                index++;
                builder.Bold = true;
                builder.Write(index.ToString() + ". " + (per.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + per.hoten);
                builder.Bold = false;
                builder.Write(", sinh năm " +
                    (per.ngaysinh.HasValue ? per.ngaysinh.Value.ToString("yyyy") : ".......") + ", " + per.loaigiayto + " số " + per.sogiayto + " do " + per.noicap_gt +
                    " cấp ngày " + (per.ngaycap_gt.HasValue ? per.ngaycap_gt.Value.ToString("dd/MM/yyyy") : "..............") + "; Đăng ký thường trú tại " +
                    per.hktt + ".");
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            }

            string chudatchung = string.Empty, chudat1 = string.Empty, chudat2 = string.Empty;
            chudat1 = (chudat.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + chudat.hoten;
            chudat2 = !string.IsNullOrEmpty(chudat.hoten1) ? (chudat.gioitinh1.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + chudat.hoten1 : null;
            chudatchung = chudat1 + (!string.IsNullOrEmpty(chudat2) ? " và " + chudat2 : null);

            // Lời khai
            /*
             Ông Vũ Doãn Biếc sinh năm 1930, đã chết năm 2008 theo Trích lục khai tử số 24/TLKT-BS do UBND xã Bắc Sơn cấp ngày 10/05/2023. 
            Ông Biếc đã sinh sống tại thửa đất trên từ năm 1930. Ông Vũ Doãn Biếc kết hôn với bà Lê Thị Bé. 
            Bà Lê Thị Bé, sinh năm 1935, CCCD số 031135004591 do Cục cảnh sát cấp ngày 29/07/2022; 
            Đăng ký thường trú tại Thôn 1, xã Bắc Sơn, huyện An Dương, thành phố Hải Phòng.
             */
            builder.MoveToMergeField("ChuNguonGoc");
            if (chudat.ngaychet.HasValue)
            {
                buff = chudat1 + " sinh năm " + (chudat.ngaysinh.HasValue ? chudat.ngaysinh.Value.ToString("yyyy") : "......") + ", đã chết năm " +
                chudat.ngaychet.Value.ToString("yyyy") + " theo " + chudat.giaytochet;
            }
            else
            {
                buff = chudat1 + " sinh năm " + (chudat.ngaysinh.HasValue ? chudat.ngaysinh.Value.ToString("yyyy") : "......" + ", " + chudat.loaigiayto + " số " + chudat.sogiayto + " do " + chudat.noicap_gt +
                " cấp ngày " + (chudat.ngaycap_gt.HasValue ? chudat.ngaycap_gt.Value.ToString("dd/MM/yyyy") : "..............") + "; Đăng ký thường trú tại " +
                       chudat.hktt);
            }
            buff += " đã sinh sống tại thửa đất nêu trên từ năm " + (chudat.ngaysinh.HasValue ? chudat.ngaysinh.Value.ToString("yyyy") : "......") + ". ";
            buff += chudat1 + " kết hôn với " + chudat2 + ". ";
            if (chudat.ngaychet1.HasValue)
            {
                buff = chudat2 + " sinh năm " + (chudat.ngaysinh1.HasValue ? chudat.ngaysinh1.Value.ToString("yyyy") : "......") + ", đã chết năm " +
                chudat.ngaychet1.Value.ToString("yyyy") + " theo " + chudat.giaytochet1;
            }
            builder.Write(buff);
            // Hàng thừa kế thứ nhất
            builder.MoveToMergeField("HangThuaKe1");
            buff = string.Empty;
            /*Ông Vũ Doãn Biếc và bà Lê Thị Bé và sinh ra 08 người con đẻ. Là Bà Vũ Thị Vượng; Bà Vũ Thị Gái; Bà Vũ Thị Khánh; Ông Vũ Doãn Tám;
             * Bà Vũ Thị Nhượng; Ông Vũ Doãn Đượng; Ông Vũ Doãn Lượng và ông Vũ Doãn Khắc. 
             * Ngoài ra, Ông Vũ Doãn Biếc và bà Lê Thị Bé không có người con đẻ, con riêng, con nuôi nào khác. */

            buff = chudatchung + " sinh ra " + ((socon < 10 ? "0" + socon : socon.ToString()) + "(" + XModels.SoDem[socon] + ") ") + "người con đẻ. Là ";
            buff += string.Join(", ", caccon.Select(x => (x.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông " : "Bà ") + x.hoten));
            buff += ". Ngoài ra, " + chudatchung + " không có người con đẻ, con riêng, con nuôi nào khác.";
            builder.Write(buff);
            /*Ông Vũ Doãn Lượng sinh năm 1962, đã chết ngày 9/10/1997 theo trích lục khai tử số 27/TLKT-BS do UBND xã Bắc Sơn cấp ngày 29/05/2023. 
             * Ông Lượng đã kết hôn với bà Lê Thị Uấn và sinh ra 03 người con là: Ông Vũ Doãn Cường; Bà Vũ Thị Hương; Ông Vũ Doãn Đường. 
             * Ngoài ra, Ông Vũ Doãn Lượng và bà Lê Thị Uấn không có người con đẻ, con riêng, con nuôi nào khác.*/
            builder.MoveToMergeField("HangThuaKe2");
            buff = string.Empty;
            var hangthuake1chet = caccon.Where(x => x.ngaychet.HasValue);
            foreach (var item in hangthuake1chet)
            {
                var hoten = (item.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + item.hoten;
                var hangthuake2 = nguoihuongthuakes.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe2 && x.fk_id == item.id);
                var vochong = hangthuake2.FirstOrDefault(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.VoChong);
                var hoten1 = vochong != null ? (vochong.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông" : "Bà") + " " + vochong.hoten : string.Empty;
                var con = hangthuake2.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.Con);
                buff = hoten;
                buff += " sinh năm " + (item.ngaysinh.HasValue ? item.ngaysinh.Value.ToString("yyyy") : "......");
                buff += ", đã chết năm " + item.ngaychet.Value.ToString("yyyy") + " theo " + item.giaytochet;
                if (string.IsNullOrEmpty(hoten1))
                {
                    buff += ". " + hoten + " không kết hôn và không có người con đẻ, con nuôi nào.";
                }
                else
                {
                    buff += ". " + hoten + " đã kết hôn với " + hoten1 + " sinh ra 0" + con.Count() + " người con là: ";
                    buff += string.Join(", ", con.Select(x => (x.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông " : "Bà ") + x.hoten));
                    buff += ". Ngoài ra, " + hoten + " và " + hoten1 + " không có người con đẻ, con riêng, con nuôi nào khác.";
                }
                builder.Writeln(buff);
            }
            /*Hiện nay, Bà Lê Thị Bé và Ông Vũ Doãn Tám đang tiếp quản, sinh sống ổn định, liên tục, 
             * có nhu cầu đăng ký xin cấp Giấy chứng nhận quyền sử dụng đất đối với toàn bộ thửa đất nêu trên (Thửa đất số 51, tờ bản đồ số 11).*/
            builder.MoveToMergeField("NhuCau");
            var dongsohuu = nguoihuongthuakes.Where(x => x.marker == (int)XModels.eYesNo.Yes).ToList();
            string tendongsohuu = string.Empty;
            tendongsohuu = string.Join(", ", dongsohuu.Select(x => (x.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông " : "Bà ") + x.hoten));

            buff = string.Empty;
            buff += "Hiện nay, " + tendongsohuu + " đang tiếp quản, sinh sống ổn định, liên tục và có nhu cầu đăng ký xin cấp " +
                "Giấy chứng nhận quyền sử dụng đất đối với toàn bộ thửa đất nêu trên (Thửa đất số " + info.b_sothua + ", tờ bản đồ số " + info.b_tobando + ").";
            builder.Write(buff);

            /// Add more
            /// 
            var NguoiTuChoi = nguoihuongthuakes.Where(x => x.marker == (int)XModels.eYesNo.No && !x.ngaychet.HasValue).ToList();
            var TenNguoiTuChoi = string.Join(", ", NguoiTuChoi.Select(x => (x.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông " : "Bà ") + x.hoten));
            dict.Add("DongSoHuu", tendongsohuu);
            dict.Add("TenNguoiTuChoi", TenNguoiTuChoi);
            dict.Add("ChuSoMucKe", chudat1);

            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }        
    }
}