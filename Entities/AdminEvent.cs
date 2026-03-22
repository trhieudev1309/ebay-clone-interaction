namespace EbayChat.Entities;

public partial class AdminEvent
{
    public int id { get; set; }

    public string eventType { get; set; } = string.Empty;

    public int? referenceId { get; set; }

    public int? userId { get; set; }

    public string message { get; set; } = string.Empty;

    public string status { get; set; } = "Pending";

    public DateTime? createdAt { get; set; }

    public virtual User? user { get; set; }
}