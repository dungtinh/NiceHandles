namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DuChiNhatKy")]
    public partial class DuChiNhatKy
    {
        public int id { get; set; }

        public long inout_id { get; set; }

        public int status { get; set; }

        [StringLength(350)]
        public string note { get; set; }

        public DateTime time { get; set; }

        public int account_id { get; set; }
    }
}
