namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KPIChiTieu")]
    public partial class KPIChiTieu
    {
        public int id { get; set; }

        public int user_id { get; set; }

        public int type { get; set; }

        public int chitieu { get; set; }
    }
}
