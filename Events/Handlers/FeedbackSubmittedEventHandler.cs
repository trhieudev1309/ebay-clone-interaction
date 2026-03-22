using EbayChat.Entities;

namespace EbayChat.Events.Handlers
{
    public class FeedbackSubmittedEventHandler : AdminEventHandlerBase<FeedbackSubmittedEvent>
    {
        public FeedbackSubmittedEventHandler(
            CloneEbayDbContext context,
            IAdminEventNotifier notifier,
            ILogger<FeedbackSubmittedEventHandler> logger)
            : base(context, notifier, logger)
        {
        }

        protected override string BuildMessage(FeedbackSubmittedEvent @event)
        {
            var subject = @event.Payload != null && @event.Payload.TryGetValue("Subject", out var value)
                ? value
                : "General";

            return $"User feedback submitted by User #{@event.UserId}. Subject: {subject}.";
        }
    }
}