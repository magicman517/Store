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

var keycloak = builder.AddKeycloak("Keycloak", 8081)
    .WithDataVolume()
    .WithReference(usersDb)
    .WaitFor(usersDb)
    .WithRealmImport("../KeycloakReams")
    .WithEnvironment("KC_HTTP_ENABLED", "true")
    .WithEnvironment("KC_PROXY_HEADERS", "xforwarded")
    .WithEnvironment("KC_HTTP_RELATIVE_PATH", "/auth")
    .WithEnvironment("KC_DB", "postgres")
    .WithEnvironment("KC_DB_URL_HOST", postgres.Resource.PrimaryEndpoint.Property(EndpointProperty.Host))
    .WithEnvironment("KC_DB_URL_PORT", postgres.Resource.PrimaryEndpoint.Property(EndpointProperty.Port))
    .WithEnvironment("KC_DB_URL_DATABASE", usersDb.Resource.DatabaseName)
    .WithEnvironment("KC_DB_USERNAME", "postgres")
    .WithEnvironment("KC_DB_PASSWORD", postgres.Resource.PasswordParameter);

var web = builder.AddBunApp(
        "Web",
        Path.Combine(builder.AppHostDirectory, "..", "Web"),
        "dev")
    .WithBunPackageInstallation()
    .WithHttpEndpoint(env: "PORT", port: 5173)
    .WithReference(usersDb)
    .WaitFor(usersDb);

var authApi = builder.AddProject<Projects.Auth_API>("AuthAPI")
    .WithReference(usersDb)
    .WaitFor(usersDb);
var usersApi = builder.AddProject<Projects.Users_API>("UsersAPI")
    .WithReference(usersDb)
    .WaitFor(usersDb);
var cartApi = builder.AddProject<Projects.Cart_API>("CartAPI");
var catalogApi = builder.AddProject<Projects.Catalog_API>("CatalogAPI");
var notificationsApi = builder.AddProject<Projects.Notifications_API>("NotificationsAPI");

var api = builder.AddYarp("ReverseProxy")
    .WithHostPort(8080)
    .WithConfiguration(cfg =>
    {
        cfg.AddRoute("/auth/{**catch-all}", keycloak);
        cfg.AddRoute("/users/{**catch-all}", usersApi).WithTransformPathRemovePrefix("/users");
        cfg.AddRoute("/catalog/{**catch-all}", catalogApi).WithTransformPathRemovePrefix("/catalog");
        cfg.AddRoute("/cart/{**catch-all}", cartApi).WithTransformPathRemovePrefix("/cart");
        cfg.AddRoute("/notifications/{**catch-all}", notificationsApi).WithTransformPathRemovePrefix("/notifications");
    });

await builder.Build().RunAsync();