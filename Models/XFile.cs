namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public partial class XFile
    {
        public XFile()
        {
            obj = new File();
        }
        public File obj { get; set; }
        public enum eType
        {
            HSTiepNhan,
            HS1Cua,
            HSCTV,
            GIAPHA,
            SMKBDGT,
            P1CHD,
            PTBS,
            PXL,
            DKNTC,
            KQSL,
            ANHCHUP,
            THUE,

        }
        public static Dictionary<int, string> sType = new Dictionary<int, string>()
        {
            {(int)eType.HSTiepNhan, "Hồ sơ tiếp nhận" },
            {(int)eType.HS1Cua, "Hồ sơ 1 cửa" },
            {(int)eType.HSCTV, "File xác nhận dự phòng" },

            {(int)eType.GIAPHA, "Gia phả" },
            {(int)eType.SMKBDGT, "Sổ mục kê, BĐ giải thửa" },
            {(int)eType.P1CHD, "Phiếu 1 cửa hướng dẫn" },
            {(int)eType.PTBS, "Phiếu trả bổ sung hồ sơ" },
            {(int)eType.PXL, "Phiếu xin lỗi, hẹn lại ngày trả KQ" },
            {(int)eType.DKNTC, "Đơn phản ánh, khiếu nại, tố cáo" },
            {(int)eType.KQSL, "Kết quả sao lục" },
            {(int)eType.ANHCHUP, "Ảnh chụp" },
            {(int)eType.THUE, "Thuế, lệ phí" },
        };
        public static Dictionary<int, string> sTypeLevel = new Dictionary<int, string>()
        {
            {(int)eType.HSTiepNhan, "Scan thông tin nhập" },
            {(int)eType.HS1Cua, "Scan thông tin nhập" },
            {(int)eType.HSCTV, "Scan thông tin nhập" },
            {(int)eType.GIAPHA, "Scan thông tin nhập" },
            {(int)eType.SMKBDGT, "Scan thông tin nhập" },

            {(int)eType.P1CHD, "Phiếu VPĐK" },
            {(int)eType.PTBS, "Phiếu VPĐK" },
            {(int)eType.PXL, "Phiếu VPĐK" },
            {(int)eType.DKNTC, "Phiếu VPĐK" },
            {(int)eType.KQSL, "Phiếu VPĐK" },
            {(int)eType.ANHCHUP, "Khác" },
            {(int)eType.THUE, "Phiếu VPĐK" },
        };
    }
}
