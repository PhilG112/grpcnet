using GrpcNet.Server.Services;
using ProtoBuf.Grpc.Server;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("Redis")!;
var conn = ConnectionMultiplexer.Connect(connString);

builder.Services.AddSingleton<IConnectionMultiplexer>(conn);
builder.Services.AddSerilog(cfg => cfg.ReadFrom.Configuration(builder.Configuration));
builder.Services.AddCodeFirstGrpc();

var app = builder.Build();

app.MapGrpcService<TicketStoreService>();
app.MapGet("/", () => "This application only supports gRPC.");

app.Run();
