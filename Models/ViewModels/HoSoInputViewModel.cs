using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace NiceHandles.Models
{
    public class HoSoInputViewModel
    {
        // === THÔNG TIN HỒ SƠ (Readonly - chỉ hiển thị) ===
        public int HoSoId { get; set; }
        public string HoSoName { get; set; }
        public string ContractName { get; set; }
        public string ServiceName { get; set; }
        public int ServiceId { get; set; }
        public string AddressName { get; set; }

        // === THÔNG TIN THỬA ĐẤT (LandParcel) ===
        public LandParcelDto LandParcel { get; set; }

        // === THÔNG TIN BIẾN ĐỘNG (VariationInfo) ===
        public VariationInfoDto VariationInfo { get; set; }

        // === DANH SÁCH NGƯỜI LIÊN QUAN (PersonInfo + HoSoPerson) ===
        public List<PersonInfoDto> Persons { get; set; }

        // === DROPDOWN DATA ===
        public SelectList PersonRoles { get; set; }
        public SelectList DocumentTypes { get; set; }
        public SelectList Genders { get; set; }
        public SelectList VariationTypes { get; set; }
        public SelectList LandPositions { get; set; }


        public HoSoInputViewModel()
        {
            LandParcel = new LandParcelDto();
            VariationInfo = new VariationInfoDto();
            Persons = new List<PersonInfoDto>();
            InitializeDropdowns();
        }

        private void InitializeDropdowns()
        {
            PersonRoles = new SelectList(new[]
            {
                new { Value = 0, Text = "Chủ sở hữu" },
                new { Value = 1, Text = "Người mua/nhận" },
                new { Value = 2, Text = "Người thừa kế" },
                new { Value = 3, Text = "Thành viên hộ gia đình" },
                new { Value = 99, Text = "Khác" }
            }, "Value", "Text");

            DocumentTypes = new SelectList(new[]
            {
                new { Value = "CCCD", Text = "Căn cước công dân" },
                new { Value = "CMND", Text = "Chứng minh nhân dân" },
                new { Value = "Căn cước", Text = "Căn cước" },
                new { Value = "Số định danh", Text = "Số định danh cá nhân" }
            }, "Value", "Text");

            Genders = new SelectList(new[]
            {
                new { Value = "Nam", Text = "Nam" },
                new { Value = "Nữ", Text = "Nữ" }
            }, "Value", "Text");

            VariationTypes = new SelectList(new[]
            {
                new { Value = "Mua bán", Text = "Mua bán" },
                new { Value = "Tặng cho", Text = "Tặng cho" },
                new { Value = "Thừa kế", Text = "Thừa kế" },
                new { Value = "Chia tách", Text = "Chia tách" },
                new { Value = "Hợp thửa", Text = "Hợp thửa" },
                new { Value = "Cấp đổi", Text = "Cấp đổi" },
                new { Value = "Cấp lại", Text = "Cấp lại" }
            }, "Value", "Text");

            LandPositions = new SelectList(new[]
            {
                new { Value = "1", Text = "Vị trí 1" },
                new { Value = "2", Text = "Vị trí 2" },
                new { Value = "3", Text = "Vị trí 3" },
                new { Value = "4", Text = "Vị trí 4" }
            }, "Value", "Text");
        }
    }

    // DTO cho LandParcel
    public class LandParcelDto
    {
        public int Id { get; set; }
        public int HosoId { get; set; }

        [Display(Name = "Số giấy chứng nhận")]
        public string CertificateNumber { get; set; }

        [Display(Name = "Số thửa")]
        public string ParcelNumber { get; set; }

        [Display(Name = "Tờ bản đồ")]
        public string MapSheet { get; set; }

        [Display(Name = "Diện tích thực tế (m²)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? ActualArea { get; set; }

        [Display(Name = "Diện tích theo GCN (m²)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? CertifiedArea { get; set; }

        [Display(Name = "Mục đích sử dụng")]
        public string UsagePurpose { get; set; }

        [Display(Name = "Ngày cấp GCN")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? IssueDate { get; set; }

        [Display(Name = "Nơi cấp")]
        public string Issuer { get; set; }

        [Display(Name = "Số vào sổ")]
        public string BookNumber { get; set; }

        [Display(Name = "Địa chỉ thửa đất")]
        public string Address { get; set; }
        public SelectList UsagePurposes { get; set; }
        public LandParcelDto()
        {
            UsagePurposes = new SelectList(new[]
           {
                new { Value = "Đất ở", Text = "Đất ở" },
                new { Value = "Đất ở tại đô thị", Text = "Đất ở tại đô thị" },
                new { Value = "Đất ở tại nông thôn", Text = "Đất ở tại nông thôn" },
                new { Value = "Đất nông nghiệp", Text = "Đất nông nghiệp" },
                new { Value = "Đất phi nông nghiệp", Text = "Đất phi nông nghiệp" }
            }, "Value", "Text");
        }
    }

    // DTO cho VariationInfo
    public class VariationInfoDto
    {
        public int Id { get; set; }
        public int HosoId { get; set; }

        [Display(Name = "Loại biến động")]
        public string VariationType { get; set; }

        [Display(Name = "Số hợp đồng/công chứng")]
        public string ContractNumber { get; set; }

        [Display(Name = "Văn phòng công chứng")]
        public string NotaryOffice { get; set; }

        [Display(Name = "Ngày công chứng")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? NotaryDate { get; set; }

        [Display(Name = "Giá trị hợp đồng (VNĐ)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? ContractAmount { get; set; }

        [Display(Name = "Lý do miễn giảm thuế")]
        public string TaxReductionReason { get; set; }

        [Display(Name = "Vị trí thửa đất")]
        public string LandPosition { get; set; }
    }

    // DTO cho PersonInfo với HoSoPerson relationship
    public class PersonInfoDto
    {
        // PersonInfo fields
        public int Id { get; set; }
        public int HosoId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Giới tính")]
        public string Gender { get; set; }

        [Display(Name = "Loại giấy tờ")]
        public string DocumentType { get; set; }

        [Display(Name = "Số CCCD/CMND")]
        public string DocumentNumber { get; set; }

        [Display(Name = "Ngày cấp")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? IssueDate { get; set; }

        [Display(Name = "Nơi cấp")]
        public string Issuer { get; set; }

        [Display(Name = "Mã số thuế")]
        public string TaxCode { get; set; }

        [Display(Name = "Địa chỉ thường trú")]
        public string DocumentAddress { get; set; }

        // For deceased persons
        [Display(Name = "Ngày mất")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DeathDate { get; set; }

        [Display(Name = "Giấy tờ chứng tử")]
        public string DeathDocument { get; set; }

        // HoSoPerson relationship fields
        [Display(Name = "Vai trò")]
        public int Role { get; set; } // Maps to HoSoPersonRole enum

        [Display(Name = "Là người chính")]
        public bool IsPrimary { get; set; }

        [Display(Name = "Đứng tên trên GCN")]
        public bool IsCertificateHolder { get; set; }

        // For relationship tracking
        public int? HeirId { get; set; } // ID of person they inherit from

        // Temporary flag for new persons
        public bool IsNew { get; set; }
    }
}