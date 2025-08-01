namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LanDau")]
    public partial class LanDau
    {
        public int id { get; set; }

        public int? cogiayto { get; set; }
    }
}
