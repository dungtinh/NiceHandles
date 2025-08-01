namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ThongTinCaNhan")]
    public partial class ThongTinCaNhan
    {
        public int id { get; set; }

        public int? sohokhau_id { get; set; }

        public int infomation_id { get; set; }

        public int? fk_id { get; set; }

        [StringLength(50)]
        public string hoten { get; set; }

        public DateTime? ngaysinh { get; set; }

        public DateTime? ngaychet { get; set; }

        [StringLength(250)]
        public string giaytochet { get; set; }

        [StringLength(50)]
        public string loaigiayto { get; set; }

        [StringLength(50)]
        public string sogiayto { get; set; }

        [StringLength(50)]
        public string gioitinh { get; set; }

        [StringLength(50)]
        public string quoctich { get; set; }

        [StringLength(250)]
        public string hktt { get; set; }

        public DateTime? ngaycap_gt { get; set; }

        [StringLength(150)]
        public string noicap_gt { get; set; }

        [StringLength(50)]
        public string masothue { get; set; }

        public int? quanhe { get; set; }

        public int? hangthuake { get; set; }

        [StringLength(50)]
        public string ghichuquanhe { get; set; }

        public int? marker { get; set; }

        [StringLength(150)]
        public string note { get; set; }

        public int? type { get; set; }

        [StringLength(50)]
        public string hoten1 { get; set; }

        public DateTime? ngaysinh1 { get; set; }

        public DateTime? ngaychet1 { get; set; }

        [StringLength(250)]
        public string giaytochet1 { get; set; }

        [StringLength(50)]
        public string loaigiayto1 { get; set; }

        [StringLength(50)]
        public string sogiayto1 { get; set; }

        [StringLength(50)]
        public string gioitinh1 { get; set; }

        [StringLength(50)]
        public string quoctich1 { get; set; }

        [StringLength(250)]
        public string hktt1 { get; set; }

        public DateTime? ngaycap_gt1 { get; set; }

        [StringLength(150)]
        public string noicap_gt1 { get; set; }

        [StringLength(50)]
        public string masothue1 { get; set; }

        public int? quanhe1 { get; set; }

        [StringLength(50)]
        public string ghichuquanhe1 { get; set; }
        public int? songtrendat { get; set; }
        
    }
}
