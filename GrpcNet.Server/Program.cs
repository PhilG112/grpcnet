using GrpcNet.Server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProtoBuf.Grpc.Server;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(opts =>
{
    opts.Authority = builder.Configuration["Authority"];
    opts.Audience = builder.Configuration["Audience"];
    opts.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true
    };
});

builder.Services.AddAuthorization();

var connString = builder.Configuration.GetConnectionString("Redis")!;
var conn = ConnectionMultiplexer.Connect(connString);

builder.Services.AddSingleton<IConnectionMultiplexer>(conn);
builder.Services.AddSerilog(cfg => cfg.ReadFrom.Configuration(builder.Configuration));
builder.Services.AddCodeFirstGrpc();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<TicketStoreService>();
app.MapGet("/", () => "This application only supports gRPC.");

app.Run();
