namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PayPlan")]
    public partial class PayPlan
    {
        public int id { get; set; }

        public int? dschoduyet_id { get; set; }

        public int account_id { get; set; }

        public int category_id { get; set; }

        public int type { get; set; }

        public int currency { get; set; }

        public long amount { get; set; }

        [StringLength(500)]
        public string note { get; set; }

        public DateTime time { get; set; }

        public int? contract_id { get; set; }

        public int status { get; set; }
    }
}
