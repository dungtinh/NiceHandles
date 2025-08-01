using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR.Hosting;
using Newtonsoft.Json;
using NiceHandles.Models;
using Markdig;

namespace NiceHandles.Controllers
{
    public class ChatController : Controller
    {
        private readonly string apiKey = ConfigurationManager.AppSettings["OpenAI_API_Key"];
        private readonly string assistantId = ConfigurationManager.AppSettings["OpenAI_Assistant_Id"];
        private readonly NHModel db = new NHModel();

        public async Task<ActionResult> Index(Guid? sessionId)
        {
            var sessions = db.ChatSessions.OrderByDescending(s => s.CreatedAt).ToList();

            ChatSession currentSession;
            if (sessionId.HasValue)
            {
                currentSession = db.ChatSessions.Find(sessionId.Value);

                ViewBag.CurrentSessionId = currentSession.Id;
                ViewBag.Messages = db.ChatMessages
                .Where(m => m.ChatSessionId == sessionId)
                .OrderBy(m => m.CreatedAt)
                .ToList();

                ViewBag.ChatFiles = db.ChatFiles
                    .Where(f => f.ChatSessionId == sessionId)
                    .OrderByDescending(f => f.UploadedAt)
                    .ToList();
                ViewBag.CurrentSession = currentSession;
            }
            return View(sessions);
        }

        [HttpPost]
        public async Task<ActionResult> Ask(Guid sessionId, string userMessage)
        {
            var session = db.ChatSessions.Find(sessionId);
            if (session == null)
            {
                var thread = await CreateThread();
                session = new ChatSession
                {
                    Id = sessionId,
                    Name = userMessage.Length > 250 ? (userMessage.Substring(0, 250) + "...") : userMessage,
                    CreatedAt = DateTime.Now,
                    AssistantThreadId = thread["id"].ToString()
                };
                db.ChatSessions.Add(session);
                db.SaveChanges();
            }
            var threadId = session.AssistantThreadId;
            var jsonMsg = JsonConvert.SerializeObject(new
            {
                role = "user",
                content = userMessage
            });

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

                // Gửi tin nhắn vào thread
                var msgRes = await httpClient.PostAsync(
                    $"https://api.openai.com/v1/threads/{threadId}/messages",
                    new StringContent(jsonMsg, Encoding.UTF8, "application/json"));

                db.ChatMessages.Add(new ChatMessage
                {
                    ChatSessionId = session.Id,
                    Role = "user",
                    Content = userMessage,
                    CreatedAt = DateTime.Now
                });
                db.SaveChanges();

                msgRes.EnsureSuccessStatusCode();
                //var result = await msgRes.Content.ReadAsStringAsync();
                //if (!msgRes.IsSuccessStatusCode)
                //{
                //    // Ghi log nếu cần
                //    System.Diagnostics.Debug.WriteLine("❌ Upload thất bại: " + result);
                //    return null;
                //}

                // Tạo run
                object runData;
                var fileIds = db.ChatFiles
                .Where(f => f.ChatSessionId == sessionId && f.FileId != null)
                .Select(f => f.FileId)
                .ToList();

                // Nếu không có file, chỉ gửi assistant_id
                if (fileIds == null || !fileIds.Any())
                {
                    runData = new
                    {
                        assistant_id = assistantId
                    };
                }
                else
                {
                    runData = new
                    {
                        assistant_id = assistantId,
                        tool_resources = new
                        {
                            code_interpreter = new
                            {
                                file_ids = fileIds
                            }
                        }
                    };
                }

                var jsonRun = JsonConvert.SerializeObject(runData);

                var runRes = await httpClient.PostAsync(
                    $"https://api.openai.com/v1/threads/{threadId}/runs",
                    new StringContent(jsonRun, Encoding.UTF8, "application/json"));

                runRes.EnsureSuccessStatusCode();

                var runObj = JsonConvert.DeserializeObject<dynamic>(await runRes.Content.ReadAsStringAsync());
                string runId = runObj.id;

                // Chờ assistant phản hồi
                string status = "queued";
                while (status == "queued" || status == "in_progress")
                {
                    await Task.Delay(1000);
                    var statusRes = await httpClient.GetAsync($"https://api.openai.com/v1/threads/{threadId}/runs/{runId}");
                    var statusObj = JsonConvert.DeserializeObject<dynamic>(await statusRes.Content.ReadAsStringAsync());
                    status = statusObj.status;
                }
                // Lấy message mới nhất
                var msgListRes = await httpClient.GetAsync($"https://api.openai.com/v1/threads/{threadId}/messages");

                var chkRes = await msgListRes.Content.ReadAsStringAsync();
                var msgList = JsonConvert.DeserializeObject<dynamic>(chkRes);
                string reply = msgList.data[0].content[0].text.value;
                reply = Markdown.ToHtml(reply);
                return Json(new { reply });
            }
        }

        [HttpPost]
        public async Task<ActionResult> UploadFile(HttpPostedFileBase file, Guid sessionId)
        {
            if (file?.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                file.SaveAs(filePath);

                var fileId = await UploadFileToOpenAI(filePath);
                var session = db.ChatSessions.Find(sessionId);
                if (session != null)
                {
                    db.ChatFiles.Add(new ChatFile
                    {
                        ChatSessionId = sessionId,
                        FileName = fileName,
                        FileId = fileId,
                        UploadedAt = DateTime.Now
                    });
                    db.SaveChanges();
                    //await Ask(sessionId, "Mô tả file vừa upload: " + fileId);
                }
            }
            return RedirectToAction("Index", new { sessionId });
        }

        private async Task<dynamic> CreateThread()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

                var response = await httpClient.PostAsync("https://api.openai.com/v1/threads",
                    new StringContent("{}", Encoding.UTF8, "application/json"));

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<dynamic>(content);
            }
        }

        private async Task<string> UploadFileToOpenAI(string filePath)
        {
            using (var httpClient = new HttpClient())
            using (var form = new MultipartFormDataContent())
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

                using (var stream = System.IO.File.OpenRead(filePath))
                {
                    var content = new StreamContent(stream);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    form.Add(content, "file", Path.GetFileName(filePath));
                    form.Add(new StringContent("assistants"), "purpose");

                    var response = await httpClient.PostAsync("https://api.openai.com/v1/files", form);
                    var result = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        // Ghi log nếu cần
                        System.Diagnostics.Debug.WriteLine("❌ Upload thất bại: " + result);
                        return null;
                    }

                    var json = JsonConvert.DeserializeObject<dynamic>(result);
                    return json?.id;
                }
            }
        }
        [HttpPost]
        public ActionResult ChangeName(Guid id, string name)
        {
            var session = db.ChatSessions.Find(id);
            if (session != null && !string.IsNullOrWhiteSpace(name))
            {
                session.Name = name.Trim();
                db.SaveChanges();
            }
            return RedirectToAction("Index", new { sessionId = id });
        }
        [HttpPost]
        public ActionResult DeleteSession(Guid id)
        {
            var session = db.ChatSessions.Find(id);
            if (session != null)
            {
                // Xóa tin nhắn
                var messages = db.ChatMessages.Where(m => m.ChatSessionId == id).ToList();
                db.ChatMessages.RemoveRange(messages);

                // Xóa file
                var files = db.ChatFiles.Where(f => f.ChatSessionId == id).ToList();
                db.ChatFiles.RemoveRange(files);

                // Xóa session
                db.ChatSessions.Remove(session);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
