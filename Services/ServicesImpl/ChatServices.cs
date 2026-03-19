using EbayChat.Entities;
using EbayChat.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EbayChat.Services.ServicesImpl
{
    public class ChatServices : IChatServices
    {
        private readonly CloneEbayDbContext _context;
        public ChatServices(CloneEbayDbContext context)
        {
            _context = context;
        }
        // Lấy danh sách hộp chat của người dùng, senderId là Id lấy từ session
        public Task<IEnumerable<BoxChatDTO>> GetBoxChats(int userId)
        {
            var boxChats = _context.Messages
                .Where(m => m.senderId == userId || m.receiverId == userId)
                .Include(m => m.sender)
                .Include(m => m.receiver)
                .AsEnumerable()
                .GroupBy(m => m.senderId == userId ? m.receiverId : m.senderId)
                .Select(g => g
                    .OrderByDescending(m => m.timestamp)
                    .FirstOrDefault()
                )
                .OrderByDescending(m => m.timestamp)
                .Select(m => new BoxChatDTO
                {
                    SenderId = m.sender.id,
                    ReceiverId = m.receiver.id,
                    Name = m.senderId == userId ? m.receiver.username : m.sender.username,
                    Avatar = m.senderId == userId ? m.receiver.avatarURL : m.sender.avatarURL,
                    LastMessage = m.content,
                    Time = Utils.DateHelper.FormatChatTime(m.timestamp.ToString()),
                    Seen = m.seen
                })
                .AsEnumerable();

            return Task.FromResult(boxChats);
        }

        public async Task<IEnumerable<Message>> GetAllMessagesBySenderAndReceiver(int senderId, int receiverId)
        {
            return await _context.Messages
                .Where(m =>
                    (m.senderId == senderId && m.receiverId == receiverId) ||
                    (m.senderId == receiverId && m.receiverId == senderId))
                .OrderBy(m => m.timestamp)
                .ToListAsync();
        }

        public async Task MarkAsSeen(int senderId, int receiverId)
        {
            // Get all messages sent by 'sender' to 'receiver' that are not seen yet
            var unseenMessages = await _context.Messages
                .Where(m => m.senderId == senderId && m.receiverId == receiverId && m.seen == false)
                .ToListAsync();

            if (unseenMessages.Any())
            {
                foreach (var msg in unseenMessages)
                {
                    msg.seen = true;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task SendMessage(int senderId, int receiverId, String content, bool seen)
        {
            var newMessage = new Message
            {
                senderId = senderId,
                receiverId = receiverId,
                content = content,
                timestamp = DateTime.UtcNow.AddHours(7),
                seen = seen
            };
            _context.Messages.Add(newMessage);
            await _context.SaveChangesAsync();
        }

    }
}
