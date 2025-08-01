namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        public int UserId { get; set; }

        [StringLength(100)]
        public string UserName { get; set; }

        [StringLength(150)]
        public string Email { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
