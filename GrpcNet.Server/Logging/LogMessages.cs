namespace GrpcNet.Server.Logging;

public static partial class LogMessages
{
    [LoggerMessage(
        message: "The ticket request is received for {TicketId}")]
    public static partial void TicketRequestReceived(this ILogger logger, LogLevel level, string ticketId);
}
