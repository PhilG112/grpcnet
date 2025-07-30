using System.Runtime.Serialization;

namespace GrpcNet.Proto.Contracts.Contracts.Requests;

[DataContract]
public class TicketRequest
{
    [DataMember(Order = 1)]
    public string SerializedTicket { get; set; }

    [DataMember(Order = 2)]
    public string TicketKey { get; set; }

    [DataMember(Order = 3)]
    public TimeSpan Expiry { get; set; }
}

