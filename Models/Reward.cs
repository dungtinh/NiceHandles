namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Reward")]
    public partial class Reward
    {
        public int id { get; set; }

        public int contract_id { get; set; }

        public int account_id { get; set; }

        public long? amount { get; set; }

        [StringLength(250)]
        public string note { get; set; }

        public int? sta { get; set; }

        public DateTime? created_date { get; set; }

        public DateTime? decision_date { get; set; }
    }
}
