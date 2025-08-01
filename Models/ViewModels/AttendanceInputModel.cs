using System;

namespace NiceHandles.ViewModels
{
    public class AttendanceInputModel
    {
        public DateTime Date { get; set; } = DateTime.Now.Date;
        public DateTime? MorningCheckInManual { get; set; }
        public DateTime? MorningCheckOutManual { get; set; }
        public DateTime? AfternoonCheckInManual { get; set; }
        public DateTime? AfternoonCheckOutManual { get; set; }
        public string LateReason { get; set; }
    }

}
