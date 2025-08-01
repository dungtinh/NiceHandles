namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Reporter")]
    public partial class Reporter
    {
        public long id { get; set; }

        public int account_id { get; set; }

        public DateTime time { get; set; }

        [Required]
        [StringLength(50)]
        public string thoigian { get; set; }

        [Required]
        [StringLength(250)]
        public string name { get; set; }

        [Required]
        [StringLength(500)]
        public string description { get; set; }

        [StringLength(150)]
        public string result { get; set; }

        [StringLength(250)]
        public string note { get; set; }
    }
}
