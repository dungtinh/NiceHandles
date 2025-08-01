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

    public partial class XStep
    {
        public XStep()
        {
            obj = new Step();
        }
        public Step obj { get; set; }
        public List<HttpPostedFileBase> f { get; set; }
        public int t { get; set; }
        public long id { get; set; }
        public enum ePhong
        {
            TiepNhan,
            DoVe,
            ThamDinhTrichDo,
            HoanThien,
            HSLuu,
        }
        public static Dictionary<int, string> sYesNo = new Dictionary<int, string>()
        {
            {(int)ePhong.TiepNhan, "Phòng tiếp nhận" },
            {(int)ePhong.DoVe, "Phòng đo vẽ" },
            {(int)ePhong.ThamDinhTrichDo, "Phòng thẩm định trích đo" },
            {(int)ePhong.HoanThien, "Phòng hoàn thiện HS" },
            {(int)ePhong.HSLuu, "Phòng lưu trữ" },
        };
    }
}
