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
    public class GroupMember_AccountController : Controller
    {
        private NHModel db = new NHModel();

        // GET: GroupMember_Account
        public ActionResult Index()
        {
            return View(db.GroupMember_Account.ToList());
        }

        // GET: GroupMember_Account/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupMember_Account groupMember_Account = db.GroupMember_Account.Find(id);
            if (groupMember_Account == null)
            {
                return HttpNotFound();
            }
            return View(groupMember_Account);
        }

        // GET: GroupMember_Account/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GroupMember_Account/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,account_id,groupmember_id")] GroupMember_Account groupMember_Account)
        {
            if (ModelState.IsValid)
            {
                var exited = db.GroupMember_Account.Count(x => x.account_id == groupMember_Account.account_id && x.groupmember_id == groupMember_Account.groupmember_id);
                if (exited == 0)
                {
                    db.GroupMember_Account.Add(groupMember_Account);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(groupMember_Account);
        }

        // GET: GroupMember_Account/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupMember_Account groupMember_Account = db.GroupMember_Account.Find(id);
            if (groupMember_Account == null)
            {
                return HttpNotFound();
            }
            return View(groupMember_Account);
        }

        // POST: GroupMember_Account/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,account_id,groupmember_id")] GroupMember_Account groupMember_Account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(groupMember_Account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(groupMember_Account);
        }

        // GET: GroupMember_Account/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupMember_Account groupMember_Account = db.GroupMember_Account.Find(id);
            if (groupMember_Account == null)
            {
                return HttpNotFound();
            }
            return View(groupMember_Account);
        }

        // POST: GroupMember_Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GroupMember_Account groupMember_Account = db.GroupMember_Account.Find(id);
            db.GroupMember_Account.Remove(groupMember_Account);
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
