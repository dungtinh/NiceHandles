namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class wf_hoso_step
    {
        public long id { get; set; }

        public long hoso_id { get; set; }

        public DateTime time { get; set; }

        [StringLength(500)]
        public string note { get; set; }

        public int account_id { get; set; }

        public int step_id { get; set; }

        public int? next_step { get; set; }
    }
}
