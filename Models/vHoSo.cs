namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("vHoSo")]
    public partial class vHoSo
    {
        [Key]
        [Column(Order = 0)]
        public DateTime time { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int address_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(150)]
        public string address_name { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(150)]
        public string service_name { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int partner { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(150)]
        public string partner_name { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(50)]
        public string account_name { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Key]
        [Column(Order = 8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int contract_id { get; set; }

        [Key]
        [Column(Order = 9)]
        [StringLength(150)]
        public string name { get; set; }

        [Key]
        [Column(Order = 10)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int account_id_soanthao { get; set; }

        [Key]
        [Column(Order = 11)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int priority { get; set; }

        [Key]
        [Column(Order = 12)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int trang4 { get; set; }

        [Key]
        [Column(Order = 13)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int status { get; set; }

        [StringLength(50)]
        public string sodienthoai { get; set; }

        [Key]
        [Column(Order = 14)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int service_id { get; set; }

        [Key]
        [Column(Order = 15)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int account_id { get; set; }

        [Key]
        [Column(Order = 16)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int step_id { get; set; }

        [Key]
        [Column(Order = 17)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int time_exp { get; set; }

        [StringLength(50)]
        public string sodienthoai_mota { get; set; }

        [Key]
        [Column(Order = 18)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int deadline { get; set; }

        public int? ngoaigiao { get; set; }

        public DateTime? time_complete { get; set; }

        [Key]
        [Column(Order = 19)]
        public DateTime time_created { get; set; }

        public int? light { get; set; }
    }
}
