using NiceHandles.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace NiceHandles.Controllers
{
    public class HolidayController : Controller
    {
        private readonly NHModel _context = new NHModel();

        public ActionResult Index()
        {
            return View(_context.Holidays.OrderBy(h => h.Date).ToList());
        }

        [HttpPost]
        public ActionResult Add(DateTime date, string description)
        {
            if (!_context.Holidays.Any(h => h.Date == date))
            {
                _context.Holidays.Add(new Holiday { Date = date, Description = description });
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(int id, string description)
        {
            var holiday = _context.Holidays.Find(id);
            if (holiday != null)
            {
                holiday.Description = description;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var holiday = _context.Holidays.Find(id);
            if (holiday != null)
            {
                _context.Holidays.Remove(holiday);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }


}