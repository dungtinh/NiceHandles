namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("task")]
    public partial class task
    {
        public long id { get; set; }

        [Required]
        [StringLength(500)]
        public string name { get; set; }

        public long job_id { get; set; }

        public int account_id_created { get; set; }

        public int account_id { get; set; }

        public DateTime time { get; set; }

        public DateTime time_exp { get; set; }

        public int? progress_type { get; set; }

        public int? label { get; set; }

        [StringLength(500)]
        public string note { get; set; }

        public int status { get; set; }
    }
}
