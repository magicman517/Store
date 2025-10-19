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
    .WithEnvironment("KC_HTTP_RELATIVE_PATH", "/_/auth")
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
    .WithHttpEndpoint(env: "PORT", port: 5173);

var authApi = builder.AddProject<Projects.Auth_API>("AuthAPI")
    .WithReference(keycloak);

var usersApi = builder.AddProject<Projects.Users_API>("UsersAPI")
    .WithReference(keycloak);

var cartApi = builder.AddProject<Projects.Cart_API>("CartAPI")
    .WithReference(keycloak);

var catalogApi = builder.AddProject<Projects.Catalog_API>("CatalogAPI")
    .WithReference(keycloak);

var notificationsApi = builder.AddProject<Projects.Notifications_API>("NotificationsAPI")
    .WithReference(keycloak);

var proxy = builder.AddYarp("Gateway")
    .WithHostPort(8080)
    .WithConfiguration(cfg =>
    {
        cfg.AddRoute("/_/auth/{**catch-all}", keycloak);
        cfg.AddRoute("/_/api/auth/{**catch-all}", authApi).WithTransformPathRemovePrefix("/_/api/auth");
        cfg.AddRoute("/_/api/users/{**catch-all}", usersApi).WithTransformPathRemovePrefix("/_/api/users");
        cfg.AddRoute("/_/api/catalog/{**catch-all}", catalogApi).WithTransformPathRemovePrefix("/_/api/catalog");
        cfg.AddRoute("/_/api/cart/{**catch-all}", cartApi).WithTransformPathRemovePrefix("/_/api/cart");
        cfg.AddRoute("/_/api/notifications/{**catch-all}", notificationsApi).WithTransformPathRemovePrefix("/_/api/notifications");

        cfg.AddRoute("/{**catch-all}", web.GetEndpoint("http"));
    });

await builder.Build().RunAsync();