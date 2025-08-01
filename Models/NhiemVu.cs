namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NhiemVu")]
    public partial class NhiemVu
    {
        public int id { get; set; }

        public int contract_id { get; set; }

        public int step_id { get; set; }

        public string ghichu { get; set; }
    }
}
