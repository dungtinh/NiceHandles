using Newtonsoft.Json;
using NiceHandles.Models;
using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Drawing.Drawing2D;

namespace NiceHandles.Controllers
{
    public class CCCDController : Controller
    {
        private NHModel db = new NHModel();
        // GET: CCCD
        public ActionResult Index()
        {
            return View();
        }
        private const string ApiEndpoint = "https://api.fpt.ai/vision/idr/vnm";

        [HttpPost]
        public async Task<ActionResult> ReadCCCD(HttpPostedFileBase inputFile)
        {
            string apiKey = db.Settings.Find(3).data;
            if (inputFile == null || inputFile.ContentLength == 0)
            {
                return Json(new { success = false, message = "Please select a file." });
            }

            try
            {
                if (inputFile.ContentType.StartsWith("image/"))
                {
                    // Xử lý ảnh
                    Image originalImage = Image.FromStream(inputFile.InputStream);
                    Image[] splitImages = SplitImageIfTwoSides(originalImage);
                    originalImage.Dispose();

                    return await ProcessImages(splitImages, inputFile.FileName, apiKey);
                }
                else if (inputFile.ContentType == "application/pdf")
                {
                    // Xử lý PDF
                    Image[] pdfImages = ConvertPdfToImages(inputFile.InputStream);

                    if (pdfImages.Length == 0)
                    {
                        return Json(new { success = false, message = "The PDF file appears to be empty or invalid." });
                    }
                    if (pdfImages.Length > 1)
                    {
                        return Json(new { success = false, message = "The PDF file must contain one page." });
                    }

                    Image[] processedImages = SplitImageIfTwoSides(pdfImages[0]);
                    pdfImages[0].Dispose();
                    return await ProcessImages(processedImages, inputFile.FileName, apiKey);
                }
                else
                {
                    return Json(new { success = false, message = "Invalid file type." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }
        private async Task<ActionResult> ProcessImages(Image[] images, string originalFileName, string apiKey)
        {
            CardData combinedData = new CardData(); // KHÔNG khởi tạo giá trị mặc định
            bool hasFront = false;
            bool hasBack = false;
            bool hasError = false;

            foreach (var img in images)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, ImageFormat.Jpeg);
                        ms.Position = 0;
                        string fileName = Path.GetFileNameWithoutExtension(originalFileName) + "_" + Guid.NewGuid().ToString() + ".jpg";
                        ApiResponse currentResponse = await ReadCCCDFromStreamAsync(ms, fileName, apiKey);

                        if (currentResponse.errorCode == 0 && currentResponse.data != null && currentResponse.data.Count > 0)
                        {
                            CardData currentData = currentResponse.data[0];

                            if (currentData.type == "new" || currentData.type == "old" || currentData.type.Contains("front"))
                            {
                                MergeFrontData(combinedData, currentData);
                                hasFront = true;
                            }
                            else if (currentData.type == "new_back" || currentData.type == "old_back" || currentData.type.Contains("back"))
                            {
                                MergeBackData(combinedData, currentData);
                                hasBack = true;
                            }
                        }
                        else
                        {
                            hasError = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    hasError = true;
                }
                finally
                {
                    img.Dispose();
                }
            }

            ApiResponse finalResponse = new ApiResponse
            {
                errorCode = hasError ? 1 : 0,  // Nếu có lỗi nào thì errorCode != 0
                errorMessage = hasError ? "Error processing one or more images." : "",
                data = new List<CardData> { combinedData } // Trả về MỘT CardData
            };
            if (!string.IsNullOrEmpty(finalResponse.errorMessage))
            {
                finalResponse.errorCode = 1;
            }
            if (!hasFront)
            {
                finalResponse.errorMessage += " Front side of ID card is missing.";
            }
            if (!hasBack)
            {
                finalResponse.errorMessage += " Back side of ID card is missing.";
            }
            return Json(new { success = !hasError, data = finalResponse });
        }

        // Hàm merge dữ liệu mặt TRƯỚC (CHỈ kiểm tra null hoặc rỗng)
        private void MergeFrontData(CardData combined, CardData front)
        {
            if (!string.IsNullOrEmpty(front.id)) combined.id = front.id;
            if (!string.IsNullOrEmpty(front.id_prob)) combined.id_prob = front.id_prob;
            if (!string.IsNullOrEmpty(front.name)) combined.name = front.name;
            if (!string.IsNullOrEmpty(front.name_prob)) combined.name_prob = front.name_prob;
            if (!string.IsNullOrEmpty(front.dob)) combined.dob = front.dob;
            if (!string.IsNullOrEmpty(front.dob_prob)) combined.dob_prob = front.dob_prob;
            if (!string.IsNullOrEmpty(front.sex)) combined.sex = front.sex;
            if (!string.IsNullOrEmpty(front.sex_prob)) combined.sex_prob = front.sex_prob;
            if (!string.IsNullOrEmpty(front.nationality)) combined.nationality = front.nationality;
            if (!string.IsNullOrEmpty(front.nationality_prob)) combined.nationality_prob = front.nationality_prob;
            if (!string.IsNullOrEmpty(front.home)) combined.home = front.home;
            if (!string.IsNullOrEmpty(front.home_prob)) combined.home_prob = front.home_prob;
            if (!string.IsNullOrEmpty(front.address)) combined.address = front.address;
            if (!string.IsNullOrEmpty(front.address_prob)) combined.address_prob = front.address_prob;

            if (front.address_entities != null)
            {
                if (combined.address_entities == null) combined.address_entities = new AddressEntities(); // Khởi tạo nếu chưa có
                if (!string.IsNullOrEmpty(front.address_entities.province)) combined.address_entities.province = front.address_entities.province;
                if (!string.IsNullOrEmpty(front.address_entities.district)) combined.address_entities.district = front.address_entities.district;
                if (!string.IsNullOrEmpty(front.address_entities.ward)) combined.address_entities.ward = front.address_entities.ward;
                if (!string.IsNullOrEmpty(front.address_entities.street)) combined.address_entities.street = front.address_entities.street;
            }

            if (!string.IsNullOrEmpty(front.doe)) combined.doe = front.doe;
            if (!string.IsNullOrEmpty(front.doe_prob)) combined.doe_prob = front.doe_prob;
            if (!string.IsNullOrEmpty(front.type)) combined.type = front.type;
            if (!string.IsNullOrEmpty(front.type_new)) combined.type_new = front.type_new;
        }

        // Hàm merge dữ liệu mặt SAU (CHỈ kiểm tra null hoặc rỗng)
        private void MergeBackData(CardData combined, CardData back)
        {
            if (!string.IsNullOrEmpty(back.ethnicity)) combined.ethnicity = back.ethnicity;
            if (!string.IsNullOrEmpty(back.ethnicity_prob)) combined.ethnicity_prob = back.ethnicity_prob;
            if (!string.IsNullOrEmpty(back.religion)) combined.religion = back.religion;
            if (!string.IsNullOrEmpty(back.religion_prob)) combined.religion_prob = back.religion_prob;
            if (!string.IsNullOrEmpty(back.features)) combined.features = back.features;
            if (!string.IsNullOrEmpty(back.features_prob)) combined.features_prob = back.features_prob;
            if (!string.IsNullOrEmpty(back.issue_date)) combined.issue_date = back.issue_date;
            if (!string.IsNullOrEmpty(back.issue_date_prob)) combined.issue_date_prob = back.issue_date_prob;
            if (!string.IsNullOrEmpty(back.issue_loc)) combined.issue_loc = back.issue_loc;
            if (!string.IsNullOrEmpty(back.issue_loc_prob)) combined.issue_loc_prob = back.issue_loc_prob;
            if (!string.IsNullOrEmpty(back.type)) combined.type = back.type;
            if (!string.IsNullOrEmpty(back.type_new)) combined.type_new = back.type_new;
        }

        private async Task<ApiResponse> ReadCCCDFromStreamAsync(Stream imageStream, string fileName, string apiKey)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-key", apiKey);

                using (var content = new MultipartFormDataContent())
                {
                    var imageContent = new StreamContent(imageStream);
                    imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                    content.Add(imageContent, "image", fileName);

                    HttpResponseMessage response = await client.PostAsync("https://api.fpt.ai/vision/idr/vnm/", content);

                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    ApiResponse apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
                    apiResponse.filename = fileName;
                    return apiResponse;

                }
            }
        }

