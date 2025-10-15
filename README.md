# Store - Microservices E-Commerce Platform

A microservices-based e-commerce platform built with .NET 9.0 and .NET Aspire for orchestration.

## Architecture Overview

This project consists of several microservices:

- **Auth API** - Authentication and authorization service
- **Catalog API** - Product catalog management
- **Cart API** - Shopping cart service
- **Notifications API** - Email and notification service
- **Gateway API** - API Gateway for routing requests

## Prerequisites

Before you can run this project, ensure you have the following installed:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (required for running dependencies)
- [Git](https://git-scm.com/downloads) (for cloning the repository)

### Verify Installation

Check that you have the correct versions installed:

```bash
dotnet --version    # Should be 9.0.x or later
docker --version    # Should be installed and running
```

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/magicman517/Store.git
cd Store
```

### 2. Restore Dependencies

Restore all NuGet packages for the solution:

```bash
dotnet restore
```

### 3. Build the Solution

Build all projects in the solution:

```bash
dotnet build
```

## Running the Project for Development

This project uses [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/) to orchestrate all services and their dependencies. Aspire will automatically start all required infrastructure (PostgreSQL, RabbitMQ, MinIO, Valkey, etc.) using Docker containers.

### Start the Application

Run the Aspire AppHost project:

```bash
dotnet run --project src/Aspire.AppHost/Aspire.AppHost.csproj
```

This will:
- Start all required Docker containers (PostgreSQL, RabbitMQ, MinIO, Valkey, MailPit)
- Start all microservices (Auth, Catalog, Cart, Notifications, Gateway)
- Open the Aspire Dashboard in your browser

### Aspire Dashboard

Once started, the Aspire Dashboard will be available at:
- **Dashboard URL**: Usually `http://localhost:15888` or `https://localhost:17299`

The dashboard provides:
- Real-time service status and health checks
- Logs from all services
- Metrics and telemetry
- Access to service endpoints and UI tools:
  - **Gateway API**: Main API endpoint with Scalar documentation at `/reference`
  - **PgAdmin**: PostgreSQL administration tool
  - **RabbitMQ Management**: Message queue management UI
  - **MailPit**: Email testing interface

### Accessing the Services

When running, the following services are available:

- **Gateway API**: Check the Aspire Dashboard for the dynamically assigned port
  - Swagger/Scalar API docs: `http://localhost:<port>/reference`
  - Health check: `http://localhost:<port>/health`

- **PgAdmin**: Check the Aspire Dashboard for the PostgreSQL Admin UI
- **RabbitMQ Management**: Check the Aspire Dashboard for RabbitMQ UI
- **MailPit**: Check the Aspire Dashboard for the email testing interface

### Stopping the Application

To stop all services, press `Ctrl+C` in the terminal where the AppHost is running. Aspire will gracefully shut down all services and containers.

## Running Tests

This project uses xUnit for testing with .NET Aspire's testing framework for integration tests.

### Run All Tests

```bash
dotnet test
```

### Run Tests with Detailed Output

```bash
dotnet test --verbosity normal
```

### Run Tests Without Building

If you've already built the solution:

```bash
dotnet test --no-build
```

### List Available Tests

To see all available tests without running them:

```bash
dotnet test --list-tests
```

### Test Project Structure

Tests are located in the `tests/Store.Tests` directory and include:
- Integration tests that verify the entire application stack
- Health check tests for all services
- End-to-end API tests

**Note**: Integration tests automatically start all required services and dependencies using .NET Aspire's testing framework. Tests may take some time to run as they spin up the full application stack.

## Development Workflow

### Making Code Changes

1. Make your code changes in the appropriate service
2. The Aspire AppHost supports hot reload for many changes
3. For structural changes, restart the AppHost

### Adding New Services

1. Create your new service project
2. Add a reference to it in `src/Aspire.AppHost/Aspire.AppHost.csproj`
3. Register it in `src/Aspire.AppHost/AppHost.cs`

### Viewing Logs

All service logs are aggregated in the Aspire Dashboard. You can:
- Filter logs by service
- Search log content
- View structured logging data

## Infrastructure Services

The application uses the following infrastructure services (automatically managed by Aspire):

- **PostgreSQL**: Relational database for Auth, Catalog, and Cart services
- **RabbitMQ**: Message broker for inter-service communication
- **MinIO**: S3-compatible object storage for file uploads
- **Valkey**: Redis-compatible cache (formerly Redis)
- **MailPit**: Email testing tool for development
- **OpenAI (Groq)**: AI API endpoint (requires API key configuration)

All infrastructure services run in Docker containers managed by Aspire.

## Troubleshooting

### Docker is not running

Ensure Docker Desktop is running before starting the application. Aspire requires Docker to run infrastructure services.

### Port conflicts

If you encounter port conflicts, check the Aspire Dashboard logs. Aspire will try to assign available ports automatically, but you may need to stop other services using the same ports.

### Database migrations

Database migrations are applied automatically when the Auth API starts. If you encounter database issues:

1. Stop the application
2. Remove the PostgreSQL data volume
3. Restart the application to recreate the database

### Build errors

If you encounter build errors after pulling new changes:

```bash
dotnet clean
dotnet restore
dotnet build
```

## Project Structure

```
Store/
├── src/
│   ├── Aspire.AppHost/          # Aspire orchestration project
│   ├── Aspire.ServiceDefaults/  # Shared Aspire service configuration
│   ├── Common/                  # Shared utilities and helpers
│   ├── Messaging/               # Shared messaging contracts
│   └── Services/
│       ├── Auth/                # Authentication service
│       ├── Cart/                # Shopping cart service
│       ├── Catalog/             # Product catalog service
│       ├── Gateway/             # API Gateway
│       └── Notifications/       # Notification service
└── tests/
    └── Store.Tests/             # Integration tests
```

## Additional Resources

- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [.NET 9.0 Documentation](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-9/overview)
- [Docker Documentation](https://docs.docker.com/)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
