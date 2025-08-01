namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class wf_hoso_note
    {
        public long id { get; set; }

        public long wf_hoso_id { get; set; }

        public DateTime time { get; set; }

        [StringLength(500)]
        public string note { get; set; }
    }
}
