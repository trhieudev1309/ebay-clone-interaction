namespace EbayChat.Events
{
    public abstract record EventBase(
        int UserId,
        int RelatedEntityId,
        IReadOnlyDictionary<string, string>? Payload = null) : IEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();

        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        public string EventType => GetType().Name;
    }
}