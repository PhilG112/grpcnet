using Grpc.Net.Client;
using GrpcNet.Proto.Contracts.Contracts;
using ProtoBuf.Grpc.Client;

namespace GrpcNet.Api;

public class GrpcClientFactory
{
    private static readonly GrpcChannel _channel = GrpcChannel.ForAddress("http://localhost:5103");

    public static ITicketService CreateTicketServiceClient()
    {
        return _channel.CreateGrpcService<ITicketService>();
    }
}

