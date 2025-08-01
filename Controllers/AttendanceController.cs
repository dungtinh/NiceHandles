using NiceHandles.Models;
using System.Linq;
using System.Web.Mvc;
using System;
using NiceHandles.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Ajax.Utilities;

namespace NiceHandles.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly NHModel db = new NHModel();

        public ActionResult MonthlyAttendance(int? employeeId, string month)
        {
            if (!employeeId.HasValue || employeeId == 0)
            {
                var username = User.Identity.GetUserName();
                var acc = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
                employeeId = acc.id;
            }
            DateTime selectedMonth;
            if (!string.IsNullOrEmpty(month) && DateTime.TryParseExact(month + "-01", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out selectedMonth))
            {
                // OK
            }
            else
            {
                selectedMonth = DateTime.Now;
            }
            var employee = db.Accounts.Find(employeeId);
            var holidays = db.Holidays.Where(h => h.Date.Month == selectedMonth.Month && h.Date.Year == selectedMonth.Year).ToList();

            var attendances = db.Attendances
                .Where(a => a.EmployeeId == employeeId && a.Date.Month == selectedMonth.Month && a.Date.Year == selectedMonth.Year)
                .ToList();

            int totalDays = DateTime.DaysInMonth(selectedMonth.Year, selectedMonth.Month);
            int workDays = Enumerable.Range(1, totalDays)
                .Select(day => new DateTime(selectedMonth.Year, selectedMonth.Month, day))
                .Count(d => d.DayOfWeek != DayOfWeek.Sunday && !holidays.Any(h => h.Date == d));

            decimal dailySalary = employee.luong / workDays;
            decimal minuteSalary = (dailySalary / 8) / 60;

            decimal deducted = 0;
            int lateCount = 0;
            int totalLateMinutes = attendances.Sum(a =>
            {
                int late = 0;
                if (a.MorningCheckInManual.HasValue && a.MorningCheckInManual.Value.TimeOfDay > new TimeSpan(7, 30, 0))
                    late += (int)(a.MorningCheckInManual.Value.TimeOfDay - new TimeSpan(7, 30, 0)).TotalMinutes;
                if (a.AfternoonCheckInManual.HasValue && a.AfternoonCheckInManual.Value.TimeOfDay > new TimeSpan(13, 30, 0))
                    late += (int)(a.AfternoonCheckInManual.Value.TimeOfDay - new TimeSpan(13, 30, 0)).TotalMinutes;
                if (late > 30)
                {
                    lateCount += 1;
                    deducted += 100000;
                }
                else if (late > 0)
                {
                    lateCount += 1;
                    deducted += 50000;
                }
                return late;
            });

            int presentDays = attendances.Count(a => a.MorningCheckInManual != null || a.AfternoonCheckInManual != null);
            //decimal deducted = totalLateMinutes * minuteSalary;
            decimal finalSalary = (dailySalary * presentDays) - deducted;

            var model = new MonthlyAttendanceViewModel
            {
                EmployeeId = employeeId.Value,
                EmployeeName = employee.fullname,
                CurrentMonth = selectedMonth,
                Attendances = attendances,
                Holidays = holidays,
                TotalWorkDays = workDays,
                DailySalary = dailySalary,
                FinalSalary = finalSalary,
                TotalLateMinutes = totalLateMinutes,
                DeductedSalary = deducted,
                TotalLateDays = lateCount
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult SaveNote(int employeeId, DateTime date, string note)
        {
            var record = db.Attendances.FirstOrDefault(x => x.EmployeeId == employeeId && x.Date == date);
            if (record == null)
            {
                record = new Attendance { EmployeeId = employeeId, Date = date };
                db.Attendances.Add(record);
            }

            record.LateReason = note;
            db.SaveChanges();
            return RedirectToAction("MonthlyAttendance", new { employeeId, month = date.Month, year = date.Year });
        }
        [HttpPost]
        public ActionResult SubmitCheck(int employeeId, DateTime date, string type, string time)
        {
            var attendance = db.Attendances.FirstOrDefault(a => a.EmployeeId == employeeId && a.Date == date);
            if (attendance == null)
            {
                attendance = new Attendance
                {
                    EmployeeId = employeeId,
                    Date = date
                };
                db.Attendances.Add(attendance);
            }

            if (TimeSpan.TryParse(time, out TimeSpan parsedTime))
            {
                DateTime dateTime = date.Date + parsedTime;

                switch (type)
                {
                    case "MorningIn":
                        attendance.MorningCheckInManual = dateTime;
                        break;
                    case "MorningOut":
                        attendance.MorningCheckOutManual = dateTime;
                        break;
                    case "AfternoonIn":
                        attendance.AfternoonCheckInManual = dateTime;
                        break;
                    case "AfternoonOut":
                        attendance.AfternoonCheckOutManual = dateTime;
                        break;
                }
                db.SaveChanges();
            }

            return RedirectToAction("MonthlyAttendance", new { employeeId = employeeId, month = date.ToString("yyyy-MM") });
        }

    }

}