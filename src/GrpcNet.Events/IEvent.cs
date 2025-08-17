namespace GrpcNet.Events;

public interface IEvent
{
    public Guid EventId => Guid.NewGuid();

    public DateTime CreatedAt => DateTime.UtcNow;
}