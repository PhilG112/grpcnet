using Grpc.Net.Client;
using GrpcNet.Api.TicketStore;
using GrpcNet.Proto.Contracts.Contracts;
using GrpcNet.Proto.Contracts.Contracts.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using ProtoBuf.Grpc.Client;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(opts =>
{
    opts.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    opts.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opts.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opts =>
{
    opts.Authority = builder.Configuration["Authority"];
    opts.ClientId = builder.Configuration["ClientId"];
    opts.ClientSecret = builder.Configuration["ClientSecret"];
    opts.CallbackPath = "/signin-oidc";
    opts.SignedOutCallbackPath = "/signout-callback-oidc";
    opts.ResponseType = "code";
    opts.UsePkce = true;

    if (builder.Environment.IsDevelopment())
    {
        opts.RequireHttpsMetadata = false;
    }

    // Add other scopes as necessary such as infsec:roles
    opts.Scope.Add("profile");
    opts.Scope.Add("openid");
    opts.Scope.Add("offline_access");
    opts.GetClaimsFromUserInfoEndpoint = true;
    opts.ClaimActions.MapAll();

    // Must be true, this is required to save access/id and refresh tokens against the users session.
    opts.SaveTokens = true;

    opts.Events.OnRedirectToIdentityProvider = context =>
    {
        context.ProtocolMessage.SetParameter("audience", builder.Configuration["Audience"]);
        return Task.CompletedTask;
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opts =>
{
    opts.Cookie.Name = "grpcnet.cookie";
    opts.Cookie.SameSite = SameSiteMode.Lax;
    opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    opts.Cookie.HttpOnly = true;

    opts.SessionStore = new TicketStoreShim(
        builder.Services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>());
});

builder.Services.AddAuthorization();

builder.Services.AddTransient<ICustomTicketStore, CustomTicketStore>();
builder.Services.AddSingleton(GrpcChannel.ForAddress("http://localhost:5103"));
builder.Services.AddSingleton(provider =>
{
    var chan = provider.GetRequiredService<GrpcChannel>();
    return chan.CreateGrpcService<ITicketService>();
});

builder.Services.AddDataProtection()
    .SetApplicationName("grpcnet")
    .PersistKeysToStackExchangeRedis(
            ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

builder.Services.AddSingleton<IDataProtector>(sp =>
{
    var protectionProvider = sp.GetRequiredService<IDataProtectionProvider>();

    return protectionProvider.CreateProtector("grpcnet");
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.MapGet("/login", async (HttpContext ctx, IAuthenticationService authService) =>
{
    await authService.ChallengeAsync(
       ctx,
       OpenIdConnectDefaults.AuthenticationScheme,
       new AuthenticationProperties { RedirectUri = "/" });
});

app.MapGet("/logout", async (HttpContext ctx, IAuthenticationService authService) =>
{
    await authService.SignOutAsync(ctx, CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties {  RedirectUri = "/" });
    await authService.SignOutAsync(
        ctx,
        OpenIdConnectDefaults.AuthenticationScheme,
        new AuthenticationProperties { RedirectUri = "/" });
});

app.MapGet("/resource", async (HttpContext ctx) =>
{
    var cookies = ctx.Request.Cookies;
    var i = await ctx.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    return Results.Json(new { ok = "ok" });
}).RequireAuthorization(new AuthorizeAttribute());

app.UseHttpsRedirection();

app.Run();