namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;
    using System.Web.Mvc;

    public partial class XBangChamCong
    {
        public enum eCong
        {
            CaNgay,
            NuaNgay,
            CoPhep,
            KhongPhep
        }
        public static Dictionary<int, string> sCong = new Dictionary<int, string>()
        {
            {(int)eCong.CaNgay, "Cả ngày" },
            {(int)eCong.NuaNgay, "Nửa ngày" },
            {(int)eCong.CoPhep, "Nghỉ có phép" },
            {(int)eCong.KhongPhep, "Nghỉ không phép" },
        };
        public static Dictionary<int, string> sCongMau = new Dictionary<int, string>()
        {
            {(int)eCong.CaNgay, "green" },
            {(int)eCong.NuaNgay, "yellowgreen" },
            {(int)eCong.CoPhep, "gray" },
            {(int)eCong.KhongPhep, "red" },
        };
        public static Dictionary<int, string> sCongMa = new Dictionary<int, string>()
        {
            {(int)eCong.CaNgay, "C" },
            {(int)eCong.NuaNgay, "1/2" },
            {(int)eCong.CoPhep, "P" },
            {(int)eCong.KhongPhep, "0" },
        };
    }
}
