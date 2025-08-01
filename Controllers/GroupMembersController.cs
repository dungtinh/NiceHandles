using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    public class GroupMembersController : Controller
    {
        private NHModel db = new NHModel();

        // GET: GroupMembers
        public ActionResult Index()
        {
            return View(db.vSettingGroupMembers.ToList());
        }

        // GET: GroupMembers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupMember groupMember = db.GroupMembers.Find(id);
            if (groupMember == null)
            {
                return HttpNotFound();
            }
            return View(groupMember);
        }

        // GET: GroupMembers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GroupMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,account_id,name")] GroupMember groupMember)
        {
            if (ModelState.IsValid)
            {
                db.GroupMembers.Add(groupMember);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(groupMember);
        }

        // GET: GroupMembers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupMember groupMember = db.GroupMembers.Find(id);
            if (groupMember == null)
            {
                return HttpNotFound();
            }
            return View(groupMember);
        }

        // POST: GroupMembers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,account_id,name")] GroupMember groupMember)
        {
            if (ModelState.IsValid)
            {
                db.Entry(groupMember).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(groupMember);
        }

        // GET: GroupMembers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupMember groupMember = db.GroupMembers.Find(id);
            if (groupMember == null)
            {
                return HttpNotFound();
            }
            return View(groupMember);
        }

        // POST: GroupMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GroupMember groupMember = db.GroupMembers.Find(id);
            db.GroupMembers.Remove(groupMember);
            db.SaveChanges();
            return RedirectToAction("Index");
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
