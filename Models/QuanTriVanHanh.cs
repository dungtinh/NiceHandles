namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("QuanTriVanHanh")]
    public partial class QuanTriVanHanh
    {
        public int id { get; set; }

        public string plans { get; set; }

        public string workflows { get; set; }

        public string reports { get; set; }

        public int? no { get; set; }
    }
}
