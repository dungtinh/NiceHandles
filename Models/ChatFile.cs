namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ChatFile
    {
        public int Id { get; set; }

        public Guid ChatSessionId { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(100)]
        public string FileId { get; set; }

        public DateTime UploadedAt { get; set; }

        public virtual ChatSession ChatSession { get; set; }
    }
}
