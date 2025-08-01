using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NiceHandles.Models
{
    public class Xdi1cua
    {
        public di1cua obj { get; set; }
        public Contract contract { get; set; }
        public Service service { get; set; }
        public Infomation infomation { get; set; }
        public Customer customer { get; set; }
        public CanBo canbo { get; set; }
        public CanBo motcua { get; set; }
        public bool hasVBDI { get; set; }
        public bool hasVBDEN { get; set; }
        public enum eStatus
        {
            ChoNop,
            DaNop,
            TraLai,
            HoanThanh,
            Ra1Cua,
            ThanhCong,

        }
        public static Dictionary<int, string> sStatus = new Dictionary<int, string>()
        {
            {(int)eStatus.ChoNop, "Chờ nộp" },
            {(int)eStatus.DaNop, "Đã nộp" },
            {(int)eStatus.TraLai, "Trả lại" },
            {(int)eStatus.HoanThanh, "Hoàn thành" },
            {(int)eStatus.Ra1Cua, "Ra 1 cửa" },
            {(int)eStatus.ThanhCong, "Thành công" },
        };

        public enum eFileType
        {
            TBVPDK,
            DDN,
        }
        public static Dictionary<int, string> sFileType = new Dictionary<int, string>()
        {
            {(int)eFileType.TBVPDK, "TB từ VPĐK" },
            {(int)eFileType.DDN, "Đơn đã nộp" },
        };

        public static Dictionary<int, string> sStatusColor = new Dictionary<int, string>()
        {
            {(int)eStatus.ChoNop, "<span class='btn btn-warning' style='width: 75px;'>Chờ nộp<span>" },
            {(int)eStatus.DaNop, "<span class='btn btn-success' style='width: 75px;'>Đã nộp<span>" },
            {(int)eStatus.TraLai, "<span class='btn btn-danger' style='width: 75px;'>Trả lại<span>" },
            {(int)eStatus.HoanThanh, "<span class='btn btn-primary' style='width: 75px;'>Hoàn thành<span>" },
            {(int)eStatus.Ra1Cua, "<span class='btn btn-info' style='width: 75px;'>Ra 1 cửa<span>" },
            {(int)eStatus.ThanhCong, "<span class='btn btn-muted' style='width: 75px;'>Thành công<span>" },
        };

        public enum eCanhBao
        {
            Khong,
            NhacNho,
            PhanAnh,
            ToCao,

        }
        public static Dictionary<int, string> sCanhBao = new Dictionary<int, string>()
        {
            {(int)eCanhBao.Khong, "-" },
            {(int)eCanhBao.NhacNho, "Nhắc nhở" },
            {(int)eCanhBao.PhanAnh, "Phản ánh" },
            {(int)eCanhBao.ToCao, "Tố cáo" },
        };
        public static Dictionary<int, string> sCanhBaoColor = new Dictionary<int, string>()
        {
            {(int)eCanhBao.Khong, "<a href='#' class='canhbao text-dark'>-</a>" },
            {(int)eCanhBao.NhacNho, "<a href='#' class='canhbao text-success'>Nhắc nhở</a>" },
            {(int)eCanhBao.PhanAnh, "<a href='#' class='canhbao text-warning'>Phản ánh</a>" },
            {(int)eCanhBao.ToCao, "<a href='#' class='canhbao text-danger'>Tố cáo</a>" },
        };

    }
}