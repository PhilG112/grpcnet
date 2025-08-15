using System.Runtime.Serialization;

namespace GrpcNet.Proto.Contracts.Contracts.Requests;

[DataContract]
public class GetTicketRequest
{
    [DataMember(Order = 1)]
    public string TicketKey { get; set; }
}

