namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class fk_service_document
    {
        public int id { get; set; }

        public int service_id { get; set; }

        public int document_id { get; set; }
    }
}
