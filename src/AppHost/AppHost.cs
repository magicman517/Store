using Aspire.Hosting.Yarp.Transforms;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("Postgres")
    .WithPgAdmin(containerName: "PgAdmin")
    .WithDataVolume();
var usersDb = postgres.AddDatabase("UsersDB");
var ordersDb = postgres.AddDatabase("OrdersDB");
var catalogDb = postgres.AddDatabase("CatalogDB");
var cartDb = postgres.AddDatabase("CartDB");

var rabbitMq = builder.AddRabbitMQ("RabbitMQ")
    .WithManagementPlugin()
    .WithDataVolume();

var mongoDb = builder.AddMongoDB("MongoDB")
    .WithDbGate(containerName: "DbGate")
    .WithDataVolume();

var redis = builder.AddRedis("Redis")
    .WithRedisInsight(containerName: "RedisInsight")
    .WithDataVolume();

var mailPit = builder.AddMailPit("MailPit")
    .WithDataVolume("MailPitData");

var minio = builder.AddMinioContainer("MinIO")
    .WithDataVolume();

var usersApi = builder.AddProject<Projects.Users_API>("UsersAPI")
    .WithReference(usersDb)
    .WaitFor(usersDb);

var cartApi = builder.AddProject<Projects.Cart_API>("CartAPI")
    .WithReference(cartDb)
    .WithReference(redis)
    .WaitFor(cartDb)
    .WaitFor(redis);

var catalogApi = builder.AddProject<Projects.Catalog_API>("CatalogAPI")
    .WithReference(catalogDb)
    .WithReference(redis)
    .WaitFor(catalogDb)
    .WaitFor(redis);

var notificationsApi = builder.AddProject<Projects.Notifications_API>("NotificationsAPI")
    .WithReference(rabbitMq)
    .WithReference(mailPit)
    .WithReference(minio)
    .WaitFor(rabbitMq)
    .WaitFor(mailPit)
    .WaitFor(minio);

var proxy = builder.AddYarp("Gateway")
    .WithHostPort(8080)
    .WithConfiguration(cfg =>
    {
        cfg.AddRoute("/users/{**catch-all}", usersApi).WithTransformPathRemovePrefix("/users");
        cfg.AddRoute("/catalog/{**catch-all}", catalogApi).WithTransformPathRemovePrefix("/catalog");
        cfg.AddRoute("/cart/{**catch-all}", cartApi).WithTransformPathRemovePrefix("/cart");
        cfg.AddRoute("/notifications/{**catch-all}", notificationsApi).WithTransformPathRemovePrefix("/notifications");
    });

await builder.Build().RunAsync();