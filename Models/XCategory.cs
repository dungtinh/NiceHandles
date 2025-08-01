namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Web.Mvc;

    public partial class XCategory
    {
        public XCategory()
        {
            obj = new Category();
        }
        public Category obj { get; set; }
        public SelectListItem[] categories;
        public enum eType
        {
            Chi,
            Thu,
        }
        public static Dictionary<int, string> sType = new Dictionary<int, string>()
        {
            {(int)eType.Chi, "Chi" },
            {(int)eType.Thu, "Thu" },
        };
        public enum eParent
        {
            VanPhong,
            KhachHang,
        }

        public static Dictionary<int, string> sParent = new Dictionary<int, string>()
        {
            {(int)eParent.VanPhong, "Ngoài hợp đồng" },
            {(int)eParent.KhachHang, "Trong hợp đồng" },
        };
        public enum eWf
        {
            Khong,
            Co,
        }

        public static Dictionary<int, string> sWf = new Dictionary<int, string>()
        {
            {(int)eWf.Co, "Có" },
            {(int)eWf.Khong, "Không" },
        };
        public enum eKind
        {
            UnKind,
            NO,
            Quy,
            Dove,
            Rose,
            Remunerate,
            Partner
        }

        public static Dictionary<int, string> sKind = new Dictionary<int, string>()
        {
            {(int)eKind.UnKind, "Không phân loại" },
            {(int)eKind.NO, "Tiền nợ" },
            {(int)eKind.Quy, "Tiền quỹ" },
            {(int)eKind.Dove, "Đo vẽ" },
            {(int)eKind.Rose, "Hoa hồng" },
            {(int)eKind.Remunerate, "Thưởng" },
            {(int)eKind.Partner, "Đối tác" },
        };
        public enum ePair
        {
            UnPair,
            VayTra,
            TamUng,
            Stock,
            Exchange,
        }

        public static Dictionary<int, string> sPair = new Dictionary<int, string>()
        {
            {(int)ePair.UnPair, "Không phân loại" },
            {(int)ePair.VayTra, "Vay trả" },
            {(int)ePair.TamUng, "Tạm ứng" },
            {(int)ePair.Stock, "Cổ phần" },
            {(int)ePair.Exchange, "Chuyển đổi" },
        };
        public enum eNhom
        {
            Thu,
            VanPhong,
            ThietBi,
            ChoVay,
            CoTuc,
            Luong,
            HoSo,
            Marketing,
            HR,
            Khac,
        }

        public static Dictionary<int, string> sNhom = new Dictionary<int, string>()
        {
            {(int)eNhom.Thu, "Thu" },
            {(int)eNhom.VanPhong, "Chi văn phòng" },
            {(int)eNhom.ThietBi, "Chi trang thiết bị" },
            {(int)eNhom.ChoVay, "Chi cho vay" },
            {(int)eNhom.CoTuc, "Chi cổ tức" },
            {(int)eNhom.Luong, "Chi trả lương" },
            {(int)eNhom.HoSo, "Chi hồ sơ" },
            {(int)eNhom.Marketing, "Chi marketing" },
            {(int)eNhom.HR, "Chi HR" },
            {(int)eNhom.Khac, "Khác" },
        };
        public enum eDuyet
        {
            Co,
            Khong
        }

        public static Dictionary<int, string> sDuyet = new Dictionary<int, string>()
        {
            {(int)eDuyet.Co, "Có" },
            {(int)eDuyet.Khong, "Không" },
        };
        public enum eTheoDoi
        {
            Co,
            Khong
        }

        public static Dictionary<int, string> sTheoDoi = new Dictionary<int, string>()
        {
            {(int)eTheoDoi.Co, "Có" },
            {(int)eTheoDoi.Khong, "Không" },
        };
        public enum eTamUng
        {
            No,
            Yes
        }

        public static Dictionary<int, string> sTamUng = new Dictionary<int, string>()
        {
            {(int)eTamUng.No, "No" },
            {(int)eTamUng.Yes, "Yes" },
        };
    }
}
