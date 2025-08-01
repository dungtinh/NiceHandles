using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Tables;
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
        void GenTachThua_CamKetSuDungChung(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict = AddToDictAuto(info, dict);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            var nguoihuongthuakes = db.ThongTinCaNhans.Where(x => x.infomation_id == info.id && x.hangthuake.HasValue).ToList();
            var thanhphan = nguoihuongthuakes.Where(x => x.marker == (int)XModels.eYesNo.Yes).ToList();
            string buff = String.Empty;
            int index = 0;
            builder.MoveToMergeField("ThanhPhan");
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
            var pagecount = doc.PageCount;
            if (pagecount % 2 == 1)
            {
                doc.AppendDocument(new Aspose.Words.Document(), Aspose.Words.ImportFormatMode.KeepSourceFormatting);
            }
        }
    }
}