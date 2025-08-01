namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PartnerAccount")]
    public partial class PartnerAccount
    {
        public int? account_id { get; set; }

        [Key]
        [StringLength(150)]
        public string fullname { get; set; }

        [StringLength(150)]
        public string name { get; set; }

        [StringLength(50)]
        public string sodienthoai { get; set; }

        [StringLength(50)]
        public string id_no { get; set; }

        [StringLength(50)]
        public string id_type { get; set; }

        public DateTime? birthday { get; set; }

        [StringLength(50)]
        public string gender { get; set; }

        public int? id { get; set; }

        public int? sothutu { get; set; }
    }
}
