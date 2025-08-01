namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KPI")]
    public partial class KPI
    {
        public int id { get; set; }

        public DateTime ngaythang { get; set; }

        public int user_id { get; set; }

        [StringLength(2000)]
        public string ghichu { get; set; }
    }
}
