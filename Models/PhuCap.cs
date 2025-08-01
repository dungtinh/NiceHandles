namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PhuCap")]
    public partial class PhuCap
    {
        public int id { get; set; }

        public int account_id { get; set; }

        [Required]
        [StringLength(150)]
        public string ten { get; set; }

        public int sotien { get; set; }

        public int loai { get; set; }

        public DateTime ngaythang { get; set; }

        public int trangthai { get; set; }
    }
}
