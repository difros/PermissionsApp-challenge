# PermissionsApp

This is a .NET 8 Web API project for managing employee permissions.

## Project Structure

- `API/PermissionsApp`: Main API project (Startup, Program, etc.)
- `PermissionsApp.Application`: Application layer (DTOs, services)
- `PermissionsApp.Domain`: Domain models and interfaces
- `PermissionsApp.Infraestructure`: EF Core configuration, migrations, and repository implementations

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/)

### 1. Clone the repository
```bash
git clone https://github.com/difros/PermissionsApp-challenge.git
cd PermissionsApp
```

### 2. Build and run with Docker Compose
```bash
docker-compose up --build
```
This will:
- Build the API
- Start SQL Server in a container (please be pacient, this process can take a while)
- Apply database migrations on app startup (please check this point below)
- Start the API

You can now access the API at: `http://localhost:8080`
Also, you can now access to Swagger at: `http://localhost:8080/swagger`

---

<!-- ## Database Migrations

If you modify the database schema, you can add and apply migrations manually:

### Add a new migration
```bash
dotnet ef migrations add MigrationName -s ./API/PermissionsApp -p ./PermissionsApp.Infraestructure
```

### Apply migrations to the database
```bash
dotnet ef database update -s ./API/PermissionsApp -p ./PermissionsApp.Infraestructure
```

> These commands require the EF Core CLI:
```bash
dotnet tool install --global dotnet-ef
``` -->

---

## Automatic Database Creation

The app is configured to automatically:
- Create the database (if it doesn't exist)
- Apply any pending EF Core migrations

However, to ensure the tables and initial data are set up correctly, you need to manually execute the SQL script provided. This script will create the necessary tables and seed initial data for Permission Types.

### Manual Execution of SQL Script
1. Connect to your SQL Server instance using SQL Server Management Studio (SSMS) or another SQL client.
- Server name: `localhost,1433`
- Authentication: `SQL Server Authentication`
- Login: `sa`
- Password: `Chall_Perm1510n`
2. Select the PermissionsDB database.
3. Execute the `initDB.sql` script to create tables and seed initial data.

---

## Seeding Elasticsearch

The app uses a service `IElasticsearchService` that:
- Initializes the Elasticsearch index on startup

Make sure Elasticsearch is up and running if this service is used.

---

## Useful Endpoints
- `GET /api/permissions` → List all permissions
- `POST /api/permissions` → Create a new permission
- `PUT /api/permissions/{id}` → Update a permission
- `GET /api/permission-type` → List permission types

---


