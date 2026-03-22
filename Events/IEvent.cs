namespace EbayChat.Events
{
    public interface IEvent
    {
        Guid EventId { get; }
        DateTime Timestamp { get; }
        int UserId { get; }
        int RelatedEntityId { get; }
        IReadOnlyDictionary<string, string>? Payload { get; }
        string EventType { get; }
    }
}