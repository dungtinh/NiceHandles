using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiceHandles.Models
{
    public class XDocument
    {
        public Document obj { get; set; }
        public enum eType
        {
            SangTen,
            LanDau,
            TachThua,
            ChuyenVuon,
            DinhChinhCapDoi,
            CapLai,
            ManhTrichDo,
            DonThu,
            LoaiKhac
        }
        public static Dictionary<int, string> sType = new Dictionary<int, string>()
        {
            {(int)eType.SangTen, "Sang tên" },
            {(int)eType.LanDau, "Cấp lần đầu" },
            {(int)eType.TachThua, "Tách, hợp thửa" },
            {(int)eType.ChuyenVuon, "Chuyển vườn" },
            {(int)eType.DinhChinhCapDoi, "Đính chính/Cấp đổi" },
            {(int)eType.CapLai, "Cấp lại" },
            {(int)eType.ManhTrichDo, "Mảnh trích đo" },
            {(int)eType.DonThu, "Đơn thư" },
            {(int)eType.LoaiKhac, "Loại khác" },
        };
    }
}