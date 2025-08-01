namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GiaoNhanGiayTo")]
    public partial class GiaoNhanGiayTo
    {
        public int id { get; set; }

        public int contract_id { get; set; }

        public int account_id { get; set; }

        [Required]
        [StringLength(250)]
        public string giayto { get; set; }

        [StringLength(50)]
        public string noinhan { get; set; }

        [StringLength(50)]
        public string nguoinhan { get; set; }

        public DateTime ngaygiao { get; set; }

        public DateTime? ngayhan { get; set; }

        public int trangthai { get; set; }
    }
}
