namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("syslog")]
    public partial class syslog
    {
        public long id { get; set; }

        public int account_id { get; set; }

        [Required]
        [StringLength(250)]
        public string message { get; set; }

        public DateTime created_time { get; set; }

        [Required]
        [StringLength(50)]
        public string caption { get; set; }
    }
}
