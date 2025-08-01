namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ChatMessage
    {
        public int Id { get; set; }

        public Guid ChatSessionId { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ChatSession ChatSession { get; set; }
    }
}
