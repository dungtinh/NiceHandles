namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Step")]
    public partial class Step
    {
        public int id { get; set; }

        public int category_id { get; set; }

        [StringLength(20)]
        public string code { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        public string description { get; set; }

        public int? next { get; set; }

        public int? prev { get; set; }

        [StringLength(50)]
        public string color { get; set; }

        public int? sort { get; set; }
    }
}
