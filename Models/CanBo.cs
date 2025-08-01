namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CanBo")]
    public partial class CanBo
    {
        public int id { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        public int address_id { get; set; }

        public int? motcua { get; set; }

        public int trangthai { get; set; }

        public int? thutu { get; set; }

        [StringLength(50)]
        public string phone { get; set; }
    }
}
