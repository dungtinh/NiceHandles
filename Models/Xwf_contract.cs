using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiceHandles.Models
{
    public class Xwf_contract
    {
        public wf_contract obj { get; set; }
        public SelectListItem[] account { get; set; }
        public string account_name { get; set; }
        public string contract_name { get; set; }
        public enum eStatus
        {
            Processing,
            Complete,
            Cancel
        }
        public static Dictionary<int, string> sStatus = new Dictionary<int, string>()
        {
            {(int)eStatus.Processing, "Đang thực hiện" },
            {(int)eStatus.Complete, "Đã hoàn thành" },
            {(int)eStatus.Cancel, "Hủy" },
        };
    }
}