using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiceHandles.Models
{
    public class XViecPhaiLam
    {
        public static ViecPhaiLam CreateModel(int hoso_id, int account_id, int progress_type, int day)
        {
            var obj = new ViecPhaiLam();
            obj.hoso_id = hoso_id;
            obj.name = "Lịch nhắc tự động";
            obj.progress_type = progress_type;
            obj.time = DateTime.Now;
            obj.time_progress = DateTime.Now.AddDays(day);
            obj.account_id = account_id;
            return obj;
        }
    }
}