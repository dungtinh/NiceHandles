namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NguoiHop")]
    public partial class NguoiHop
    {
        public int id { get; set; }

        public int cuochop_id { get; set; }

        public int account_id { get; set; }
    }
}
