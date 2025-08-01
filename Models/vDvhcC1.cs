namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vDvhcC1
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(150)]
        public string AddressName { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int address_id { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(50)]
        public string name { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(50)]
        public string code { get; set; }
    }
}
