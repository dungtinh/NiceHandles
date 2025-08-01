namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DSMotCua")]
    public partial class DSMotCua
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(150)]
        public string name { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(150)]
        public string canbo_name { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(150)]
        public string customer_name { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int contract_id { get; set; }

        [Key]
        [Column(Order = 5)]
        public DateTime ngaynop { get; set; }

        [Key]
        [Column(Order = 6)]
        public DateTime ngaytra { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int canbo1cua { get; set; }

        [StringLength(150)]
        public string canbophutrach { get; set; }

        [StringLength(150)]
        public string maphieuhen { get; set; }

        [StringLength(500)]
        public string ghichu { get; set; }

        [Key]
        [Column(Order = 8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int trangthai { get; set; }

        [StringLength(50)]
        public string c_xa { get; set; }

        [StringLength(50)]
        public string e_giayuyquyenso { get; set; }

        [StringLength(150)]
        public string e_hoten { get; set; }

        [Key]
        [Column(Order = 9)]
        [StringLength(150)]
        public string service_name { get; set; }

        [Key]
        [Column(Order = 10)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int contract_status { get; set; }

        [Key]
        [Column(Order = 11)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int step { get; set; }

        [Key]
        [Column(Order = 12)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int type { get; set; }

        [Key]
        [Column(Order = 13)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int service_id { get; set; }

        [Key]
        [Column(Order = 14)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int account_id { get; set; }

        [Key]
        [Column(Order = 15)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int address_id { get; set; }

        [Key]
        [Column(Order = 16)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int partner { get; set; }

        [Key]
        [Column(Order = 17)]
        public DateTime time { get; set; }

        public DateTime? exp { get; set; }
    }
}
