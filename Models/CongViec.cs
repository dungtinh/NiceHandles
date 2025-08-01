namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CongViec")]
    public partial class CongViec
    {
        public int id { get; set; }

        public int category_id { get; set; }

        public int service_id { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        public int sort { get; set; }

        public int required { get; set; }

        public int? step_id { get; set; }
    }
}
