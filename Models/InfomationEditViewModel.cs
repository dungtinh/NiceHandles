using System.Collections.Generic;
using NiceHandles.Models;

namespace NiceHandles.ViewModels
{
    public class InfomationEditViewModel
    {
        public HoSo HoSo { get; set; }

        // PersonInfo với các Role khác nhau
        public List<PersonInfo> Owners { get; set; } = new List<PersonInfo>(); // Role = 0: Chủ đất
        public List<PersonInfo> Buyers { get; set; } = new List<PersonInfo>(); // Role = 1: Người mua
        public List<PersonInfo> Heirs { get; set; } = new List<PersonInfo>(); // Role = 2: Người nhận thừa kế

        // Thông tin thửa đất
        public LandParcel LandParcel { get; set; }

        // Thông tin biến động
        public VariationInfo VariationInfo { get; set; }

        // Service code để check điều kiện hiển thị
        public string ServiceCode { get; set; }
    }
}