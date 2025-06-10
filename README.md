# Login and Registration Backend (.NET)

Authentication and user management API built with .NET 9.0, ASP.NET Core Identity, and JWT authentication. Supports traditional username/password authentication with OAuth integration prepared for Google and Microsoft (OAuth setup pending).

## Features

- User Registration & Authentication with password validation
- JWT Token Authentication for stateless authentication
- ASP.NET Core Identity for user management and security
- SQLite Database for development and testing
- OAuth Integration framework (requires provider credentials)
- CORS Support for frontend integration
- Input validation and error handling
- Password complexity enforcement

## Technology Stack

- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core with SQLite
- ASP.NET Core Identity
- JWT Bearer Authentication
- OpenAPI/Swagger (development mode)

## Project Structure

```
Login-and-Registration-Backend-.NET-/
├── Controllers/
│   └── AuthController.cs          # Authentication endpoints
├── Data/
│   └── ApplicationDbContext.cs    # Database context
├── Models/
│   ├── ApplicationUser.cs         # User entity
│   └── User.cs                    # DTOs and request models
├── Services/
│   ├── IUserService.cs           # User service interface
│   ├── UserService.cs            # User management implementation
│   ├── IJwtService.cs            # JWT service interface
│   └── JwtService.cs             # JWT token operations
├── Migrations/                    # Entity Framework migrations
├── Properties/
│   └── launchSettings.json       # Launch configuration
├── Program.cs                     # Application entry point
├── appsettings.json              # Configuration
└── *.csproj                      # Project dependencies
```

## Installation & Setup

### Prerequisites

- .NET 9.0 SDK
- Code editor (Visual Studio, VS Code, or Rider)

### Quick Start

1. Clone the repository
2. Restore dependencies: `dotnet restore`
3. Run the application: `dotnet run`
4. Access the API at `http://localhost:5000` or `https://localhost:7001`

The database will be automatically created on first run.

## Configuration

### JWT Settings (appsettings.json)

```json
"Jwt": {
  "Key": "your-secret-key-minimum-32-characters",
  "Issuer": "LoginRegistrationAPI",
  "Audience": "LoginRegistrationUsers"
}
```

### OAuth Configuration (Placeholder)

```json
"Authentication": {
  "Google": {
    "ClientId": "your-google-client-id",
    "ClientSecret": "your-google-client-secret"
  },
  "Microsoft": {
    "ClientId": "your-microsoft-client-id",
    "ClientSecret": "your-microsoft-client-secret"
  }
}
```

### Database Connection

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=auth.db;"
}
```

CORS is configured for `localhost:5174` (frontend).

## API Endpoints

| Method | Endpoint | Description | Authentication |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | None |
| POST | `/api/auth/login` | User login | None |
| GET | `/api/auth/profile` | Get user profile | JWT Required |
| GET | `/api/auth/test` | Health check | None |
| GET | `/api/auth/google-login` | Google OAuth login | None |
| GET | `/api/auth/oauth-success` | OAuth callback | None |

### Request/Response Examples

#### User Registration
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

#### User Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "john_doe",
  "password": "SecurePass123!"
}
```

Response:
```json
{
  "user": {
    "id": "user-guid",
    "username": "john_doe",
    "email": "john@example.com"
  },
  "token": "jwt-token-string"
}
```

#### Get Profile (Authenticated)
```http
GET /api/auth/profile
Authorization: Bearer <jwt-token>
```

## Security Features

### Password Requirements
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter  
- At least one digit
- Unique email addresses required

### JWT Configuration
- 7-day token expiration
- HMAC SHA256 signature
- Includes user ID, username, and email claims

### Database Security
- Unique constraints on username and email
- Hashed passwords using ASP.NET Core Identity
- SQL injection protection through Entity Framework

## Services Architecture

### UserService
Handles user operations:
- User registration and validation
- Password verification
- User lookup operations
- JWT token generation
- Password hashing and verification

### JwtService  
Manages JWT tokens:
- Token generation with user claims
- Token validation and parsing
- Security key management

### ApplicationDbContext
Entity Framework context with ASP.NET Core Identity integration.

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.AspNetCore.OpenApi | 9.0.5 | API documentation |
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 9.0.0 | User management |
| Microsoft.EntityFrameworkCore.Sqlite | 9.0.0 | Database provider |
| Microsoft.AspNetCore.Authentication.JwtBearer | 9.0.0 | JWT authentication |
| Microsoft.AspNetCore.Authentication.Google | 9.0.0 | Google OAuth |
| Microsoft.AspNetCore.Authentication.MicrosoftAccount | 9.0.0 | Microsoft OAuth |
| System.IdentityModel.Tokens.Jwt | 8.0.1 | JWT token handling |

## Known Issues & TODO

### OAuth Integration
- Google and Microsoft OAuth configured but require client credentials
- OAuth endpoints implemented but need provider setup
- Callback URLs need registration with OAuth providers

### Future Enhancements
- Email verification for registration
- Password reset functionality
- Two-factor authentication
- User roles and permissions
- Account lockout policies
- Refresh token implementation
- Production database migration

## Development

### Running in Development
```bash
dotnet run --environment Development
```

### Database Migrations
```bash
# Add migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## License

MIT License - see LICENSE file for details.
