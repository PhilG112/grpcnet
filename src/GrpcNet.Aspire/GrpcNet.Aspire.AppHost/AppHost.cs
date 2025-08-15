var builder = DistributedApplication.CreateBuilder(args);

var grpcServer = builder.AddProject<Projects.GrpcNet_Server>("GrpcService");

builder.AddProject<Projects.GrpcNet_Api>("GrpcApi")
    .WithReference(grpcServer);

builder.AddProject<Projects.Ticket_Processor>("TicketProcessor");


builder.Build().Run();
