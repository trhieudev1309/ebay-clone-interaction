namespace EbayChat.Events
{
    public interface IEventDispatcher
    {
        Task PublishAsync(IEvent @event);
    }
}