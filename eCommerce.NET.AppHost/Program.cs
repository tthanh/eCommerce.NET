var builder = DistributedApplication.CreateBuilder(args);

var weatherApi = builder.AddProject<Projects.eCommerce_NET_MinimalApi>("weatherapi")
    .WithExternalHttpEndpoints();

builder.AddNpmApp("react", "../eCommerce.NET.React")
    .WithReference(weatherApi)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