        // Các hàm hỗ trợ (giả định - bạn cần triển khai thực tế)
        private Image[] SplitImageIfTwoSides(Image originalImage)
        {
            if (originalImage.Height > originalImage.Width)
            {
                int halfHeight = originalImage.Height / 2;
                Rectangle rectTop = new Rectangle(0, 0, originalImage.Width, halfHeight);
                Rectangle rectBottom = new Rectangle(0, halfHeight, originalImage.Width, originalImage.Height - halfHeight);

                Bitmap bmpTop = new Bitmap(rectTop.Width, rectTop.Height); // Kích thước đúng
                using (Graphics gTop = Graphics.FromImage(bmpTop))
                {
                    // Vẽ VÀO một Rectangle trên bmpTop, có kích thước bằng bmpTop
                    gTop.DrawImage(originalImage, new Rectangle(0, 0, bmpTop.Width, bmpTop.Height), rectTop, GraphicsUnit.Pixel);
                }

                Bitmap bmpBottom = new Bitmap(rectBottom.Width, rectBottom.Height); // Kích thước đúng
                using (Graphics gBottom = Graphics.FromImage(bmpBottom))
                {
                    // Vẽ VÀO một Rectangle trên bmpBottom, có kích thước bằng bmpBottom
                    gBottom.DrawImage(originalImage, new Rectangle(0, 0, bmpBottom.Width, bmpBottom.Height), rectBottom, GraphicsUnit.Pixel);
                }
                return new Image[] { bmpTop, bmpBottom };
            }
            else
            {
                Bitmap clone = new Bitmap(originalImage.Width, originalImage.Height, originalImage.PixelFormat);
                using (Graphics g = Graphics.FromImage(clone))
                {
                    g.DrawImage(originalImage, new Rectangle(0, 0, clone.Width, clone.Height));
                }
                return new Image[] { clone };
            }
        }

