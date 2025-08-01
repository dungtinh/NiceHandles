namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class fk_congviec_hoso
    {
        public long id { get; set; }

        public int congviec_id { get; set; }

        public int hoso_id { get; set; }

        public int status { get; set; }
    }
}
