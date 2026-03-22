namespace EbayChat.Services
{
    public static class ChatCacheKeyHelper
    {
        // Format: chat:{smallerUserId}:{largerUserId}
        public static string BuildConversationKey(int userId1, int userId2)
        {
            var min = Math.Min(userId1, userId2);
            var max = Math.Max(userId1, userId2);
            return $"chat:{min}:{max}";
        }
    }
}