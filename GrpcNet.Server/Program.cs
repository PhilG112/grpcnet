using GrpcNet.Server.Services;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connString = builder.Configuration.GetConnectionString("Redis")!;
var conn = ConnectionMultiplexer.Connect(connString);

builder.Services.AddSingleton<IConnectionMultiplexer>(conn);
builder.Services.AddSerilog(cfg => cfg.ReadFrom.Configuration(builder.Configuration));
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<TicketStoreService>();
app.MapGet("/", () => "This application only supports gRPC.");

app.Run();
