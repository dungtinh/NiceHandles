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
                .Include(h => h.Service)
                .Include(h => h.LandParcel)
                .Include(h => h.HoSoPersons.Select(hp => hp.PersonInfo))
                .Include(h => h.VariationInfo)
                .SingleOrDefault(h => h.id == id);

            if (hoso == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách Owners - Chủ đất (Role = 0)
            var owners = hoso.HoSoPersons
                .Where(hp => hp.Role == (int)HoSoPersonRole.Owner)
                .OrderBy(hp => hp.Id)
                .Select(hp => hp.PersonInfo)
                .ToList();

            // Lấy danh sách Buyers - Người mua (Role = 1)
            var buyers = hoso.HoSoPersons
                .Where(hp => hp.Role == (int)HoSoPersonRole.Buyer)
                .OrderBy(hp => hp.Id)
                .Select(hp => hp.PersonInfo)
                .ToList();

            // Lấy danh sách Heirs - Người nhận thừa kế (Role = 2)
            var heirs = hoso.HoSoPersons
                .Where(hp => hp.Role == (int)HoSoPersonRole.Heir)
                .OrderBy(hp => hp.Id)
                .Select(hp => hp.PersonInfo)
                .ToList();

            var viewModel = new InfomationEditViewModel
            {
                HoSo = hoso,
                Owners = owners.Any() ? owners : new List<PersonInfo> { new PersonInfo() },
                Buyers = buyers,
                Heirs = heirs,
                LandParcel = hoso.LandParcel ?? new LandParcel(),
                VariationInfo = hoso.VariationInfo ?? new VariationInfo(),
                ServiceCode = hoso.Service?.code ?? ""
            };

            ViewBag.ServiceCode = hoso.Service?.code ?? "";
            ViewBag.Addresses = new SelectList(db.Addresses.OrderBy(a => a.name), "id", "name");

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
                        .Include(h => h.Service)
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

                    // Thêm Owners mới - Chủ đất (Role = 0)
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
                                address_id = owner.address_id,
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
                                Role = (int)HoSoPersonRole.Owner // Chủ đất
                            };

                            db.HoSoPersons.Add(hosoPerson);
                        }
                    }

                    // Thêm Buyers mới - Người mua (Role = 1)
                    if (model.Buyers != null)
                    {
                        foreach (var buyer in model.Buyers.Where(b => !string.IsNullOrEmpty(b.FullName)))
                        {
                            var personInfo = new PersonInfo
                            {
                                FullName = buyer.FullName ?? "---",
                                DocumentType = buyer.DocumentType ?? "---",
                                DocumentNumber = buyer.DocumentNumber ?? "---",
                                BirthDate = buyer.BirthDate,
                                Gender = buyer.Gender ?? "---",
                                address_id = buyer.address_id,
                                IssueDate = buyer.IssueDate,
                                Issuer = buyer.Issuer ?? "---",
                                TaxCode = buyer.TaxCode ?? "---",
                                DeathDate = buyer.DeathDate,
                                DeathDocument = buyer.DeathDocument,
                                HeirId = buyer.HeirId
                            };

                            db.PersonInfoes.Add(personInfo);

                            var hosoPerson = new HoSoPerson
                            {
                                HoSoId = hoso.id,
                                PersonInfo = personInfo,
                                Role = (int)HoSoPersonRole.Buyer // Người mua
                            };

                            db.HoSoPersons.Add(hosoPerson);
                        }
                    }

                    // Thêm Heirs mới - Người nhận thừa kế (Role = 2)
                    if (model.Heirs != null)
                    {
                        foreach (var heir in model.Heirs.Where(h => !string.IsNullOrEmpty(h.FullName)))
                        {
                            var personInfo = new PersonInfo
                            {
                                FullName = heir.FullName ?? "---",
                                DocumentType = heir.DocumentType ?? "---",
                                DocumentNumber = heir.DocumentNumber ?? "---",
                                BirthDate = heir.BirthDate,
                                Gender = heir.Gender ?? "---",
                                address_id = heir.address_id,
                                IssueDate = heir.IssueDate,
                                Issuer = heir.Issuer ?? "---",
                                TaxCode = heir.TaxCode ?? "---",
                                DeathDate = heir.DeathDate,
                                DeathDocument = heir.DeathDocument,
                                HeirId = heir.HeirId
                            };

                            db.PersonInfoes.Add(personInfo);

                            var hosoPerson = new HoSoPerson
                            {
                                HoSoId = hoso.id,
                                PersonInfo = personInfo,
                                Role = (int)HoSoPersonRole.Heir // Người nhận thừa kế
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

                    // Reload data for view
                    ViewBag.ServiceCode = db.HoSoes.Find(id)?.Service?.code ?? "";
                    ViewBag.Addresses = new SelectList(db.Addresses.OrderBy(a => a.name), "id", "name");

                    return View(model);
                }
            }
        }

        // API: Get addresses for Select2
        public JsonResult GetAddresses(string q = "")
        {
            var addresses = db.Addresses
                .Where(a => a.name.Contains(q))
                .OrderBy(a => a.name)
                .Select(a => new { id = a.id, text = a.name })
                .Take(20)
                .ToList();

            return Json(addresses, JsonRequestBehavior.AllowGet);
        }

    }
}
