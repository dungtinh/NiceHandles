using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiceHandles.Models
{
    public class XJob
    {
        public Job obj { get; set; }
        public enum eStatus
        {
            New,
            Processing,
            Checking,
            Complete,
            Cancel
        }
        public static string[] sStatusColor = new string[] { "text-warning", "text-danger", "text-info", "text-primary", "text-muted" };
        public static Dictionary<int, string> sStatus = new Dictionary<int, string>()
        {
            {(int)eStatus.New, "Mới" },
            {(int)eStatus.Processing, "Đang thực hiện" },
            {(int)eStatus.Checking, "Đang kiểm tra" },
            {(int)eStatus.Complete, "Hoàn thành" },
            {(int)eStatus.Cancel, "Hủy" },
        };
    }
}