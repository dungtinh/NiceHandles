namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Dove")]
    public partial class Dove
    {
        public int id { get; set; }

        public int hoso_id { get; set; }

        public int status { get; set; }

        public DateTime time { get; set; }

        [StringLength(250)]
        public string scan_bia { get; set; }

        public DateTime? time_start { get; set; }

        public DateTime? time_tradiem { get; set; }

        public DateTime? time_vexong { get; set; }

        public DateTime? time_dongdau { get; set; }

        public DateTime? time_thamdinh { get; set; }

        [StringLength(500)]
        public string note { get; set; }

        public int account_id { get; set; }
    }
}
