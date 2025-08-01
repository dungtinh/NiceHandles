namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("vTienDo")]
    public partial class vTienDo
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime time { get; set; }

        public DateTime? exp { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int status { get; set; }

        public int? workflow { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Priority { get; set; }

        [StringLength(50)]
        public string sodienthoai { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(150)]
        public string address_name { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(150)]
        public string service_name { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(150)]
        public string step_name { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(150)]
        public string account_name { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(150)]
        public string partner_name { get; set; }

        [Key]
        [Column(Order = 9)]
        [StringLength(150)]
        public string customer_name { get; set; }

        [Key]
        [Column(Order = 10)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int customer_id { get; set; }

        [Key]
        [Column(Order = 11)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int address_id { get; set; }

        [Key]
        [Column(Order = 12)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int step { get; set; }

        [Key]
        [Column(Order = 13)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int account_id { get; set; }

        [Key]
        [Column(Order = 14)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int partner { get; set; }

        [StringLength(20)]
        public string step_code { get; set; }

        [StringLength(50)]
        public string step_color { get; set; }

        [StringLength(50)]
        public string partner_code { get; set; }

        [StringLength(150)]
        public string link_giapha { get; set; }

        public string message { get; set; }

        [Key]
        [Column(Order = 15)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int type { get; set; }

        [Key]
        [Column(Order = 16)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int service_id { get; set; }

        [Key]
        [Column(Order = 17)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int soanthao { get; set; }
    }
}