        private Image[] ConvertPdfToImages(Stream pdfStream)
        {
            // Cần sử dụng thư viện để convert PDF sang ảnh (ví dụ: iTextSharp, PdfiumViewer, Ghostscript,...)
            // Đây chỉ là HÀM GIẢ ĐỊNH, bạn phải triển khai thực tế
            // Ví dụ sử dụng iText 7 (bạn cần cài đặt package iText7)            
            try
            {
                using (var document = PdfDocument.Load(pdfStream))
                {
                    var images = new Image[document.PageCount];
                    //Only render the first page
                    if (document.PageCount > 0)
                    {
                        images[0] = document.Render(0, 300, 300, PdfRenderFlags.CorrectFromDpi);
                    }
                    return images;
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                Console.WriteLine(ex.Message);
            }
            return new Image[0]; // Cần return mảng Image[]
        }

        // Các class ApiResponse, CardData, AddressEntities giữ nguyên như trước
        //public async Task<ActionResult> ReadCCCD(HttpPostedFileBase inputFile) // Keep HttpPostedFileBase
        //{
        //    if (inputFile == null || inputFile.ContentLength == 0)
        //    {
        //        // Return JSON error for AJAX handling
        //        return Json(new { success = false, message = "Please select a file." });
        //    }

        //    try
        //    {
        //        List<ApiResponse> apiResponses = new List<ApiResponse>();

        //        if (inputFile.ContentType.StartsWith("image/"))
        //        {
        //            // Handle single image
        //            Image originalImage = Image.FromStream(inputFile.InputStream);
        //            Image[] splitImages = SplitImageIfTwoSides(originalImage);
        //            originalImage.Dispose();

        //            foreach (var img in splitImages)
        //            {
        //                using (MemoryStream ms = new MemoryStream())
        //                {
        //                    img.Save(ms, ImageFormat.Jpeg);
        //                    ms.Position = 0;
        //                    string fileName = Path.GetFileNameWithoutExtension(inputFile.FileName) + "_" + Guid.NewGuid().ToString() + ".jpg";
        //                    apiResponses.Add(await ReadCCCDFromStreamAsync(ms, fileName));
        //                }
        //                img.Dispose();
        //            }
        //        }
        //        else if (inputFile.ContentType == "application/pdf")
        //        {
        //            // Convert PDF to image(s), then process each
        //            Image[] pdfImages = ConvertPdfToImages(inputFile.InputStream);

        //            if (pdfImages.Length == 0)
        //            {
        //                return Json(new { success = false, message = "The PDF file appears to be empty or invalid." });
        //            }

        //            if (pdfImages.Length > 1)
        //            {
        //                return Json(new { success = false, message = "The PDF file appears to contain more than one page" });
        //            }

        //            // Process each PDF page (only one in this case)
        //            foreach (Image img in pdfImages)
        //            {
        //                Image[] processedImages = SplitImageIfTwoSides(img);
        //                img.Dispose();
        //                foreach (var splitImg in processedImages)
        //                {
        //                    using (MemoryStream ms = new MemoryStream())
        //                    {
        //                        splitImg.Save(ms, ImageFormat.Jpeg);
        //                        ms.Position = 0;
        //                        string fileName = Path.GetFileNameWithoutExtension(inputFile.FileName) + "_" + Guid.NewGuid().ToString() + ".jpg";
        //                        apiResponses.Add(await ReadCCCDFromStreamAsync(ms, fileName));
        //                    }
        //                    splitImg.Dispose();
        //                }
        //            }

        //            foreach (Image img in pdfImages)
        //            {
        //                img.Dispose();
        //            }
        //        }
        //        else
        //        {
        //            // Return JSON error for AJAX handling
        //            return Json(new { success = false, message = "Invalid file type." });
        //        }

        //        // Return JSON success with the data
        //        return Json(new { success = true, data = apiResponses });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Return JSON error for AJAX handling
        //        return Json(new { success = false, message = "An error occurred: " + ex.Message });
        //    }
        //}
        //private async Task<ApiResponse> ReadCCCDFromStreamAsync(Stream imageStream, string fileName)
        //{
        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            client.DefaultRequestHeaders.Add("api_key", ApiKey);

        //            using (var content = new MultipartFormDataContent())
        //            {
        //                var streamContent = new StreamContent(imageStream);
        //                streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
        //                content.Add(streamContent, "image", fileName);

        //                var response = await client.PostAsync(ApiEndpoint, content);

        //                if (response.IsSuccessStatusCode)
        //                {
        //                    var responseBody = await response.Content.ReadAsStringAsync();
        //                    return JsonConvert.DeserializeObject<ApiResponse>(responseBody);
        //                }
        //                else
        //                {
        //                    throw new Exception($"HTTP Error: {response.StatusCode} - {response.ReasonPhrase}");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //private Image[] ConvertPdfToImages(Stream pdfStream)
        //{
        //    using (var document = PdfDocument.Load(pdfStream))
        //    {
        //        var images = new Image[document.PageCount];
        //        //Only render the first page
        //        if (document.PageCount > 0)
        //        {
        //            images[0] = document.Render(0, 300, 300, PdfRenderFlags.CorrectFromDpi);
        //        }
        //        return images;
        //    }
        //}
        //// Intelligently splits the image if it contains two sides, otherwise returns the original.
        //private Image[] SplitImageIfTwoSides(Image originalImage)
        //{
        //    // Simple heuristic:  If the image is significantly taller than wide,
        //    // assume it contains both front and back and split it vertically.
        //    // Adjust the ratio (1.2) as needed.
        //    if (originalImage.Height > originalImage.Width * 1.2)
        //    {
        //        int width = originalImage.Width;
        //        int height = originalImage.Height / 2;

        //        Bitmap topImage = new Bitmap(width, height);
        //        Bitmap bottomImage = new Bitmap(width, height);

        //        using (Graphics gTop = Graphics.FromImage(topImage))
        //        {
        //            gTop.DrawImage(originalImage, new Rectangle(0, 0, width, height), new Rectangle(0, 0, width, height), GraphicsUnit.Pixel);
        //        }

        //        using (Graphics gBottom = Graphics.FromImage(bottomImage))
        //        {
        //            gBottom.DrawImage(originalImage, new Rectangle(0, 0, width, height), new Rectangle(0, height, width, height), GraphicsUnit.Pixel);
        //        }

        //        return new Image[] { topImage, bottomImage };
        //    }
        //    else
        //    {
        //        // If it doesn't appear to have two sides, return the original image in an array.
        //        return new Image[] { (Image)originalImage.Clone() };
        //    }
        //}
    }
}