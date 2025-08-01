using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiceHandles.Models
{
    public class Xdidiachinh
    {
        public didiachinh obj { get; set; }
        public Contract contract { get; set; }
        public Service service { get; set; }
        public Infomation infomation { get; set; }
        public Customer customer { get; set; }
        public Address address { get; set; }
        public enum eIsXa
        {
            XA,
            VPDK
        }
        public static Dictionary<int, string> sIsXa = new Dictionary<int, string>()
        {
            {(int)eIsXa.XA, "UBND xã" },
            {(int)eIsXa.VPDK, "VPĐK" },
        };
    }
}