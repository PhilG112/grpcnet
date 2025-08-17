using GrpcNet.Events;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;

namespace Ticket.Processor;
public class Worker(
    IDataProtector protector,       // already configured with the correct purpose in DI
    IEventBus eb,
    ILogger<Worker> logger) : BackgroundService
{
    private static readonly TicketSerializer TicketSerializer = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Ticket processor starting…");

        // Keep a strong reference to the subscription
        // Adapt the 'await using' vs 'using' based on your IEventBus return type.
        await eb.SubscribeAsync<TicketCreatedEvent>(Handle);

        logger.LogInformation("Subscribed to {Event}. Waiting for events…", nameof(TicketCreatedEvent));

        try
        {
            // Keep the worker alive until shutdown
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException) { /* normal */ }
        finally
        {
            logger.LogInformation("Ticket processor stopping…");
        }
    }

    private async Task Handle(TicketCreatedEvent e)
    {
        try
        {
            logger.LogInformation("Processing ticket {Key}", e.TicketKey);

            var ticket = Deserialize(e.SerializedTicket);
            if (ticket is null)
            {
                logger.LogWarning("Failed to deserialize ticket {Key}", e.TicketKey);
                return;
            }

            var claims = ticket.Principal.Claims.Select(c => new { c.Type, c.Value }).ToList();
            logger.LogInformation("Ticket {Key} claims: {@Claims}", e.TicketKey, claims);
        }
        catch (FormatException ex)
        {
            logger.LogError(ex, "Invalid Base64 for {Key}", e.TicketKey);
        }
        catch (CryptographicException ex)
        {
            logger.LogError(ex, "Unprotect failed for {Key}. Check app name/key ring/purpose.", e.TicketKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error for {Key}", e.TicketKey);
        }

        await Task.CompletedTask;
    }

    private AuthenticationTicket? Deserialize(string payload)
    {
        var protectedBytes = Convert.FromBase64String(payload);
        var unprotectedBytes = protector.Unprotect(protectedBytes);
        return TicketSerializer.Deserialize(unprotectedBytes);
    }
}