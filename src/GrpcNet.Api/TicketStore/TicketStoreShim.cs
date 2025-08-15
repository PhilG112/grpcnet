using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace GrpcNet.Api.TicketStore;

public class TicketStoreShim(IHttpContextAccessor httpContextAccessor) : ITicketStore
{
    private ICustomTicketStore InnerStore => httpContextAccessor.HttpContext.RequestServices.GetRequiredService<ICustomTicketStore>();

    public Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        return InnerStore.StoreAsync(ticket);
    }

    public Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        return InnerStore.RenewAsync(key, ticket);
    }

    public Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        return InnerStore.RetrieveAsync(key);
    }

    public Task RemoveAsync(string key)
    {
        return InnerStore.RemoveAsync(key);
    }
}

