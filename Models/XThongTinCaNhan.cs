using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiceHandles.Models
{
    public class XThongTinCaNhan
    {
        public ThongTinCaNhan obj { get; set; }
        public string ngaysinh { get; set; }
        public string ngaycap_gt { get; set; }
        public string ngaysinh1 { get; set; }
        public string ngaycap_gt1 { get; set; }

        public string ngaychet { get; set; }
        public string ngaychet1 { get; set; }

        public enum eType
        {
            LanDau,
            TachThua,
            HopThua,
            UyQuyen
        }
        public static Dictionary<int, string> sType = new Dictionary<int, string>()
        {
            {(int)eType.LanDau, "Lần đầu" },
            {(int)eType.TachThua, "Tách thửa" },
            {(int)eType.HopThua, "Hợp thửa" },
            {(int)eType.UyQuyen, "Ủy quyền" },
        };
        public enum eQuanHe
        {
            BoMeChong,
            BoMeVo,
            VoChong,
            Con,
            Chau,
        }
        public static Dictionary<int, string> sQuanHe = new Dictionary<int, string>()
        {
            {(int)eQuanHe.BoMeChong, "Bố mẹ chồng" },
            {(int)eQuanHe.BoMeVo, "Bố mẹ vợ" },
            {(int)eQuanHe.VoChong, "Vợ chồng" },
            {(int)eQuanHe.Con, "Con" },
            {(int)eQuanHe.Chau, "Cháu" },
        };
        public enum eHangThuaKe
        {
            ChuDat,
            HangThuaKe1,
            HangThuaKe2,
        }
        public static Dictionary<int, string> sHangThuaKe = new Dictionary<int, string>()
        {
            {(int)eHangThuaKe.ChuDat, "Chủ đất" },
            {(int)eHangThuaKe.HangThuaKe1, "Hàng thừa kế thứ nhất" },
            {(int)eHangThuaKe.HangThuaKe2, "Hàng thừa kế thứ hai" },
        };
    }
}