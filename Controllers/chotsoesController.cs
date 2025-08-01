using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class chotsoesController : Controller
    {
        private NHModel db = new NHModel();

        // GET: chotsoes
        public ActionResult Index()
        {
            ViewBag.db = db;
            return View(db.chotsoes.ToList());
        }

        // GET: chotsoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            chotso chotso = db.chotsoes.Find(id);
            if (chotso == null)
            {
                return HttpNotFound();
            }
            return View(chotso);
        }

        // GET: chotsoes/Create
        public ActionResult Create()
        {
            ViewBag.db = db;
            ViewBag.THUCHI = db.InOuts.Where(x => !x.state.HasValue || x.state == 0);

            var lstYIO = (from io in db.InOuts
                          where io.status != (int)XInOut.eStatus.Huy
                          select new { T = io.type, AM = io.amount, AC = io.account_id, VT = io.category_id });
            var thu = lstYIO.Where(x => x.T == (int)XCategory.eType.Thu);
            var chi = lstYIO.Where(x => x.T == (int)XCategory.eType.Chi);

            long YI = thu.Count() > 0 ? thu.Sum(x => x.AM) : 0;
            long YO = chi.Count() > 0 ? chi.Sum(x => x.AM) : 0;
            long YIO = YI - YO;
            ViewBag.YCurrent = YIO;
            ViewBag.YIn = YI;
            ViewBag.YOut = YO;
            /// Giang  = 3, Tín  = 2, Duy = 4
            /// 
            var cateVTs = db.Categories.Where(c => c.pair == (int)XCategory.ePair.VayTra).Select(x => x.id).ToArray();
            // Giang
            var thuGiang = thu.Where(x => cateVTs.Contains(x.VT) && x.AC == 3);
            var chiGiang = chi.Where(x => cateVTs.Contains(x.VT) && x.AC == 3);
            long thuNoGiang = thuGiang.Count() > 0 ? thuGiang.Sum(x => x.AM) : 0;
            long chiNoGiang = chiGiang.Count() > 0 ? chiGiang.Sum(x => x.AM) : 0;
            long NoGiang = chiNoGiang - thuNoGiang;
            ViewBag.NoGiang = NoGiang;
            // Tín
            var thuTin = thu.Where(x => cateVTs.Contains(x.VT) && x.AC == 2);
            var chiTin = chi.Where(x => cateVTs.Contains(x.VT) && x.AC == 2);
            long thuNoTin = thuTin.Count() > 0 ? thuTin.Sum(x => x.AM) : 0;
            long chiNoTin = chiTin.Count() > 0 ? chiTin.Sum(x => x.AM) : 0;
            long NoTin = chiNoTin - thuNoTin;
            ViewBag.NoTin = NoTin;
            // Duy
            var thuDuy = thu.Where(x => cateVTs.Contains(x.VT) && x.AC == 4);
            var chiDuy = chi.Where(x => cateVTs.Contains(x.VT) && x.AC == 4);
            long thuNoDuy = thuDuy.Count() > 0 ? thuDuy.Sum(x => x.AM) : 0;
            long chiNoDuy = chiDuy.Count() > 0 ? chiDuy.Sum(x => x.AM) : 0;
            long NoDuy = chiNoDuy - thuNoDuy;
            ViewBag.NoDuy = NoDuy;

            return View();
        }

        // POST: chotsoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(chotso chotso)
        {
            // var comment            
            var lst = db.InOuts.Where(x => !x.state.HasValue || x.state == (int)XModels.eStatus.Processing).OrderBy(x => x.time);

            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            chotso.account_id = us.id;
            chotso.nokhac = 0;
            chotso.sta = (int)XModels.eStatus.Processing;

            if (ModelState.IsValid)
            {
                db.chotsoes.Add(chotso);
                db.SaveChanges();
                foreach (var item in lst)
                {
                    var fk = new fk_chotso_thuchi();
                    fk.thuchi_id = item.id;
                    fk.chotso_id = chotso.id;
                    db.fk_chotso_thuchi.Add(fk);
                    item.state = (int)XModels.eStatus.Complete;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.db = db;
            ViewBag.THUCHI = lst;
            return View(chotso);
        }

        // GET: chotsoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            chotso chotso = db.chotsoes.Find(id);
            if (chotso == null)
            {
                return HttpNotFound();
            }
            return View(chotso);
        }

        // POST: chotsoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,ngaythang,account_id,ghichu")] chotso chotso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chotso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chotso);
        }

        // GET: chotsoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            chotso chotso = db.chotsoes.Find(id);
            if (chotso == null)
            {
                return HttpNotFound();
            }
            return View(chotso);
        }

        // POST: chotsoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            chotso chotso = db.chotsoes.Find(id);
            db.chotsoes.Remove(chotso);
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
        [HttpGet]
        public JsonResult ChoSo(int? id)
        {
            var chotso = db.chotsoes.Find(id);
            var account = db.chotsoes.Find(chotso.account_id);
            string NGAYTHANG = "Đến ngày: " + chotso.ngaythang.ToString("dd-MM-yyyy");
            //Where(x => x.chotso_id == id).OrderBy(x=>x.thoig)
            var rs = (from fk in db.fk_chotso_thuchi
                      join tc in db.InOuts on fk.thuchi_id equals tc.id
                      join ac in db.Accounts on tc.account_id equals ac.id
                      where fk.chotso_id == id
                      select new ChotSo() { LYDO = tc.note, TAIKHOAN = ac.fullname, TIME = tc.time, TYPE = tc.type, AMOUNT = tc.amount }).ToList();

            foreach (var item in rs)
            {
                item.LOAI = XCategory.sType[item.TYPE];
                item.THOIGIAN = item.TIME.ToString("dd/MM/yyyy");
                item.SOTIEN = item.AMOUNT.ToString("N0");
            }

            long THU = rs.Where(x => x.TYPE == (int)XCategory.eType.Thu).Sum(x => x.AMOUNT);
            long CHI = rs.Where(x => x.TYPE == (int)XCategory.eType.Chi).Sum(x => x.AMOUNT);

            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/CHOTSO.xls");
            string webpart = "ChotSo_" + chotso.id + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    // Tổng thu chi
                    var lstYIO = (from io in db.InOuts
                                  where io.status == (int)XInOut.eStatus.DaThucHien
                                  select new { T = io.type, AM = io.amount }).ToArray();
                    var thu = lstYIO.Where(x => x.T == (int)XCategory.eType.Thu);
                    var chi = lstYIO.Where(x => x.T == (int)XCategory.eType.Chi);
                    var lstTamUng = from io in db.InOuts
                                    join c in db.Categories on io.category_id equals c.id
                                    where io.type == (int)XCategory.eType.Thu && c.pair == (int)XCategory.ePair.TamUng && io.status == (int)XInOut.eStatus.DaThucHien
                                    select io.amount;
                    long tamungthutruoc = 0;
                    if (lstTamUng != null && lstTamUng.Count() > 0)
                    {
                        tamungthutruoc = lstTamUng.Sum();
                    }
                    tamungthutruoc += 0;

                    long YI = thu.Count() > 0 ? thu.Sum(x => x.AM) : 0;
                    YI = YI - tamungthutruoc;
                    long YO = chi.Count() > 0 ? chi.Sum(x => x.AM) : 0;
                    long YIO = YI - YO;
                    // End thu chi

                    flexCelReport.SetValue("TONGTHUCHI", YIO);
                    flexCelReport.SetValue("NGAYTHANG", NGAYTHANG);
                    flexCelReport.SetValue("THU", THU.ToString("N0"));
                    flexCelReport.SetValue("CHI", CHI.ToString("N0"));
                    flexCelReport.SetValue("THUCHI", (THU - CHI).ToString("N0"));
                    flexCelReport.AddTable("TB", rs);
                    flexCelReport.Run(sr, fs);
                }
            }
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult BanInChotSo(int? id)
        {
            var chotso = db.chotsoes.Find(id);
            string NGAYTHANG = "Đến ngày: " + chotso.ngaythang.ToString("dd-MM-yyyy");
            //Where(x => x.chotso_id == id).OrderBy(x=>x.thoig)

            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/BanInChotSo.xls");
            string webpart = "BanInChotSo_" + chotso.id + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("NGAYTHANG", NGAYTHANG);
                    flexCelReport.SetValue("THU", chotso.tienthu.ToString("N0"));
                    flexCelReport.SetValue("CHI", chotso.tienchi.ToString("N0"));
                    flexCelReport.SetValue("TON", chotso.tiencon.ToString("N0"));
                    flexCelReport.SetValue("CHENH", chotso.tienchenhlech.ToString("N0"));
                    flexCelReport.SetValue("NOTONG", (chotso.nogiang + chotso.noduy + chotso.notin + chotso.nokhac).ToString("N0"));
                    flexCelReport.SetValue("NOGIANG", chotso.nogiang.ToString("N0"));
                    flexCelReport.SetValue("NODUY", chotso.noduy.ToString("N0"));
                    flexCelReport.SetValue("NOTIN", chotso.notin.ToString("N0"));
                    flexCelReport.SetValue("NOTE", chotso.ghichu);
                    flexCelReport.Run(sr, fs);
                }
            }
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }
    }
    public class ChotSo
    {
        public long AMOUNT { get; set; }
        public DateTime TIME { get; set; }
        public string THOIGIAN { get; set; }
        public string TAIKHOAN { get; set; }
        public string LOAI { get; set; }
        public string SOTIEN { get; set; }
        public string LYDO { get; set; }
        public int TYPE { get; set; }
    }
}
