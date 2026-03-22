namespace EbayChat.Events
{
    public sealed record SellerReportedEvent(
        int UserId,
        int RelatedEntityId,
        IReadOnlyDictionary<string, string>? Payload = null)
        : EventBase(UserId, RelatedEntityId, Payload);
}