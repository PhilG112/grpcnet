namespace GrpcNet.Server.Logging;

public static partial class LogMessages
{
    [LoggerMessage(
        message: "The ticket request is received for {TicketId}")]
    public static partial void TicketRequestReceived(this ILogger logger, LogLevel level, string ticketId);
    
    [LoggerMessage(
        message: "Log user claims: {@Claims}")]
    public static partial void LogUserClaims(this ILogger logger, LogLevel level, IEnumerable<object> claims);
}
