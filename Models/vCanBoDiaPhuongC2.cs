namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class vCanBoDiaPhuongC2
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
        [StringLength(150)]
        public string name { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(50)]
        public string chucvu { get; set; }

        [StringLength(150)]
        public string gmap { get; set; }

        [StringLength(50)]
        public string phoneno { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int thuoccap { get; set; }

        [StringLength(250)]
        public string note { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int fk_id { get; set; }
    }
}
