namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("File")]
    public partial class File
    {
        public long id { get; set; }

        [StringLength(150)]
        public string name { get; set; }

        [StringLength(150)]
        public string url { get; set; }

        public int? type { get; set; }

        public int contract_id { get; set; }

        public int? hoso_id { get; set; }
    }
}
