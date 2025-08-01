namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GroupField")]
    public partial class GroupField
    {
        public int id { get; set; }

        [Required]
        [StringLength(250)]
        public string name { get; set; }

        public int no { get; set; }

        public int document_id { get; set; }
    }
}
