// 📡 SignalR Hub: ChatHub.cs
using Microsoft.AspNet.SignalR;
using NiceHandles.Models;
using System;
using System.Linq;

namespace NiceHandles.Hubs
{
    public class ChatHub : Hub
    {
        public void BroadcastMessage(string user, string message, Guid sessionId)
        {
            // Gửi message real-time tới tất cả clients
            Clients.All.ReceiveMessage(user, message);

            // Lưu vào cơ sở dữ liệu
            SaveMessageToDatabase(user, message, sessionId);
        }

        private void SaveMessageToDatabase(string user, string message, Guid sessionId)
        {
            try
            {
                using (var db = new NHModel())
                {
                    var session = db.ChatSessions.FirstOrDefault(s => s.Id == sessionId);
                    if (session == null) return;

                    db.ChatMessages.Add(new ChatMessage
                    {
                        ChatSessionId = session.Id,
                        Role = user.ToLower() == "ai" ? "assistant" : "user",
                        Content = message,
                        CreatedAt = DateTime.Now
                    });

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Có thể log lỗi nếu cần
            }
        }
    }
}