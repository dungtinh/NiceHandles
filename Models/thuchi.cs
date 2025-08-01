namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("thuchi")]
    public partial class thuchi
    {
        public long id { get; set; }

        public int account_id { get; set; }

        public DateTime thoigian { get; set; }

        public long sotien { get; set; }

        [StringLength(150)]
        public string lydo { get; set; }

        public int loai { get; set; }

        public int trangthai { get; set; }

        public DateTime? ngaychotso { get; set; }
    }
}
