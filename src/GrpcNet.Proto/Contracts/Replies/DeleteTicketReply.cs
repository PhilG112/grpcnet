using System.Runtime.Serialization;

namespace GrpcNet.Proto.Contracts.Contracts.Replies;

[DataContract]
public class DeleteTicketReply
{
    [DataMember(Order = 1)]
    public bool Success { get; set; }
}
