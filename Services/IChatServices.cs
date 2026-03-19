using EbayChat.Entities;
using EbayChat.Models.DTOs;

namespace EbayChat.Services
{
    public interface IChatServices
    {
        Task<IEnumerable<BoxChatDTO>> GetBoxChats(int userId);
        Task<IEnumerable<Message>> GetAllMessagesBySenderAndReceiver(int senderId, int receiverId);
        Task MarkAsSeen(int senderId, int receiverId);

        Task SendMessage(int senderId, int receiverId, String content, bool seen);
    }
}
