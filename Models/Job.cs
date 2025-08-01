namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Job")]
    public partial class Job
    {
        public long id { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        public DateTime start_date { get; set; }

        public DateTime exp_date { get; set; }

        public int created_by { get; set; }

        public int process_by { get; set; }

        [StringLength(500)]
        public string note { get; set; }

        public int status { get; set; }

        public int? hoso_id { get; set; }

        public int? progress_type { get; set; }

        public int? label { get; set; }
    }
}
