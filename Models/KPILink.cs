namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KPILink")]
    public partial class KPILink
    {
        public int id { get; set; }

        public int type { get; set; }

        [Required]
        [StringLength(250)]
        public string link { get; set; }

        public int KPI_id { get; set; }
    }
}
