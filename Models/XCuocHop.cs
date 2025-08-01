namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Web.Mvc;

    public partial class XCuocHop
    {
        public XCuocHop()
        {
            obj = new CuocHop();
        }
        public CuocHop obj { get; set; }

        public enum eLoai
        {
            HopNguonGoc,
            HopToDo,
            Khac
        }
        public static Dictionary<int, string> sLoai = new Dictionary<int, string>()
        {
            {(int)eLoai.HopNguonGoc, "Họp nguồn gốc" },
            {(int)eLoai.HopToDo, "Họp bản vẽ" },
            {(int)eLoai.Khac, "Loại khác" },
        };
        public enum eMucDo
        {
            BinhThuong,
            Gap,
        }
        public static Dictionary<int, string> sMucDo = new Dictionary<int, string>()
        {
            {(int)eMucDo.BinhThuong, "Bình thường" },
            {(int)eMucDo.Gap, "Gấp" },
        };
        public enum eStatus
        {
            Cho,
            Pending,
            Duyet,
            HoiDong,
        }
        public static Dictionary<int, string> sStatus = new Dictionary<int, string>()
        {
            {(int)eStatus.Cho, "Chưa duyệt" },
            {(int)eStatus.Pending, "Xem xét" },
            {(int)eStatus.Duyet, "Giang duyệt" },
            {(int)eStatus.HoiDong, "Hội đồng duyệt" },
        };
    }
}
