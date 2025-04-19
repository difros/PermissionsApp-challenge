# PermissionsApp

This is a .NET 8 Web API project with React frontend for managing employee permissions.

## Project Structure

- `API/PermissionsApp`: Main API project (Startup, Program, etc.)
- `PermissionsApp.Application`: Application layer (DTOs, services)
- `PermissionsApp.Domain`: Domain models and interfaces
- `PermissionsApp.Infraestructure`: EF Core configuration, migrations, and repository implementations
- `PermissionsApp.Tests/`: Set of unit and integration tests
- `webapp/`: React frontend application
  - `src/app/`: Redux configuration
  - `src/components/`: Reusable components
  - `src/features/`: Specific functionalities
  - `src/services/`: Services for API communication
  - `src/types/`: TypeScript type definitions

## Technologies Used

### Backend
- .NET Core API
- Entity Framework Core
- SQL Server
- Elasticsearch
- Apache Kafka

### Frontend
- React with TypeScript
- Redux (Redux Toolkit)
- Material-UI
- React Router
- Vite (build tool)

## Features

- Permission listing
- Creation of new permissions
- Editing existing permissions
- Viewing permission types

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/)
- Node.js (for local frontend development)

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
- Start SQL Server in a container (please be patient, this process can take a while)
- Apply database migrations on app startup
- Build and serve the frontend
- Start the API

### 3. Access the application
- Frontend: http://localhost:3000
- API Backend: http://localhost:8080
- Swagger UI: http://localhost:8080/swagger
- Elasticsearch: http://localhost:9200
- Kafka: localhost:9092

---

## Local Development

### Backend
The backend is ready to run after the Docker setup above.

### Frontend

1. Navigate to the frontend folder:
```
cd webapp
```

2. Install dependencies:
```
npm install
```

3. Run the development server:
```
npm run dev
```

4. Access the frontend at http://localhost:3000

### Building the Frontend

```
cd webapp
npm run build
```

### Frontend Technical Details

The frontend uses React, TypeScript and Vite, providing:

- Hot Module Replacement (HMR) for fast development
- TypeScript configuration for type-checking
- Official Vite plugins:
  - [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) using Babel for Fast Refresh
  - [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) using SWC for Fast Refresh

#### ESLint Configuration

The project includes an ESLint configuration for type checking:

```js
export default tseslint.config({
  extends: [
    ...tseslint.configs.recommendedTypeChecked,
    // Alternatively, for stricter rules:
    // ...tseslint.configs.strictTypeChecked,
    // Optionally, for stylistic rules:
    // ...tseslint.configs.stylisticTypeChecked,
  ],
  languageOptions: {
    parserOptions: {
      project: ['./tsconfig.node.json', './tsconfig.app.json'],
      tsconfigRootDir: import.meta.dirname,
    },
  },
})
```

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


