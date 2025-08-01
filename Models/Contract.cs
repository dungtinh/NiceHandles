namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Contract")]
    public partial class Contract
    {
        public int id { get; set; }

        [StringLength(50)]
        public string code { get; set; }

        [Required]
        [StringLength(150)]
        public string name { get; set; }

        public int address_id { get; set; }

        public int service_id { get; set; }

        public long amount { get; set; }

        public long consuming { get; set; }

        public DateTime time { get; set; }

        public int time_exp { get; set; }

        public int deadline { get; set; }

        public int status { get; set; }

        public string note { get; set; }

        public int account_id { get; set; }

        public int Priority { get; set; }

        public int partner { get; set; }

        [StringLength(50)]
        public string sodienthoai { get; set; }

        [StringLength(50)]
        public string sodienthoai_mota { get; set; }

        public int type { get; set; }

        public int rose { get; set; }

        public int outrose { get; set; }

        public int dove { get; set; }

        public int remunerate { get; set; }

        public int soanthao { get; set; }

        [StringLength(250)]
        public string HardFileLink { get; set; }

        public int loai { get; set; }

        public int nguoiky_id { get; set; }

        public int nguoitheodoi_id { get; set; }

        [StringLength(150)]
        public string googlemap { get; set; }
    }
}
