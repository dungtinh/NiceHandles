namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class fk_CanBo_Address
    {
        public int id { get; set; }

        public int canbo_id { get; set; }

        public int address_id { get; set; }
    }
}
