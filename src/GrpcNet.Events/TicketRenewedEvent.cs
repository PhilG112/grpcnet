namespace GrpcNet.Events;

public class TicketRenewedEvent : TicketEvent, IEvent
{
    public required string SerializedTicket { get; set; }
    public required DateTime RenewedAtUtc { get; set; }
}

