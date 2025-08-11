using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using Aspose.Words.Drawing;
using FlexCel.Report;
using Microsoft.AspNet.Identity;
using NiceHandles.Models;
using NiceHandles.ViewModels;
using PagedList;
using static NiceHandles.Models.XHoSo;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Manager,Member")]
    public partial class HoSoesController 
    {
        public ActionResult ImportData(int id)
        {
            var hoso = db.HoSoes
                .Include(h => h.LandParcel)
                .Include(h => h.HoSoPersons.Select(hp => hp.PersonInfo))
                .Include(h => h.VariationInfo)
                .SingleOrDefault(h => h.id == id);

            if (hoso == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách Owners (Role = 0)
            var owners = hoso.HoSoPersons
                .Where(hp => hp.Role == 0) // Role = 0 for Owners
                .OrderBy(hp => hp.Id)
                .Select(hp => hp.PersonInfo)
                .ToList();

            // Lấy danh sách BuyersOrSellers (Role = 1)
            var buyersOrSellers = hoso.HoSoPersons
                .Where(hp => hp.Role == 1) // Role = 1 for BuyersOrSellers
                .OrderBy(hp => hp.Id)
                .Select(hp => hp.PersonInfo)
                .ToList();

            var viewModel = new InfomationEditViewModel
            {
                HoSo = hoso,
                Owners = owners.Any() ? owners : new List<PersonInfo> { new PersonInfo() },
                BuyersOrSellers = buyersOrSellers,
                LandParcel = hoso.LandParcel ?? new LandParcel(),
                VariationInfo = hoso.VariationInfo ?? new VariationInfo()
            };

            return View(viewModel);
        }

        // POST: HoSoes/ImportData/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ImportData(int id, InfomationEditViewModel model)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var hoso = db.HoSoes
                        .Include(h => h.LandParcel)
                        .Include(h => h.HoSoPersons.Select(hp => hp.PersonInfo))
                        .Include(h => h.VariationInfo)
                        .SingleOrDefault(h => h.id == id);

                    if (hoso == null)
                    {
                        return HttpNotFound();
                    }

                    // Xóa tất cả HoSoPerson và PersonInfo cũ
                    var oldHoSoPersons = hoso.HoSoPersons.ToList();
                    foreach (var oldHP in oldHoSoPersons)
                    {
                        if (oldHP.PersonInfo != null)
                        {
                            db.PersonInfoes.Remove(oldHP.PersonInfo);
                        }
                        db.HoSoPersons.Remove(oldHP);
                    }

                    // Thêm Owners mới
                    if (model.Owners != null)
                    {
                        foreach (var owner in model.Owners.Where(o => !string.IsNullOrEmpty(o.FullName)))
                        {
                            var personInfo = new PersonInfo
                            {
                                FullName = owner.FullName ?? "---",
                                DocumentType = owner.DocumentType ?? "---",
                                DocumentNumber = owner.DocumentNumber ?? "---",
                                BirthDate = owner.BirthDate,
                                Gender = owner.Gender ?? "---",                                                                
                                Address = owner.Address,
                                IssueDate = owner.IssueDate,
                                Issuer = owner.Issuer ?? "---",
                                TaxCode = owner.TaxCode ?? "---",                                
                                DeathDate = owner.DeathDate,
                                DeathDocument = owner.DeathDocument,
                                HeirId = owner.HeirId
                            };

                            db.PersonInfoes.Add(personInfo);

                            var hosoPerson = new HoSoPerson
                            {
                                HoSoId = hoso.id,
                                PersonInfo = personInfo,
                                Role = 0 // Owner
                            };

                            db.HoSoPersons.Add(hosoPerson);
                        }
                    }

                    // Thêm BuyersOrSellers mới
                    if (model.BuyersOrSellers != null)
                    {
                        foreach (var buyerSeller in model.BuyersOrSellers.Where(bs => !string.IsNullOrEmpty(bs.FullName)))
                        {
                            var personInfo = new PersonInfo
                            {
                                FullName = buyerSeller.FullName ?? "---",
                                DocumentType = buyerSeller.DocumentType ?? "---",
                                DocumentNumber = buyerSeller.DocumentNumber ?? "---",
                                BirthDate = buyerSeller.BirthDate,
                                Gender = buyerSeller.Gender ?? "---",
                                Address = buyerSeller.Address,                                
                                IssueDate = buyerSeller.IssueDate,
                                Issuer = buyerSeller.Issuer ?? "---",
                                TaxCode = buyerSeller.TaxCode ?? "---",                                
                                DeathDate = buyerSeller.DeathDate,
                                DeathDocument = buyerSeller.DeathDocument,
                                HeirId = buyerSeller.HeirId
                            };

                            db.PersonInfoes.Add(personInfo);

                            var hosoPerson = new HoSoPerson
                            {
                                HoSoId = hoso.id,
                                PersonInfo = personInfo,
                                Role = 1 // BuyerOrSeller
                            };

                            db.HoSoPersons.Add(hosoPerson);
                        }
                    }

                    // Update hoặc tạo mới LandParcel
                    if (model.LandParcel != null)
                    {
                        if (hoso.LandParcel == null)
                        {
                            hoso.LandParcel = new LandParcel();
                            db.LandParcels.Add(hoso.LandParcel);
                        }

                        hoso.LandParcel.CertificateNumber = model.LandParcel.CertificateNumber ?? "---";
                        hoso.LandParcel.ParcelNumber = model.LandParcel.ParcelNumber ?? "---";
                        hoso.LandParcel.MapSheet = model.LandParcel.MapSheet ?? "---";
                        hoso.LandParcel.Address = model.LandParcel.Address;
                        hoso.LandParcel.ActualArea = model.LandParcel.ActualArea;
                        hoso.LandParcel.CertifiedArea = model.LandParcel.CertifiedArea;
                        hoso.LandParcel.UsagePurpose = model.LandParcel.UsagePurpose ?? "---";
                        hoso.LandParcel.IssueDate = model.LandParcel.IssueDate;
                        hoso.LandParcel.Issuer = model.LandParcel.Issuer ?? "---";
                        hoso.LandParcel.BookNumber = model.LandParcel.BookNumber ?? "---";                        
                    }

                    // Update hoặc tạo mới VariationInfo
                    if (model.VariationInfo != null)
                    {
                        if (hoso.VariationInfo == null)
                        {
                            hoso.VariationInfo = new VariationInfo();
                            db.VariationInfoes.Add(hoso.VariationInfo);
                        }

                        hoso.VariationInfo.VariationType = model.VariationInfo.VariationType ?? "---";                        
                        hoso.VariationInfo.ContractNumber = model.VariationInfo.ContractNumber ?? "---";
                        hoso.VariationInfo.NotaryOffice = model.VariationInfo.NotaryOffice ?? "---";
                        hoso.VariationInfo.NotaryDate = model.VariationInfo.NotaryDate;
                        hoso.VariationInfo.ContractAmount = model.VariationInfo.ContractAmount;
                        hoso.VariationInfo.TaxReductionReason = model.VariationInfo.TaxReductionReason ?? "---";
                        hoso.VariationInfo.LandPosition = model.VariationInfo.LandPosition ?? "---";                        
                    }

                    db.SaveChanges();
                    transaction.Commit();

                    TempData["Success"] = "Dữ liệu đã được lưu thành công!";
                    return RedirectToAction("ImportData", new { id = hoso.id });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    ModelState.AddModelError("", "Lỗi khi lưu dữ liệu: " + ex.Message);
                    return View(model);
                }
            }
        }

        // AJAX: Load dữ liệu từ bảng Infomation cũ
        [HttpGet]
        public JsonResult DupplicateInfomation(int id)
        {
            try
            {
                var hoso = db.HoSoes.Find(id);
                if (hoso == null)
                {
                    return Json(new { error = "Không tìm thấy hồ sơ" }, JsonRequestBehavior.AllowGet);
                }

                var info = db.Infomations.FirstOrDefault(x => x.hoso_id == hoso.id);
                if (info == null)
                {
                    return Json(new { error = "Không tìm thấy dữ liệu Infomation" }, JsonRequestBehavior.AllowGet);
                }

                // Return full Infomation object
                return Json(info, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
      
        // Helper methods cho parsing
        private DateTime? ParseDate(string dateStr)
        {
            if (string.IsNullOrEmpty(dateStr))
                return null;

            DateTime result;
            if (DateTime.TryParse(dateStr, out result))
                return result;

            return null;
        }

        private decimal? ParseDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            decimal result;
            if (decimal.TryParse(value, out result))
                return result;

            return null;
        }
      
    }
}
