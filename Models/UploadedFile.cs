namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UploadedFile
    {
        [Key]
        public int FileId { get; set; }

        public int? SessionId { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(100)]
        public string FileType { get; set; }

        public int? FileSize { get; set; }

        [StringLength(255)]
        public string OpenAIFileId { get; set; }

        public DateTime? UploadedAt { get; set; }

        public virtual ChatSession ChatSession { get; set; }
    }
}
