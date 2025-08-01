namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DuChiNhatKyWF")]
    public partial class DuChiNhatKyWF
    {
        public int id { get; set; }

        public int duchinhatky_id { get; set; }

        public int status { get; set; }

        [StringLength(350)]
        public string note { get; set; }

        public DateTime time { get; set; }

        public int account_id { get; set; }
    }
}
