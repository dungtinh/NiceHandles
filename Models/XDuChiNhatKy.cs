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

    public partial class XDuChiNhatKy
    {
        public enum eStatus
        {
            LuongGiu,
            DiThucHien,
            DaDi,
            TraLai,
        }
        public static Dictionary<int, string> sStatus = new Dictionary<int, string>()
        {
            {(int)eStatus.LuongGiu, "Lương giữ" },
            {(int)eStatus.DiThucHien, "Đi thực hiện" },
            {(int)eStatus.DaDi, "Đã thực hiện" },
            {(int)eStatus.TraLai, "Trả lại" },
        };
    }
}
