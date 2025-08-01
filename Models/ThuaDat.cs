namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ThuaDat")]
    public partial class ThuaDat
    {
        public int id { get; set; }

        public int infomation_id { get; set; }

        public int type { get; set; }

        [StringLength(50)]
        public string sogiaychungnhan { get; set; }

        [StringLength(50)]
        public string sothua { get; set; }

        [StringLength(50)]
        public string tobando { get; set; }

        [StringLength(250)]
        public string diachithuadat { get; set; }

        [StringLength(50)]
        public string dientich { get; set; }

        [StringLength(150)]
        public string hinhthucsudung { get; set; }

        [StringLength(150)]
        public string mucdichsudung { get; set; }

        [StringLength(150)]
        public string nguongoc { get; set; }

        [StringLength(150)]
        public string ghichu { get; set; }

        public DateTime? ngaycap { get; set; }

        [StringLength(150)]
        public string noicap { get; set; }

        [StringLength(50)]
        public string sovaoso { get; set; }

        [StringLength(50)]
        public string loaidat1 { get; set; }

        [StringLength(50)]
        public string dientich1 { get; set; }

        [StringLength(50)]
        public string loaidat2 { get; set; }

        [StringLength(50)]
        public string dientich2 { get; set; }

        public int? csh1 { get; set; }

        public int? csh2 { get; set; }
    }
}
