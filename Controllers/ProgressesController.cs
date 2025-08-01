using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.BuilderProperties;
using Newtonsoft.Json;
using NiceHandles.Models;

namespace NiceHandles.Controllers
{
    public class ProgressesController : Controller
    {
        private NHModel db = new NHModel();

        // GET: Progresses
        public ActionResult Index()
        {
            return View(db.Progresses.ToList());
        }

        // GET: Progresses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Progress progress = db.Progresses.Find(id);
            if (progress == null)
            {
                return HttpNotFound();
            }
            return View(progress);
        }
        public ActionResult NhiemVu(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoSo hoso = db.HoSoes.Find(id);
            if (hoso == null)
            {
                return HttpNotFound();
            }
            return View(hoso);
        }

        // GET: Progresses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Progresses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,title,phantram,description,note,status,type,sort")] Progress progress)
        {
            if (ModelState.IsValid)
            {
                db.Progresses.Add(progress);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(progress);
        }

        // GET: Progresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Progress progress = db.Progresses.Find(id);
            if (progress == null)
            {
                return HttpNotFound();
            }
            return View(progress);
        }
        public ActionResult Setting(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Progress progress = db.Progresses.Find(id);
            if (progress == null)
            {
                return HttpNotFound();
            }
            return View(progress);
        }

        // POST: Progresses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(Progress progress)
        {
            if (ModelState.IsValid)
            {
                db.Entry(progress).State = EntityState.Modified;
                db.SaveChanges();
            }
            return View(progress);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Setting(Progress progress)
        {
            ViewBag.hoso_id = Request["hoso_id"];
            if (ModelState.IsValid)
            {
                db.Entry(progress).State = EntityState.Modified;
                db.SaveChanges();
            }
            return View(progress);
        }

        // GET: Progresses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Progress progress = db.Progresses.Find(id);
            if (progress == null)
            {
                return HttpNotFound();
            }
            return View(progress);
        }

        // POST: Progresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Progress progress = db.Progresses.Find(id);
            db.Progresses.Remove(progress);
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

