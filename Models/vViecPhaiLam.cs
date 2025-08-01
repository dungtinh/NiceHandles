namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("vViecPhaiLam")]
    public partial class vViecPhaiLam
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string title { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(150)]
        public string name { get; set; }

        [Key]
        [Column(Order = 3)]
        public DateTime time { get; set; }

        [Key]
        [Column(Order = 4)]
        public DateTime time_progress { get; set; }

        [StringLength(150)]
        public string result { get; set; }

        [Key]
        [Column(Order = 5)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int hoso_id { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(150)]
        public string hoso_name { get; set; }

        [Key]
        [Column(Order = 7)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int service_id { get; set; }

        [Key]
        [Column(Order = 8)]
        [StringLength(150)]
        public string service_name { get; set; }

        [Key]
        [Column(Order = 9)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int progress_type { get; set; }

        [Key]
        [Column(Order = 10)]
        [StringLength(150)]
        public string fullname { get; set; }

        [Key]
        [Column(Order = 11)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int status { get; set; }

        [Key]
        [Column(Order = 12)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int account_id { get; set; }
    }
}
