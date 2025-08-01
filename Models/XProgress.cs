using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiceHandles.Models
{
    public class XProgress
    {
        public enum eType
        {
            NhiemVu,
            DieuTra,
            GiaPha,
            BanVe,
            GiayToHoTich,
            CongChung,
            GiayToXa,
            DonThu,
            GhiChu,
        }
        public static Dictionary<int, string> sType = new Dictionary<int, string>()
        {
            {(int)eType.NhiemVu, "Nhiệm vụ" },
            {(int)eType.DieuTra, "Điều tra cơ bản" },
            {(int)eType.GiaPha, "Gia phả" },
            {(int)eType.BanVe, "Bản vẽ" },
            {(int)eType.GiayToHoTich, "Giấy tờ hộ tịch" },
            {(int)eType.CongChung, "Công chứng" },
            {(int)eType.GiayToXa, "Giấy tờ xã" },
            {(int)eType.DonThu, "Đơn thư" },
            {(int)eType.GhiChu, "Ghi chú quá trình thực hiện" },
        };
        public enum eBellType
        {
            Blue,
            Yellow,
            Red,
        }
        public static Dictionary<int, string> sBellType = new Dictionary<int, string>()
        {
            {(int)eBellType.Blue, "Xanh" },
            {(int)eBellType.Yellow, "Vàng" },
            {(int)eBellType.Red, "Đỏ" },
        };
    }
}