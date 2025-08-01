namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Document")]
    public partial class Document
    {
        public int id { get; set; }

        [StringLength(50)]
        public string code { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        [StringLength(250)]
        public string template { get; set; }

        public int type { get; set; }
    }
}
