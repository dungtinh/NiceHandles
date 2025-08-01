namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Social")]
    public partial class Social
    {
        public int id { get; set; }

        public int kpi_id { get; set; }

        public int? type { get; set; }

        [StringLength(150)]
        public string name { get; set; }

        [StringLength(150)]
        public string id_no { get; set; }

        public int? nghenghiep { get; set; }

        [StringLength(350)]
        public string note { get; set; }
    }
}
