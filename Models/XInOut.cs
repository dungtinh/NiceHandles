using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiceHandles.Models
{
    public class XInOut
    {
        public InOut obj { get; set; }
        public SelectListItem[] types { get; set; }
        public SelectListItem[] categories { get; set; }
        public SelectListItem[] account { get; set; }
        public long amount { get; set; }
        public string time { get; set; }
        public string account_name { get; set; }
        public string category_name { get; set; }
        public enum eStatus
        {
            ChoDuyet,
            DaDuyet,
            DaThucHien,
            Huy
        }
        public static Dictionary<int, string> sStatus = new Dictionary<int, string>()
        {
            {(int)eStatus.ChoDuyet, "Chờ duyệt" },
            {(int)eStatus.DaDuyet, "Đã duyệt" },
            {(int)eStatus.DaThucHien, "Đã thực hiện"},
            {(int)eStatus.Huy, "Hủy" },
        };
    }
    public class InoutMoney
    {
        public DateTime ngaythang { get; set; }
        public long amount { get; set; }
        public string code { get; set; }
        public string note { get; set; }
    }
}