namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("GroupMember")]
    public partial class GroupMember
    {
        public int id { get; set; }

        public int account_id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }
    }
}
