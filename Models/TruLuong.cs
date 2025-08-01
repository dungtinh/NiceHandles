namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TruLuong")]
    public partial class TruLuong
    {
        public int id { get; set; }

        public int account_id { get; set; }

        public DateTime thoigian { get; set; }

        public int sotien { get; set; }

        [Required]
        [StringLength(150)]
        public string note { get; set; }
    }
}
