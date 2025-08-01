namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("vPayPlan")]
    public partial class vPayPlan
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int account_id { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int type { get; set; }

        [StringLength(500)]
        public string note { get; set; }

        [Key]
        [Column(Order = 3)]
        public DateTime time { get; set; }

        [Key]
        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int status { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(150)]
        public string fullname { get; set; }

        [Key]
        [Column(Order = 6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int category_id { get; set; }

        [Key]
        [Column(Order = 7)]
        [StringLength(150)]
        public string category_name { get; set; }

        [Key]
        [Column(Order = 8)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int contract_id { get; set; }

        [Key]
        [Column(Order = 9)]
        [StringLength(150)]
        public string customer_name { get; set; }

        [Key]
        [Column(Order = 10)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long amount { get; set; }
    }
}
