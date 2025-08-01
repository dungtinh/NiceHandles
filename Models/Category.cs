namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Category")]
    public partial class Category
    {
        public int id { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        public int parent_id { get; set; }

        public int type { get; set; }

        public int wf { get; set; }

        public int kind { get; set; }

        public int pair { get; set; }

        public int? nhom { get; set; }

        public int duyet { get; set; }

        public int theodoi { get; set; }

        public int tamung { get; set; }
    }
}
