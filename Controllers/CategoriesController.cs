using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Accounting,Stocker")]
    public class CategoriesController : Controller
    {
        private NHModel db = new NHModel();

        // GET: Categories
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Index()
        {
            return View(db.Categories.Where(x => x.status == (int)XModels.eStatus.Processing).ToList());
        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        public ActionResult Create(int parent_id, int type)
        {
            Category category = new Category();
            category.parent_id = parent_id;
            category.type = type;
            return View(category);
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
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
        [Authorize(Roles = "SuperAdmin,Accounting,Stocker")]
        public ActionResult FillCategory(int cate, int? p)
        {
            if (p != null && p.HasValue)
            {
                p = (int)XCategory.eParent.KhachHang;
            }
            else
                p = (int)XCategory.eParent.VanPhong;
            var lst = db.Categories.Where(c => c.type == cate && c.parent_id == p.Value && c.status == (int)XModels.eStatus.Processing).OrderBy(x => x.name);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = "SuperAdmin,Accounting,Stocker")]
        public ActionResult FillCategoryContract(int type, int parent_id)
        {
            var lst = db.Categories.Where(c => c.type == type && c.parent_id == parent_id
                        && c.status == (int)XModels.eStatus.Processing
                        //&& c.kind != (int)XCategory.eKind.Partner
                        //&& c.kind != (int)XCategory.eKind.Rose
                        //&& c.kind != (int)XCategory.eKind.Dove
                        //&& c.kind != (int)XCategory.eKind.Remunerate
                        ).OrderBy(x => x.name);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
    }
}
