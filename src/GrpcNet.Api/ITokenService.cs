namespace GrpcNet.Api;

public interface ITokenService
{
    Task<string> GetTokenAsync(CancellationToken cancellationToken = default);
}