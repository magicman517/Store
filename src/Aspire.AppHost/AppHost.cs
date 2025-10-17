using Aspire.Hosting.Yarp.Transforms;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("StoreProductionEnvironment");

var mailPit = builder.AddMailPit("MailPit")
    .WithDataVolume("MailPitData");

var minio = builder.AddMinioContainer("MinIO")
    .WithDataVolume();

var rabbitMq = builder.AddRabbitMQ("RabbitMQ")
    .WithManagementPlugin()
    .WithDataVolume();

var valkey = builder.AddValkey("Valkey")
    .WithDataVolume();

var openAi = builder.AddOpenAI("OpenAI")
    .WithEndpoint("https://api.groq.com/openai/v1");
var gptOss20b = openAi
    .AddModel("GPT-OSS-20b", "openai/gpt-oss-20b")
    .WithHealthCheck();
var gptOss120b = openAi
    .AddModel("GPT-OSS-120b", "openai/gpt-oss-120b")
    .WithHealthCheck();

// var mongoDb = builder.AddMongoDB("MongoDB")
//     .WithDbGate(containerName: "DbGate")
//     .WithDataVolume();

var postgres = builder.AddPostgres("Postgres")
    .WithDataVolume()
    .WithPgAdmin(containerName: "PgAdmin");
var authDb = postgres.AddDatabase("AuthDB");
var catalogDb = postgres.AddDatabase("CatalogDB");
var cartDb = postgres.AddDatabase("CartDB");

var authApi = builder.AddProject<Projects.Auth_API>("AuthAPI")
    .WithHttpHealthCheck("/health")
    .WithReference(authDb)
    .WaitFor(authDb);

var catalogApi = builder.AddProject<Projects.Catalog_API>("CatalogAPI")
    .WithReference(catalogDb)
    .WaitFor(catalogDb)
    .WithReference(minio)
    .WaitFor(minio)
    .WithReference(valkey)
    .WaitFor(valkey);

var cartApi = builder.AddProject<Projects.Cart_API>("CartAPI")
    .WithReference(cartDb)
    .WaitFor(cartDb)
    .WithReference(valkey)
    .WaitFor(valkey);

var notificationsApi = builder.AddProject<Projects.Notifications_API>("NotificationsAPI")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(mailPit)
    .WaitFor(mailPit);

var gateway = builder.AddYarp("Gateway")
    .WithHostPort(8080)
    .WithConfiguration(cfg =>
    {
        cfg.AddRoute("/auth/{**catch-all}", authApi).WithTransformPathRemovePrefix("/auth");
        cfg.AddRoute("/catalog/{**catch-all}", catalogApi).WithTransformPathRemovePrefix("/catalog");
        cfg.AddRoute("/cart/{**catch-all}", cartApi).WithTransformPathRemovePrefix("/cart");
        cfg.AddRoute("/notifications/{**catch-all}", notificationsApi).WithTransformPathRemovePrefix("/notifications");
    });

builder.Build().Run();