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

3. Create a new Web Service using the Blueprint option (which will use your render.yaml file)
   - Alternatively, select "Deploy from GitHub repo" and configure manually

4. Configure the following environment variables in the Render dashboard:
   - `ConnectionStrings__DefaultConnection`: Your production database connection string
   - `JwtSettings__SecretKey`: Your secure JWT secret key (at least 32 characters long)

5. Click "Create Web Service"

6. Render will automatically build and deploy your API using the provided Dockerfile

7. Once deployed, your API will be available at `https://internship-api.onrender.com` (or your custom subdomain)

### Environment Variables for Render Deployment

```
ASPNETCORE_ENVIRONMENT=Production
PORT=10000
ConnectionStrings__DefaultConnection=Server=your-server.database.windows.net;Database=internDb;User Id=your-username;Password=your-password;TrustServerCertificate=True;
JwtSettings__SecretKey=your-secure-secret-key-at-least-32-chars-long
JwtSettings__Issuer=InternshipAPI
JwtSettings__Audience=InternshipAPIClients
JwtSettings__ExpiryMinutes=120
```

### Database Options for Render Free Tier

1. **Azure SQL Server**: Azure offers a basic tier with a free option (DTU-based)
   - Connection string format: `Server=your-server.database.windows.net;Database=internDb;User Id=your-username;Password=your-password;TrustServerCertificate=True;`

2. **ElephantSQL**: Free PostgreSQL hosting (requires code changes)
   - You'll need to update your code to use Npgsql instead of SQL Server
   - Connection string format: `Host=your-elephant-sql-server;Database=your-db;Username=your-username;Password=your-password;`

3. **Railway**: Offers PostgreSQL with a free tier (requires code changes)

### Free Tier Optimization Tips

1. **Cold Starts**: Render free tier spins down after 15 minutes of inactivity
   - First request after inactivity will be slower
   - Consider using a service like UptimeRobot to ping your API every 14 minutes

2. **Resource Limits**: Free tier has 512 MB RAM and 0.1 CPU
   - Optimize your API to be memory-efficient
   - Use async/await patterns for better performance

3. **Monthly Usage**: Limited to 750 hours per month
   - This is enough for continuous operation

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
