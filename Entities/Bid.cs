namespace EbayChat.Entities;

public partial class Bid
{
    public int id { get; set; }

    public int? productId { get; set; }

    public int? bidderId { get; set; }

    public decimal? amount { get; set; }

    public DateTime? bidTime { get; set; }

    public virtual User? bidder { get; set; }

    public virtual Product? product { get; set; }
}
