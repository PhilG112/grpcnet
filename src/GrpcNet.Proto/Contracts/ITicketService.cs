using GrpcNet.Proto.Contracts.Contracts.Replies;
using GrpcNet.Proto.Contracts.Contracts.Requests;
using ProtoBuf.Grpc;
using System.ServiceModel;

namespace GrpcNet.Proto.Contracts.Contracts;

[ServiceContract]
public interface ITicketService
{
    [OperationContract]
    Task<CreateTicketReply> SetTicketAsync(CreateTicketRequest ticketRequest, CallContext ctx = default);

    [OperationContract]
    Task<GetTicketReply> GetTicketAsync(GetTicketRequest ticketRequest, CallContext ctx = default);

    [OperationContract]
    Task<DeleteTicketReply> DeleteTicketAsync(DeleteTicketRequest ticketRequest, CallContext ctx = default);
}

