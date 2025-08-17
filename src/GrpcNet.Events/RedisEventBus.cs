
using StackExchange.Redis;
using System.Text.Json;

namespace GrpcNet.Events;

public class RedisEventBus(IConnectionMultiplexer cm) : IEventBus
{
    private readonly ISubscriber _sub = cm.GetSubscriber();
    private static string ChannelFor(Type t) => $"event:{t.Name}";
    private static string ChannelFor<T>() => ChannelFor(typeof(T));

    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var db = cm.GetDatabase();
        var channel = ChannelFor(@event.GetType());

        var serializedEvent = JsonSerializer.Serialize(@event);
        await _sub.PublishAsync(RedisChannel.Literal(channel), serializedEvent);
    }
        
    public async Task SubscribeAsync<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
    {
        var db = cm.GetDatabase();
        var channel = ChannelFor<TEvent>();
        await _sub.SubscribeAsync(RedisChannel.Literal(channel), (_, value) =>
        {
            if (!value.HasValue)
            {
                return;
            }

            var data = JsonSerializer.Deserialize<TEvent>(value);
            if (data is not null)
            {
                handler(data);
            }
        });
    }
}
