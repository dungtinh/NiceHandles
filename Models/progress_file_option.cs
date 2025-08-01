namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class progress_file_option
    {
        public int id { get; set; }

        public int progress_id { get; set; }

        public int hoso_id { get; set; }

        public int status { get; set; }
    }
}
