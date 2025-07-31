using System.Runtime.Serialization;

namespace GrpcNet.Proto.Contracts.Contracts.Replies;

[DataContract]
public class CreateTicketReply
{
    [DataMember(Order = 1)]
    public bool Success { get; set; }

    [DataMember(Order = 2)]
    public string Message { get; set; }
}

