var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");
var rabbitMq = builder.AddRabbitMQ("eventbus");
var postgres = builder.AddPostgres("postgres").WithImageTag("latest");

var identityDb = postgres.AddDatabase("identitydb");
var orderDb = postgres.AddDatabase("orderingdb");

var identityApi = builder.AddProject<Projects.eCommerce_NET_IdentityAPI>("identityapi")
    .WithExternalHttpEndpoints()
    .WithReference(identityDb);

var weatherApi = builder.AddProject<Projects.eCommerce_NET_MinimalApi>("weatherapi")
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
    .WithReference(weatherApi)
    .WithEnvironment("BROWSER", "none") // Disable opening browser on npm start
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
