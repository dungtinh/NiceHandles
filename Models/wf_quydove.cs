namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class wf_quydove
    {
        public long id { get; set; }

        public int kid { get; set; }

        public DateTime time { get; set; }

        public int account_id { get; set; }

        [StringLength(250)]
        public string note { get; set; }
    }
}
