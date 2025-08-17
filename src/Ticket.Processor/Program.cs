using GrpcNet.Events;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;
using Ticket.Processor;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.AddServiceDefaults();

var redis = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis"));

builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddDataProtection()
    .SetApplicationName("grpcnet")
    .PersistKeysToStackExchangeRedis(redis, "grpcnet-keys");

builder.Services.AddSingleton<IEventBus, RedisEventBus>();

builder.Services.AddSingleton<IDataProtector>(sp =>
{
    var protectionProvider = sp.GetRequiredService<IDataProtectionProvider>();

    return protectionProvider.CreateProtector("grpcnet");
});

var host = builder.Build();
host.Run();
