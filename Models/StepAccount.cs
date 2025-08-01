namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StepAccount")]
    public partial class StepAccount
    {
        public int id { get; set; }

        public int account_id { get; set; }

        public int step_id { get; set; }
    }
}
