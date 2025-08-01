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

    public partial class XPayplan
    {
        public enum eStatus
        {
            ChoDuyet,
            TrinhDuyet,
            DaDuyet,
            DaThucHien,
            Huy,
        }
        public static Dictionary<int, string> sStatus = new Dictionary<int, string>()
        {
            {(int)eStatus.ChoDuyet, "Chờ duyệt" },
            {(int)eStatus.TrinhDuyet, "Trình duyệt" },
            {(int)eStatus.DaDuyet, "Đã duyệt" },
            {(int)eStatus.DaThucHien, "Đã thực hiện" },
            {(int)eStatus.Huy, "Hủy" },
        };
    }
}
