using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Parser.SyntaxTree;
using NiceHandles.Models;
using PagedList;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Manager,Member,OutSide")]
    public class DoveController : Controller
    {
        private NHModel db = new NHModel();

        // GET: NhiemVus        
        public ActionResult Index(string Search_Data, string Filter_Value, int? Page_No)
        {
            if (Search_Data != null)
            {
                Page_No = 1;
            }
            else
            {
                Search_Data = Filter_Value;
            }
            ViewBag.FilterValue = Search_Data;

            var steps = db.Steps.Where(x => x.category_id != (int)XStep.ePhong.HSLuu).Select(x => x.id).ToArray();
            //var steps = db.Steps.Where(x => x.category_id == (int)XStep.ePhong.DoVe).Select(x => x.id).ToArray();

            var results = from io in db.DSBanLamViecs
                          join s in steps on io.step equals s
                          where
                               io.customer_name.ToUpper().Contains(!string.IsNullOrEmpty(Search_Data) ? Search_Data.ToUpper() : io.customer_name.ToUpper())
                          orderby io.Priority descending, io.time descending
                          select io;
            int Size_Of_Page = 150;
            int No_Of_Page = (Page_No ?? 1);
            return View(results.ToPagedList(No_Of_Page, Size_Of_Page));
        }
        public ActionResult Dove(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var nv = db.NhiemVus.Where(x => x.contract_id == id).FirstOrDefault();
            if (nv == null)
            {
                nv = new NhiemVu();
                nv.step_id = db.Steps.First().id;
                nv.contract_id = id.Value;
                db.NhiemVus.Add(nv);
                db.SaveChanges();
            }
            var contract = db.Contracts.Find(id);                        
            var address = db.Addresses.Find(contract.address_id);
            ViewBag.Address = address;
            ViewBag.Contract = contract;
            //var sid = db.fk_contract_service.Where(x => x.contract_id == id).First().service_id;
            var service = db.Services.Find(contract.service_id);
            ViewBag.Service = service;
            var fs = db.Files.Where(x => x.contract_id == id);
            Dictionary<int, string> FILES = new Dictionary<int, string>();
            foreach (var per in XFile.sType)
            {
                string temp = string.Empty;
                foreach (var item in fs.Where(x => x.type == per.Key))
                {
                    temp += "<a href='" + item.url + "' target='_blank' >" + item.name + "</a><span id='" + item.id + "' class='del'>x</span> | ";
                }
                FILES.Add(per.Key, temp);
            }
            ViewBag.FILES = FILES;
            return View(nv);
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
