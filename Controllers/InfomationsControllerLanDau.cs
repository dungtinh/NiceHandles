using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Tables;
using NiceHandles.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using Table = Aspose.Words.Tables.Table;

namespace NiceHandles.Controllers
{
    public partial class InfomationsController
    {
        void LanDau_TongHop(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            var dicttemp = dict.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            doc.RemoveAllChildren();

            // I, TỜ KHAI 4DK            
            var docTK = db.Documents.First(x => x.code.Equals("LanDau_4DK"));
            string pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            var doctemp = new Aspose.Words.Document(pathTemp);
            GenNormal(info, doctemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            // 4addk
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                             entry => entry.Value);
            docTK = db.Documents.First(x => x.code.Equals("LanDau_4aDK"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            LanDau_4aDK(info, doctemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);

            // TỜ KHAI ĐĂNG KÝ THUẾ
            if (string.IsNullOrEmpty(info.a_masothue))
            {
                dicttemp = dict.ToDictionary(entry => entry.Key,
                                                 entry => entry.Value);
                docTK = db.Documents.First(x => x.code.Equals("LanDau_DKT"));
                pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
                doctemp = new Aspose.Words.Document(pathTemp);
                LanDau_DKT(info, doctemp, dicttemp);
                doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
            // TỜ KHAI LỆ PHÍ TRƯỚC BẠ
            //TKLPTB_BD
            docTK = db.Documents.First(x => x.code.Equals("LanDau_LPTB"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                                 entry => entry.Value);
            LanDau_LPTB(info, doctemp, dicttemp);

            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);

            // TỜ KHAI THUẾ PHI NÔNG NGHIỆP
            docTK = db.Documents.First(x => x.code.Equals("LanDau_TPNN"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                                 entry => entry.Value);
            LanDau_TPNN(info, doctemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            // TỜ KHAI THUẾ TNCN
            var nguoihuongthuakes = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake.HasValue).ToList();
            var chudat = nguoihuongthuakes.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.ChuDat).SingleOrDefault();
            if (chudat != null)
            {
                docTK = db.Documents.First(x => x.code.Equals("LanDau_TNCN"));
                pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
                doctemp = new Aspose.Words.Document(pathTemp);
                dicttemp = dict.ToDictionary(entry => entry.Key,
                                                     entry => entry.Value);
                LanDau_TNCN(info, doctemp, dicttemp);
                doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
            // END            
        }
        void LanDau_4aDK(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            var danhsach = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.marker.HasValue && x.marker == (int)XModels.eYesNo.Yes).ToList();
            if (danhsach.Count > 0)
            {
                Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
                builder.MoveToMergeField("danhsach");
                string buff = String.Empty;
                int index = 0;
                Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
                var cell = table.Rows[3].FirstCell;

                Style style = doc.Styles.Add(StyleType.Paragraph, "MyStyle1");
                style.Font.Name = "Times New Roman";
                style.Font.Size = 12;
                style.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                style.ParagraphFormat.SpaceAfter = 6;
                style.ParagraphFormat.SpaceBefore = 6;
                style.ParagraphFormat.LineSpacingRule = LineSpacingRule.AtLeast;
                style.ParagraphFormat.LineSpacing = 11.7;
                foreach (var per in danhsach)
                {
                    index++;
                    cell = table.Rows[2 + index].FirstCell;
                    builder.MoveTo(cell.FirstParagraph);
                    builder.ParagraphFormat.Style = style;
                    builder.Write(index.ToString());
                    cell = table.Rows[2 + index].Cells[1];
                    builder.MoveTo(cell.FirstParagraph);
                    builder.ParagraphFormat.Style = style;
                    builder.Write(per.hoten);
                    //Năm sinh
                    cell = table.Rows[2 + index].Cells[2];
                    builder.MoveTo(cell.FirstParagraph);
                    builder.ParagraphFormat.Style = style;
                    builder.Write(per.ngaysinh.Value.ToString("yyyy"));
                    //Loại giấy tờ
                    cell = table.Rows[2 + index].Cells[3];
                    builder.MoveTo(cell.FirstParagraph);
                    builder.ParagraphFormat.Style = style;
                    builder.Write(per.loaigiayto);
                    //Số giấy tờ
                    cell = table.Rows[2 + index].Cells[4];
                    builder.MoveTo(cell.FirstParagraph);
                    builder.ParagraphFormat.Style = style;
                    builder.Write(per.sogiayto);
                    //Ngày cấp
                    cell = table.Rows[2 + index].Cells[5];
                    builder.MoveTo(cell.FirstParagraph);
                    builder.ParagraphFormat.Style = style;
                    builder.Write(per.ngaycap_gt.Value.ToString("dd/MM/yyyy"));
                    //Nơi cấp
                    cell = table.Rows[2 + index].Cells[6];
                    builder.MoveTo(cell.FirstParagraph);
                    builder.ParagraphFormat.Style = style;
                    builder.Write(per.noicap_gt);
                    //Địa chỉ
                    cell = table.Rows[2 + index].Cells[7];
                    builder.MoveTo(cell.FirstParagraph);
                    builder.ParagraphFormat.Style = style;
                    builder.Write(per.hktt);
                    //Insert Row
                    Row clonedRow = (Row)table.LastRow.Clone(true);
                    foreach (Cell c in clonedRow.Cells)
                    {
                        c.RemoveAllChildren();
                        c.EnsureMinimum();
                    }
                    table.Rows[2 + index].ParentNode.InsertAfter(clonedRow, table.Rows[2 + index]);
                }
            }
            else
            {
                string pathTemp = Server.MapPath("~/App_Data/templates/2024MS04aDK_bak.docx");
                var doctemp = new Aspose.Words.Document(pathTemp);
                GenNormal(info, doctemp, dict);
                doc.RemoveAllChildren();
                doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
            var pagecount = doc.PageCount;
            if (pagecount % 2 == 1)
            {
                doc.AppendDocument(new Aspose.Words.Document(), Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void LanDau_DKT(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
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
        void LanDau_LPTB(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
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
        void LanDau_TPNN(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            if (string.IsNullOrEmpty(info.a_hoten1))
            {
                dict.Add("a_tyle", "100%");
                dict.Add("a_tyle1", "");
            }
            else
            {
                dict.Add("a_tyle", "50%");
                dict.Add("a_tyle1", "50%");
            }
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void LanDau_TNCN(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void LanDau_GUQ(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);

            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            var nguoihuongthuakes = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake.HasValue).ToList();
            var dongsohuu = nguoihuongthuakes.Where(x => x.marker == (int)XModels.eYesNo.Yes).ToList();
            var SoDongSoHuu = dongsohuu.Count > 9 ? dongsohuu.Count.ToString() : "0" + dongsohuu.Count.ToString();
            SoDongSoHuu += "(" + XModels.SoDem[dongsohuu.Count] + ")";
            string tendongsohuu = string.Empty;
            tendongsohuu = string.Join(", ", dongsohuu.Select(x => (x.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông " : "Bà ") + x.hoten));
            var thanhphan = dongsohuu;
            string buff = String.Empty;
            int index = 0;
            builder.MoveToMergeField("ThanhPhan");
            var nhanxung = string.Empty; bool flag = false; var nhucau = string.Empty;
            string rs2 = string.Empty;
            if (thanhphan.Count > 1 || (thanhphan.Count == 0 && !string.IsNullOrEmpty(info.a_hoten1)))
            {
                nhanxung = "chúng tôi";
                flag = true;
                nhucau = "Chúng tôi có nhu cầu làm thủ tục xin cấp chung 01 Giấy chứng nhận quyền sử dụng đất lần đầu, cùng ghi tên trên Giấy chứng nhận quyền sử dụng đất";
                if (thanhphan.Count > 0)
                {
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
                }
                else
                {
                    builder.Writeln(Utils.ToTTCN2(info, dict, ref rs2));
                    builder.Writeln(rs2);
                }
            }
            else
            {
                nhanxung = "tôi";
                nhucau = "Tôi có nhu cầu làm thủ tục xin cấp Giấy chứng nhận quyền sử dụng đất lần đầu";
                builder.Writeln(Utils.ToTTCN2(info, dict, ref rs2));
                builder.Writeln(rs2);
            }
            dict.Add("nhucau", nhucau);
            dict.Add("nhanxung", nhanxung);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
            var pagecount = doc.PageCount;
            if (pagecount % 2 == 1)
            {
                doc.AppendDocument(new Aspose.Words.Document(), Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
        }
        void LanDau_AnhEmKhongTrangChap(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);

            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

            builder.MoveToMergeField("ThanhPhan");

            var nguoihuongthuakes = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake.HasValue).ToList();
            var dongsohuu = nguoihuongthuakes.Where(x => x.marker == (int)XModels.eYesNo.Yes).ToList();
            var chudat = nguoihuongthuakes.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.ChuDat).SingleOrDefault();
            var thanhphan = nguoihuongthuakes.Where(x => !x.ngaychet.HasValue).OrderBy(x => x.hangthuake).ThenBy(x => x.fk_id).ToList();
            thanhphan = thanhphan.Where(x => !x.marker.HasValue || x.marker.Value == (int)XModels.eYesNo.No).ToList();
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

            string tendongsohuu = string.Empty;
            tendongsohuu = string.Join(", ", dongsohuu.Select(x => (x.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông " : "Bà ") + x.hoten));
            var NguoiTuChoi = nguoihuongthuakes.Where(x => x.marker == (int)XModels.eYesNo.No && !x.ngaychet.HasValue).ToList();
            var TenNguoiTuChoi = string.Join(", ", NguoiTuChoi.Select(x => (x.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông " : "Bà ") + x.hoten));
            dict.Add("DongSoHuu", tendongsohuu);
            dict.Add("TenNguoiTuChoi", TenNguoiTuChoi);
            dict.Add("ChuSoMucKe", chudatchung);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
            var pagecount = doc.PageCount;
            if (pagecount % 2 == 1)
            {
                doc.AppendDocument(new Aspose.Words.Document(), Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
        }
        void LanDau_TongHopCongChung(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            var dicttemp = dict.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            doc.RemoveAllChildren();

            // I, ỦY QUYỀN
            var docTK = db.Documents.First(x => x.code.Equals("LanDau_GUQ"));
            string pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            var doctemp = new Aspose.Words.Document(pathTemp);
            LanDau_GUQ(info, doctemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            // 4addk
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                             entry => entry.Value);
            docTK = db.Documents.First(x => x.code.Equals("LanDau_AnhEmKhongTrangChap"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            LanDau_AnhEmKhongTrangChap(info, doctemp, dicttemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);

            //TKLPTB_BD
            docTK = db.Documents.First(x => x.code.Equals("VBTTCKUQ"));
            pathTemp = Server.MapPath("~/App_Data/templates/" + docTK.template);
            doctemp = new Aspose.Words.Document(pathTemp);
            dicttemp = dict.ToDictionary(entry => entry.Key,
                                                 entry => entry.Value);
            GenVBTTCKUQ(info, doctemp, dicttemp);

            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);

        }
        void LanDau_DDN_KiemTra(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            var onedoor = db.di1cua.Single(x => x.hoso_id == info.hoso_id && x.trangthai < (int)Xdi1cua.eStatus.ThanhCong);
            var quahan = (DateTime.Now - onedoor.ngaytra).Days;
            dict.Add("maphieuhen", onedoor.maphieuhen);
            dict.Add("ngaynop", onedoor.ngaynop.ToString("dd/MM/yyyy"));
            dict.Add("ngayhen", onedoor.ngaytra.ToString("dd/MM/yyyy"));
            dict.Add("quahan", quahan < 10 ? "0" + quahan.ToString() : quahan.ToString());
            var ngaythuc = (DateTime.Now - onedoor.ngaynop).Days;
            int ngaylamviec = 0;
            var ngaynop = onedoor.ngaynop;
            for (DateTime date = ngaynop; date < DateTime.Now; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    ngaylamviec++;
            }
            dict.Add("ngaylamviec", ngaylamviec < 10 ? "0" + ngaylamviec.ToString() : ngaylamviec.ToString());
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenVBTTCKUQ(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
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
            dict.Add("TenChuNguonGoc", chudatchung);
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
                buff += chudat2 + " sinh năm " + (chudat.ngaysinh1.HasValue ? chudat.ngaysinh1.Value.ToString("yyyy") : "......") + ", đã chết năm " +
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
            var soho = db.SoHoKhaus.Where(x => x.infomation_id == info.id).ToList();
            builder.MoveToMergeField("SongTrenDat");
            index = 0;
            foreach (var item in soho)
            {
                index++;
                builder.Bold = true;
                builder.Writeln();
                builder.Writeln(index.ToString() + ". Hộ gia đình thứ " + XModels.SoThu[index - 1]);
                var nhankhau = db.ThongTinCaNhans.Where(x => x.sohokhau_id == item.id && x.infomation_id == info.id).ToList();
                var chuho = nhankhau.FirstOrDefault();
                //nhankhau.Remove(chuho);
                Aspose.Words.Tables.Table table = builder.StartTable();
                builder.InsertCell();
                builder.CellFormat.VerticalAlignment = Aspose.Words.Tables.CellVerticalAlignment.Center;
                builder.ParagraphFormat.Alignment = Aspose.Words.ParagraphAlignment.Center;
                builder.ParagraphFormat.FirstLineIndent = 0;
                builder.Write("TT");
                builder.InsertCell();
                builder.Write("Họ, chữ đệm và tên");
                builder.InsertCell();
                builder.Write("Ngày, tháng, năm sinh");
                builder.InsertCell();
                builder.Write("Số định danh cá nhân/CMND");
                builder.InsertCell();
                builder.Write("Quan hệ với  chủ hộ");
                builder.EndRow();
                builder.Bold = false;
                foreach (var nk in nhankhau)
                {
                    builder.InsertCell();
                    builder.Write((nhankhau.IndexOf(nk) + 1).ToString());
                    builder.InsertCell();
                    builder.Write(string.IsNullOrEmpty(nk.hoten) ? "" : nk.hoten);
                    builder.InsertCell();
                    builder.Write(nk.ngaysinh.HasValue ? nk.ngaysinh.Value.ToString("dd/MM/yyyy") : "");
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
                    builder.EndRow();
                }
                builder.EndTable();
            }

            var dongsohuu = nguoihuongthuakes.Where(x => x.marker == (int)XModels.eYesNo.Yes).ToList();
            var SoDongSoHuu = dongsohuu.Count > 9 ? dongsohuu.Count.ToString() : "0" + dongsohuu.Count.ToString();
            SoDongSoHuu += "(" + XModels.SoDem[dongsohuu.Count] + ")";
            string tendongsohuu = string.Empty;
            tendongsohuu = string.Join(", ", dongsohuu.Select(x => (x.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông " : "Bà ") + x.hoten));
            /// Add more
            /// 
            var NguoiTuChoi = nguoihuongthuakes.Where(x => x.marker == (int)XModels.eYesNo.No && !x.ngaychet.HasValue).ToList();
            var TenNguoiTuChoi = string.Join(", ", NguoiTuChoi.Select(x => (x.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông " : "Bà ") + x.hoten));
            dict.Add("DongSoHuu", tendongsohuu);
            dict.Add("SoDongSoHuu", SoDongSoHuu);
            dict.Add("TenNguoiTuChoi", TenNguoiTuChoi);
            dict.Add("ChuSoMucKe", chudat1);
            dict.Add("SoHo", soho.Count.ToString());
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());

            // Gia Phả
            var web = new HtmlAgilityPack.HtmlWeb()
            {
                AutoDetectEncoding = false,
                OverrideEncoding = Encoding.UTF8
            };
            var documentForListItem = web.Load("http://giapha.dodactracdia.vn/home/?hoso_id=" + info.hoso_id);
            string pathTemp = Server.MapPath("~/GiaPha/public/sodohuyetthong.docx");
            var doctemp = new Aspose.Words.Document(pathTemp);
            doc.AppendDocument(doctemp, Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            var pagecount = doc.PageCount;
            if (pagecount % 2 == 1)
            {
                doc.AppendDocument(new Aspose.Words.Document(), Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
        }
        void LanDau_HutDienTich(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            var dientich = string.IsNullOrEmpty(info.b_dientich) ? 0 : Convert.ToDecimal(info.b_dientich);
            var dientichbia = string.IsNullOrEmpty(info.b_dientichtrenbia) ? 0 : Convert.ToDecimal(info.b_dientichtrenbia);

            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);

            var nguoihuongthuakes = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake.HasValue).ToList();
            var dongsohuu = nguoihuongthuakes.Where(x => x.marker == (int)XModels.eYesNo.Yes).ToList();
            var SoDongSoHuu = dongsohuu.Count > 9 ? dongsohuu.Count.ToString() : "0" + dongsohuu.Count.ToString();
            SoDongSoHuu += "(" + XModels.SoDem[dongsohuu.Count] + ")";
            string tendongsohuu = string.Empty;
            tendongsohuu = string.Join(", ", dongsohuu.Select(x => (x.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông " : "Bà ") + x.hoten));
            var thanhphan = dongsohuu;
            string buff = String.Empty;
            int index = 0;
            builder.MoveToMergeField("chusudung");
            var nhanxung = string.Empty;
            string rs2 = string.Empty;
            if (thanhphan.Count > 1 || (thanhphan.Count == 0 && !string.IsNullOrEmpty(info.a_hoten1)))
            {
                nhanxung = "chúng tôi";
                if (thanhphan.Count > 0)
                {
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
                }
                else
                {
                    builder.Writeln(Utils.ToTTCN2(info, dict, ref rs2));
                    builder.Writeln(rs2);
                }
            }
            else
            {
                nhanxung = "tôi";
                builder.Writeln(Utils.ToTTCN2(info, dict, ref rs2));
                builder.Writeln(rs2);
            }
            dict.Add("b_dientich_giam", (dientichbia - dientich).ToString());
            dict.Add("nhanxung", nhanxung);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void LanDau_DDN_NopTienVuotHanMuc(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            var nguoihuongthuakes = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake.HasValue).ToList();
            var dongsohuu = nguoihuongthuakes.Where(x => x.marker == (int)XModels.eYesNo.Yes).ToList();
            var SoDongSoHuu = dongsohuu.Count > 9 ? dongsohuu.Count.ToString() : "0" + dongsohuu.Count.ToString();
            SoDongSoHuu += "(" + XModels.SoDem[dongsohuu.Count] + ")";
            string tendongsohuu = string.Empty;
            tendongsohuu = string.Join(", ", dongsohuu.Select(x => (x.gioitinh.ToUpper().Trim().Equals("NAM") ? "Ông " : "Bà ") + x.hoten));
            var thanhphan = dongsohuu;
            string buff = String.Empty;
            int index = 0;
            builder.MoveToMergeField("chusudung");
            var nhanxung = string.Empty;
            string rs2 = string.Empty;
            if (thanhphan.Count > 1 || (thanhphan.Count == 0 && !string.IsNullOrEmpty(info.a_hoten1)))
            {
                nhanxung = "chúng tôi";
                if (thanhphan.Count > 0)
                {
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
                }
                else
                {
                    builder.Writeln(Utils.ToTTCN2(info, dict, ref rs2));
                    builder.Writeln(rs2);
                }
            }
            else
            {
                nhanxung = "tôi";
                builder.Writeln(Utils.ToTTCN2(info, dict, ref rs2));
                builder.Writeln(rs2);
            }
            dict.Add("nhanxung", nhanxung);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenHDUQ_VPL(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            builder.MoveToMergeField("ThanhPhan");

            var nguoihuongthuakes = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake.HasValue).ToList();

            var thanhphan = nguoihuongthuakes.Where(x => !x.ngaychet.HasValue).OrderBy(x => x.hangthuake).ThenBy(x => x.fk_id).ToList();
            int index = 0;
            foreach (var per in thanhphan)
            {
                index++;
                builder.Bold = true;
                builder.Write(index.ToString() + ". " + " " + per.hoten.ToUpper());
                builder.Bold = false;
                builder.Write(", " + per.loaigiayto + " số " + per.sogiayto);
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            }
            builder.MoveToMergeField("DongSoHuu");
            var dongsohuu = nguoihuongthuakes.Where(x => x.marker == (int)XModels.eYesNo.Yes).ToList();
            index = 0;
            foreach (var per in dongsohuu)
            {
                index++;
                builder.Bold = true;
                builder.Write(index.ToString() + ". " + " " + per.hoten.ToUpper());
                builder.Bold = false;
                builder.Write(", " + per.loaigiayto + " số " + per.sogiayto);
                builder.InsertBreak(Aspose.Words.BreakType.ParagraphBreak);
            }
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void LanDau_DonToCaoChamMuon(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            var onedoor = db.di1cua.Single(x => x.hoso_id == info.hoso_id && x.trangthai < (int)Xdi1cua.eStatus.ThanhCong);
            var quahan = (DateTime.Now - onedoor.ngaytra).Days;
            dict.Add("maphieuhen", onedoor.maphieuhen);
            dict.Add("ngaynop", onedoor.ngaynop.ToString("dd/MM/yyyy"));
            dict.Add("ngayhen", onedoor.ngaytra.ToString("dd/MM/yyyy"));
            dict.Add("quahan", quahan < 10 ? "0" + quahan.ToString() : quahan.ToString());

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
        void LanDau_6VBTuThoaThuan(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);

            var giapha = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake.HasValue).ToList();
            var dungtensmk = giapha.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.ChuDat).SingleOrDefault();

            var nguoidungten = new List<ThongTinCaNhan>();
            var nguoilapvanban = new List<ThongTinCaNhan>();

            dict.Add("dungtensmk", (!string.IsNullOrEmpty(dungtensmk.gioitinh) ? dungtensmk.gioitinh.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà") + " " + dungtensmk.hoten);
            dict.Add("dungtensmk1", (!string.IsNullOrEmpty(dungtensmk.gioitinh1) ? dungtensmk.gioitinh1.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà" : "Ông/Bà") + " " + dungtensmk.hoten1);
            dict.Add("vochongsmk", (!string.IsNullOrEmpty(dungtensmk.gioitinh) ? dungtensmk.gioitinh.Trim().ToUpper().Equals("NAM") ? "vợ" : "chồng" : "vợ/chồng"));
            dict.Add("ngaysinhsmk", dungtensmk.ngaysinh.HasValue ? dungtensmk.ngaysinh.Value.ToString("yyyy") : "");
            dict.Add("ngaysinhsmk1", dungtensmk.ngaysinh1.HasValue ? dungtensmk.ngaysinh1.Value.ToString("yyyy") : "");
            dict.Add("giaytosmk", dungtensmk.sogiayto ?? "");
            dict.Add("giaytosmk1", dungtensmk.sogiayto1 ?? "");
            dict.Add("quanhesmk", "");
            dict.Add("quanhesmk1", (dungtensmk.gioitinh1.Trim().ToUpper().Equals("NAM") ? "Chồng bà " : "Vợ ông ") + dungtensmk.hoten);
            dict.Add("songchetsmk", (dungtensmk.ngaychet.HasValue ? "Đã chết" : ""));
            dict.Add("songchetsmk1", (dungtensmk.ngaychet1.HasValue ? "Đã chết" : ""));

            if (!dungtensmk.ngaychet.HasValue)
            {
                nguoilapvanban.Add(dungtensmk);
                //nguoidungten.Add(dungtensmk);
            }
            if (!dungtensmk.ngaychet1.HasValue)
            {
                var smk1 = new ThongTinCaNhan
                {
                    id = dungtensmk.id,
                    hoten = dungtensmk.hoten1,
                    gioitinh = dungtensmk.gioitinh1,
                    ngaysinh = dungtensmk.ngaysinh1,
                    sogiayto = dungtensmk.sogiayto1,
                    noicap_gt = dungtensmk.noicap_gt1,
                    ngaycap_gt = dungtensmk.ngaycap_gt1,
                    marker = (int)XModels.eYesNo.Yes,
                    quanhe = (int)XThongTinCaNhan.eQuanHe.VoChong
                };
                nguoilapvanban.Add(smk1);
                //nguoidungten.Add(smk1);
            }

            var hangthuake1 = giapha.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1).OrderBy(x => x.ngaysinh).ToList();
            dict.Add("soconruot", "0" + hangthuake1.Count().ToString());
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            Table table = (Table)doc.GetChild(NodeType.Table, 0, true);
            Row clonedRow = (Row)table.LastRow.Clone(true);
            Style style = doc.Styles.Add(StyleType.Paragraph, "MyStyle1");
            style.Font.Name = "Times New Roman";
            style.Font.Size = 14;
            style.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            style.ParagraphFormat.SpaceAfter = 0;
            style.ParagraphFormat.SpaceBefore = 0;
            style.ParagraphFormat.LineSpacingRule = LineSpacingRule.AtLeast;
            style.ParagraphFormat.LineSpacing = 11;
            foreach (Cell c in clonedRow.Cells)
            {
                c.RemoveAllChildren();
                c.EnsureMinimum();
                foreach (Aspose.Words.Paragraph para in c.Paragraphs)
                {
                    para.ParagraphFormat.Style = style;
                }
            }
            string buff = String.Empty;
            int index = 0, index2 = 0;

            foreach (var per in hangthuake1)
            {
                // Thêm dòng người thừa kế thứ..
                table.AppendChild(clonedRow.Clone(true));
                var cell = table.LastRow.FirstCell;
                cell.CellFormat.HorizontalMerge = CellMerge.First;
                for (int i = 1; i < table.LastRow.Cells.Count; i++)
                {
                    table.LastRow.Cells[i].CellFormat.HorizontalMerge = CellMerge.Previous;
                }
                builder.MoveTo(cell.FirstParagraph);
                index++;
                builder.Bold = true;
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                buff = "Người con thứ " + XModels.SoThu[index - 1];
                builder.Write(buff);
                builder.Bold = false;
                // Thông tin hàng thừa kế 1
                table.AppendChild(clonedRow.Clone(true));
                cell = table.LastRow.FirstCell;
                builder.MoveTo(cell.FirstParagraph);
                builder.Write("1");
                cell = table.LastRow.Cells[1];
                builder.MoveTo(cell.FirstParagraph);
                builder.Write(per.hoten);
                cell = table.LastRow.Cells[2];
                builder.MoveTo(cell.FirstParagraph);
                builder.Write(per.ngaysinh.HasValue ? per.ngaysinh.Value.ToString("yyyy") : "");
                cell = table.LastRow.Cells[3];
                builder.MoveTo(cell.FirstParagraph);
                builder.Write(per.sogiayto ?? "");
                cell = table.LastRow.Cells[4];
                builder.MoveTo(cell.FirstParagraph);
                per.ghichuquanhe = ghichuquanhe(dungtensmk, per);
                builder.Write(per.ghichuquanhe);
                cell = table.LastRow.Cells[5];
                builder.MoveTo(cell.FirstParagraph);
                builder.Write(per.songtrendat.HasValue ? (per.songtrendat.Value == (int)XModels.eYesNo.Yes ? "Ở trên đất" : "Không ở trên đất") : "Không ở trên đất");
                cell = table.LastRow.Cells[6];
                builder.MoveTo(cell.FirstParagraph);
                builder.Write(per.ngaychet.HasValue ? "Đã chết" : (per.ngaysinh.HasValue ? ((DateTime.Now - per.ngaysinh.Value).TotalDays / 365 > 15 ? "Còn sống" : "Dưới 15 tuổi") : "Còn sống"));

                var hangthuake2 = giapha.Where(x => x.fk_id == per.id).OrderBy(x => x.ngaysinh).ToList();
                index2 = 1;
                // Người lập văn bản là người thừa kế còn sống và trên 15 tuổi 
                if (!per.ngaychet.HasValue)
                {
                    nguoilapvanban.Add(per);
                }
                if (per.marker == (int)XModels.eYesNo.Yes)
                {
                    nguoidungten.Add(per);
                }
                foreach (var per2 in hangthuake2)
                {
                    index2++;
                    table.AppendChild(clonedRow.Clone(true));
                    cell = table.LastRow.FirstCell;
                    builder.MoveTo(cell.FirstParagraph);
                    builder.Write(index2.ToString());
                    cell = table.LastRow.Cells[1];
                    builder.MoveTo(cell.FirstParagraph);
                    builder.Write(per2.hoten);
                    cell = table.LastRow.Cells[2];
                    builder.MoveTo(cell.FirstParagraph);
                    builder.Write(per2.ngaysinh.HasValue ? per2.ngaysinh.Value.ToString("yyyy") : "");
                    cell = table.LastRow.Cells[3];
                    builder.MoveTo(cell.FirstParagraph);
                    builder.Write(per2.sogiayto ?? "");
                    cell = table.LastRow.Cells[4];
                    builder.MoveTo(cell.FirstParagraph);
                    per2.ghichuquanhe = ghichuquanhe(per, per2);
                    builder.Write(per2.ghichuquanhe);
                    cell = table.LastRow.Cells[5];
                    builder.MoveTo(cell.FirstParagraph);
                    builder.Write(per2.songtrendat.HasValue ? (per2.songtrendat.Value == (int)XModels.eYesNo.Yes ? "Ở trên đất" : "Không ở trên đất") : "Không ở trên đất");
                    cell = table.LastRow.Cells[6];
                    builder.MoveTo(cell.FirstParagraph);
                    builder.Write(per2.ngaychet.HasValue ? "Đã chết" : (per2.ngaysinh.HasValue ? ((DateTime.Now - per2.ngaysinh.Value).TotalDays / 365 > 15 ? "Còn sống" : "Dưới 15 tuổi") : "Còn sống"));

                    if (!per2.ngaychet.HasValue)
                    {
                        nguoilapvanban.Add(per2);
                    }
                    if (per2.marker == (int)XModels.eYesNo.Yes)
                    {
                        nguoidungten.Add(per2);
                    }
                }
            }
            dict.Add("songuoidungten", "0" + nguoidungten.Count().ToString());
            table = (Table)doc.GetChild(NodeType.Table, 1, true);
            clonedRow = (Row)table.LastRow.Clone(true);
            foreach (Cell c in clonedRow.Cells)
            {
                c.RemoveAllChildren();
                c.EnsureMinimum();
                foreach (Aspose.Words.Paragraph para in c.Paragraphs)
                {
                    para.ParagraphFormat.Style = style;
                }
            }
            buff = String.Empty;
            index = 0;
            foreach (var per in nguoidungten)
            {
                index++;
                table.AppendChild(clonedRow.Clone(true));
                var cell = table.LastRow.FirstCell;
                builder.MoveTo(cell.FirstParagraph);
                builder.Write(index.ToString());
                cell = table.LastRow.Cells[1];
                builder.MoveTo(cell.FirstParagraph);
                builder.Write(per.hoten);
                cell = table.LastRow.Cells[2];
                builder.MoveTo(cell.FirstParagraph);
                builder.Write(per.ngaysinh.HasValue ? per.ngaysinh.Value.ToString("yyyy") : "");
                cell = table.LastRow.Cells[3];
                builder.MoveTo(cell.FirstParagraph);
                builder.Write(per.sogiayto);
            }
            table = (Table)doc.GetChild(NodeType.Table, 2, true);
            clonedRow = (Row)table.LastRow.Clone(true);

            //STYLE
            Style style2 = doc.Styles.Add(StyleType.Paragraph, "MyStyle2");
            style2.Font.Name = "Times New Roman";
            style2.Font.Size = 14;
            style2.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            style2.ParagraphFormat.SpaceAfter = 20;
            style2.ParagraphFormat.SpaceBefore = 20;
            //style2.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;            

            foreach (Cell c in clonedRow.Cells)
            {
                c.RemoveAllChildren();
                c.EnsureMinimum();
                foreach (Aspose.Words.Paragraph para in c.Paragraphs)
                {
                    para.ParagraphFormat.Style = style2;
                }
            }
            buff = String.Empty;
            index = 0;
            foreach (var per in nguoilapvanban)
            {
                index++;
                table.AppendChild(clonedRow.Clone(true));
                var cell = table.LastRow.FirstCell;
                builder.MoveTo(cell.FirstParagraph);
                builder.Write(index.ToString());
                cell = table.LastRow.Cells[1];
                builder.MoveTo(cell.FirstParagraph);
                builder.Write(per.hoten);
                cell = table.LastRow.Cells[2];
                builder.MoveTo(cell.FirstParagraph);
                builder.Write(per.ngaysinh.HasValue ? per.ngaysinh.Value.ToString("yyyy") : "");
                cell = table.LastRow.Cells[3];
                builder.MoveTo(cell.FirstParagraph);
                builder.Write(per.sogiayto);
            }
            foreach (var per in nguoilapvanban)
            {
                buff += (per.gioitinh.Trim().ToUpper().Equals("NỮ") ? "bà " : "ông ") + per.hoten + ", " + per.loaigiayto + " " + per.sogiayto + "; ";
            }
            dict.Add("danhsachtennguoilap", buff);

            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
            var pagecount = doc.PageCount;
            if (pagecount % 2 == 1)
            {
                doc.AppendDocument(new Aspose.Words.Document(), Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
        }
        string ghichuquanhe(ThongTinCaNhan chinh, ThongTinCaNhan phu)
        {
            string result = "";
            if (phu.quanhe.Value == (int)XThongTinCaNhan.eQuanHe.VoChong)
            {
                if (phu.gioitinh.Trim().ToUpper().Equals("NỮ"))
                    result = "Vợ ông " + chinh.hoten;
                else
                    result = "Chồng bà " + chinh.hoten;
            }
            else
            {
                result = XThongTinCaNhan.sQuanHe[phu.quanhe.Value] + " " + (chinh.gioitinh.Trim().ToUpper().Equals("NAM") ? "ông" : "bà") + " " + chinh.hoten;
            }
            return result;
        }
    }
}