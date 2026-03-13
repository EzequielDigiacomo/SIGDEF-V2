var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.SIGDEF_Api>("sigdef-api");

builder.Build().Run();
