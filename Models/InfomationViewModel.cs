using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiceHandles.Models
{
    public class InfomationViewModel
    {
        public int Id { get; set; }
        // Các trường khác của Infomation

        // Danh sách chủ sở hữu
        public List<PersonInfo> PersonInfos { get; set; }

        public InfomationViewModel()
        {
            PersonInfos = new List<PersonInfo>();
        }
    }
}