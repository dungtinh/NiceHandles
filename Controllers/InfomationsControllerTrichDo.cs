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
        void GenQRCode(Infomation info, HoSo hoso, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            AddToDictAuto(info, dict);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            dict.Add("link", hoso.link_filecad);
            dict.Add("DiaChi", info.b_diachithuadat);
            builder.MoveToMergeField("QRCode");
            builder.InsertImage(Server.MapPath("~/public/" + hoso.link_filecad_qr),
            RelativeHorizontalPosition.Margin,
            0,
            RelativeVerticalPosition.Margin,
            0,
            100,
            100,
            WrapType.Square);

            builder.MoveToMergeField("QRDiaChi");
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
        void GenHDDD(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            dict.Add("a_hoten", !string.IsNullOrEmpty(info.a_hoten) ? info.a_hoten : ".......................");
            dict.Add("a_gioitinh", !string.IsNullOrEmpty(info.a_gioitinh) ? (info.a_gioitinh.Trim().ToUpper().Equals("NAM") ? "Ông" : "Bà") : "Ông/bà");
            dict.Add("a_loaigiayto", !string.IsNullOrEmpty(info.a_loaigiayto) ? info.a_loaigiayto : ".......................");
            dict.Add("a_sogiayto", !string.IsNullOrEmpty(info.a_sogiayto) ? info.a_sogiayto : ".......................");
            dict.Add("a_hktt", !string.IsNullOrEmpty(info.a_hktt) ? info.a_hktt : "............................................................................................");
            dict.Add("b_sothua", !string.IsNullOrEmpty(info.b_sothua) ? info.b_sothua : ".......................");
            dict.Add("b_tobando", !string.IsNullOrEmpty(info.b_tobando) ? info.b_tobando : ".......................");
            dict.Add("b_dientich", !string.IsNullOrEmpty(info.b_dientich) ? info.b_dientich : ".......................");
            dict.Add("b_diachithuadat", !string.IsNullOrEmpty(info.b_diachithuadat) ? info.b_diachithuadat : "............................................................................................");
            dict.Add("b_sogiaychungnhan", !string.IsNullOrEmpty(info.b_sogiaychungnhan) ? info.b_sogiaychungnhan : ".......................");
            dict.Add("b_ngaycap", info.b_ngaycap.HasValue ? info.b_ngaycap.Value.ToString("dd/MM/yyyy") : "...............");

            string b_khoangdientich = "..........";
            string b_giangoainghiep = "..........";
            string b_gianoinghiep = "..........";
            string b_tongtien = "..........";
            string b_thuevat = "..........";
            string b_thanhtoan = "..........";
            string b_tienbangchu = "...................................................................................";
            string b_danhmucthunoi = "..........";
            string b_danhmucthungoai = "..........";

            if (!string.IsNullOrWhiteSpace(info.b_dientich))
            {
                string uiSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                var temp = info.b_dientich.Replace(".", "").Replace(",", uiSep);
                double area = Convert.ToDouble(temp);

                if (area <= 100)
                    area = 100;
                else if (area <= 300)
                    area = 300;
                else if (area <= 500)
                    area = 500;
                else if (area <= 1000)
                    area = 1000;
                else if (area <= 3000)
                    area = 3000;
                else if (area <= 10000)
                    area = 10000;

                var PriceByAreas = db.PriceByAreas.Where(x => x.area == area);

                if (PriceByAreas.Count() > 0)
                {
                    var ngoainghiep = PriceByAreas.Single(x => x.type == (int)XPriceByArea.eType.DDNgoai);
                    b_danhmucthungoai = ngoainghiep.note;
                    b_khoangdientich = ngoainghiep.text;
                    b_giangoainghiep = ngoainghiep.price.ToString("N0");
                    var noinghiep = PriceByAreas.Single(x => x.type == (int)XPriceByArea.eType.DDNoi);
                    b_khoangdientich = noinghiep.text;
                    b_danhmucthunoi = noinghiep.note;
                    b_gianoinghiep = noinghiep.price.ToString("N0");
                    var tongtien = noinghiep.price + ngoainghiep.price;
                    b_tongtien = tongtien.ToString("N0");
                    int vat = tongtien * 8 / 100;
                    b_thuevat = vat.ToString("N0");
                    int tong = tongtien + vat;
                    b_thanhtoan = tong.ToString("N0");
                    b_tienbangchu = Utils.NumberToText(tong);
                }
            }

            dict.Add("b_khoangdientich", b_khoangdientich);
            dict.Add("b_giangoainghiep", b_giangoainghiep);
            dict.Add("b_gianoinghiep", b_gianoinghiep);
            dict.Add("b_tongtien", b_tongtien);
            dict.Add("b_thuevat", b_thuevat);
            dict.Add("b_thanhtoan", b_thanhtoan);
            dict.Add("b_tienbangchu", b_tienbangchu);
            dict.Add("b_danhmucthungoai", b_danhmucthungoai);
            dict.Add("b_danhmucthunoi", b_danhmucthunoi);

            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenHDCLTL(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            AddToDictAuto(info, dict);

            string b_khoangdientich = "..........";
            string b_giangoainghiep = "..........";
            string b_danhmucthunoi = "..........";
            string b_danhmucthungoai = "..........";
            string b_gianoinghiep = "..........";
            string b_giatrichluc = "..........";
            string b_danhmuctrichluc = "..........";
            string b_tongtien = "..........";
            string b_thuevat = "..........";
            string b_thanhtoan = "..........";
            string b_tienbangchu = "...................................................................................";

            int x_soluong = db.TachThuas.Count(x => x.infomation_id == info.id);
            dict.Add("x_soluong", x_soluong > 10 ? x_soluong.ToString() : "0" + x_soluong);


            if (!string.IsNullOrWhiteSpace(info.b_dientich))
            {
                string uiSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                var temp = info.b_dientich.Replace(".", "").Replace(",", uiSep);
                double area = Convert.ToDouble(temp);
                int soluong = 0;
                if (area <= 100)
                    area = 100;
                else if (area <= 300)
                    area = 300;
                else if (area <= 500)
                    area = 500;
                else if (area <= 1000)
                    area = 1000;
                else if (area <= 3000)
                    area = 3000;
                else if (area <= 10000)
                    area = 10000;

                if (x_soluong == 1)
                    soluong = 1;
                else if (x_soluong < 5)
                    soluong = 4;
                else if (x_soluong <= 10)
                    soluong = 10;
                else
                    soluong = 0;

                var PriceByAreas = db.PriceByAreas.Where(x => x.area == area);
                //var TL = db.PriceByAreas.Single(x => x.type == (int)XPriceByArea.eType.TL && x.area == soluong);

                if (PriceByAreas.Count() > 0)
                {
                    var ngoainghiep = PriceByAreas.Single(x => x.type == (int)XPriceByArea.eType.CLNgoai);
                    b_khoangdientich = ngoainghiep.text;
                    b_giangoainghiep = ngoainghiep.price.ToString("N0");
                    b_danhmucthungoai = ngoainghiep.note;
                    var noinghiep = PriceByAreas.Single(x => x.type == (int)XPriceByArea.eType.CLNoi);
                    b_khoangdientich = noinghiep.text;
                    b_gianoinghiep = noinghiep.price.ToString("N0");
                    b_danhmucthunoi = noinghiep.note;

                    //b_giatrichluc = TL.price.ToString("N0");
                    //b_danhmuctrichluc = TL.note;
                    //var tongtien = noinghiep.price + ngoainghiep.price + (TL.price * x_soluong);
                    var tongtien = noinghiep.price + ngoainghiep.price;
                    b_tongtien = tongtien.ToString("N0");
                    int vat = tongtien * 8 / 100;
                    b_thuevat = vat.ToString("N0");
                    int tong = tongtien + vat;
                    b_thanhtoan = tong.ToString("N0");
                    b_tienbangchu = Utils.NumberToText(tong);
                }

                dict.Add("b_khoangdientich", b_khoangdientich);
                dict.Add("b_giangoainghiep", b_giangoainghiep);
                dict.Add("b_gianoinghiep", b_gianoinghiep);
                dict.Add("b_tongtien", b_tongtien);
                dict.Add("b_thuevat", b_thuevat);
                dict.Add("b_thanhtoan", b_thanhtoan);
                dict.Add("b_tienbangchu", b_tienbangchu);
                dict.Add("b_danhmucthungoai", b_danhmucthungoai);
                dict.Add("b_danhmucthunoi", b_danhmucthunoi);
                //dict.Add("x_danhmuctrichluc", b_danhmuctrichluc);
                //dict.Add("x_giatrichluc", b_giatrichluc);
                //dict.Add("x_khoang", TL.text);
                //dict.Add("x_thanhtien", (TL.price * x_soluong).ToString("N0"));

            }
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenTrichDo(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            AddToDictAuto(info, dict);
            string chusohuu = Utils.ToTTCN3(info, dict, 0, true, "", "", "địa chỉ: ");
            dict.Add("ttchusohuu", chusohuu);
            var hoso = db.HoSoes.Find(info.hoso_id);
            var service = db.Services.Find(hoso.service_id);
            if (service.code.ToUpper().Equals("LANDAU"))
            {
                dict["b_mucdichsudung"] = "..................................................";
            }
            dict.Add("tendichvu", service.name);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
        void GenQuyTrinhDoVe(Infomation info, Aspose.Words.Document doc, Dictionary<string, string> dict)
        {
            var ngaythang = "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
            var hoso = db.HoSoes.Find(info.hoso_id);
            var service = db.Services.Find(hoso.service_id);
            var contract = db.Contracts.Find(hoso.contract_id);
            dict.Add("hoso_name", hoso.name);
            dict.Add("ngaythang", ngaythang);
            var sodienthoai = contract.sodienthoai + " (" + contract.sodienthoai_mota + ")";
            dict.Add("sodienthoai", sodienthoai);
            dict.Add("tendichvu", service.name);
            var address = db.Addresses.Find(contract.address_id);
            dict.Add("diachi", address.name);
            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            builder.MoveToMergeField("note");
            builder.InsertHtml(string.IsNullOrEmpty(hoso.note) ? "" : hoso.note);
            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());
        }
    }
}