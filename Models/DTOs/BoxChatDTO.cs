namespace EbayChat.Models.DTOs
{
    public class BoxChatDTO
    {
        public int ReceiverId { get; set; }
        public int SenderId { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? LastMessage { get; set; }
        public string? Time { get; set; }
        public bool? Seen { get; set; }
    }
}
