namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InOutChoDuyet")]
    public partial class InOutChoDuyet
    {
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        [StringLength(250)]
        public string note { get; set; }

        public DateTime time { get; set; }

        public int status { get; set; }
    }
}
