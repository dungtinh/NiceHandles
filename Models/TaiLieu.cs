namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TaiLieu")]
    public partial class TaiLieu
    {
        public int id { get; set; }

        [StringLength(150)]
        public string name { get; set; }

        [StringLength(150)]
        public string link { get; set; }

        [StringLength(250)]
        public string note { get; set; }

        public int type { get; set; }
    }
}
