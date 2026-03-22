using EbayChat.Entities;

namespace EbayChat.Events.Handlers
{
    public abstract class AdminEventHandlerBase<TEvent> : IEventHandler<TEvent>
        where TEvent : IEvent
    {
        private readonly CloneEbayDbContext _context;
        private readonly IAdminEventNotifier _notifier;
        private readonly ILogger _logger;

        protected AdminEventHandlerBase(
            CloneEbayDbContext context,
            IAdminEventNotifier notifier,
            ILogger logger)
        {
            _context = context;
            _notifier = notifier;
            _logger = logger;
        }

        public async Task HandleAsync(TEvent @event)
        {
            var adminEvent = new AdminEvent
            {
                eventType = @event.EventType,
                referenceId = @event.RelatedEntityId,
                userId = @event.UserId,
                message = BuildMessage(@event),
                status = "Pending",
                createdAt = @event.Timestamp
            };

            _context.AdminEvents.Add(adminEvent);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Admin event persisted. EventId: {EventId}, Type: {EventType}, AdminEventId: {AdminEventId}",
                @event.EventId, @event.EventType, adminEvent.id);

            try
            {
                await _notifier.NotifyAdminsAsync(adminEvent, @event);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to push SignalR admin notification. EventId: {EventId}, Type: {EventType}",
                    @event.EventId, @event.EventType);
            }
        }

        protected abstract string BuildMessage(TEvent @event);
    }
}