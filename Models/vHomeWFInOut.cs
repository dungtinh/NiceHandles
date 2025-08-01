namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("vHomeWFInOut")]
    public partial class vHomeWFInOut
    {
        [Key]
        [Column(Order = 0)]
        public DateTime time { get; set; }

        [StringLength(150)]
        public string wfNote { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(150)]
        public string fullname { get; set; }

        [StringLength(500)]
        public string IONote { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(150)]
        public string name { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long amount { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int type { get; set; }
    }
}
