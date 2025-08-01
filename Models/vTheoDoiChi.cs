namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("vTheoDoiChi")]
    public partial class vTheoDoiChi
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int status { get; set; }

        [StringLength(350)]
        public string note { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long inout_id { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long amount { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(150)]
        public string contract_name { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(150)]
        public string category_name { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(150)]
        public string service_name { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(150)]
        public string address_name { get; set; }

        [Key]
        [Column(Order = 8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int category_id { get; set; }

        [Key]
        [Column(Order = 9)]
        public DateTime time { get; set; }

        public int? contract_id { get; set; }

        [Key]
        [Column(Order = 10)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int account_id { get; set; }

        [Key]
        [Column(Order = 11)]
        [StringLength(150)]
        public string fullname { get; set; }
    }
}
