namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SoHoKhau")]
    public partial class SoHoKhau
    {
        public int id { get; set; }

        public long infomation_id { get; set; }

        [Required]
        [StringLength(50)]
        public string so { get; set; }

        [StringLength(50)]
        public string tenchuho { get; set; }

        [StringLength(250)]
        public string diachi { get; set; }
    }
}
