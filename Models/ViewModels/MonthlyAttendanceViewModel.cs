using NiceHandles.Models;
using System;
using System.Collections.Generic;

namespace NiceHandles.ViewModels
{
    public class MonthlyAttendanceViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime CurrentMonth { get; set; }

        public List<Attendance> Attendances { get; set; }
        public List<Holiday> Holidays { get; set; }

        public int TotalWorkDays { get; set; }
        public decimal DailySalary { get; set; }
        public decimal FinalSalary { get; set; }
        public int TotalLateMinutes { get; set; }
        public int TotalLateDays { get; set; }
        public decimal DeductedSalary { get; set; }
    }


}
