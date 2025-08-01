namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ViecPhaiLam")]
    public partial class ViecPhaiLam
    {
        public int id { get; set; }

        public int hoso_id { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        public int progress_type { get; set; }

        public DateTime time { get; set; }

        public DateTime time_progress { get; set; }

        [StringLength(150)]
        public string result { get; set; }

        public int bell_type { get; set; }

        public int account_id { get; set; }

        public int account_id_created { get; set; }
    }
}
