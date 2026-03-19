using EbayChat.Services;
using Microsoft.AspNetCore.SignalR;

namespace EbayChat.Hubs
{
    public class EbayChatHub : Hub
    {

        private readonly IChatServices _chatServices;

        public EbayChatHub(IChatServices chatServices) => _chatServices = chatServices;

        // Tạo group duy nhất cho 1-1 conversation (đảm bảo user nhỏ trước)
        private static string ConversationGroup(int user1, int user2)
        {
            var min = Math.Min(user1, user2);
            var max = Math.Max(user1, user2);
            return $"chat-{min}-{max}";
        }

        // Khi mở box chat => client gọi hàm này để join vào group hội thoại
        public async Task JoinConversation(int userId, int otherId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, ConversationGroup(userId, otherId));
        }




    }
}
