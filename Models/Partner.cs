namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Partner")]
    public partial class Partner
    {
        public int id { get; set; }

        [StringLength(50)]
        public string code { get; set; }

        [StringLength(50)]
        public string disname { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        [StringLength(50)]
        public string sodienthoai { get; set; }

        public int sothutu { get; set; }

        public int account_id { get; set; }

        [StringLength(50)]
        public string id_no { get; set; }

        [StringLength(50)]
        public string id_type { get; set; }

        public DateTime? birthday { get; set; }

        [StringLength(50)]
        public string gender { get; set; }

        [StringLength(150)]
        public string address { get; set; }

        [StringLength(50)]
        public string bankno { get; set; }

        [StringLength(150)]
        public string bankname { get; set; }

        [StringLength(150)]
        public string bankbrand { get; set; }

        public DateTime? pro_date { get; set; }

        [StringLength(150)]
        public string pro_by { get; set; }

        public int? cbdp_id { get; set; }
    }
}
