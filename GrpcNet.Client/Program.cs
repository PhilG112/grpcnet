using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using GrpcNet.Client;
using System.Diagnostics;

using var chan = GrpcChannel.ForAddress("http://localhost:5103");

var client = new Ticket.TicketClient(chan);
var tl = new List<TimeSpan>(10000);
for (var i = 0; i < 10000; i++)
{
    var startTs = Stopwatch.GetTimestamp();
    var reply = client.StoreTicket(new TicketRequest { Key = "myKey", Expiry = Duration.FromTimeSpan(new TimeSpan(1, 10, 10)) });
    var elapsed = Stopwatch.GetElapsedTime(startTs);
    tl.Add(elapsed);
}

var avg = tl.Average(ts => ts.Milliseconds);
Console.WriteLine($"Average time for 10000 requests: {avg} ms");
