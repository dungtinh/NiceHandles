namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class fk_chotso_thuchi
    {
        public long id { get; set; }

        public long thuchi_id { get; set; }

        public int chotso_id { get; set; }
    }
}
