using Aspose.Words.Rendering;
using Newtonsoft.Json;
using NiceHandles.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Security;
using Microsoft.AspNet.Identity;
using System.Reflection;
using Aspose.Words;
using FlexCel.Report;
using Microsoft.Owin.BuilderProperties;
using System.Diagnostics.Contracts;
using System.IO;
using System.Xml.Linq;
using System.Web.UI.WebControls.WebParts;
using Aspose.Words.Drawing;
using System.Drawing;
using Aspose.Words.Markup;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NiceHandles.Controllers
{
    [Authorize(Roles = "SuperAdmin,Manager,Member")]
    public class HomeController : Controller
    {
        private NHModel db = new NHModel();
        [ValidateInput(false)]
        public ActionResult Send(stickeynote obj)
        {
            Account account = db.Accounts.Find(Convert.ToInt32(obj.id));
            account.sticknote = obj.txt;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public ActionResult SendX(string contents)
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            us.sticknote = contents;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [ValidateInput(false)]
        public ActionResult SendNotice(stickeynote obj)
        {
            NiceHandles.Models.Setting st = db.Settings.Where(x => x.code.Equals("TRAINNING")).First();
            st.data = obj.txt;
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult Send(string strObj)
        //{
        //    var obj = JsonConvert.DeserializeObject<stickeynote>(strObj);
        //    Account account = db.Accounts.Find(Convert.ToInt32(obj.id));
        //    account.sticknote = obj.txt;
        //    db.SaveChanges();
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Refresh()
        {
            List<stickeynote> lst = new List<stickeynote>();
            Account account = db.Accounts.Find(4);
            lst.Add(new stickeynote() { id = "4", txt = account.sticknote });
            account = db.Accounts.Find(3);
            lst.Add(new stickeynote() { id = "3", txt = account.sticknote });
            account = db.Accounts.Find(2);
            lst.Add(new stickeynote() { id = "2", txt = account.sticknote });
            //account = db.Accounts.Find(7);
            //lst.Add(new stickeynote() { id = "7", txt = account.sticknote });
            db.SaveChanges();
            return Json(lst.ToArray(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult RefreshX()
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            return Json(us.sticknote, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RefreshNotice()
        {
            NiceHandles.Models.Setting st = db.Settings.Where(x => x.code.Equals("TRAINNING")).First();
            return Json(st.data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Cad(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoSo hoso = db.HoSoes.Find(id);
            ViewBag.GoogleLink = hoso.link_filecad;
            return View();
        }
        public ActionResult Notice(string Search_Data, string Filter_Value, int? Page_No)
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
            int Size_Of_Page = 20;
            int No_Of_Page = (Page_No ?? 1);
            IQueryable<Notice> result = db.Notices;
            if (!string.IsNullOrEmpty(Search_Data))
            {
                result = result.Where(x => x.title.Contains(Search_Data) || x.contents.Contains(Search_Data));
            }
            return View(result.OrderByDescending(x => x.update_date).ToPagedList(No_Of_Page, Size_Of_Page));
        }
        public ActionResult GiaPha()
        {
            ViewBag.db = db;
            return View();
        }
        public ActionResult SoDoHuyetThong()
        {
            ViewBag.db = db;
            var hoso_id = 2406;
            var hoso = db.HoSoes.Find(hoso_id);
            var contract = db.Contracts.Find(hoso.contract_id);
            var address = db.Addresses.Find(contract.address_id);

            var infomation = db.Infomations.Single(x => x.hoso_id == hoso_id);
            var ttcns = db.ThongTinCaNhans.Where(x => x.infomation_id == infomation.id).ToArray();
            var chudat = ttcns.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.ChuDat).SingleOrDefault();
            var HTK1CHONGTREN = ttcns.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1 && x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeChong);
            var bo = HTK1CHONGTREN.SingleOrDefault(x => x.gioitinh.ToUpper().Equals("NAM"));
            var me = HTK1CHONGTREN.SingleOrDefault(x => x.gioitinh.ToUpper().Equals("NỮ"));
            var HTK1VOTREN = ttcns.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1 && x.quanhe == (int)XThongTinCaNhan.eQuanHe.BoMeVo);
            var bo1 = HTK1VOTREN.SingleOrDefault(x => x.gioitinh.ToUpper().Equals("NAM"));
            var me1 = HTK1VOTREN.SingleOrDefault(x => x.gioitinh.ToUpper().Equals("NỮ"));
            if (chudat == null)
            {
                chudat = new ThongTinCaNhan();
                chudat.type = (int)XThongTinCaNhan.eType.LanDau;
                chudat.gioitinh = XModels.sGender[0];
                chudat.gioitinh1 = XModels.sGender[1];
                chudat.hangthuake = (int)XThongTinCaNhan.eHangThuaKe.ChuDat;
                chudat.infomation_id = (int)infomation.id;
                db.ThongTinCaNhans.Add(chudat);
                db.SaveChanges();
            }
            if (chudat.ngaychet.HasValue && bo == null)
            {
                bo = new ThongTinCaNhan();
                bo.fk_id = chudat.id;
                bo.quanhe = (int)XThongTinCaNhan.eQuanHe.BoMeChong;
                bo.type = (int)XThongTinCaNhan.eType.LanDau;
                bo.gioitinh = XModels.sGender[0];
                bo.hangthuake = (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1;
                bo.infomation_id = chudat.infomation_id;
                db.ThongTinCaNhans.Add(bo);
            }
            if (chudat.ngaychet.HasValue && me == null)
            {
                me = new ThongTinCaNhan();
                me.fk_id = chudat.id;
                me.quanhe = (int)XThongTinCaNhan.eQuanHe.BoMeChong;
                me.type = (int)XThongTinCaNhan.eType.LanDau;
                me.gioitinh = XModels.sGender[1];
                me.hangthuake = (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1;
                me.infomation_id = chudat.infomation_id;
                db.ThongTinCaNhans.Add(me);
            }
            if (chudat.ngaychet1.HasValue && bo1 == null)
            {
                bo1 = new ThongTinCaNhan();
                bo1.fk_id = chudat.id;
                bo1.quanhe = (int)XThongTinCaNhan.eQuanHe.BoMeVo;
                bo1.type = (int)XThongTinCaNhan.eType.LanDau;
                bo1.gioitinh = XModels.sGender[0];
                bo1.hangthuake = (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1;
                bo1.infomation_id = chudat.infomation_id;
                db.ThongTinCaNhans.Add(bo1);
            }
            if (chudat.ngaychet1.HasValue && me1 == null)
            {
                me1 = new ThongTinCaNhan();
                me1.fk_id = chudat.id;
                me1.quanhe = (int)XThongTinCaNhan.eQuanHe.BoMeVo;
                me1.type = (int)XThongTinCaNhan.eType.LanDau;
                me1.gioitinh = XModels.sGender[1];
                me1.hangthuake = (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1;
                me1.infomation_id = chudat.infomation_id;
                db.ThongTinCaNhans.Add(me1);
            }

            db.SaveChanges();

            var conde = ttcns.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe1 && x.quanhe == (int)XThongTinCaNhan.eQuanHe.Con).ToList();
            var hang1 = new List<ThongTinCaNhan>();
            var hang2 = new List<ThongTinCaNhan>();
            foreach (var item in conde)
            {
                var daure = ttcns.Where(x => x.quanhe == (int)XThongTinCaNhan.eQuanHe.VoChong && x.fk_id == item.id);
                hang1.AddRange(daure);
            }
            hang2 = ttcns.Where(x => x.hangthuake == (int)XThongTinCaNhan.eHangThuaKe.HangThuaKe2 && x.quanhe == (int)XThongTinCaNhan.eQuanHe.Con).ToList();
            var socon = hang1.Count;
            var sochau = hang2.Count;

            var cols = socon > sochau ? socon : sochau; cols = cols * 4;

            string path = Server.MapPath("~/public/");
            string pathTemp = Server.MapPath("~/App_Data/templates/SoDoHuyetThong.docx");
            string webpart = "sodohuyetthong.docx";

            Aspose.Words.Document doc = new Aspose.Words.Document(pathTemp);
            webpart += ".docx";
            path += webpart;
            var dict = new Dictionary<string, string>();

            doc.MailMerge.Execute(dict.Keys.ToArray(), dict.Values.ToArray());

            Aspose.Words.DocumentBuilder builder = new Aspose.Words.DocumentBuilder(doc);
            //builder.MoveToMergeField("danhsachnguoicamket");
            //builder.
            Shape arrow = new Shape(doc, ShapeType.Line);
            arrow.Width = 200;
            arrow.Stroke.Color = Color.Red;
            arrow.Stroke.StartArrowType = ArrowType.Arrow;
            arrow.Stroke.StartArrowLength = ArrowLength.Long;
            arrow.Stroke.StartArrowWidth = ArrowWidth.Wide;
            arrow.Stroke.EndArrowType = ArrowType.Diamond;
            arrow.Stroke.EndArrowLength = ArrowLength.Long;
            arrow.Stroke.EndArrowWidth = ArrowWidth.Wide;
            arrow.Stroke.DashStyle = DashStyle.Dash;
            arrow.Stroke.Opacity = 0.5;


            builder.InsertNode(arrow);

            Shape line = new Shape(doc, ShapeType.Line);
            line.Top = 40;
            line.Width = 200;
            line.Height = 20;
            line.StrokeWeight = 5.0;
            line.Stroke.EndCap = EndCap.Round;

            builder.InsertNode(line);

            // 3 -  Arrow with a green fill:
            Shape filledInArrow = new Shape(doc, ShapeType.Arrow);
            filledInArrow.Width = 200;
            filledInArrow.Height = 40;
            filledInArrow.Top = 100;
            filledInArrow.Fill.Color = Color.Green;
            //filledInArrow.Fill. = true;

            builder.InsertNode(filledInArrow);

            // 4 -  Arrow with a flipped orientation filled in with the Aspose logo:
            Shape filledInArrowImg = new Shape(doc, ShapeType.Arrow);
            filledInArrowImg.Width = 200;
            filledInArrowImg.Height = 40;
            filledInArrowImg.Top = 160;
            filledInArrowImg.FlipOrientation = FlipOrientation.Both;

            byte[] imageBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/favicon.ico"));

            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                Image image = Image.FromStream(stream);
                // When we flip the orientation of our arrow, we also flip the image that the arrow contains.
                // Flip the image the other way to cancel this out before getting the shape to display it.
                image.RotateFlip(RotateFlipType.RotateNoneFlipXY);

                filledInArrowImg.ImageData.SetImage(image);
                filledInArrowImg.Stroke.JoinStyle = JoinStyle.Round;

                builder.InsertNode(filledInArrowImg);
            }

            Shape textBoxShape = new Shape(doc, ShapeType.TextBox);
            TextBox textBox = textBoxShape.TextBox;
            builder.InsertNode(textBoxShape);
            textBoxShape.FillColor = Color.Red;
            Paragraph p = new Paragraph(doc);



            textBoxShape.AppendChild(p);
            builder.MoveTo(textBoxShape.LastParagraph);
            builder.Writeln("Hello world!");
            builder.Write("Hello again!");
            // Move the document builder to inside the TextBox and add text.

            doc.Save(path);
            ViewBag.URL = "/public/" + webpart;
            return View();
        }
        public ActionResult Mission()
        {
            return View();
        }
        public ActionResult Index()
        {
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            ViewBag.NOTE = us.sticknote;
            return View();
        }
        public ActionResult GetNotice()
        {
            var chuyevuon = db.Services.Where(x => x.code == "chuyenvuon").Single();
            var username = User.Identity.GetUserName();
            var us = db.Accounts.Where(x => x.UserName.Equals(username)).Single();
            List<KeyValuePair<string, string>> notices = new List<KeyValuePair<string, string>>();
            // Hạn thực hiện
            var missions = db.Jobs.Where(x => x.exp_date <= DateTime.Today && x.status < (int)XJob.eStatus.Complete &&
                            (x.process_by == us.id || x.created_by == us.id)).ToArray();
            foreach (var item in missions)
            {
                var tasks = db.tasks.Where(x => x.job_id == item.id && x.status == (int)XModels.eStatus.Processing).Select(x => x.name).ToArray();
                notices.Add(new KeyValuePair<string, string>(item.name + "; " + item.note, string.Join("; ", tasks)));
            }
            // Hồ sơ 1 cửa
            var ods = db.vDi1Cua.Where(x => x.account_id == us.id && x.status == (int)XHoSo.eStatus.Processing && x.service_id != chuyevuon.id);
            foreach (var item in ods)
            {
                var danop = (DateTime.Now - item.ngaynop).TotalDays;
                if (danop >= 3 && danop <= 5)
                {
                    notices.Add(new KeyValuePair<string, string>("Hạn kiểm tra", "Hồ sơ " + item.name + ", " + " đã nộp 5 ngày"));
                }
                var toihan = (DateTime.Now - item.ngaytra).TotalDays;
                if (toihan >= -7 && toihan <= -5)
                {
                    notices.Add(new KeyValuePair<string, string>("Hạn trả kết quả", "Hồ sơ " + item.name + ", " + " sắp đến hạn"));
                }
                else if (toihan == -2)
                {
                    notices.Add(new KeyValuePair<string, string>("Hạn trả kết quả", "Hồ sơ " + item.name + ", " + " còn 2 ngày đến hạn"));
                }
                else if (toihan == -1)
                {
                    notices.Add(new KeyValuePair<string, string>("Hạn trả kết quả", "Hồ sơ " + item.name + " còn 1 ngày đến hạn"));
                }
                else if (toihan == 0)
                {
                    notices.Add(new KeyValuePair<string, string>("Hạn trả kết quả", "Hồ sơ " + item.name + " đến hạn"));
                }
                else if (toihan == 1)
                {
                    notices.Add(new KeyValuePair<string, string>("Hạn trả kết quả", "Hồ sơ " + item.name + " đã quá hạn"));
                }
                else if (toihan > 7)
                {
                    notices.Add(new KeyValuePair<string, string>("Hạn trả kết quả", "Hồ sơ " + item.name + " quá chậm"));
                }
            }
            var lastWeek = DateTime.Now.AddDays(7);
            var pendings = db.vPendings.Where(x => (x.account_id == us.id || x.soanthao == us.id) && x.status == (int)XContract.eStatus.Processing && x.service_id != chuyevuon.id).GroupBy(x => x.id);
            foreach (var item in pendings)
            {
                var last = item.OrderBy(x => x.time).Last().time;
                if ((DateTime.Now - last).TotalDays > 10)
                {
                    var contract = db.Contracts.Find(item.Key);
                    var service = db.Services.Find(contract.service_id);
                    notices.Add(new KeyValuePair<string, string>("Bước thực hiện", "Hồ sơ " + contract.name + ", " + service.name + " quá lâu không có hành động"));
                }
            }
            return Json(notices, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Plugin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Plugin(string txtLink)
        {
            if (!string.IsNullOrEmpty(txtLink))
            {
                string path = Server.MapPath("~/public");
                //string url = "https://chart.googleapis.com/chart?cht=qr&chl=" + txtLink + "&choe=UTF-8&chs=400x400";
                var filename = "/temp/qr" + DateTime.Now.ToString("ddMMyyHHmmss") + ".jpg";
                WebClient webClient = new WebClient();
                try
                {
                    string url = "https://quickchart.io/qr?text=" + txtLink + "&size=400";
                    webClient.DownloadFile(url, path + filename);
                }
                catch
                {
                    string url = "https://chart.googleapis.com/chart?cht=qr&chl=" + txtLink + "&choe=UTF-8&chs=400x400";
                    webClient.DownloadFile(url, path + filename);
                }
                ViewBag.URL = filename;
            }
            return View();
        }
    }
    public class stickeynote
    {
        public string txt { get; set; }
        public string id { get; set; }
    }

}