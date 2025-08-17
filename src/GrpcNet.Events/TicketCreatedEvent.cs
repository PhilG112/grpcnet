namespace GrpcNet.Events;

public class TicketCreatedEvent : IEvent
{
    public required string SerializedTicket { get; set; }

    public required string TicketKey { get; set; }
}

