namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Setting")]
    public partial class Setting
    {
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string code { get; set; }

        public string data { get; set; }
    }
}
