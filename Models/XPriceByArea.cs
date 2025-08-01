using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiceHandles.Models
{
    public class XPriceByArea
    {
        public PriceByArea obj { get; set; }
        public enum eType
        {
            DDNoi,
            DDNgoai,
            CLNoi,
            CLNgoai,
            TL,
        }
        public static Dictionary<int, string> sType = new Dictionary<int, string>()
        {
            {(int)eType.DDNoi, "Đo đạc nội nghiệp" },
            {(int)eType.DDNgoai, "Đo đạc ngoại nghiệp" },
            {(int)eType.CLNoi, "Chỉnh lý nội nghiệp" },
            {(int)eType.CLNgoai, "Chỉnh lý ngoại nghiệp" },
            {(int)eType.TL, "Trích lục hồ sơ địa chính" },
        };
    }
}