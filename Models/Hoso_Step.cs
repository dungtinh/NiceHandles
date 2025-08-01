namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Hoso_Step
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long id { get; set; }
    }
}
