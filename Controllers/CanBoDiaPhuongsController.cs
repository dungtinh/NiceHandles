using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NiceHandles.Models;
using PagedList;

namespace NiceHandles.Controllers
{
    public class CanBoDiaPhuongsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: CanBoDiaPhuongs
        public ActionResult Index(string Search_Data, int? address_id, string Filter_Value)
        {
            address_id = address_id ?? db.Addresses.First().id;
            ViewBag.XA = address_id;
            if (Search_Data == null)
            {
                Search_Data = Filter_Value;
            }
            ViewBag.FilterValue = Search_Data;
            ViewBag.address_id = address_id;

            vCanBoDiaPhuongC1[] results = (from vcb in db.vCanBoDiaPhuongC1
                                           where (
                                           (vcb.name.ToUpper().Contains(!String.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : vcb.name.ToUpper())) &&
                                           vcb.address_id == (address_id.HasValue ? address_id.Value : vcb.address_id)
                                           )
                                           orderby vcb.name
                                           select vcb).ToArray();

            int Size_Of_Page = 200;
            int No_Of_Page = 1;
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }

        // GET: CanBoDiaPhuongs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CanBoDiaPhuong canBoDiaPhuong = db.CanBoDiaPhuongs.Find(id);
            if (canBoDiaPhuong == null)
            {
                return HttpNotFound();
            }
            return View(canBoDiaPhuong);
        }

        // GET: CanBoDiaPhuongs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CanBoDiaPhuongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,chucvu,gmap,phoneno,thuoccap,note,fk_id")] CanBoDiaPhuong canBoDiaPhuong)
        {
            if (ModelState.IsValid)
            {
                db.CanBoDiaPhuongs.Add(canBoDiaPhuong);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(canBoDiaPhuong);
        }

        // GET: CanBoDiaPhuongs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CanBoDiaPhuong canBoDiaPhuong = db.CanBoDiaPhuongs.Find(id);
            if (canBoDiaPhuong == null)
            {
                return HttpNotFound();
            }
            ViewBag.CAPXA = canBoDiaPhuong.thuoccap == (int)XModels.eThuocCap.cap2 ? canBoDiaPhuong.fk_id : db.DvhcC1.Single(x => x.id == canBoDiaPhuong.fk_id).address_id;
            if (canBoDiaPhuong.thuoccap == (int)XModels.eThuocCap.cap1) ViewBag.CAPTHON = canBoDiaPhuong.fk_id;
            return View(canBoDiaPhuong);
        }

        // POST: CanBoDiaPhuongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,chucvu,gmap,phoneno,thuoccap,note,fk_id")] CanBoDiaPhuong canBoDiaPhuong)
        {
            if (ModelState.IsValid)
            {
                db.Entry(canBoDiaPhuong).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CAPXA = canBoDiaPhuong.thuoccap == (int)XModels.eThuocCap.cap2 ? canBoDiaPhuong.fk_id : db.DvhcC1.Single(x => x.address_id == canBoDiaPhuong.fk_id).address_id;
            if (canBoDiaPhuong.thuoccap == (int)XModels.eThuocCap.cap1) ViewBag.CAPTHON = canBoDiaPhuong.fk_id;
            return View(canBoDiaPhuong);
        }

        // GET: CanBoDiaPhuongs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CanBoDiaPhuong canBoDiaPhuong = db.CanBoDiaPhuongs.Find(id);
            if (canBoDiaPhuong == null)
            {
                return HttpNotFound();
            }
            return View(canBoDiaPhuong);
        }

        // POST: CanBoDiaPhuongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CanBoDiaPhuong canBoDiaPhuong = db.CanBoDiaPhuongs.Find(id);
            db.CanBoDiaPhuongs.Remove(canBoDiaPhuong);
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

        public JsonResult GetThon(int xa)
        {
            var lst = db.DvhcC1.Where(c => c.address_id == xa);
            return Json(lst, JsonRequestBehavior.AllowGet);
        }
    }
}
