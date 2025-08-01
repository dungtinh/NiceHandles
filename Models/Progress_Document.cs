namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Progress_Document
    {
        public int id { get; set; }

        public int progress_type { get; set; }

        public int document_id { get; set; }
    }
}
