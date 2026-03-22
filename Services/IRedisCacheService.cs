using EbayChat.Entities;

namespace EbayChat.Services
{
    public interface IRedisCacheService
    {
        Task<List<Message>> GetMessagesAsync(string conversationKey);
        Task SetMessagesAsync(string conversationKey, List<Message> messages, TimeSpan? expiry = null);
        Task InvalidateMessagesAsync(string conversationKey);
    }
}