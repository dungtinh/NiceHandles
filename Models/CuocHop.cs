namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CuocHop")]
    public partial class CuocHop
    {
        public int id { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        public int type { get; set; }

        [StringLength(500)]
        public string vande { get; set; }

        [StringLength(500)]
        public string ketqua { get; set; }

        [StringLength(1000)]
        public string ghichu { get; set; }

        public int taoboi { get; set; }

        public DateTime? time_duyet { get; set; }

        public DateTime time { get; set; }

        public int hoso_id { get; set; }

        [StringLength(1000)]
        public string noidung { get; set; }

        [StringLength(250)]
        public string bienban { get; set; }

        [StringLength(250)]
        public string tailieuthamkhao { get; set; }

        public int mucdo { get; set; }

        public int status { get; set; }
    }
}
