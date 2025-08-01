namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Progress")]
    public partial class Progress
    {
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string title { get; set; }

        public int? phantram { get; set; }

        [StringLength(150)]
        public string description { get; set; }

        [StringLength(250)]
        public string note { get; set; }

        public int status { get; set; }

        public int type { get; set; }

        public int sort { get; set; }
    }
}
