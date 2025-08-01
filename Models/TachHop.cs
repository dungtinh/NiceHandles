namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TachHop")]
    public partial class TachHop
    {
        public long id { get; set; }

        public int infomation_id { get; set; }

        public int type { get; set; }

        [StringLength(50)]
        public string a_loaigiayto { get; set; }

        [StringLength(50)]
        public string a_sogiayto { get; set; }

        [StringLength(150)]
        public string a_hoten { get; set; }

        public DateTime? a_ngaysinh { get; set; }

        [StringLength(50)]
        public string a_gioitinh { get; set; }

        [StringLength(50)]
        public string a_quoctich { get; set; }

        [StringLength(250)]
        public string a_nguyenquan { get; set; }

        [StringLength(250)]
        public string a_hktt { get; set; }

        [StringLength(50)]
        public string a_thon { get; set; }

        [StringLength(50)]
        public string a_xa { get; set; }

        [StringLength(50)]
        public string a_huyen { get; set; }

        [StringLength(50)]
        public string a_tinh { get; set; }

        public DateTime? a_ngaycap_gt { get; set; }

        [StringLength(150)]
        public string a_noicap_gt { get; set; }

        [StringLength(50)]
        public string a_masothue { get; set; }

        [StringLength(50)]
        public string a_loaigiayto1 { get; set; }

        [StringLength(50)]
        public string a_sogiayto1 { get; set; }

        [StringLength(150)]
        public string a_hoten1 { get; set; }

        public DateTime? a_ngaysinh1 { get; set; }

        [StringLength(50)]
        public string a_gioitinh1 { get; set; }

        [StringLength(50)]
        public string a_quoctich1 { get; set; }

        [StringLength(250)]
        public string a_nguyenquan1 { get; set; }

        [StringLength(250)]
        public string a_hktt1 { get; set; }

        public DateTime? a_ngaycap_gt1 { get; set; }

        [StringLength(150)]
        public string a_noicap_gt1 { get; set; }

        public int? b_csh { get; set; }

        [StringLength(150)]
        public string b_hoten { get; set; }

        public DateTime? b_ngaysinh { get; set; }

        [StringLength(50)]
        public string b_loaigiayto { get; set; }

        [StringLength(50)]
        public string b_sogiayto { get; set; }

        [StringLength(250)]
        public string b_diachithuongtru { get; set; }

        [StringLength(50)]
        public string b_sogiaychungnhan { get; set; }

        [StringLength(50)]
        public string b_sothua { get; set; }

        [StringLength(50)]
        public string b_tobando { get; set; }

        [StringLength(50)]
        public string b_thon { get; set; }

        [StringLength(50)]
        public string b_xa { get; set; }

        [StringLength(50)]
        public string b_huyen { get; set; }

        [StringLength(50)]
        public string b_tinh { get; set; }

        [StringLength(250)]
        public string b_diachithuadat { get; set; }

        [StringLength(50)]
        public string b_dientich { get; set; }

        [StringLength(150)]
        public string b_hinhthucsudung { get; set; }

        [StringLength(150)]
        public string b_mucdichsudung { get; set; }

        [StringLength(150)]
        public string b_nguongoc { get; set; }

        [StringLength(50)]
        public string b_ghichu { get; set; }

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

        [StringLength(50)]
        public string b_loainguongoc { get; set; }

        [StringLength(50)]
        public string b_chuyenquyen { get; set; }

        [StringLength(50)]
        public string c_xaphuong { get; set; }

        [StringLength(50)]
        public string c_xa { get; set; }

        [StringLength(500)]
        public string c_baocao_veviec { get; set; }

        [StringLength(500)]
        public string c_dondenghi_noidung { get; set; }

        [StringLength(50)]
        public string d_sogiayto { get; set; }

        [StringLength(150)]
        public string d_hoten { get; set; }

        public DateTime? d_ngaysinh { get; set; }

        [StringLength(50)]
        public string d_gioitinh { get; set; }

        [StringLength(50)]
        public string d_quoctich { get; set; }

        [StringLength(250)]
        public string d_nguyenquan { get; set; }

        [StringLength(250)]
        public string d_hktt { get; set; }

        [StringLength(50)]
        public string d_loaigiayto { get; set; }

        public DateTime? d_ngaycap_gt { get; set; }

        [StringLength(150)]
        public string d_noicap_gt { get; set; }

        [StringLength(50)]
        public string d_masothue { get; set; }

        [StringLength(50)]
        public string d_sogiayto1 { get; set; }

        [StringLength(50)]
        public string d_loaigiayto1 { get; set; }

        [StringLength(50)]
        public string d_hoten1 { get; set; }

        public DateTime? d_ngaysinh1 { get; set; }

        [StringLength(50)]
        public string d_gioitinh1 { get; set; }

        [StringLength(250)]
        public string d_hktt1 { get; set; }

        public DateTime? d_ngaycap_gt1 { get; set; }

        [StringLength(150)]
        public string d_noicap_gt1 { get; set; }

        [StringLength(50)]
        public string d_loaibiendong { get; set; }

        [StringLength(150)]
        public string d_noidungbiendong { get; set; }

        [StringLength(150)]
        public string d_lydobiendong { get; set; }

        [StringLength(50)]
        public string d_sohopdong { get; set; }

        [StringLength(50)]
        public string d_noicongchung { get; set; }

        public DateTime? d_ngaycongchung { get; set; }

        public int? d_tienhopdong { get; set; }

        [StringLength(50)]
        public string d_vitrithuadat { get; set; }

        [StringLength(50)]
        public string d_thon { get; set; }

        [StringLength(50)]
        public string d_xa { get; set; }

        [StringLength(50)]
        public string d_huyen { get; set; }

        [StringLength(50)]
        public string d_tinh { get; set; }

        [StringLength(50)]
        public string e_sogiayto { get; set; }

        [StringLength(150)]
        public string e_hoten { get; set; }

        public DateTime? e_ngaysinh { get; set; }

        [StringLength(50)]
        public string e_gioitinh { get; set; }

        [StringLength(50)]
        public string e_quoctich { get; set; }

        [StringLength(250)]
        public string e_nguyenquan { get; set; }

        [StringLength(250)]
        public string e_hktt { get; set; }

        public DateTime? e_ngaycap_gt { get; set; }

        [StringLength(150)]
        public string e_noicap_gt { get; set; }

        [StringLength(50)]
        public string e_masothue { get; set; }

        [StringLength(50)]
        public string e_giayuyquyenso { get; set; }

        public DateTime? e_ngayuyquyen { get; set; }

        public int? e_doituong { get; set; }
    }
}
