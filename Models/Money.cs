namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Money")]
    public partial class Money
    {
        public long id { get; set; }

        public long amount { get; set; }

        [Required]
        [StringLength(36)]
        public string code { get; set; }
    }
}
