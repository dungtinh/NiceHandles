namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class wf_hoso
    {
        public long id { get; set; }

        public int hoso_id { get; set; }

        [StringLength(500)]
        public string note { get; set; }

        public DateTime time { get; set; }

        public int account_id { get; set; }

        public long step_id { get; set; }

        public int? next_step { get; set; }
    }
}
