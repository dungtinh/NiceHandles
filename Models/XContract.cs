using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NiceHandles.Models
{
    public class XContract
    {
        public List<HttpPostedFileBase> HardFileLinks { get; set; }
        public Contract contract { get; set; }
        public HoSo hoso { get; set; }        

        public enum eStatus
        {
            Processing,
            Complete,
            Cancel
        }
        public static Dictionary<int, string> sStatus = new Dictionary<int, string>()
        {
            {(int)eStatus.Processing, "Đang thực hiện" },
            {(int)eStatus.Complete, "Đã xong" },
            {(int)eStatus.Cancel, "Hủy" },
        };
        public enum eType
        {
            Normal,
            P4,
        }
        public enum eLoai
        {
            CN,
            CT,
        }
        public static Dictionary<int, string> sType = new Dictionary<int, string>()
        {
            {(int)eType.Normal, "Thường" },
            {(int)eType.P4, "Trang 4" },
        };
        public static Dictionary<int, string> sLoai = new Dictionary<int, string>()
        {
            {(int)eLoai.CN, "Cá nhân" },
            {(int)eLoai.CT, "Công ty" },
        };
        public enum eWorkflow
        {
            DiaChinh,
            MotCua,
            HoanThanh
        }
        public static Dictionary<int, string> sWorkflow = new Dictionary<int, string>()
        {
            {(int)eWorkflow.DiaChinh, "Lấy thông tin địa chính" },
            {(int)eWorkflow.MotCua, "Nộp 1 cửa" },
            {(int)eWorkflow.HoanThanh, "Hoàn thành" },
        };

        public enum ePriority
        {
            Low,
            Normal,
            High,
            Important,
        }
        public static Dictionary<int, string> sPriority = new Dictionary<int, string>()
        {
            {(int)ePriority.Normal, "Thường" },
            {(int)ePriority.Low, "Thấp" },
            {(int)ePriority.High, "Cao" },
            {(int)ePriority.Important, "Quan trọng" },
        };

    }
}