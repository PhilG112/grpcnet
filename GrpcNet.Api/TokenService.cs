using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;

namespace GrpcNet.Api;

public class TokenService(
    IConfiguration configuration,
    IHttpClientFactory httpClientFactory,
    IMemoryCache cache,
    ILogger<TokenService> logger) : ITokenService
{
    public async Task<string> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        var result = await cache.GetOrCreateAsync($"{nameof(TokenService)}.{nameof(GetTokenAsync)}", async entry =>
        {
            logger.LogInformation("NOT FOUND IN CACHE GETTING TOKEN FROM AUTH SERVER");
            Console.WriteLine("NOT FOUND IN CACHE GETTING TOKEN FROM AUTH SERVER");
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(50);
            var client = httpClientFactory.CreateClient();
        
            var authUrl = configuration["Authority"];
            var clientId = configuration["ClientId"];
            var clientSecret = configuration["ClientSecret"];
            var audience = configuration["Audience"];
        
            var queryParams = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "grant_type", "client_credentials" },
                { "audience", audience },
            };
        
            var request = new HttpRequestMessage(HttpMethod.Post, $"{authUrl}/oauth/token")
            {
                Content = new FormUrlEncodedContent(queryParams)
            };
        
            var response = await client.SendAsync(request, cancellationToken);
        
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to retrieve token: {response.StatusCode}");
            }
        
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content);
            return tokenResponse.AccessToken;
        });

        return result;
    }

    private record TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; }
        
        [JsonPropertyName("expires_in")]
        public int ExpiresInSeconds { get; init; }
    }
}