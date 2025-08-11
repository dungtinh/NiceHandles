using System.Collections.Generic;
using NiceHandles.Models;

namespace NiceHandles.ViewModels
{
    public class InfomationEditViewModel
    {
        public HoSo HoSo { get; set; }
        public List<PersonInfo> Owners { get; set; } = new List<PersonInfo>();
        public List<PersonInfo> BuyersOrSellers { get; set; } = new List<PersonInfo>();
        public LandParcel LandParcel { get; set; }
        public VariationInfo VariationInfo { get; set; } // Chỉ một biến động
    }
}