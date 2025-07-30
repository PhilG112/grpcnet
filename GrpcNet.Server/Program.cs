using GrpcNet.Proto.Contracts.Contracts;
using GrpcNet.Server.Services;
using ProtoBuf.Grpc.Server;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("Redis")!;
var conn = ConnectionMultiplexer.Connect(connString);

builder.Services.AddSingleton<IConnectionMultiplexer>(conn);
builder.Services.AddSerilog(cfg => cfg.ReadFrom.Configuration(builder.Configuration));
//builder.Services.AddTransient<ITicketService, TicketStoreService>();
builder.Services.AddCodeFirstGrpc();
//builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<TicketStoreService>();
app.MapGet("/", () => "This application only supports gRPC.");

app.Run();
