var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("StoreProductionEnvironment");

var mailPit = builder.AddMailPit("MailPit")
    .WithDataVolume("MailPitData");

var minio = builder.AddMinioContainer("MinIO")
    .WithDataVolume();

var rabbitMq = builder.AddRabbitMQ("RabbitMQ")
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

var postgres = builder.AddPostgres("Postgres")
    .WithDataVolume()
    .WithPgWeb(containerName: "PgWeb");
var authDb = postgres.AddDatabase("AuthDB");
var catalogDb = postgres.AddDatabase("CatalogDB");
var cartDb = postgres.AddDatabase("CartDB");

var authApi = builder.AddProject<Projects.Auth_API>("AuthAPI")
    .WithReference(authDb)
    .WaitFor(authDb)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq);

var catalogApi = builder.AddProject<Projects.Catalog_API>("CatalogAPI")
    .WithReference(catalogDb)
    .WaitFor(catalogDb)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(minio)
    .WaitFor(minio)
    .WithReference(valkey)
    .WaitFor(valkey);

var cartApi = builder.AddProject<Projects.Cart_API>("CartAPI")
    .WithReference(cartDb)
    .WaitFor(cartDb)
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(valkey)
    .WaitFor(valkey);

var notificationsApi = builder.AddProject<Projects.Notifications_API>("NotificationsAPI")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithReference(mailPit)
    .WaitFor(mailPit);

builder.Build().Run();