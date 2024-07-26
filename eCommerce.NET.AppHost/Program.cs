var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");
var rabbitMq = builder.AddRabbitMQ("eventbus");
var postgres = builder.AddPostgres("postgres").WithImageTag("latest");

var identityDb = postgres.AddDatabase("identity-db");
var orderDb = postgres.AddDatabase("ordering-db");

var identityApi = builder.AddProject<Projects.eCommerce_NET_IdentityAPI>("identity-api")
    .WithExternalHttpEndpoints()
    .WithReference(identityDb);

var weatherApi = builder.AddProject<Projects.eCommerce_NET_MinimalApi>("weather-api")
    .WithExternalHttpEndpoints();

var basketApi = builder.AddProject<Projects.eCommerce_NET_BasketAPI>("basket-api")
    .WithReference(redis)
    .WithReference(rabbitMq);
//.WithEnvironment("Identity__Url", identityEndpoint);

var api = builder.AddProject<Projects.eCommerce_NET_API>("api")
    .WithReference(basketApi)
    .WithReference(identityApi)
    .WithExternalHttpEndpoints();

builder.AddNpmApp("react", "../eCommerce.NET.React")
    .WithReference(api)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
