using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using NiceHandles.Models;
using Microsoft.AspNet.Identity;
using System.Reflection;

namespace NiceHandles
{
    public class Common
    {
        static NHModel db = new NHModel();
        public static IQueryable<Document> GetDocumentsByService(int _id)
        {
            var result = from fk in db.fk_service_document
                         join dc in db.Documents on fk.document_id equals dc.id
                         where fk.service_id == _id
                         select dc;
            return result;
        }
        public static IQueryable<Document> GetDocuments()
        {
            var result = from dc in db.Documents
                         select dc;
            return result;
        }
        public static IQueryable<Account> cAccounts()
        {
            var result = from rs in db.Accounts
                         where rs.sta == (int)XAccount.eStatus.Processing
                         select rs;
            return result;
        }
    }
    public class NHTrans
    {
        public static void SaveLog(NHModel db, int account_id, string caption, string message)
        {
            var log = new syslog();
            log.message = message;
            log.caption = caption;
            log.account_id = account_id;
            log.created_time = DateTime.Now;
            db.syslogs.Add(log);
        }
    }
    public static class Utils
    {
        /// 
        public const string AI_API_KEY = "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";
        public static string LessString(string longstring, int length, string suffix)
        {
            if (!string.IsNullOrEmpty(longstring))
            {
                var len = longstring.Length;
                if (len > length)
                {
                    longstring = longstring.Substring(0, length) + suffix;
                }
            }
            return longstring;
        }
        public static string SetCodeInout(Category cate)
        {
            string result = (cate.type == (int)XCategory.eType.Thu ? "T" : "C");
            string[] names = cate.name.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            result += string.Join("", names.Select(x => x.Substring(0, 1))).ToUpper();
            result += DateTime.Now.ToString("ddMMyy");
            return result;
        }
        /// Chuyển phần nguyên của số thành chữ
        /// 
        /// Số double cần chuyển thành chữ
        /// Chuỗi kết quả chuyển từ số
        public static string NumberToText(double inputNumber, bool suffix = true)
        {
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            // -12345678.3445435 => "-12345678"
            string sNumber = inputNumber.ToString("#");
            double number = Convert.ToDouble(sNumber);
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;   // last -> first

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "một " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            result = result.Trim();
            if (isNegative) result = "Âm " + result;
            return result + (suffix ? " đồng chẵn" : "");
        }
        public static string DecimalToText(double inputNumber)
        {
            string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
            bool isNegative = false;

            // -12345678.3445435 => "-12345678"            
            double number = Math.Truncate(inputNumber);
            string sNumber = number.ToString("#");
            if (number < 0)
            {
                number = -number;
                sNumber = number.ToString();
                isNegative = true;
            }


            int ones, tens, hundreds;

            int positionDigit = sNumber.Length;   // last -> first

            string result = " ";


            if (positionDigit == 0)
                result = unitNumbers[0] + result;
            else
            {
                // 0:       ###
                // 1: nghìn ###,###
                // 2: triệu ###,###,###
                // 3: tỷ    ###,###,###,###
                int placeValue = 0;

                while (positionDigit > 0)
                {
                    // Check last 3 digits remain ### (hundreds tens ones)
                    tens = hundreds = -1;
                    ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                    positionDigit--;
                    if (positionDigit > 0)
                    {
                        tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                        positionDigit--;
                        if (positionDigit > 0)
                        {
                            hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
                            positionDigit--;
                        }
                    }

                    if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
                        result = placeValues[placeValue] + result;

                    placeValue++;
                    if (placeValue > 3) placeValue = 1;

                    if ((ones == 1) && (tens > 1))
                        result = "một " + result;
                    else
                    {
                        if ((ones == 5) && (tens > 0))
                            result = "lăm " + result;
                        else if (ones > 0)
                            result = unitNumbers[ones] + " " + result;
                    }
                    if (tens < 0)
                        break;
                    else
                    {
                        if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
                        if (tens == 1) result = "mười " + result;
                        if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
                    }
                    if (hundreds < 0) break;
                    else
                    {
                        if ((hundreds > 0) || (tens > 0) || (ones > 0))
                            result = unitNumbers[hundreds] + " trăm " + result;
                    }
                    result = " " + result;
                }
            }
            result = result.Trim();
            if (isNegative) result = "Âm " + result;
            number = (inputNumber - Math.Truncate(inputNumber)) * 10;
            if (number > 0)
            {
                result += " phẩy " + NumberToText(number, false);
            }
            return result;
        }
        static Regex ConvertToUnsign_rg = null;

