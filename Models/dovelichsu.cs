namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("dovelichsu")]
    public partial class dovelichsu
    {
        public int id { get; set; }

        public int hoso_id { get; set; }

        public int account_id { get; set; }

        [Required]
        [StringLength(500)]
        public string name { get; set; }

        [StringLength(250)]
        public string url { get; set; }

        public DateTime time { get; set; }

        public DateTime? time_exp { get; set; }

        public int dove_account { get; set; }
    }
}
