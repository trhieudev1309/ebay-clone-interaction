using Microsoft.AspNetCore.SignalR;

namespace EbayChat.Hubs
{
    public class AdminNotificationHub : Hub
    {
        public const string AdminGroup = "admins";

        public async Task JoinAdminDashboard()
        {
            var role = Context.GetHttpContext()?.Session.GetString("role")?.Trim();
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new HubException("Only Admin can subscribe to admin notifications.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, AdminGroup);
        }
    }
}