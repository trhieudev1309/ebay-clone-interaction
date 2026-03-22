using EbayChat.Entities;

namespace EbayChat.Events.Handlers
{
    public class ReturnRequestedEventHandler : AdminEventHandlerBase<ReturnRequestedEvent>
    {
        public ReturnRequestedEventHandler(
            CloneEbayDbContext context,
            IAdminEventNotifier notifier,
            ILogger<ReturnRequestedEventHandler> logger)
            : base(context, notifier, logger)
        {
        }

        protected override string BuildMessage(ReturnRequestedEvent @event)
        {
            return $"Return/refund requested for Order #{@event.RelatedEntityId} by User #{@event.UserId}.";
        }
    }
}