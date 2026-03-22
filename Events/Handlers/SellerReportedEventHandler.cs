using EbayChat.Entities;

namespace EbayChat.Events.Handlers
{
    public class SellerReportedEventHandler : AdminEventHandlerBase<SellerReportedEvent>
    {
        public SellerReportedEventHandler(
            CloneEbayDbContext context,
            IAdminEventNotifier notifier,
            ILogger<SellerReportedEventHandler> logger)
            : base(context, notifier, logger)
        {
        }

        protected override string BuildMessage(SellerReportedEvent @event)
        {
            return $"Seller report submitted against Seller #{@event.RelatedEntityId} by User #{@event.UserId}.";
        }
    }
}