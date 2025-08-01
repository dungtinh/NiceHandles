namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class wf_inout
    {
        public long id { get; set; }

        public long kid { get; set; }

        public int status { get; set; }

        public DateTime time { get; set; }

        public int account_id { get; set; }

        [StringLength(150)]
        public string note { get; set; }

        public long? intout_id { get; set; }
    }
}
