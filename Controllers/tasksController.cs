using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    public class tasksController : Controller
    {
        private NHModel db = new NHModel();

        // GET: tasks
        public ActionResult Index()
        {
            return View(db.tasks.ToList());
        }

        // GET: tasks/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            task task = db.tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }
        public JsonResult DetailsX(long? id)
        {
            task task = db.tasks.Find(id);
            return Json(task, JsonRequestBehavior.AllowGet);
        }

        // GET: tasks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(task t)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var job = db.Jobs.Find(t.job_id);
            t.status = (int)XModels.eStatus.Processing;
            t.account_id = job.process_by;
            t.account_id_created = job.created_by;
            t.label = job.label;
            t.progress_type = job.progress_type;
            t.time_exp = job.exp_date;
            t.time = DateTime.Now;
            db.tasks.Add(t);
            job.status = (int)XJob.eStatus.Processing;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: tasks/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            task task = db.tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // POST: tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(task t)
        {
            if (ModelState.IsValid)
            {
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(t);
        }
        [HttpPost]
        public ActionResult StatusChange(int id, bool b)
        {
            task task = db.tasks.Find(id);
            task.status = b ? (int)XModels.eStatus.Complete : (int)XModels.eStatus.Processing;
            task.time = DateTime.Now;
            db.SaveChanges();
            return Json(id, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveX(task t)
        {
            if (t.id == 0)
            {
                var username = User.Identity.GetUserName();
                var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
                var job = db.Jobs.Find(t.job_id);
                t.status = (int)XModels.eStatus.Processing;
                t.account_id = job.process_by;
                t.account_id_created = job.created_by;
                t.label = job.label;
                t.progress_type = job.progress_type;
                t.time_exp = job.exp_date;
                t.time = DateTime.Now;
                db.tasks.Add(t);
                job.status = (int)XJob.eStatus.Processing;
            }
            else
            {
                db.Entry(t).State = EntityState.Modified;
            }
            db.SaveChanges();
            return Json(t, JsonRequestBehavior.AllowGet);
        }

        // GET: tasks/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            task task = db.tasks.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            return View(task);
        }

        // POST: tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            task task = db.tasks.Find(id);
            db.tasks.Remove(task);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult DeleteX(long id)
        {
            task task = db.tasks.Find(id);
            db.tasks.Remove(task);
            db.SaveChanges();
            return Json(id, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
