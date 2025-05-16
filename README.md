# Internship API

A backend API with Swagger UI and JWT bearer authentication for managing interns and HR users. Deployable to Render's free plan.

## Features

- JWT Bearer Authentication
- Role-based Authorization (HR and Intern roles)
- Swagger UI Documentation
- Entity Framework Core with SQL Server
- Docker support for SQL Server

## API Endpoints

### Authentication

- POST `/api/auth/login` - Login with username and password
- POST `/api/auth/register` - Register a new user

### HR Endpoints (Requires HR Role)

- GET `/api/hr/interns` - Get all interns
- GET `/api/hr/interns/{id}` - Get intern by ID
- POST `/api/hr/interns` - Create a new intern
- PUT `/api/hr/interns/{id}` - Update an intern
- DELETE `/api/hr/interns/{id}` - Delete an intern

### Intern Endpoints (Requires Intern Role)

- GET `/api/intern/welcome` - Get welcome message
- GET `/api/intern/profile` - Get current intern profile
- GET `/api/intern/list` - Get list of all interns (limited information)
- PUT `/api/intern/profile` - Update own profile (limited fields)

## Deployment to Render

This API is configured for deployment to Render's free plan using Docker.

### Deployment Steps

1. Create a free account on [Render](https://render.com/)

2. Connect your GitHub repository to Render

3. Create a new Web Service and select "Deploy from GitHub repo"

4. Select your repository and branch

5. Configure the following environment variables in the Render dashboard:
   - `ConnectionStrings__DefaultConnection`: Your production database connection string
   - `JwtSettings__SecretKey`: Your secure JWT secret key

6. Click "Create Web Service"

7. Render will automatically build and deploy your API using the provided Dockerfile

8. Once deployed, your API will be available at `https://internship-api.onrender.com` (or your custom subdomain)

### Database Options for Render

1. **PostgreSQL on Render**: Render offers a free PostgreSQL database that you can use instead of SQL Server. You'll need to update your code to use Npgsql instead of SQL Server.

2. **External Database**: You can use an external database service like Azure SQL, ElephantSQL, or Supabase.

## Local Setup Instructions

### Prerequisites

- .NET 9.0 SDK
- Docker (for SQL Server container)
- Entity Framework Core tools (`dotnet tool install --global dotnet-ef`)

### Database Setup

1. Start the SQL Server container:
   ```
   docker-compose up -d
   ```

2. Apply database migrations:
   ```
   dotnet ef database update
   ```

### Running the Application

```
dotnet run
```

The API will be available at `https://localhost:5001` and the Swagger UI will be accessible at the root URL.

### Default Credentials

- HR User:
  - Username: admin
  - Password: admin123

## JWT Authentication

The API uses JWT Bearer tokens for authentication. To access protected endpoints:

1. Obtain a token by making a POST request to `/api/auth/login`
2. Include the token in the Authorization header of subsequent requests:
   ```
   Authorization: Bearer {your_token}
   ```

## Docker SQL Server Configuration

The SQL Server container is configured with the following settings:

- Server: localhost,1433
- Database: InternshipDB
- Username: sa
- Password: YourStrong@Passw0rd

You can modify these settings in the `docker-compose.yml` and `appsettings.json` files.
