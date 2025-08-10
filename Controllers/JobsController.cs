using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;
using PagedList;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Manager,Member")]
    public class JobsController : Controller
    {
        private NHModel db = new NHModel();

        // GET: Jobs
        public ActionResult Index()
        {
            //DateTime history = DateTime.Now.AddDays(-7);
            //ViewBag.Duy = db.Jobs.Where(x => x.process_by == 4 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            //ViewBag.Tin = db.Jobs.Where(x => x.process_by == 2 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            //ViewBag.Giang = db.Jobs.Where(x => x.process_by == 3 && (x.status != (int)XJob.eStatus.Complete || x.start_date > history)).OrderBy(x => x.status).ToArray();
            ViewBag.db = db;
            return View();
        }
        public ActionResult Issue(string Search_Data, string Filter_Value, int? Page_No, int? user, int? bell, int? sta = null)
        {
            ViewBag.db = db;
            var username = User.Identity.GetUserName();
            var acc = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            if (user.HasValue)
            {
                acc = db.Accounts.Find(user);
            }
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }
            IQueryable<Job> Jobs;
            if (sta.HasValue && sta > (int)XJob.eStatus.Processing)
            {
                Jobs = db.Jobs.Where(x => x.process_by == acc.id && x.label == (bell.HasValue ? bell : x.label) && (
                    x.name.ToUpper().Contains(String.IsNullOrEmpty(Search_Data) ? x.name.ToUpper() : Search_Data.ToUpper()) ||
                    x.note.ToUpper().Contains(String.IsNullOrEmpty(Search_Data) ? x.note.ToUpper() : Search_Data.ToUpper())
                ) && (x.status == sta));
            }
            else if (!sta.HasValue || (sta.HasValue && sta < (int)XJob.eStatus.Complete && sta > -1))
            {
                Jobs = db.Jobs.Where(x => x.process_by == acc.id && x.label == (bell.HasValue ? bell : x.label) && (
                    x.name.ToUpper().Contains(String.IsNullOrEmpty(Search_Data) ? x.name.ToUpper() : Search_Data.ToUpper()) ||
                    x.note.ToUpper().Contains(String.IsNullOrEmpty(Search_Data) ? x.note.ToUpper() : Search_Data.ToUpper())
                ) && x.status < (int)XJob.eStatus.Complete);
            }
            else
            {
                Jobs = db.Jobs.Where(x => x.process_by == acc.id && x.label == (bell.HasValue ? bell : x.label) && (
                    x.name.ToUpper().Contains(String.IsNullOrEmpty(Search_Data) ? x.name.ToUpper() : Search_Data.ToUpper()) ||
                    x.note.ToUpper().Contains(String.IsNullOrEmpty(Search_Data) ? x.note.ToUpper() : Search_Data.ToUpper())
                ));
            }

            ViewBag.FilterValue = Search_Data;
            ViewBag.STA = sta;
            ViewBag.USER = user;
            ViewBag.BELL = bell;

            int Size_Of_Page = 30;
            int No_Of_Page = (Page_No ?? 1);
            return View(Jobs.OrderByDescending(x => x.id).ToPagedList(No_Of_Page, Size_Of_Page));
        }
        public ActionResult BanLamViec()
        {
            var username = User.Identity.GetUserName();
            var acc = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            return View();
        }

        // GET: Jobs/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // GET: Jobs/Create
        public ActionResult Create()
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            if (User.IsInRole("SuperAdmin") || User.IsInRole("Manager") || User.IsInRole("Accounting"))
            {
                ViewBag.ACCOUNTS = new SelectList(db.Accounts.Where(x => x.sta == (int)XAccount.eStatus.Processing), "id", "fullname", us);
            }
            else
            {
                ViewBag.ACCOUNTS = new SelectList(db.Accounts.Where(x => x.id == us.id), "id", "fullname");
            }
            ViewBag.db = db;
            ViewBag.Title = "Tạo nhiệm vụ";
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Job job)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            if (ModelState.IsValid)
            {
                job.status = (int)XJob.eStatus.Processing;
                db.Jobs.Add(job);
                db.SaveChanges();
                return RedirectToAction("Edit", new { id = job.id });
            }
            if (User.IsInRole("SuperAdmin") || User.IsInRole("Manager") || User.IsInRole("Accounting"))
            {
                ViewBag.ACCOUNTS = new SelectList(db.Accounts.Where(x => x.sta == (int)XAccount.eStatus.Processing), "id", "fullname", us);
            }
            else
            {
                ViewBag.ACCOUNTS = new SelectList(db.Accounts.Where(x => x.id == us.id), "id", "fullname");
            }
            return View(job);
        }
        [HttpPost]
        public JsonResult Add(Job job)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity.GetUserName();
                var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
                job.start_date = DateTime.Now;
                job.exp_date = DateTime.Now.AddDays(1);
                job.status = (int)XJob.eStatus.Processing;
                job.created_by = us.id;
                db.Jobs.Add(job);
                db.SaveChanges();
                return Json(job, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddTask(task t)
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

            var ca = db.tasks.Where(x => x.job_id == t.job_id).Count();
            var cf = db.tasks.Where(x => x.job_id == t.job_id && x.status == (int)XModels.eStatus.Complete).Count();

            return Json(new { id = t.id, name = t.name, j = job.name + "(" + cf + "/" + ca + ")" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CheckTask(task t)
        {
            var ot = db.tasks.Find(t.id);
            ot.status = t.status == 1 ? (int)XModels.eStatus.Complete : (int)XModels.eStatus.Processing;
            t.time = DateTime.Now;
            db.SaveChanges();
            var ca = db.tasks.Where(x => x.job_id == ot.job_id).Count();
            var cf = db.tasks.Where(x => x.job_id == ot.job_id && x.status == (int)XModels.eStatus.Complete).Count();
            var job = db.Jobs.Find(ot.job_id);
            //if (ca == cf)
            //{
            //    job.status = (int)XJob.eStatus.Complete;
            //    db.SaveChanges();
            //}
            //else
            //{
            //    job.status = (int)XJob.eStatus.Processing;
            //    db.SaveChanges();
            //}
            return Json(job.name + "(" + cf + "/" + ca + ")", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DelTask(task ct)
        {
            var t = db.tasks.Find(ct.id);
            var job = db.Jobs.Find(t.job_id);
            db.tasks.Remove(t);
            db.SaveChanges();

            var ca = db.tasks.Where(x => x.job_id == t.job_id).Count();
            var cf = db.tasks.Where(x => x.job_id == t.job_id && x.status == (int)XJob.eStatus.Complete).Count();
            if (ca == cf)
                job.status = (int)XJob.eStatus.Complete;
            else
                job.status = (int)XJob.eStatus.Processing;
            db.SaveChanges();

            return Json(new { j = job.id, name = job.name + "(" + cf + "/" + ca + ")" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult DelJob(Job cj)
        {
            var t = db.tasks.Where(x => x.job_id == cj.id);
            db.tasks.RemoveRange(t);
            var job = db.Jobs.Find(cj.id);
            db.Jobs.Remove(job);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        // GET: Jobs/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            ViewBag.db = db;
            ViewBag.Title = job.name;
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Job job)
        {
            if (ModelState.IsValid)
            {
                db.Entry(job).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Issue");
            }
            return View(job);
        }

        // GET: Jobs/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Job job = db.Jobs.Find(id);
            db.Jobs.Remove(job);
            db.SaveChanges();
            return RedirectToAction("Issue");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public JsonResult Print(int? id)
        {
            List<Account> P = new List<Account>();
            List<Job> J = new List<Job>();
            List<task> T = new List<task>();
            P = db.Accounts.Where(x => (x.id == (id.HasValue ? id.Value : x.id))).ToList();
            J = db.Jobs.Where(x => x.status < (int)XJob.eStatus.Complete).ToList();
            var jids = J.Select(k => k.id).ToArray();
            T = db.tasks.Where(x => jids.Contains(x.job_id)).OrderBy(x => x.status).ToList();

            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/Jobs.xls");
            string webpart = "NhiemVu_" + DateTime.Now.ToString("ssmmHH") + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("NGAYTHANG", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
                    flexCelReport.AddTable("P", P);
                    flexCelReport.AddTable("J", (J.Select(x => new { process_by = x.process_by, id = x.id, name = x.name, status = (x.status == (int)XJob.eStatus.Complete ? "X" : "") })).ToArray());
                    flexCelReport.AddTable("T", (T.Select(x => new { job_id = x.job_id, name = x.name, status = (x.status == (int)XModels.eStatus.Complete ? "X" : "") })).ToArray());
                    flexCelReport.AddRelationship("P", "J", "id", "process_by");
                    flexCelReport.AddRelationship("J", "T", "id", "job_id");
                    flexCelReport.Run(sr, fs);
                }
            }            
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DailyReport()
        {
            List<Account> P = new List<Account>();
            List<Job> J = new List<Job>();
            List<task> T = new List<task>();

            var username = User.Identity.GetUserName();
            var acc = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            P = db.Accounts.Where(x => (x.id == acc.id)).ToList();
            var now = DateTime.Now.AddHours(0 - DateTime.Now.Hour);
            T = db.tasks.Where(x => x.time > now).OrderByDescending(x => x.status).ThenBy(x => x.time).ToList();
            var lstJ = T.Select(x => x.job_id).Distinct().ToArray();

            J = db.Jobs.Where(x => lstJ.Contains(x.id) && x.process_by == acc.id).ToList();

            // Contract

            var wf = db.wf_contract.Where(x => x.time > now && x.account_id == acc.id).OrderByDescending(x => x.status).ThenBy(x => x.time).ToList();
            var lstContract = wf.Select(x => x.contract_id).Distinct().ToArray();
            var contracts = db.Contracts.Where(x => lstContract.Contains(x.id)).ToList();

            /// Union

            var jJ = J.Select(x => new objRptTemp1() { process_by = x.process_by, id = x.id, name = x.name, status = (x.status == (int)XJob.eStatus.Complete ? "X" : "") }).ToList();
            var jT = T.Select(x => new objRptTemp2() { job_id = x.job_id, name = x.name, status = (string.IsNullOrEmpty(x.note) ? (x.status == (int)XModels.eStatus.Complete ? "Đã xong" : "Đang thực hiện") : x.note), time = x.time.ToString("HH:mm") }).ToList();

            var jC = (from x in contracts
                      select new objRptTemp1() { process_by = x.account_id, id = x.id, name = x.name, status = "X" }).ToList();
            var jW = wf.Select(x => new objRptTemp2() { job_id = x.contract_id, name = x.note, status = "Đã xong", time = x.time.ToString("HH:mm") }).ToArray();
            jJ.AddRange(jC);
            jT.AddRange(jW);


            // Hồ sơ
            var wfHoSo = db.wf_hoso.Where(x => x.time > now && x.account_id == acc.id).OrderByDescending(x => x.time).ToList();
            var lstHoSo = wfHoSo.Select(x => x.hoso_id).Distinct().ToList();
            //var hosophutrachs = db.HoSoes.Where(x => x.account_id == acc.id && x.status == (int)XHoSo.eStatus.Processing).Select(x => x.id).ToList();
            //var viecphailams = db.ViecPhaiLams.Where(x => x.time > now && hosophutrachs.Contains(x.hoso_id)).OrderByDescending(x => x.time).ToList();
            var viecphailams = db.ViecPhaiLams.Where(x => x.time > now && x.account_id == acc.id).OrderByDescending(x => x.time).ToList();
            var lstHoSoVPL = viecphailams.Select(x => x.hoso_id).Distinct().ToList();
            lstHoSo.AddRange(lstHoSoVPL);
            lstHoSo = lstHoSo.Distinct().ToList();
            //1, Tiến độ

            var hosoes = db.HoSoes.Where(x => lstHoSo.Contains(x.id)).ToList();

            var jH = (from x in hosoes
                      select new objRptTemp1() { process_by = acc.id, id = x.id, name = x.name, status = "X" }).ToList();
            var jWH = wfHoSo.Select(x => new objRptTemp2() { job_id = x.hoso_id, name = x.note, status = "Đã xong", time = x.time.ToString("HH:mm") }).ToArray();
            jJ.AddRange(jH);
            jT.AddRange(jWH);
            //2, Lịch nhắc nhớ                        
            var jVPL = viecphailams.Select(x => new objRptTemp2() { job_id = x.hoso_id, name = x.name, status = x.result, time = x.time.ToString("HH:mm") }).ToArray();
            jT.AddRange(jVPL);


            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/DailyReport.xls");
            string webpart = "DailyReport_" + DateTime.Now.ToString("ssmmHH") + ".xls";
            FlexCel.Report.FlexCelReport flexCelReport = new FlexCel.Report.FlexCelReport();
            using (FileStream fs = System.IO.File.Create(path + webpart))
            {
                using (FileStream sr = System.IO.File.OpenRead(pathTemp))
                {
                    flexCelReport.SetValue("NGAYTHANG", "Ngày " + DateTime.Now.Day + " tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year);
                    flexCelReport.AddTable("P", P);
                    //flexCelReport.AddTable("J", (J.Select(x => new { process_by = x.process_by, id = x.id, name = x.name, status = (x.status == (int)XJob.eStatus.Complete ? "X" : "") })).ToArray());
                    //flexCelReport.AddTable("T", (T.Select(x => new { job_id = x.job_id, name = x.name, status = (x.status == (int)XModels.eStatus.Complete ? "Đã xong" : "Đang thực hiện"), time = x.time.ToString("HH:mm") })).ToArray());
                    flexCelReport.AddTable("J", jJ);
                    flexCelReport.AddTable("T", jT);
                    flexCelReport.AddRelationship("P", "J", "id", "process_by");
                    flexCelReport.AddRelationship("J", "T", "id", "job_id");
                    flexCelReport.Run(sr, fs);
                }
            }
            db.SaveChanges();
            return Json("/public/" + webpart, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetContract(string term, int? page, string _type)
        {
            int p = page ?? 0;
            int ps = 25;
            var lstCT = (from ct in db.Contracts
                         join add in db.Addresses on ct.address_id equals add.id
                         where ct.status == (int)XContract.eStatus.Processing && (ct.name.ToUpper().Contains(String.IsNullOrEmpty(term) ? ct.name.ToUpper() : term))
                         select new { id = ct.id, cus = ct.name, add = add.name }).OrderBy(x => x.cus).Skip(p * ps).Take(ps).ToList();
            data d = new data();
            List<item> lst = new List<item>();
            foreach (var item in lstCT)
            {
                item it = new item();
                it.name = item.cus;
                it.text = item.add;
                it.id = item.id;
                lst.Add(it);
            }
            d.total_count = lst.Count;
            d.items = lst;
            return Json(d, JsonRequestBehavior.AllowGet);
        }
        public ActionResult HoanThanh(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Job job = db.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }
            job.status = (int)XJob.eStatus.Complete;
            db.SaveChanges();

            return RedirectToAction("Issue");
        }
        public sealed class objRptTemp1
        {
            public int process_by { get; set; }
            public long id { get; set; }
            public string name { get; set; }
            public string status { get; set; }
        }
        public sealed class objRptTemp2
        {
            public long job_id { get; set; }
            public string name { get; set; }
            public string status { get; set; }
            public string time { get; set; }
        }
    }

}
