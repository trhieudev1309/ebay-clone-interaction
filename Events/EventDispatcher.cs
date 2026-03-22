using Microsoft.Extensions.DependencyInjection;

namespace EbayChat.Events
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventDispatcher> _logger;

        public EventDispatcher(IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task PublishAsync(IEvent @event)
        {
            _logger.LogInformation(
                "Publishing event. EventId: {EventId}, Type: {EventType}, UserId: {UserId}, RelatedEntityId: {RelatedEntityId}",
                @event.EventId, @event.EventType, @event.UserId, @event.RelatedEntityId);

            using var scope = _serviceProvider.CreateScope();

            var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
            var handlers = scope.ServiceProvider.GetServices(handlerType).ToList();

            if (handlers.Count == 0)
            {
                _logger.LogWarning(
                    "No handlers registered for event type {EventType}. EventId: {EventId}",
                    @event.EventType, @event.EventId);
                return;
            }

            var method = handlerType.GetMethod("HandleAsync");
            var tasks = new List<Task>(handlers.Count);

            foreach (var handler in handlers)
            {
                var task = method?.Invoke(handler, new object[] { @event }) as Task;
                if (task != null)
                {
                    tasks.Add(task);
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}