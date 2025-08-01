namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Val")]
    public partial class Val
    {
        public long id { get; set; }

        public int contract_id { get; set; }

        public int field_id { get; set; }

        [StringLength(500)]
        public string value { get; set; }
    }
}
