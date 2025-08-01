namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("vContract")]
    public partial class vContract
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long amount { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long consuming { get; set; }

        [Key]
        [Column(Order = 3)]
        public DateTime time { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int status { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int address_id { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(150)]
        public string address_name { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int service_id { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(150)]
        public string service_name { get; set; }

        [Key]
        [Column(Order = 9)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int partner { get; set; }

        [Key]
        [Column(Order = 10)]
        [StringLength(150)]
        public string partner_name { get; set; }

        [Key]
        [Column(Order = 11)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int rose { get; set; }

        [Key]
        [Column(Order = 12)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int outrose { get; set; }

        [Key]
        [Column(Order = 13)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int dove { get; set; }

        [StringLength(250)]
        public string HardFileLink { get; set; }

        [Key]
        [Column(Order = 14)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int loai { get; set; }

        [Key]
        [Column(Order = 15)]
        [StringLength(150)]
        public string fullname { get; set; }

        [Key]
        [Column(Order = 16)]
        [StringLength(150)]
        public string name { get; set; }

        [Key]
        [Column(Order = 17)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int account_id { get; set; }

        [StringLength(50)]
        public string code { get; set; }
    }
}
