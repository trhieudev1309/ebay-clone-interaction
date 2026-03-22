using EbayChat.Entities;

namespace EbayChat.Events
{
    public interface IAdminEventNotifier
    {
        Task NotifyAdminsAsync(AdminEvent adminEvent, IEvent sourceEvent);
    }
}