using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiceHandles.Models
{
    public class XAccount
    {
        public Account obj { get; set; }
        public SelectListItem[] roles { get; set; }
        public string role { get; set; }
        public enum eStatus
        {
            Processing,
            Cancel
        }
        public static Dictionary<int, string> sStatus = new Dictionary<int, string>()
        {
            {(int)eStatus.Processing, "Đang thực hiện" },
            {(int)eStatus.Cancel, "Hủy" },
        };
    }
}