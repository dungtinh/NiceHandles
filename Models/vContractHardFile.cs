namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("vContractHardFile")]
    public partial class vContractHardFile
    {
        [StringLength(250)]
        public string HardFileLink { get; set; }

        [Key]
        [Column(Order = 0)]
        [StringLength(150)]
        public string address_name { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(150)]
        public string customer_name { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(150)]
        public string service_name { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(150)]
        public string partner_name { get; set; }

        [Key]
        [Column(Order = 4)]
        public DateTime time { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int address_id { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int customer_id { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int service_id { get; set; }

        [Key]
        [Column(Order = 8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int partner { get; set; }

        [Key]
        [Column(Order = 9)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Key]
        [Column(Order = 10)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int status { get; set; }
    }
}
