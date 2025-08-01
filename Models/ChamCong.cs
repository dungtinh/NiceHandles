namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ChamCong")]
    public partial class ChamCong
    {
        public int id { get; set; }

        public int user_id { get; set; }

        public DateTime time { get; set; }
    }
}