        public static string ConvertToUnsign(string strInput)
        {
            if (ReferenceEquals(ConvertToUnsign_rg, null))
            {
                ConvertToUnsign_rg = new Regex("p{IsCombiningDiacriticalMarks}+");
            }
            var temp = strInput.Normalize(NormalizationForm.FormD);
            return ConvertToUnsign_rg.Replace(temp, string.Empty).ToLower();
        }
        public static void SpilitAddress(string hktt, ref string thon, ref string xa, ref string huyen, ref string tinh, ref bool isCity, ref bool isSo, ref string address)
        {
            bool isTT = false;
            if (!string.IsNullOrEmpty(hktt))
            {
                var dvhc = hktt.Split(',');
                foreach (var item in dvhc)
                {
                    var replace = item.Trim().Split(' ').ToList();
                    replace.RemoveAt(0);
                    if (item.ToUpper().Contains("THÔN") || item.ToUpper().Contains("TỔ") || item.Trim().ToUpper().StartsWith("SỐ"))
                    {
                        thon = String.Join(" ", replace);
                        if (item.ToUpper().Contains("TỔ") || item.Trim().ToUpper().StartsWith("SỐ"))
                        {
                            if (item.Trim().ToUpper().StartsWith("SỐ"))
                            {
                                isCity = true;
                                isSo = true;
                            }
                            else
                            {
                                thon = "Tổ " + thon;
                            }
                        }
                    }
                    else if (item.ToUpper().Contains("XÃ") || item.ToUpper().Contains("PHƯỜNG") || item.ToUpper().Contains("THỊ TRẤN"))
                    {
                        xa = String.Join(" ", replace);
                        if (item.Trim().ToUpper().StartsWith("THỊ TRẤN"))
                        {
                            xa = "Thị " + xa;
                            isTT = true;
                        }
                        if (item.ToUpper().Contains("PHƯỜNG"))
                            isCity = true;
                    }
                    else if (item.ToUpper().Contains("HUYỆN") || item.ToUpper().Contains("QUẬN"))
                    {
                        huyen = String.Join(" ", replace);
                        if (item.ToUpper().Contains("QUẬN"))
                            isCity = true;
                    }
                    else if (item.ToUpper().Contains("TỈNH") || item.ToUpper().Contains("THÀNH PHỐ") || item.ToUpper().Contains("TP"))
                    {
                        if (item.ToUpper().Contains("THÀNH PHỐ"))
                            replace.RemoveAt(0);
                        tinh = String.Join(" ", replace);
                    }
                }
                if (isCity)
                    address = (isSo ? "số " : "tổ ") + thon + ", phường " + xa;
                else
                    address = isTT ? thon + ", " + xa : "thôn " + thon + ", xã " + xa;
            }
        }
        public static string ToTTCN(string gender, string name, DateTime? birthday, string typeofid, string idno, string recAdd, DateTime? recDate, bool isAdd)
        {
            var rs = string.Empty;
            if (!string.IsNullOrEmpty(name))
            {
                rs += isAdd ? " và " : "";
                rs += (gender.Trim().ToUpper().Equals("NAM") ? "ông " : "bà ");
                rs += name.ToUpper();
                rs += "; năm sinh: " + (birthday.HasValue ? birthday.Value.ToString("yyyy") : "......") + "; ";
                rs += typeofid + ": " + idno +
                    (string.IsNullOrEmpty(recAdd) ? "" : "; nơi cấp: " + recAdd + "; ngày cấp: " + recDate.Value.ToString("dd/MM/yyyy"));
            }
            return rs;
        }

        public static string AddNamSinh(DateTime birthday)
        {
            var rs = string.Empty;
            rs += "Năm sinh: " + birthday.ToString("yyyy");
            return rs;
        }
        public static string AddCard(string typeofid, string idno, string recAdd, DateTime recDate)
        {
            var rs = string.Empty;
            rs += typeofid + ": " + idno + "; Nơi cấp: " + recAdd + "; Ngày cấp: " + recDate.ToString("dd/MM/yyyy");
            return rs;
        }
        public static string AddAddress(string address, string pre = "")
        {
            var rs = string.Empty;
            rs += pre + address;
            return rs;
        }
        public static string AddSpe(bool sperator)
        {
            var rs = string.Empty;
            if (sperator)
                rs += "; ";
            return rs;
        }

