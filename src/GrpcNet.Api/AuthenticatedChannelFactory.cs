using Grpc.Core;
using Grpc.Net.Client;

namespace GrpcNet.Api;

public static class AuthenticatedChannelFactory
{
    public static GrpcChannel CreateChannel(ITokenService service)
    {
        var credentials = CallCredentials.FromInterceptor(async (_, metadata) =>
        {
            var authToken = await service.GetTokenAsync();
            metadata.Add("Authorization", $"Bearer {authToken}");
        });
        
        var channel = GrpcChannel.ForAddress("https://localhost:7245", new GrpcChannelOptions
        {
            Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
        });
        return channel;
    }
}

