using Grpc.Net.Client;
using GrpcNet.Client;
using System.Diagnostics;

using var chan = GrpcChannel.ForAddress("http://localhost:5103");

var client = new Greeter.GreeterClient(chan);

for (var i = 0; i < 10; i++)
{
    var startTs = Stopwatch.GetTimestamp();
    var reply = await client.SayHelloAsync(new HelloRequest { Name = "World" });
    var elapsed = Stopwatch.GetElapsedTime(startTs);
    Console.WriteLine($"Elapsed time: {elapsed.TotalMilliseconds} ms");
}