        [HttpPost]
        public JsonResult UploadProgressFile(progress_file progressfile)
        {
            var hoso = db.HoSoes.Find(progressfile.hoso_id);
            var contract = db.Contracts.Find(hoso.contract_id);
            var address = db.Addresses.Find(contract.address_id);
            if (Request.Files.Count > 0)
            {
                var f0 = Request.Files[0];

                string subPath = Server.MapPath("~/public/" + address.code);
                bool exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id);
                exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id + "/" + XModels.THUMUCHOSO);
                exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                f0.SaveAs(subPath + "/" + f0.FileName);
                progressfile.url = "/public/" + address.code + "/" + contract.id + "/" + XModels.THUMUCHOSO + "/" + f0.FileName;
                progressfile.name = f0.FileName;
                db.progress_file.Add(progressfile);
                var username = User.Identity.GetUserName();
                var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
                NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Thực hiện upload file (" + hoso.name + ")");
                db.SaveChanges();
            }
            var result = " | <a href='" + progressfile.url + "' target='_blank'>" + progressfile.name + "</a>";
            //result = JsonConvert.SerializeObject(result);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveHoSoNote(int hoso_id, string note)
        {
            var hoso = db.HoSoes.Find(hoso_id);
            hoso.note = note;
            db.SaveChanges();
            return Json(hoso_id, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveHopDongNote(int contract_id, string note)
        {
            var contract = db.Contracts.Find(contract_id);
            contract.note = note;
            db.SaveChanges();
            return Json(contract.id, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ProgressFileStatus(progress_file_option option)
        {
            var optionOld = db.progress_file_option.FirstOrDefault(x => x.hoso_id == option.hoso_id && x.progress_id == option.progress_id);
            if (optionOld == null)
            {
                db.progress_file_option.Add(option);
            }
            else
            {
                optionOld.status = option.status;
            }
            var progress = db.Progresses.Find(option.progress_id);
            if (progress.type == (int)XProgress.eType.BanVe)
            {
                var dove = db.Doves.SingleOrDefault(x => x.hoso_id == option.hoso_id);
                if (dove != null)
                {
                    dove.status = option.status;
                }
            }
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult TaoLich(ViecPhaiLam viecPhaiLam)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var hoso = db.HoSoes.Find(viecPhaiLam.hoso_id);

            var job = db.Jobs.Where(x => x.hoso_id == hoso.id && x.process_by == viecPhaiLam.account_id && x.status < (int)XJob.eStatus.Complete).FirstOrDefault();
            if (job == null)
            {
                job = new Job();
                job.created_by = us.id;
                job.exp_date = viecPhaiLam.time_progress;
                job.hoso_id = viecPhaiLam.hoso_id;
                job.label = viecPhaiLam.bell_type;
                //job.name = viecPhaiLam.name;
                job.name = hoso.name;
                job.process_by = viecPhaiLam.account_id;
                job.progress_type = viecPhaiLam.progress_type;
                job.start_date = DateTime.Now;
                job.status = (int)XJob.eStatus.Processing;
                db.Jobs.Add(job);
                db.SaveChanges();
            }
            else
            {
                if (job.exp_date < viecPhaiLam.time_progress)
                    job.exp_date = viecPhaiLam.time_progress;
                if (!job.label.HasValue || job.label.Value < viecPhaiLam.bell_type)
                    job.label = viecPhaiLam.bell_type;
                job.progress_type = viecPhaiLam.progress_type;
            }
            var t = new task();
            t.status = (int)XModels.eStatus.Processing;
            t.time = DateTime.Now;
            t.account_id = viecPhaiLam.account_id;
            t.account_id_created = viecPhaiLam.account_id_created;
            t.label = viecPhaiLam.bell_type;
            t.name = viecPhaiLam.name;
            t.progress_type = viecPhaiLam.progress_type;
            t.time_exp = viecPhaiLam.time_progress;
            t.job_id = job.id;
            db.tasks.Add(t);

            NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Tạo mới lịch hẹn (" + viecPhaiLam.name + ")");
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult LuuKetQua(ViecPhaiLam viecPhaiLam)
        {
            //var viecPhaiLamOld = db.ViecPhaiLams.OrderByDescending(x => x.time).First(x => x.hoso_id == viecPhaiLam.hoso_id && x.progress_type == viecPhaiLam.progress_type);
            //viecPhaiLamOld.result = viecPhaiLam.result;
            //viecPhaiLamOld.time = DateTime.Now;
            //var username = User.Identity.GetUserName();
            //var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            //NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Cập nhật kết quả (" + viecPhaiLam.name + ")");
            //db.SaveChanges();
            //return Json(true, JsonRequestBehavior.AllowGet);
            var task = db.tasks.Find(viecPhaiLam.id);
            task.time = DateTime.Now;
            task.note = viecPhaiLam.result;
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            var job = db.Jobs.Find(task.job_id);
            NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Cập nhật kết quả (" + job.name + ")");
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ReloadViecPhaiLam(int hoso_id, int progress_type)
        {
            var progress = db.Progresses.FirstOrDefault(x => x.type == progress_type);
            var option = db.progress_file_option.FirstOrDefault(x => x.hoso_id == hoso_id && x.progress_id == progress.id);
            //var lstViecPhaiLams = db.ViecPhaiLams.Where(x => x.hoso_id == hoso_id && x.progress_type == progress_type).OrderByDescending(x => x.time);
            var lstJob = db.Jobs.Where(x => x.hoso_id == hoso_id && x.progress_type == progress_type).OrderByDescending(x => x.id);
            var lstViecPhaiLams = from j in lstJob
                                  join t in db.tasks on j.id equals t.job_id
                                  orderby t.id descending
                                  select t;

            var indexMax = lstViecPhaiLams.Count();
            var index = indexMax + 1;
            string result = string.Empty;
            foreach (var item in lstViecPhaiLams)
            {
                index--;
                if (index == indexMax)
                {
                    result +=
                                "<div class='col-md-12'>" +
                                    index.ToString() + ". Ngày tạo: " + item.time.ToString("dd/MM/yyyy") + " Ngày nhắc: " + item.time_exp.ToString("dd/MM/yyyy") + " Nội dung: " + item.name +
                                "</div>" +
                                "<div class='col-md-10'>" +
                                    "<input type=\"text\" name=\"viecphailam_result\" placeholder='Kết quả: " + item.name + "' value='" + item.note + "' class=\"form-control\" />" +
                                "</div>" +
                                "<div class=\"col -md-2 icon\" style=\"padding-top: 5px;\">";
                    if (item.time_exp <= DateTime.Now && string.IsNullOrEmpty(item.note) && (option == null || option.status == (int)XModels.eLevel.Level1))
                    {
                        result += "<i class=\"fa fa-bell-on text-danger\"></i>&nbsp;";
                    }
                    result += "<button class=\"fa fa-save btn btn-info\" style=\"border: none; \" onclick=\"ProgressSaveResult(" + item.id + ", " +
                        hoso_id + ", " + progress_type + ");\"></button>" + "</div>";
                }
                else
                {
                    result +=
                          "<div class=\"col-md-12\">" +
                          index.ToString() + ". Ngày tạo: " + item.time.ToString("dd/MM/yyyy") + " Ngày nhắc: " + item.time_exp.ToString("dd/MM/yyyy") +
                          " Nội dung: " + item.name + " Kết quả: " + item.note +
                      "</div>";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult ProgressesCalculate(int hoso_id)
        {
            var progresses = db.Progresses.Where(x => x.phantram.HasValue).Select(x => new { id = x.id, phantram = x.phantram }).ToArray();
            var options = db.progress_file_option.Where(x => x.hoso_id == hoso_id && x.status == (int)XModels.eLevel.Level2).Select(x => x.progress_id).ToArray();
            var phantram = progresses.Where(x => options.Contains(x.id)).Sum(x => x.phantram);
            return Json(phantram, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SettingAddFile(TaiLieu tailieu)
        {
            if (Request.Files.Count > 0)
            {
                var f0 = Request.Files[0];
                string subPath = Server.MapPath("~/public/Progresses");
                bool exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                subPath = Server.MapPath("~/public/Progresses/" + tailieu.type);
                exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                f0.SaveAs(subPath + "/" + f0.FileName);
                tailieu.link = "/public/Progresses/" + tailieu.type + "/" + f0.FileName;
                db.TaiLieux.Add(tailieu);
                db.SaveChanges();
            }
            return Json(tailieu, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SettingDelFile(int tailieu_id)
        {
            var tailieu = db.TaiLieux.Find(tailieu_id);
            db.TaiLieux.Remove(tailieu);
            db.SaveChanges();
            return Json(tailieu, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ProgressSetting(int progress_id, int hoso_id)
        {
            ViewBag.hoso_id = hoso_id;
            var progress = db.Progresses.Find(progress_id);
            return PartialView("../progresses/setting", progress);
        }
        [HttpGet]
        public ActionResult ProgressDownload(int progress_id, int hoso_id)
        {
            var temp = new xTemp();
            temp.int1 = progress_id;
            temp.int2 = hoso_id;
            return PartialView("../progresses/ProgressDownload", temp);
        }
        [HttpGet]
        public JsonResult SettingLoadDocuments(int hoso_id, int progress_id)
        {
            var progress = db.Progresses.Find(progress_id);
            var hoso = db.HoSoes.Find(hoso_id);
            //var documents = (from fk in db.fk_service_document
            //                 join doc in db.Documents on fk.document_id equals doc.id
            //                 where fk.service_id == hoso.service_id
            //                 select doc).ToArray();
            var documents = db.Documents.ToArray();
            string result = string.Empty;
            var checkeddoc = db.Progress_Document.Where(x => x.progress_type == progress.type).Select(x => x.document_id).ToArray();
            foreach (var t in XDocument.sType)
            {
                result += "<h5 class=\"text-danger center\" style=\"float: left; width: 100%;\"><i> " + t.Value + "</i></h5>";
                foreach (var item in documents.Where(x => x.type == t.Key))
                {
                    var ischeck = checkeddoc.Contains(item.id);
                    result +=
                        "<div class=\"col-sm-6\">" +
                        "   <input type=\"checkbox\" id=\"doc" + item.id + "\" " + (ischeck ? "checked" : "") + " />" +
                        "   <label style=\"font-weight: normal; \" for=\"doc" + item.id + "\" >" + item.name + "</label>" +
                        "</div>";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult SettingDocumentCheckedChange(int document_id, int progress_id, bool ischecked)
        {
            var process = db.Progresses.Find(progress_id);
            var process_document = db.Progress_Document.Where(x => x.document_id == document_id && x.progress_type == process.type);
            if (ischecked && process_document.Count() == 0)
            {
                var obj = new Progress_Document() { document_id = document_id, progress_type = process.type };
                db.Progress_Document.Add(obj);
            }
            else
            {
                db.Progress_Document.RemoveRange(process_document);
            }
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UploadDonThu(donthu obj)
        {
            var hoso = db.HoSoes.Find(obj.hoso_id);
            var contract = db.Contracts.Find(hoso.contract_id);
            var address = db.Addresses.Find(contract.address_id);
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            obj.alarm = DateTime.Now.AddDays(10);
            obj.account_id = us.id;

            if (Request.Files.Count > 0)
            {
                var f0 = Request.Files[0];

                string subPath = Server.MapPath("~/public/" + address.code);
                bool exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id);
                exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id + "/" + XModels.THUMUCHOSO);
                exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                f0.SaveAs(subPath + "/" + f0.FileName);
                obj.url = "/public/" + address.code + "/" + contract.id + "/" + XModels.THUMUCHOSO + "/" + f0.FileName;
                db.donthus.Add(obj);

                var job = db.Jobs.Where(x => x.hoso_id == hoso.id && x.process_by == hoso.account_id && x.status < (int)XJob.eStatus.Complete).FirstOrDefault();
                if (job == null)
                {
                    job = new Job();
                    job.created_by = us.id;
                    job.exp_date = obj.alarm;
                    job.hoso_id = hoso.id;
                    job.label = (int)XProgress.eBellType.Blue;
                    job.name = hoso.name;
                    job.process_by = hoso.account_id;
                    job.progress_type = (int)XProgress.eType.DonThu;
                    job.start_date = DateTime.Now;
                    job.status = (int)XJob.eStatus.Processing;
                    db.Jobs.Add(job);
                    db.SaveChanges();
                }
                else
                {
                    if (job.exp_date < obj.alarm)
                        job.exp_date = obj.alarm;
                    job.progress_type = (int)XProgress.eType.DonThu;
                }
                var t = new task();
                t.status = (int)XModels.eStatus.Processing;
                t.time = DateTime.Now;
                t.account_id = hoso.account_id;
                t.account_id_created = us.id;
                t.label = job.label;
                t.name = "Theo dõi, đôn đốc đơn";
                t.progress_type = job.progress_type;
                t.time_exp = job.exp_date;
                t.job_id = job.id;
                db.tasks.Add(t);

                NHTrans.SaveLog(db, us.id, "ĐƠN THƯ", "Thêm mới đơn thư (" + hoso.name + ")");
                NHTrans.SaveLog(db, us.id, "HỒ SƠ", "Tạo mới lịch hẹn (" + t.name + ")");

                db.SaveChanges();
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ShowDonThuModal(int donthu_id)
        {
            var donthu = db.donthus.Find(donthu_id);
            return PartialView("../Progresses/AddVBHoiAm", donthu);
        }
        [HttpPost]
        public JsonResult UploadDonThuAction(int donthu_id, string tieude, string ngaynhan, string noidung)
        {
            DateTime dteNgayNhan = Convert.ToDateTime(ngaynhan);

            var obj = new donthu { donthu_id = donthu_id };
            var donthu = db.donthus.Find(donthu_id);
            var hoso = db.HoSoes.Find(donthu.hoso_id);
            var contract = db.Contracts.Find(hoso.contract_id);
            var address = db.Addresses.Find(contract.address_id);
            if (Request.Files.Count > 0)
            {
                var f0 = Request.Files[0];

                string subPath = Server.MapPath("~/public/" + address.code);
                bool exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id);
                exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                subPath = Server.MapPath("~/public/" + address.code + "/" + contract.id + "/" + XModels.THUMUCHOSO);
                exists = System.IO.Directory.Exists(subPath);
                if (!exists)
                    System.IO.Directory.CreateDirectory(subPath);
                f0.SaveAs(subPath + "/" + f0.FileName);
                obj.url = "/public/" + address.code + "/" + contract.id + "/" + XModels.THUMUCHOSO + "/" + f0.FileName;
                obj.name = tieude;
                obj.time = dteNgayNhan;
                obj.alarm = DateTime.Now;
                obj.lydo = noidung;
                obj.hoso_id = hoso.id;
                var username = User.Identity.GetUserName();
                var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
                obj.account_id = us.id;
                db.donthus.Add(obj);
                NHTrans.SaveLog(db, us.id, "ĐƠN THƯ", "Thực hiện upload file (" + hoso.name + ")");
                db.SaveChanges();
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ReloadDonThu(int hoso_id, int progress_type)
        {
            if (progress_type != (int)XProgress.eType.DonThu)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var files = db.donthus.Where(x => x.hoso_id == hoso_id).ToArray();
            string result = string.Empty;
            foreach (var item in files.Where(x => !x.donthu_id.HasValue || x.donthu_id.Value == 0))
            {
                var lstHoiAm = files.Where(x => x.donthu_id == item.id);
                var lstNhatKyDons = db.nhatkydonthus.Where(x => x.donthu_id == item.id).OrderByDescending(x => x.time).ToArray();
                var time_exp = lstNhatKyDons.Length > 0 ? lstNhatKyDons.Max(x => x.time_exp) : DateTime.Now;
                result += "<tr>" +
                            "<td>" +
                                item.time.ToString("dd/MM/yy") +
                            "</td>" +
                            "<td>" +
                            "<a href='" + item.url + "' target='_blank'>" +
                            item.name +
                            "</a>" +
                            "</td>" +
                            "<td>" +
                                XModels.sLoaiDonThu[item.type] +
                            "</td>" +
                            //"<td>" +
                            //    item.alarm.ToString("dd/MM/yyyy") +
                            //    (item.alarm < DateTime.Now && (lstNhatKyDons.Length == 0 || time_exp < DateTime.Now) ? "<i class=\"fa fa-bell-on text-danger\" style='margin-left: 10px;display:inline;'></i>" : "") +
                            //"</td>" +
                            "<td>" +
                               item.lydo +
                            "</td>" +
                            "<td>";
                result += "<a class=\"fa fa-plus\" style=\"cursor: pointer; font-size: 18px;margin-right: 10px;\" onclick=\"DonThuAddHoiAm(" + item.id + "); \"></a>";
                result += "<ul style=\"margin:0; padding: 0; list-style: decimal;\">";
                foreach (var perHoiAm in lstHoiAm)
                {
                    result += "<li>" +
                        perHoiAm.time.ToString("dd/MM/yy") + " <a href=\"" + perHoiAm.url + "\" target=\"_blank\"><i> " + perHoiAm.name + "</i>; </a>" +
                        "<br />Nội dung: " + perHoiAm.lydo +
                        "</li>";
                }
                result += "</ul></td>" +
             "</tr>";
                result += "<tr><td style='border: none;' colspan='5'> Nhật ký đơn:";
                result += "</td></tr>";
                result += "<tr><td style='border: none;' colspan='5'><input type='text' style='display:inline-block; margin-right: 10px;width: 60%;' class='form-control' id='txtNKDT" + item.id + "' />" +
                    "<input class='datetime form-control' id='txtNKDTTime" + item.id + "' style='width: 120px;display:inline-block; margin-right: 10px;' />" +
                    "<button class='btn btn-primary fa fa-plus' style='width: 40px;' onclick='AddNhatKyDonThu(" + item.id + "," + hoso_id + "," + ((int)XProgress.eType.DonThu) + ")'></button>";
                result += "</td></tr>";
                foreach (var perNhatKy in lstNhatKyDons)
                {
                    result += "<tr><td style='border: none;' colspan='5'><i>" + perNhatKy.time.ToString("dd/MM/yy") + " - " + perNhatKy.time_exp.ToString("dd/MM/yy") + " - " + perNhatKy.note;
                    result += "</i></td></tr>";
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddNhatKyDonThu(int donthu_id, string note, string time_exp)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();

            nhatkydonthu obj = new nhatkydonthu();
            obj.donthu_id = donthu_id;
            obj.time_exp = Convert.ToDateTime(time_exp);
            obj.note = note;
            obj.account_id = us.id;
            obj.time = DateTime.Now;
            db.nhatkydonthus.Add(obj);
            NHTrans.SaveLog(db, us.id, "ĐƠN THƯ", "Thực hiện thêm nhật ký đơn thư");
            db.SaveChanges();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
    }
}
