using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiceHandles.Models
{
    public class XDove
    {
        public enum eStatus
        {
            GoiDien,
            Flycam,
            DoDac,
            BienTap,
            KiemTra,
            KyChuDat,
            HoanThien,
            KetThuc
        }
        public static Dictionary<int, string> sStatus = new Dictionary<int, string>()
        {
            {(int)eStatus.GoiDien, "Gọi điện chủ nhà" },
            {(int)eStatus.Flycam, "Bay Flycam" },
            {(int)eStatus.DoDac, "Đo đạc" },
            {(int)eStatus.BienTap, "Biên tập" },
            {(int)eStatus.KiemTra, "Kiểm tra" },
            {(int)eStatus.KyChuDat, "Ký chủ sử dụng" },
            {(int)eStatus.HoanThien, "Hoàn thiện" },
            {(int)eStatus.KetThuc, "Kết thúc" },
        };
    }
}