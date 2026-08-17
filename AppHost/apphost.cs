#:package Aspire.Hosting.PostgreSQL@13.4.6
#:sdk Aspire.AppHost.Sdk@13.4.6
#:project ../gui/Lms.Desktop/Lms.Desktop.csproj

var builder = DistributedApplication.CreateBuilder(args);

var db = builder
    .AddPostgres("postgres")
    .WithDataVolume();

db.AddDatabase("lmsdb");

builder.AddProject<Projects.Lms_Desktop>("desktop")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
