namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BangChamCong")]
    public partial class BangChamCong
    {
        public int id { get; set; }

        public int account_id { get; set; }

        public DateTime thoigian { get; set; }

        public int ngay { get; set; }

        public int cong { get; set; }

        [StringLength(250)]
        public string note { get; set; }
    }
}
