using EbayChat.Entities;

namespace EbayChat.Events.Handlers
{
    public class DisputeCreatedEventHandler : AdminEventHandlerBase<DisputeCreatedEvent>
    {
        public DisputeCreatedEventHandler(
            CloneEbayDbContext context,
            IAdminEventNotifier notifier,
            ILogger<DisputeCreatedEventHandler> logger)
            : base(context, notifier, logger)
        {
        }

        protected override string BuildMessage(DisputeCreatedEvent @event)
        {
            return $"Dispute created for Order #{@event.RelatedEntityId} by User #{@event.UserId}.";
        }
    }
}