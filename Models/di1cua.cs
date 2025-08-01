namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class di1cua
    {
        public int id { get; set; }

        public int contract_id { get; set; }

        public int hoso_id { get; set; }

        public DateTime ngaynop { get; set; }

        public DateTime ngaytra { get; set; }

        public int canbo1cua { get; set; }

        [StringLength(150)]
        public string canbophutrach { get; set; }

        public int? noinhan_id { get; set; }

        [StringLength(150)]
        public string maphieuhen { get; set; }

        [StringLength(500)]
        public string ghichu { get; set; }

        public int trangthai { get; set; }

        public int type { get; set; }
    }
}