        public static string ToTTCN1(Infomation info, bool hoten, bool namsinh, bool card, bool address, bool aord, string preAddress = "Nơi thường trú: ")
        {
            var rs = string.Empty;
            bool first = false;
            if (aord)
            {
                if (hoten)
                {
                    rs += (info.a_gioitinh.ToUpper().Equals("NAM") ? "ông " : "bà ") + info.a_hoten.ToUpper();
                    first = true;
                }
                if (namsinh)
                {
                    rs += AddSpe(first) + AddNamSinh(info.a_ngaysinh.Value);
                    first = true;
                }
                if (card)
                {
                    rs += AddSpe(first) + AddCard(info.a_loaigiayto, info.a_sogiayto, info.a_noicap_gt, info.a_ngaycap_gt.Value);
                    first = true;
                }
                if (string.IsNullOrEmpty(info.a_hoten1))
                {
                    if (address)
                    {
                        rs += AddSpe(first) + AddAddress(info.a_hktt);
                        first = true;
                    }
                }
                else
                {
                    if (!info.a_hktt.Equals(info.a_hktt1))
                    {
                        rs += AddSpe(first) + AddAddress(info.a_hktt, preAddress);
                        first = true;
                    }
                    rs += " và " + (info.a_gioitinh1.ToUpper().Equals("NAM") ? "ông " : "bà ") + info.a_hoten1.ToUpper();
                    if (namsinh)
                    {
                        rs += AddSpe(first) + AddNamSinh(info.a_ngaysinh1.Value);
                    }
                    if (card)
                    {
                        rs += AddSpe(first) + AddCard(info.a_loaigiayto1, info.a_sogiayto1, info.a_noicap_gt1, info.a_ngaycap_gt1.Value);
                    }
                    if (address)
                    {
                        if (!info.a_hktt.Equals(info.a_hktt1))
                        {
                            rs += AddSpe(first) + AddAddress(info.a_hktt1, preAddress);
                        }
                        else
                        {
                            rs += "; Cùng đăng ký thường trú tại: " + info.a_hktt;
                        }
                    }
                }
            }
            else
            {
                if (hoten)
                {
                    rs = (info.d_gioitinh.ToUpper().Equals("NAM") ? "ông " : "bà ") + info.d_hoten.ToUpper();
                    first = true;
                }
                if (namsinh)
                {
                    rs += AddSpe(first) + AddNamSinh(info.d_ngaysinh.Value);
                    first = true;
                }
                if (card)
                {
                    rs += AddSpe(first) + AddCard(info.d_loaigiayto, info.d_sogiayto, info.d_noicap_gt, info.d_ngaycap_gt.Value);
                    first = true;
                }
                if (string.IsNullOrEmpty(info.d_hoten1))
                {
                    if (address)
                    {
                        rs += AddSpe(first) + AddAddress(info.d_hktt);
                    }
                }
                else
                {
                    if (!info.d_hktt.Equals(info.d_hktt1))
                    {
                        rs += AddSpe(first) + AddAddress(info.d_hktt, preAddress);
                    }
                    if (hoten)
                    {
                        rs += " và " + (info.d_gioitinh1.ToUpper().Equals("NAM") ? "ông " : "bà ") + info.d_hoten1.ToUpper();
                    }
                    if (namsinh)
                    {
                        rs += AddSpe(first) + AddNamSinh(info.d_ngaysinh1.Value);
                    }
                    if (card)
                    {
                        rs += AddSpe(first) + AddCard(info.d_loaigiayto1, info.d_sogiayto1, info.d_noicap_gt1, info.d_ngaycap_gt1.Value);
                    }
                    if (address)
                    {
                        if (!info.d_hktt.Equals(info.d_hktt1))
                        {
                            rs += AddSpe(first) + AddAddress(info.d_hktt1, preAddress);
                        }
                        else
                        {
                            rs += "; Cùng đăng ký thường trú tại: " + info.d_hktt;
                        }
                    }
                }
            }
            return rs;
        }
        public static string ToTTCN2(Infomation info, Dictionary<string, string> dict, ref string rs2)
        {
            string rs = string.Empty;
            rs = dict["a_gioitinh"] + " " + info.a_hoten.ToUpper() + ", sinh ngày " + dict["a_ngaysinh"] + ", " +
                         info.a_loaigiayto + " số " + info.a_sogiayto + " do " + info.a_noicap_gt + " cấp ngày " + dict["a_ngaycap_gt"] + ".";
            if (!string.IsNullOrEmpty(info.a_hoten1))
            {
                if (info.a_hktt.Equals(info.a_hktt1))
                {
                    rs += " Và " + dict["a_gioitinh1"] + " " + info.a_hoten1.ToUpper() + ", sinh ngày " + dict["a_ngaysinh1"] + ", " +
                            info.a_loaigiayto1 + " số " + info.a_sogiayto1 + " do " + info.a_noicap_gt1 + " cấp ngày " + dict["a_ngaycap_gt1"] + ".";
                    rs2 = "Cùng đăng ký thường trú tại: " + info.a_hktt + ".";
                }
                else
                {
                    rs2 = dict["a_gioitinh1"] + " " + info.a_hoten1.ToUpper() + ", sinh ngày " + dict["a_ngaysinh1"] + ", " +
                            info.a_loaigiayto1 + " số " + info.a_sogiayto1 + " do " + info.a_noicap_gt1 + " cấp ngày " + dict["a_ngaycap_gt1"] + ".";
                    rs2 += " Đăng ký thường trú tại: " + info.a_hktt1 + ".";
                    rs += " Đăng ký thường trú tại: " + info.a_hktt + ".";
                }
            }
            else
            {
                rs2 = "Đăng ký thường trú tại: " + info.a_hktt + ".";
            }
            return rs;
        }
        public static string ToTTCN3(Infomation info, Dictionary<string, string> dict, int sinhngay = 0, bool cccd = true, string noicap = " do ", string ngaycap = " cấp ngày ", string hktt = "Nơi thường trú: ")
        {
            string rs = string.Empty;
            rs = dict["a_gioitinh"] + " " + info.a_hoten.ToUpper()
                + (sinhngay == 0 ? "" : (sinhngay == 1 ? "; sinh năm " + info.a_ngaysinh.Value.ToString("yyyy") : "; sinh ngày " + info.a_ngaysinh.Value.ToString("dd/MM/yyyy")))
                + (!cccd ? "" : "; " + info.a_loaigiayto + " số " + info.a_sogiayto + (string.IsNullOrEmpty(noicap) ? "" : noicap + info.a_noicap_gt + ngaycap + dict["a_ngaycap_gt"]));
            if (!string.IsNullOrEmpty(info.a_hoten1))
            {
                if (info.a_hktt.Equals(info.a_hktt1))
                {
                    rs += " Và " + dict["a_gioitinh1"] + " " + info.a_hoten1.ToUpper()
                        + (sinhngay == 0 ? "" : (sinhngay == 1 ? "; sinh năm " + info.a_ngaysinh1.Value.ToString("yyyy") : "; sinh ngày " + info.a_ngaysinh1.Value.ToString("dd/MM/yyyy")))
                        + (!cccd ? "" : "; " + info.a_loaigiayto1 + " số " + info.a_sogiayto1 + (string.IsNullOrEmpty(noicap) ? "" : noicap + info.a_noicap_gt1 + ngaycap + dict["a_ngaycap_gt1"]))
                        + (string.IsNullOrEmpty(hktt) ? "" : "; Cùng " + hktt + info.a_hktt);
                }

                else
                {
                    rs += (string.IsNullOrEmpty(hktt) ? "" : "; " + hktt + info.a_hktt)
                        + " Và " + dict["a_gioitinh1"] + " " + info.a_hoten1.ToUpper()
                        + (sinhngay == 0 ? "" : (sinhngay == 1 ? "; sinh năm " + info.a_ngaysinh1.Value.ToString("yyyy") : "; sinh ngày " + info.a_ngaysinh1.Value.ToString("dd/MM/yyyy")))
                        + (!cccd ? "" : "; " + info.a_loaigiayto1 + " số " + info.a_sogiayto1 + (string.IsNullOrEmpty(noicap) ? "" : noicap + info.a_noicap_gt1 + ngaycap + dict["a_ngaycap_gt1"]))
                        + (string.IsNullOrEmpty(hktt) ? "" : "; " + hktt + info.a_hktt1);
                }
            }
            else
            {
                rs += (string.IsNullOrEmpty(hktt) ? "" : "; " + hktt + info.a_hktt);
            }
            return rs;
        }
        public static string ConvertToUnSign(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        public static string RemoveSpecialCharactor(string s)
        {
            string[] charsToRemove = new string[] { "@", ",", ".", ";", "'", "/", "\"", "+", ":", "\"", "*", "?", "<", ">", "|" };
            foreach (var c in charsToRemove)
            {
                s = s.Replace(c, string.Empty);
            }
            return s.Trim();
        }
        public static string GetSlug(string text)
        {
            text = ConvertToUnSign(text);
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string slug = text.Normalize(NormalizationForm.FormD).Trim().ToLower();
            slug = regex.Replace(slug, String.Empty)
              .Replace('\u0111', 'd').Replace('\u0110', 'D')
              .Replace(",", "-").Replace(".", "-").Replace("!", "")
              .Replace("(", "").Replace(")", "").Replace(";", "-")
              .Replace("/", "-").Replace("%", "ptram").Replace("&", "va")
              .Replace("?", "").Replace('"', '-').Replace(' ', '-');
            return slug;
        }
    }
    public static class HtmlHelper
    {
        public static string StripHtmlAndDecode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Bước 1: Loại bỏ thẻ HTML
            string noHtml = Regex.Replace(input, "<.*?>", string.Empty);

            // Bước 2: Giải mã các HTML entity
            string decoded = HttpUtility.HtmlDecode(noHtml);

            return decoded;
        }
    }
}