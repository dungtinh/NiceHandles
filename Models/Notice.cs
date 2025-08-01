namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Notice")]
    public partial class Notice
    {
        public int id { get; set; }

        public DateTime date { get; set; }

        [StringLength(250)]
        public string title { get; set; }

        public string contents { get; set; }

        public DateTime? update_date { get; set; }

        public int account_id { get; set; }
    }
}
