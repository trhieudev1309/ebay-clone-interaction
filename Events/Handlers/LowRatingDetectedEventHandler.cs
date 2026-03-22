using EbayChat.Entities;

namespace EbayChat.Events.Handlers
{
    public class LowRatingDetectedEventHandler : AdminEventHandlerBase<LowRatingDetectedEvent>
    {
        public LowRatingDetectedEventHandler(
            CloneEbayDbContext context,
            IAdminEventNotifier notifier,
            ILogger<LowRatingDetectedEventHandler> logger)
            : base(context, notifier, logger)
        {
        }

        protected override string BuildMessage(LowRatingDetectedEvent @event)
        {
            var rating = @event.Payload != null && @event.Payload.TryGetValue("Rating", out var value)
                ? value
                : "N/A";

            return $"Low rating detected (Rating: {rating}) for Product #{@event.RelatedEntityId} by User #{@event.UserId}.";
        }
    }
}