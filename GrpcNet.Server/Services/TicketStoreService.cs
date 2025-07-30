using Grpc.Core;
using GrpcNet.Server.Logging;

namespace GrpcNet.Server.Services
{
    public class TicketStoreService(ILogger<TicketStoreService> logger): Ticket.TicketBase
    {
        public override Task<TicketResponse> StoreTicket(TicketRequest request, ServerCallContext context)
        {
            logger.TicketRequestReceived(LogLevel.Information, request.Key);
            return Task.FromResult(new TicketResponse
            {
                Success = true
            });
        }
    }
   
}
