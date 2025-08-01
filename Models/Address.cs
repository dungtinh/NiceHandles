namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Address")]
    public partial class Address
    {
        public int id { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        [StringLength(50)]
        public string code { get; set; }

        public int? parent_id { get; set; }
    }
}
