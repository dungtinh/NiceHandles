namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class wf_contract
    {
        public long id { get; set; }

        public int contract_id { get; set; }

        public int status { get; set; }

        [StringLength(500)]
        public string note { get; set; }

        public DateTime time { get; set; }

        public int account_id { get; set; }

        public int? step_id { get; set; }

        public DateTime? time_over { get; set; }

        public long? from_id { get; set; }
    }
}
