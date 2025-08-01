namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PriceByArea")]
    public partial class PriceByArea
    {
        public int id { get; set; }

        public int type { get; set; }

        public int area { get; set; }

        [StringLength(50)]
        public string text { get; set; }

        public int price { get; set; }

        [StringLength(150)]
        public string note { get; set; }
    }
}
