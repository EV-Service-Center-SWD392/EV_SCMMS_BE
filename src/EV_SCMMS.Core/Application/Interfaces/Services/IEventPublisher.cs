namespace EV_SCMMS.Core.Application.Interfaces.Services;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event);
    void Subscribe<TEvent>(Func<TEvent, Task> handler);
}
