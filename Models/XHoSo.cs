using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiceHandles.Models
{
    public class XHoSo
    {
        public enum eStatus
        {
            Processing,
            Complete,
            Cancel
        }
        public static Dictionary<int, string> sStatus = new Dictionary<int, string>()
        {
            {(int)eStatus.Processing, "Đang thực hiện" },
            {(int)eStatus.Complete, "Hoàn thành" },
            {(int)eStatus.Cancel, "Hủy" },
        };
        public enum eNgoaiGiao
        {
            NgoaiGiao,
            DonThu
        }
        public static Dictionary<int, string> sNgoaiGiao = new Dictionary<int, string>()
        {
            {(int)eNgoaiGiao.NgoaiGiao, "Ngoại giao" },
            {(int)eNgoaiGiao.DonThu, "Đơn thư" },
        };
        public enum eView
        {
            CuaToi,
            Nhom,
            Manager
        }
        public static Dictionary<int, string> sView = new Dictionary<int, string>()
        {
            {(int)eView.CuaToi, "Của tôi" },
            {(int)eView.Nhom, "Nhóm" },
            {(int)eView.Manager, "Quản lý" },
        };
        public enum eStep
        {
            HoSoMoi,
            KiemTraGiaoViec,
            NhapThongTin,
            DoVeUyQuyen,
            ThamDinhTrichDo,
            ChoBoSung,
            HoanThien,
            Nop1Cua,
            DaHoanTat,
        }
        public static Dictionary<int, string> sStep = new Dictionary<int, string>()
        {
            {(int)eStep.HoSoMoi,"Hồ sơ mới" },
            {(int)eStep.KiemTraGiaoViec,"Kiểm tra, giao việc" },
            {(int)eStep.NhapThongTin,"Nhập thông tin" },
            {(int)eStep.DoVeUyQuyen,"Đo vẽ, ủy quyền" },
            {(int)eStep.ThamDinhTrichDo,"Thẩm định trích đo" },
            {(int)eStep.ChoBoSung,"Chờ bổ sung" },
            {(int)eStep.HoanThien,"Hoàn thiện" },
            {(int)eStep.Nop1Cua,"Nộp 1 cửa" },
            {(int)eStep.DaHoanTat,"Đã hoàn tất" },
        };
    }
}