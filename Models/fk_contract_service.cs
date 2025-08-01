namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class fk_contract_service
    {
        public int id { get; set; }

        public int contract_id { get; set; }

        public int service_id { get; set; }

        [StringLength(50)]
        public string note { get; set; }
    }
}
