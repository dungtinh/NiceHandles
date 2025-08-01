namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("donthu")]
    public partial class donthu
    {
        public int id { get; set; }

        public int hoso_id { get; set; }

        [Required]
        [StringLength(250)]
        public string name { get; set; }

        public int type { get; set; }

        public DateTime time { get; set; }

        public DateTime alarm { get; set; }

        [StringLength(250)]
        public string url { get; set; }

        public int? donthu_id { get; set; }

        public int account_id { get; set; }

        public int cachgui { get; set; }

        [StringLength(250)]
        public string lydo { get; set; }
    }
}
