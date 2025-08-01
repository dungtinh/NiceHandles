using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiceHandles.Models
{
    public class XInOutChoDuyet
    {
        public enum eStatus
        {
            Processing,
            Complete,
            Cancel
        }
        public static Dictionary<int, string> sStatus = new Dictionary<int, string>()
        {
            {(int)eStatus.Processing, "Đang thực hiện" },
            {(int)eStatus.Complete, "Hoàn thành" },
            {(int)eStatus.Cancel, "Hủy" },
        };
    }
}