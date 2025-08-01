namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CanBoDiaPhuong")]
    public partial class CanBoDiaPhuong
    {
        public int id { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        [Required]
        [StringLength(50)]
        public string chucvu { get; set; }

        [StringLength(150)]
        public string gmap { get; set; }

        [StringLength(50)]
        public string phoneno { get; set; }

        public int thuoccap { get; set; }

        [StringLength(250)]
        public string note { get; set; }

        public int fk_id { get; set; }
    }
}
