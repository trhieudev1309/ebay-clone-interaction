namespace EbayChat.Entities;

public partial class Message
{
    public int id { get; set; }

    public int? senderId { get; set; }

    public int? receiverId { get; set; }

    public string? content { get; set; }

    public DateTime? timestamp { get; set; }

    public bool? seen { get; set; } = false;

    public virtual User? receiver { get; set; }

    public virtual User? sender { get; set; }
}
