using GrpcNet.Proto.Contracts.Contracts;
using GrpcNet.Proto.Contracts.Contracts.Replies;
using GrpcNet.Proto.Contracts.Contracts.Requests;
using GrpcNet.Server.Logging;
using ProtoBuf.Grpc;
using StackExchange.Redis;

namespace GrpcNet.Server.Services
{
    public class TicketStoreService(
        IConnectionMultiplexer conn,
        ILogger<TicketStoreService> logger) : ITicketService
    {
        public async Task<DeleteTicketReply> DeleteTicketAsync(DeleteTicketRequest ticketRequest, CallContext ctx = default)
        {
            logger.TicketRequestReceived(LogLevel.Information, ticketRequest.TicketKey);
            var db = conn.GetDatabase();

            var success = await db.KeyDeleteAsync(ticketRequest.TicketKey);

            return new DeleteTicketReply
            {
                Success = success
            };
        }

        public async Task<GetTicketReply> GetTicketAsync(GetTicketRequest ticketRequest, CallContext ctx = default)
        {
            logger.TicketRequestReceived(LogLevel.Information, ticketRequest.TicketKey);
            
            var db = conn.GetDatabase();

            var value = await db.StringGetAsync(ticketRequest.TicketKey);

            return new GetTicketReply
            {
                Success = value.HasValue,
                SerializedTicket = value.ToString()
            };
        }

        public async Task<CreateTicketReply> SetTicketAsync(
            CreateTicketRequest ticketRequest,
            CallContext ctx = default)
        {
            logger.TicketRequestReceived(LogLevel.Information, ticketRequest.TicketKey);
            
            var db = conn.GetDatabase();

           await db.StringSetAsync(
                ticketRequest.TicketKey,
                ticketRequest.SerializedTicket,
                ticketRequest.Expiry);
            
            return new CreateTicketReply
            {
                Success = true
            };
        }
    }
}
