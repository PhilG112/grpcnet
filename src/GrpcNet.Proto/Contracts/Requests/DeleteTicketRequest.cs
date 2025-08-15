using System.Runtime.Serialization;

namespace GrpcNet.Proto.Contracts.Contracts.Requests;

[DataContract]
public class DeleteTicketRequest
{
    [DataMember(Order = 1)]
    public string TicketKey { get; set; }
}

