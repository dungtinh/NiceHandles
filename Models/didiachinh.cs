namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("didiachinh")]
    public partial class didiachinh
    {
        public int id { get; set; }

        public int contract_id { get; set; }

        public DateTime ngaynop { get; set; }

        public int noinop { get; set; }

        public int is_xa { get; set; }

        public int trangthai { get; set; }
    }
}
