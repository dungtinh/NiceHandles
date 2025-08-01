namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CoQuanGiaiQuyet")]
    public partial class CoQuanGiaiQuyet
    {
        public int id { get; set; }

        [StringLength(50)]
        public string code { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }
    }
}
