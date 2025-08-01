namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("nhatkydonthu")]
    public partial class nhatkydonthu
    {
        public long id { get; set; }

        public int donthu_id { get; set; }

        public int account_id { get; set; }

        public DateTime time { get; set; }

        [Required]
        [StringLength(250)]
        public string note { get; set; }

        public DateTime time_exp { get; set; }
    }
}
