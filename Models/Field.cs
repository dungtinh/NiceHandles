namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Field")]
    public partial class Field
    {
        public long id { get; set; }

        public int group_id { get; set; }

        [Required]
        [StringLength(250)]
        public string title { get; set; }

        [Required]
        [StringLength(250)]
        public string name { get; set; }

        public int no { get; set; }
    }
}
