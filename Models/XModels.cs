using Aspose.Words;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static NiceHandles.Models.XCongViec;

namespace NiceHandles.Models
{
    public class XModels
    {
        public const string Password = "dongy";
        public const string THUMUCHOSO = "HOSO";
        public List<HttpPostedFileBase> files { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public double id { get; set; }
        public int type { get; set; }
        public static string[] sGender = new string[] { "Nam", "Nữ" };
        public enum eStatus
        {
            Processing,
            Complete,
            Cancel
        }
        public static Dictionary<int, string> sStatus = new Dictionary<int, string>()
        {
            {(int)eStatus.Processing, "Đang thực hiện" },
            {(int)eStatus.Complete, "Hoàn thành" },
            {(int)eStatus.Cancel, "Hủy" },
        };
        public static Dictionary<int, string> sPayPlan = new Dictionary<int, string>()
        {
            {(int)eStatus.Processing, "Chờ duyệt" },
            {(int)eStatus.Complete, "Đã duyệt" },
            {(int)eStatus.Cancel, "Hủy" },
        };
        public enum eLevel
        {
            Level1,
            Level2,
            Level3,
            Level4,
            Level5,
            Level6,
            Level7,
        }
        public enum eYesNo
        {
            Yes,
            No
        }
        public static Dictionary<int, string> sYesNo = new Dictionary<int, string>()
        {
            {(int)eYesNo.Yes, "Có" },
            {(int)eYesNo.No, "Không" },
        };

        public enum eLoaiPhuCap
        {
            Loai1,
            Loai2,
        }
        public static Dictionary<int, string> sLoaiPhuCap = new Dictionary<int, string>()
        {
            {(int)eLoaiPhuCap.Loai1, "Loại thường" },
            {(int)eLoaiPhuCap.Loai2, "Tiêu hao" },
        };
        public enum eThuocCap
        {
            cap1,
            cap2,
        }
        public static Dictionary<int, string> sThuocCap = new Dictionary<int, string>()
        {
            {(int)eThuocCap.cap1, "Cấp thôn" },
            {(int)eThuocCap.cap2, "Cấp xã" },
        };

        public enum eLoaiTien
        {
            TienMat,
            NganHang,
        }
        public static Dictionary<int, string> sLoaiTien = new Dictionary<int, string>()
        {
            {(int)eLoaiTien.TienMat, "Tiền mặt" },
            {(int)eLoaiTien.NganHang, "Ngân hàng" },
        };
        public enum eLoaiDonThu
        {
            DeNghi,
            ToCao
        }
        public static Dictionary<int, string> sLoaiDonThu = new Dictionary<int, string>()
        {
            {(int)eLoaiDonThu.DeNghi, "Đề nghị" },
            {(int)eLoaiDonThu.ToCao, "Tố cáo" },
        };
        public enum eCachGui
        {
            CongDVC,
            CPN,
            TrucTiep,
        }
        public static Dictionary<int, string> sCachGui = new Dictionary<int, string>()
        {
            {(int)eCachGui.CongDVC, "Cổng DVC" },
            {(int)eCachGui.CPN, "Thư EVN" },
            {(int)eCachGui.TrucTiep, "Trực tiếp" },
        };
        public static string[] SoDem = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín", "mười", "mười một", "mười hai", "mười ba", "mười bốn", "mười năm" };
        public static string[] SoThu = new string[] { "nhất", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín", "mười", "mười một", "mười hai", "mười ba", "mười bốn", "mười năm" };
        public static string[] Colors = new string[] { "primary", "secondary", "success", "danger", "warning", "info", "light", "dark", "muted", "white" };
        public static string LinkDove = "http://cms.thutucnhadat.vn";

        public class cBanInTheoDoiChi
        {
            public string status_name { get; set; }

            public string note { get; set; }

            public decimal amount { get; set; }

            public string contract_name { get; set; }

            public string category_name { get; set; }

            public string fullname { get; set; }

            public string service_name { get; set; }

            public string address_name { get; set; }

        }
    }
    public class HomeModel
    {
        public int id;
        public string customer_name;
        public long sotien;
        public long phaithu;
        public long dathu;
        public DateTime? thoihan;
        public string tinhtrang;
    }
    public class ServiceDocument
    {
        public int id;
        public string name;
    }
    public class XCongViec
    {
        public enum eCategory
        {
            NhanThan,
            CongChung,
            TrichDo,
            Thue,
            OneDoor,
            NguonGoc,
        }
        public static Dictionary<int, string> sCategory = new Dictionary<int, string>()
        {
            {(int)eCategory.NhanThan, "Nhân thân" },
            {(int)eCategory.CongChung, "Công chứng" },
            {(int)eCategory.TrichDo, "Trích đo" },
            {(int)eCategory.Thue, "Thuế" },
            {(int)eCategory.OneDoor, "Một cửa" },
            {(int)eCategory.NguonGoc, "Nguồn gốc" },
        };
        public static Dictionary<int, string> sCategoryIcon = new Dictionary<int, string>()
        {
            {(int)eCategory.NhanThan, "<i class=\"fas fa-id-card\"></i>" },
            {(int)eCategory.CongChung, "<i class=\"fas fa-marker\"></i>" },
            {(int)eCategory.TrichDo, "<i class=\"fas fa-map\"></i>" },
            {(int)eCategory.Thue, "<i class=\"fas fa-tax\"></i>" },
            {(int)eCategory.OneDoor, "<i class=\"fas fa-share\"></i>" },
            {(int)eCategory.NguonGoc, "<i class=\"fas fa-source\"></i>" },
        };
    }
    public class XKPI
    {
        public enum eType
        {
            Facebook,
            Zalo,
            Group,
            Fanpage,
            Website,
            PostFacebook,
            PostZalo,
        }
        public static Dictionary<int, string> sType = new Dictionary<int, string>()
        {
            {(int)eType.Facebook, "Đăng Facebook" },
            {(int)eType.Zalo, "Đăng Zalo" },
            {(int)eType.Group, "Đăng Group FB" },
            {(int)eType.Fanpage, "Đăng Fanpage FB" },
            {(int)eType.Website, "Đăng Website" },
            {(int)eType.PostFacebook, "Kết bạn Facebook" },
            {(int)eType.PostZalo, "Kết bạn Zalo" },
        };
    }
    public class XTaiLieu
    {
        public enum eType
        {
            TaiLieu,
            DaoTao,
            QuyChe,
            MauTrang,
        }
        public static Dictionary<int, string> sType = new Dictionary<int, string>()
        {
            {(int)eType.TaiLieu, "Tài liệu" },
            {(int)eType.DaoTao, "Đào tạo" },
            {(int)eType.QuyChe, "Quy chế" },
            {(int)eType.MauTrang, "Mẫu trắng" },
        };
    }
    public class XChart
    {
        public string type { get; set; }
        public string name { get; set; }
        public string axisYType { get; set; }
        public bool showInLegend { get; set; }
        public string xValueFormatString { get; set; }
        public string yValueFormatString { get; set; }
        public XChartPoint[] dataPoints { get; set; }
    }
    public class XChartPoint
    {
        public object x { get; set; }
        public object y { get; set; }
    }
    public class xTemp
    {
        public int int1 { get; set; }
        public int int2 { get; set; }
        public int int3 { get; set; }
        public int int4 { get; set; }

        public string str1 { get; set; }
        public string str2 { get; set; }
        public string str3 { get; set; }
        public DateTime date1 { get; set; }
        public DateTime date2 { get; set; }

        public long long1 { get; set; }
        public long long2 { get; set; }
        public long long3 { get; set; }
    }
    public class ListDropDown
    {
        public int? id { get; set; }
        public string name { get; set; }
    }
    public class ListDropDownText
    {
        public string id { get; set; }
        public string name { get; set; }
    }
    public class item
    {
        public int id;
        public string name;
        public int number;
        public string text;
    }
    public class data
    {
        public IEnumerable<item> items;
        public int total_count;
    }
    public class paras
    {
        public string term;
        public string q;
        public int page;
    }
    public class search
    {
        public string Search_Data { get; set; }
        public int? account { get; set; }
        public int? contract { get; set; }
        public string Filter_Value { get; set; }
        public int? Page_No { get; set; }
    }
    public class doughnut
    {
        public long y { get; set; }
        public string label { get; set; }
    }
    public class InOutChoDuyetPrint
    {
        public string HOPDONG { get; set; }
        public string TAIKHOAN { get; set; }
        public string LOAICHITIET { get; set; }
        public decimal SOTIEN { get; set; }
        public string LYDO { get; set; }
        public string CODE { get; set; }
    }
    public class modal
    {
        public int width { get; set; }
        public int height { get; set; }
        public string url { get; set; }
    }
}