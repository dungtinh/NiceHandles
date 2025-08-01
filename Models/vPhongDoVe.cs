namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("vPhongDoVe")]
    public partial class vPhongDoVe
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(150)]
        public string name { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int account_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int service_id { get; set; }

        [StringLength(150)]
        public string link_filecad { get; set; }

        [StringLength(150)]
        public string link_gmap { get; set; }

        [StringLength(150)]
        public string link_filecad_qr { get; set; }

        [StringLength(150)]
        public string link_gmap_qr { get; set; }

        [StringLength(150)]
        public string link_flycam { get; set; }

        [StringLength(150)]
        public string a_hoten { get; set; }

        [StringLength(50)]
        public string a_gioitinh { get; set; }

        [StringLength(250)]
        public string a_hktt { get; set; }

        [StringLength(150)]
        public string a_hoten1 { get; set; }

        [StringLength(50)]
        public string a_gioitinh1 { get; set; }

        [StringLength(250)]
        public string a_hktt1 { get; set; }

        [StringLength(50)]
        public string b_sogiaychungnhan { get; set; }

        [StringLength(50)]
        public string b_sothua { get; set; }

        [StringLength(50)]
        public string b_tobando { get; set; }

        [StringLength(250)]
        public string b_diachithuadat { get; set; }

        [StringLength(50)]
        public string b_dientichtrenbia { get; set; }

        [StringLength(150)]
        public string b_hinhthucsudung { get; set; }

        [StringLength(150)]
        public string b_mucdichsudung { get; set; }

        public DateTime? b_ngaycap { get; set; }

        [StringLength(150)]
        public string b_noicap { get; set; }

        [StringLength(50)]
        public string b_sovaoso { get; set; }

        [StringLength(50)]
        public string b_loaidat1 { get; set; }

        [StringLength(50)]
        public string b_dientich1 { get; set; }

        [StringLength(50)]
        public string b_loaidat2 { get; set; }

        [StringLength(50)]
        public string b_dientich2 { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int contract_id { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int address_id { get; set; }

        [StringLength(50)]
        public string sodienthoai { get; set; }

        [StringLength(50)]
        public string sodienthoai_mota { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int partner { get; set; }

        [StringLength(50)]
        public string b_dientich { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int hoso_id { get; set; }

        [Key]
        [Column(Order = 8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int status { get; set; }

        [Key]
        [Column(Order = 9)]
        public DateTime time { get; set; }

        [StringLength(250)]
        public string scan_bia { get; set; }

        public DateTime? time_start { get; set; }

        public DateTime? time_tradiem { get; set; }

        public DateTime? time_vexong { get; set; }

        public DateTime? time_dongdau { get; set; }

        public DateTime? time_thamdinh { get; set; }

        [Key]
        [Column(Order = 10)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int dove_account { get; set; }
    }
}
