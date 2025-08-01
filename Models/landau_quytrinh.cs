namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class landau_quytrinh
    {
        public int id { get; set; }

        public int di1cua_id { get; set; }
    }
}
