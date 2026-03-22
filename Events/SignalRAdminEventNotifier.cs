using EbayChat.Entities;
using EbayChat.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace EbayChat.Events
{
    public class SignalRAdminEventNotifier : IAdminEventNotifier
    {
        private readonly IHubContext<AdminNotificationHub> _hubContext;
        private readonly ILogger<SignalRAdminEventNotifier> _logger;

        public SignalRAdminEventNotifier(
            IHubContext<AdminNotificationHub> hubContext,
            ILogger<SignalRAdminEventNotifier> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifyAdminsAsync(AdminEvent adminEvent, IEvent sourceEvent)
        {
            var payload = new
            {
                id = adminEvent.id,
                eventType = adminEvent.eventType,
                referenceId = adminEvent.referenceId,
                userId = adminEvent.userId,
                message = adminEvent.message,
                status = adminEvent.status,
                createdAt = adminEvent.createdAt,
                eventId = sourceEvent.EventId,
                sourcePayload = sourceEvent.Payload
            };

            await _hubContext.Clients.Group(AdminNotificationHub.AdminGroup)
                .SendAsync("AdminEventReceived", payload);

            _logger.LogInformation(
                "SignalR admin notification sent. EventId: {EventId}, Type: {EventType}, AdminEventId: {AdminEventId}",
                sourceEvent.EventId, sourceEvent.EventType, adminEvent.id);
        }
    }
}