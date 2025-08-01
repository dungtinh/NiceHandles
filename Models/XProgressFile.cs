using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiceHandles.Models
{
    public class XProgressFile
    {
        public enum eType
        {
            UnType,
            GiayChungNhan,
            HSTiepNhan,
            BienBanHop,
            Flycam,
            QuyHoach,
            AutoCad,
            ManhTrichDo,
            HSMotCua,
            PhieuTiepNhan,
            VanBan1CuaDi,
            VanBan1CuaDen,
            VanBanNhacNho,
            VanBanThue,
            DonThuDi,
            DonThuDen,
            KetQua
        }
        public static Dictionary<int, string> sType = new Dictionary<int, string>()
        {
            {(int)eType.UnType, "Không phân loại" },
            {(int)eType.GiayChungNhan, "Giấy chứng nhận" },
            {(int)eType.HSTiepNhan, "HS tiếp nhận" },
            {(int)eType.BienBanHop, "Biên bản họp" },
            {(int)eType.Flycam, "Ảnh Flycam" },
            {(int)eType.QuyHoach, "Quy hoạch" },
            {(int)eType.AutoCad, "File AutoCad" },
            {(int)eType.ManhTrichDo, "Mảnh trích đo" },
            {(int)eType.HSMotCua, "HS 1 cửa" },
            {(int)eType.PhieuTiepNhan, "Phiếu tiếp nhận và hẹn ngày trả kết quả" },
            {(int)eType.VanBan1CuaDi, "VB 1 cửa đi" },
            {(int)eType.VanBan1CuaDen, "VB 1 cửa đến" },
            {(int)eType.VanBanNhacNho, "VB nhắc nhở" },
            {(int)eType.VanBanThue, "Văn bản thuế" },
            {(int)eType.DonThuDi, "Đơn thư đi" },
            {(int)eType.DonThuDen, "Đơn thư đến" },
            {(int)eType.KetQua, "Kết quả" },
        };
    }
}