namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Diary")]
    public partial class Diary
    {
        public long id { get; set; }

        public DateTime ngaythang { get; set; }

        [Required]
        [StringLength(500)]
        public string noidung { get; set; }

        public int account_id { get; set; }
    }
}
