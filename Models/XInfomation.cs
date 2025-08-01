using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiceHandles.Models
{
    public class XInfomation
    {
        public Infomation obj { get; set; }
        public static Dictionary<string, string> sLoaiGiayTo = new Dictionary<string, string>()
        {
            {"CCCD", "Căn cước công dân" },
            {"CC", "Căn cước" },
            {"CMND", "Chứng minh nhân dân" },

        };
        public static string[] sXaPhuong = new string[]
        {
            "xã","thị trấn", "phường"
        };
        public static string[] sGioiTinh = new string[]
        {
            "Nam","Nữ"
        };
        public enum eDoiTuong
        {
            A,
            B,
            C,
            D,
            E,
        }
        public enum eCSH
        {
            RIENG1,
            RIENG2,
            CHUNG1,
            CHUNG2,
        }
        public static string[] sNoidungbiendong = new string[]
        {
            "Chủ sử dụng đất",
            "Diện tích",
            "Chủ sử dụng đất và diện tích",
            "Chuyển mục đích sử dụng đất",
            "Chủ sử dụng, số thửa, số tờ bản đồ, diện tích",
            "Số thửa, số tờ bản đồ, diện tích",
        };
        public static string[] sLydobiendong = new string[]
       {
            "Nhận chuyển nhượng",
            "Nhận tặng cho",
            "Nhận thừa kế",
            "Nhận thừa kế, tặng cho",
            "Tách thửa để sử dụng",
            "Tách thửa để chuyển nhượng",
            "Tách thửa để tặng cho",
            "Hợp thửa để sử dụng",
       };
        public static string[] sLoaiHopDong = new string[]
      {
            "Hợp đồng chuyển nhượng",
            "Hợp đồng tặng cho",
            "Văn bản khai nhận di sản thừa kế",
            "Văn bản khai nhận và thỏa thuận tặng cho di sản thừa kế",
      };
        public static string[] sVitrithuadat = new string[]
      {
            "Mặt tiền đường phố",
            "Mặt đường",
            "Ngõ",
            "Hẻm",
      };

        public static string[] sLoainguongoc = new string[]
    {
            "Cấp lần đầu",
            "Có GCN",
    };
        public static string[] sLydo10 = new string[]
    {
            "Do bị mất giấy chứng nhận quyền sử dụng đất",
            "Do đo đạc xác định lại diện tích, kích thước thửa đất",
    };
        public static string[] sChuyenquyen = new string[]
     {
            "",
            "Nhận chuyển nhượng đất ",
            "Nhận thừa kế đất ",
            "Nhận tặng cho đất ",
            "Nhận thừa kế, tặng cho đất ",
     };
        public static List<Obj2STR> sNguongoc = new List<Obj2STR>()
        {
           new Obj2STR( "Cấp lần đầu", "Đất ông cha để lại" ),
            new Obj2STR( "Cấp lần đầu", "Đất xen kẹt" ),
            new Obj2STR( "Cấp lần đầu", "Đất giao trái thẩm quyền" ),
            new Obj2STR(  "Cấp lần đầu", "Đất giao" ),
            new Obj2STR("Có GCN", "Nhà nước giao đất không thu tiền sử dụng đất (DG-KTT)" ),
            new Obj2STR("Có GCN", "Nhà nước giao đất có thu tiền sử dụng đất (DG-CTT)" ),
            new Obj2STR("Có GCN", "Công nhận quyền như giao đất có thu tiền sử dụng đất (CNQ-CTT)" ),
            new Obj2STR("Có GCN", "Công nhận quyền như giao đất không thu tiền sử dụng đất (CNQ-KTT)" )
        };
        public enum eService
        {
            HopThua,
            TachThua,
            LanDau,
            ChuyenNhuong,
        }
    }
    public class Obj2STR
    {
        public Obj2STR(string type, string name)
        {
            this.type = type;
            this.name = name;
        }
        public string type;
        public string name;
    }
}