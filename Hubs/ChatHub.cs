using EbayChat.Entities;
using Microsoft.AspNetCore.SignalR;

namespace EbayChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly CloneEbayDbContext _context;
        public ChatHub(CloneEbayDbContext context) => _context = context;

        private static string UserGroup(int userId)
        {
            return $"user-{userId}";
        }

        // Connect user to their own personal SignalR group to receive all incoming messages
        public async Task ConnectUser(int userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));
        }

        // Gửi tin nhắn 1-1
        public async Task SendMessage(int senderId, int receiverId, string message)
        {
            // Lưu DB
            var newMessage = new Message
            {
                senderId = senderId,
                receiverId = receiverId,
                content = message,
                timestamp = DateTime.UtcNow.AddHours(7),
            };

            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();

            var sender = await _context.Users.FindAsync(senderId);
            var traceId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

            var payload = new
            {
                id = newMessage.id,
                traceId = traceId,
                senderId = senderId,
                senderName = sender?.username ?? $"User {senderId}",
                receiverId = receiverId,
                content = message,
                sentAt = newMessage.timestamp.Value.ToString("HH:mm dd/MM/yyyy")
            };

            // Gửi vào group cá nhân của cả người gửi và người nhận
            await Clients.Group(UserGroup(receiverId)).SendAsync("ReceiveMessage", payload);
            await Clients.Group(UserGroup(senderId)).SendAsync("ReceiveMessage", payload);

            // Tự động phản hồi Admin giả lập
            if (message.ToLower().Contains("help") || message.ToLower().Contains("support"))
            {
                var adminPayload = new
                {
                    id = 0,
                    traceId = "AUTO-" + traceId,
                    senderId = 0,
                    senderName = "eBay Support",
                    receiverId = senderId,
                    content = "System: Detect 'Help' request. Our agents will contact you shortly or you can raise a Dispute in My Orders.",
                    sentAt = DateTime.UtcNow.AddHours(7).ToString("HH:mm dd/MM/yyyy")
                };
                await Clients.Group(UserGroup(senderId)).SendAsync("ReceiveMessage", adminPayload);
            }
        }

        // Đánh dấu đã đọc
        public async Task MarkAsRead(int messageId)
        {
            var msg = await _context.Messages.FindAsync(messageId);
            if (msg is null) return;

            // ví dụ: set isRead = true
            // msg.IsRead = true;

            await _context.SaveChangesAsync();
        }

        // Sự kiện "đang gõ"
        public async Task UserTyping(int senderId, int receiverId)
        {
            await Clients.Group(UserGroup(receiverId)).SendAsync("UserIsTyping", senderId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
