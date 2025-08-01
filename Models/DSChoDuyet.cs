namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DSChoDuyet")]
    public partial class DSChoDuyet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Required]
        [StringLength(250)]
        public string note { get; set; }

        public DateTime time { get; set; }

        public int status { get; set; }
    }
}
