namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("QuyDove")]
    public partial class QuyDove
    {
        public long id { get; set; }

        public long amount { get; set; }

        [StringLength(250)]
        public string note { get; set; }

        public int account_id { get; set; }

        public int type { get; set; }

        public DateTime time { get; set; }

        public int sta { get; set; }

        public int state { get; set; }
    }
}
