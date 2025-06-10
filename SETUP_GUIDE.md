# Setup Guide - Login and Registration Backend (.NET)

This guide provides detailed setup instructions for the .NET Login and Registration Backend API.

## Prerequisites

### Required Software
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- Git (for cloning the repository)
- Code editor (Visual Studio 2022, VS Code, or JetBrains Rider)

### Optional Tools
- [Postman](https://www.postman.com/) or similar API testing tool
- [DB Browser for SQLite](https://sqlitebrowser.org/) for database inspection

## Step-by-Step Setup

### 1. Clone and Navigate to Project

```bash
git clone <repository-url>
cd Login-and-Registration-Backend-.NET-/Login-and-Registration-Backend-.NET-
```

### 2. Verify .NET Installation

```bash
dotnet --version
# Should output 9.0.x or later
```

### 3. Restore NuGet Packages

```bash
dotnet restore
```

### 4. Configure Application Settings

#### Basic Configuration (appsettings.json)
The application includes default settings that work out of the box:

- **Database**: SQLite (`auth.db`) - automatically created
- **JWT Secret**: Pre-configured for development
- **CORS**: Configured for `localhost:5174`

#### Custom Configuration (Optional)
To customize settings, modify `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "your-custom-secret-key-minimum-32-characters-long",
    "Issuer": "YourAppName",
    "Audience": "YourAppUsers"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=your-database.db;"
  }
}
```

### 5. Database Setup

The application automatically creates the SQLite database on first run. If you need to manually manage migrations:

```bash
# View migration status
dotnet ef database update --dry-run

# Apply migrations (automatic on startup)
dotnet ef database update

# Create new migration (if you modify models)
dotnet ef migrations add YourMigrationName
```

### 6. Build the Application

```bash
dotnet build
```

### 7. Run the Application

```bash
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7001
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### 8. Verify Installation

#### Test Health Endpoint
```bash
curl http://localhost:5000/api/auth/test
```

**Expected Response:**
```json
"Server is running successfully!"
```

#### Access OpenAPI Documentation (Development)
Navigate to: `https://localhost:7001/openapi/v1.json`

## Configuration Details

### Environment Variables

You can override configuration using environment variables:

```bash
# Windows (PowerShell)
$env:JWT__KEY="your-production-secret-key"
$env:CONNECTIONSTRINGS__DEFAULTCONNECTION="your-production-db-connection"
dotnet run

# Linux/macOS
export JWT__KEY="your-production-secret-key"
export CONNECTIONSTRINGS__DEFAULTCONNECTION="your-production-db-connection"
dotnet run
```

### Launch Profiles

The application includes two launch profiles in `launchSettings.json`:

- **HTTP Profile**: `http://localhost:5001`
- **HTTPS Profile**: `https://localhost:7207` and `http://localhost:5001`

To use a specific profile:
```bash
dotnet run --launch-profile http
dotnet run --launch-profile https
```

### CORS Configuration

Default CORS settings allow requests from:
- `http://localhost:5174`
- `https://localhost:5174`

To modify for your frontend:

```csharp
// In Program.cs
options.AddPolicy("AllowFrontend", policy =>
{
    policy
        .WithOrigins("http://localhost:3000", "https://localhost:3000") // Your frontend URLs
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});
```

## Database Information

### SQLite Database Location
- **File**: `auth.db` (in project root)
- **Type**: SQLite 3
- **Auto-created**: Yes, on first application start

### Database Schema
The application uses ASP.NET Core Identity, which creates these tables:
- `AspNetUsers` - User accounts
- `AspNetRoles` - User roles (if used)
- `AspNetUserClaims` - User claims
- `AspNetUserLogins` - External login providers
- `AspNetUserRoles` - User-role relationships
- `AspNetUserTokens` - User tokens
- `AspNetRoleClaims` - Role claims

### Viewing Database Content
Use DB Browser for SQLite or any SQLite client to inspect the `auth.db` file.

## Security Configuration

### JWT Settings
- **Algorithm**: HMAC SHA256
- **Expiration**: 7 days
- **Claims**: User ID, Username, Email
- **Key Length**: Minimum 32 characters required

### Password Policy
Current requirements (configurable in `Program.cs`):
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit

To modify password requirements:
```csharp
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 12; // Change minimum length
    options.Password.RequireNonAlphanumeric = true; // Require special characters
    options.User.RequireUniqueEmail = true;
})
```

## Troubleshooting

### Common Issues

#### Port Already in Use
If ports 5000 or 7001 are occupied:

1. **Change ports in Program.cs:**
   ```csharp
   builder.WebHost.UseUrls("http://localhost:5002", "https://localhost:7003");
   ```

2. **Or use different launch profile ports in launchSettings.json**

#### Database Connection Issues
- Ensure write permissions in the project directory
- Check if `auth.db` file is locked by another process
- Delete `auth.db` to recreate (will lose data)

#### JWT Token Issues
- Verify JWT key is at least 32 characters
- Check system clock for token expiration issues
- Ensure consistent JWT configuration across requests

#### CORS Errors
- Verify frontend URL in CORS policy
- Check browser developer tools for specific CORS errors
- Ensure `AllowCredentials()` matches frontend configuration

### Log Levels
Modify logging in `appsettings.json` for debugging:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Development vs Production

#### Development Mode Features
- OpenAPI/Swagger documentation
- Detailed error messages
- Development exception page

#### Production Considerations
- Change JWT secret key
- Use production database (PostgreSQL, SQL Server)
- Configure proper CORS origins
- Enable HTTPS redirection
- Set up proper logging

## Getting Help

If you encounter issues not covered in this guide:

1. Check the application logs for specific error messages
2. Verify all prerequisites are installed correctly
3. Ensure all configuration values are correct
4. Review the README.md for additional context
5. Create an issue in the repository with detailed error information

## 🔄 Next Steps

After successful setup:

1. **Test the API endpoints** using Postman or curl
2. **Integrate with your frontend** application
3. **Configure OAuth providers** (Google, Microsoft) if needed
4. **Set up production environment** with appropriate security measures
5. **Implement additional features** as needed for your application
