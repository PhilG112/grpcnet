namespace GrpcNet.Events;

public class TicketCreatedEvent : TicketEvent, IEvent
{
    public required string SerializedTicket { get; set; }
}

