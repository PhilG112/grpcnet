using Grpc.Net.Client;
using GrpcNet.Proto.Contracts.Contracts;
using GrpcNet.Proto.Contracts.Contracts.Requests;
using ProtoBuf.Grpc.Client;
using System.Diagnostics;

using var chan = GrpcChannel.ForAddress("http://localhost:5103");

var client = chan.CreateGrpcService<ITicketService>();
var tl = new List<TimeSpan>(10000);
for (var i = 0; i < 10000; i++)
{
    var startTs = Stopwatch.GetTimestamp();
    var reply = await client.SetTicketAsync(new CreateTicketRequest
    {
        TicketKey = "myKey",
        Expiry = new TimeSpan(1, 10, 10),
        SerializedTicket = ""
    });
    var elapsed = Stopwatch.GetElapsedTime(startTs);
    tl.Add(elapsed);
}

var avg = tl.Average(ts => ts.Milliseconds);
Console.WriteLine($"Average time for 10000 requests: {avg} ms");
