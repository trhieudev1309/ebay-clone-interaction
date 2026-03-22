namespace EbayChat.Events
{
    public sealed record ReturnRequestedEvent(
        int UserId,
        int RelatedEntityId,
        IReadOnlyDictionary<string, string>? Payload = null)
        : EventBase(UserId, RelatedEntityId, Payload);
}