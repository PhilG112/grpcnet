using GrpcNet.Proto.Contracts.Contracts.Replies;
using GrpcNet.Proto.Contracts.Contracts.Requests;
using ProtoBuf.Grpc;
using System.ServiceModel;

namespace GrpcNet.Proto.Contracts.Contracts;

[ServiceContract]
public interface ITicketService
{
    [OperationContract]
    Task<TicketReply> SetTicketAsync(TicketRequest ticketRequest, CallContext ctx = default);
}

