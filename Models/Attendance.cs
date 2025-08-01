namespace NiceHandles.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Attendance")]
    public partial class Attendance
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        public DateTime? MorningCheckInActual { get; set; }

        public DateTime? MorningCheckOutActual { get; set; }

        public DateTime? AfternoonCheckInActual { get; set; }

        public DateTime? AfternoonCheckOutActual { get; set; }

        public DateTime? MorningCheckInManual { get; set; }

        public DateTime? MorningCheckOutManual { get; set; }

        public DateTime? AfternoonCheckInManual { get; set; }

        public DateTime? AfternoonCheckOutManual { get; set; }

        [StringLength(500)]
        public string LateReason { get; set; }

        public double TotalHours { get; set; }
    }
}
