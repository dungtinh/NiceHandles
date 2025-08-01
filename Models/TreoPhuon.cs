namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TreoPhuon")]
    public partial class TreoPhuon
    {
        public int id { get; set; }

        [StringLength(500)]
        public string Images { get; set; }

        public int DvhcC1_id { get; set; }

        [StringLength(150)]
        public string GoogleMap { get; set; }

        public DateTime NgayTreo { get; set; }

        public DateTime? NgayKiemTra { get; set; }

        [StringLength(500)]
        public string note { get; set; }
    }
}
