namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InOut")]
    public partial class InOut
    {
        public long id { get; set; }

        [Required]
        [StringLength(36)]
        public string code { get; set; }

        public int category_id { get; set; }

        public int account_id { get; set; }

        public int type { get; set; }

        [StringLength(500)]
        public string note { get; set; }

        public DateTime time { get; set; }

        public int created_by { get; set; }

        public int? contract_id { get; set; }

        public int gostock { get; set; }

        public int status { get; set; }

        public int? state { get; set; }

        public int currency { get; set; }

        public long amount { get; set; }

        public int? inoutchoduyet_id { get; set; }

        public int unlock { get; set; }

        public int? xclass { get; set; }
    }
}
