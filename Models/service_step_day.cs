namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class service_step_day
    {
        public int id { get; set; }

        public int service_id { get; set; }

        public int step_id { get; set; }

        public int red { get; set; }

        public int yellow { get; set; }
    }
}
