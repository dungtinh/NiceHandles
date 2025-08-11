using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiceHandles.Models
{
    public enum HoSoPersonRole
    {
        Owner = 0,          // Chủ sở hữu (chủ đất, chủ GCN)
        Buyer = 1,          // Người mua, nhận tặng cho
        Heir = 2,          // Người thừa kế        
        HouseholdMember = 3,// Nhân khẩu trong hộ khẩu        
        Other = 99          // Khác/Không xác định
    }
}