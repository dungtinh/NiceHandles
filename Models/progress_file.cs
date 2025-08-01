namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class progress_file
    {
        public int id { get; set; }

        public int hoso_id { get; set; }

        public int progress_id { get; set; }

        [Required]
        [StringLength(250)]
        public string name { get; set; }

        [Required]
        [StringLength(250)]
        public string url { get; set; }

        public int type { get; set; }

        public int category { get; set; }
    }
}
