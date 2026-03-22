using EbayChat.Entities;
using EbayChat.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EbayChat.Services.ServicesImpl
{
    public class ChatServices : IChatServices
    {
        private readonly CloneEbayDbContext _context;
        private readonly IRedisCacheService _redisCacheService;
        private readonly ILogger<ChatServices> _logger;

        public ChatServices(
            CloneEbayDbContext context,
            IRedisCacheService redisCacheService,
            ILogger<ChatServices> logger)
        {
            _context = context;
            _redisCacheService = redisCacheService;
            _logger = logger;
        }

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
            var conversationKey = ChatCacheKeyHelper.BuildConversationKey(senderId, receiverId);

            var cachedMessages = await _redisCacheService.GetMessagesAsync(conversationKey);
            if (cachedMessages.Count > 0)
            {
                _logger.LogInformation(
                    "Returning chat messages from cache. senderId={SenderId}, receiverId={ReceiverId}",
                    senderId,
                    receiverId);

                return cachedMessages;
            }

            _logger.LogInformation(
                "Fetching chat messages from DB. senderId={SenderId}, receiverId={ReceiverId}",
                senderId,
                receiverId);

            var messages = await _context.Messages
                .AsNoTracking()
                .Where(m =>
                    (m.senderId == senderId && m.receiverId == receiverId) ||
                    (m.senderId == receiverId && m.receiverId == senderId))
                .OrderBy(m => m.timestamp)
                .ToListAsync();

            await _redisCacheService.SetMessagesAsync(conversationKey, messages, TimeSpan.FromMinutes(10));

            return messages;
        }

        public async Task MarkAsSeen(int senderId, int receiverId)
        {
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

                var conversationKey = ChatCacheKeyHelper.BuildConversationKey(senderId, receiverId);
                await _redisCacheService.InvalidateMessagesAsync(conversationKey);
            }
        }

        public async Task SendMessage(int senderId, int receiverId, string content, bool seen)
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

            var conversationKey = ChatCacheKeyHelper.BuildConversationKey(senderId, receiverId);
            await _redisCacheService.InvalidateMessagesAsync(conversationKey);
        }
    }
}
