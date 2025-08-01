namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DuChi")]
    public partial class DuChi
    {
        public int id { get; set; }

        public int category_id { get; set; }

        public long amount { get; set; }
    }
}
