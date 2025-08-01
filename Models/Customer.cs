namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Customer")]
    public partial class Customer
    {
        public int id { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        [StringLength(500)]
        public string address { get; set; }
    }
}
