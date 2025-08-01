namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Service")]
    public partial class Service
    {
        public int id { get; set; }

        [StringLength(50)]
        public string code { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        public long? amount { get; set; }

        public int? stt { get; set; }

        public string trainning { get; set; }

        public int thoihan { get; set; }

        public int nhacnho { get; set; }

        public int phananh { get; set; }

        public int tocao { get; set; }

        public int? reward { get; set; }
    }
}
