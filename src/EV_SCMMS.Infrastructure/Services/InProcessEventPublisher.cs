using EV_SCMMS.Core.Application.Interfaces.Services;
using System.Collections.Concurrent;

namespace EV_SCMMS.Infrastructure.Services;

public class InProcessEventPublisher : IEventPublisher
{
    private readonly ConcurrentDictionary<Type, List<Func<object, Task>>> _handlers = new();

    public Task PublishAsync<TEvent>(TEvent @event)
    {
        var eventType = typeof(TEvent);
        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            var tasks = handlers.Select(h => h(@event as object)).ToArray();
            return Task.WhenAll(tasks);
        }
        return Task.CompletedTask;
    }

    public void Subscribe<TEvent>(Func<TEvent, Task> handler)
    {
        var eventType = typeof(TEvent);
        var wrapper = new Func<object, Task>(o => handler((TEvent)o));
        _handlers.AddOrUpdate(eventType, _ => new List<Func<object, Task>> { wrapper }, (_, list) => { list.Add(wrapper); return list; });
    }
}
