namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("HoSo")]
    public partial class HoSo
    {
        public int id { get; set; }

        public int contract_id { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        public DateTime time_created { get; set; }

        public DateTime? time_complete { get; set; }

        public int account_id { get; set; }

        public int account_id_soanthao { get; set; }

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

        public int step_id { get; set; }

        public string note { get; set; }

        public int priority { get; set; }

        public int trang4 { get; set; }

        public int status { get; set; }

        public int? ngoaigiao { get; set; }

        public int? light { get; set; }
    }
}
