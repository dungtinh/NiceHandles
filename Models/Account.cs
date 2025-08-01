namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Account")]
    public partial class Account
    {
        public int id { get; set; }

        [Required]
        [StringLength(150)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string disname { get; set; }

        [Required]
        [StringLength(150)]
        public string fullname { get; set; }

        public DateTime? chotso { get; set; }

        public string sticknote { get; set; }

        public int luong { get; set; }

        public int phucap { get; set; }

        [StringLength(50)]
        public string bankno { get; set; }

        [StringLength(50)]
        public string bank_name { get; set; }

        public int isNV { get; set; }

        public int hasNote { get; set; }

        [StringLength(50)]
        public string phoneno { get; set; }

        public DateTime? birthday { get; set; }

        [StringLength(50)]
        public string gender { get; set; }

        [StringLength(250)]
        public string hktt { get; set; }

        [StringLength(50)]
        public string sogiayto { get; set; }

        public DateTime? ngaycap_gt { get; set; }

        [StringLength(150)]
        public string noicap_gt { get; set; }

        public int is_uq { get; set; }

        [StringLength(150)]
        public string email { get; set; }

        public int? manager { get; set; }

        public int sta { get; set; }
    }
}
