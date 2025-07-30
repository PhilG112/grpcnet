using GrpcNet.Proto.Contracts.Contracts;
using GrpcNet.Proto.Contracts.Contracts.Replies;
using GrpcNet.Proto.Contracts.Contracts.Requests;
using GrpcNet.Server.Logging;
using ProtoBuf.Grpc;

namespace GrpcNet.Server.Services
{
    public class TicketStoreService(ILogger<TicketStoreService> logger): ITicketService
    {
        public Task<TicketReply> SetTicketAsync(
            TicketRequest ticketRequest,
            CallContext ctx = default)
        {
            logger.TicketRequestReceived(LogLevel.Information, ticketRequest.TicketKey);
            return Task.FromResult(new TicketReply { Success = true });
        }
    }
}
