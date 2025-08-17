namespace GrpcNet.Events;
public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event)
        where TEvent : IEvent;

    Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler)
        where TEvent : IEvent;
}