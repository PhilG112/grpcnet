using GrpcNet.Proto.Contracts.Contracts;
using GrpcNet.Proto.Contracts.Contracts.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;

namespace GrpcNet.Api.TicketStore;

public class CustomTicketStore(
    ILogger<CustomTicketStore> logger,
    IDataProtector protector,
    ITicketService ticketService) : ICustomTicketStore
{
    private static readonly TicketSerializer TicketSerializer = new();

    public async Task RemoveAsync(string key)
    {
        await ticketService.DeleteTicketAsync(new DeleteTicketRequest
        {
            TicketKey = key
        });
    }
    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        var reply = await ticketService.GetTicketAsync(new GetTicketRequest
        {
            TicketKey = key
        });

        return reply.Success ? Deserialize(reply.SerializedTicket) : null;
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var hexKey = CreateUniqueHexId();
        var redisKey = $"{hexKey}";
        var expiry = GetTicketExpiry(ticket);

        var serializedTicket = Serialize(ticket);

        var ticketReply = await ticketService.SetTicketAsync(new CreateTicketRequest
        {
            Expiry = expiry,
            SerializedTicket = serializedTicket,
            TicketKey = redisKey
        });

        return redisKey;
    }

    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

    public static string CreateUniqueHexId(int length = 8)
    {
        var bytes = CreateRandomKey(length);

        return BitConverter.ToString(bytes).Replace("-", "");
    }

    private static byte[] CreateRandomKey(int length)
    {
        var bytes = new byte[length];
        Rng.GetBytes(bytes);

        return bytes;
    }

    private static TimeSpan GetTicketExpiry(AuthenticationTicket ticket)
    {
        var offset = ticket.Properties.ExpiresUtc;
        if (offset == null || offset.Value == DateTimeOffset.MinValue)
        {
            return TimeSpan.FromDays(3);
        }

        return offset.Value - DateTimeOffset.UtcNow;
    }

    private AuthenticationTicket? Deserialize(string payload)
    {
        var protectedBytes = Convert.FromBase64String(payload);
        var unprotectedBytes = protector.Unprotect(protectedBytes);

        return TicketSerializer.Deserialize(unprotectedBytes);
    }

    private string Serialize(AuthenticationTicket ticket)
    {
        var unprotectedBytes = TicketSerializer.Serialize(ticket);
        var protectedBytes = protector.Protect(unprotectedBytes);
        return Convert.ToBase64String(protectedBytes);
    }
}

