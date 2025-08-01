namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("chotso")]
    public partial class chotso
    {
        public int id { get; set; }

        public DateTime ngaythang { get; set; }

        public int account_id { get; set; }

        [StringLength(500)]
        public string ghichu { get; set; }

        public long tienthu { get; set; }

        public long tienchi { get; set; }

        public long tiencon { get; set; }

        public long tienchenhlech { get; set; }

        public long nogiang { get; set; }

        public long noduy { get; set; }

        public long notin { get; set; }

        public long nokhac { get; set; }

        public int sta { get; set; }
    }
}
