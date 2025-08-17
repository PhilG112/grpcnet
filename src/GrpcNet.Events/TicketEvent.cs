namespace GrpcNet.Events;

public abstract class TicketEvent
{
    public required string TicketKey { get; set; }
}
