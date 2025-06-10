# API Documentation - Login and Registration Backend

This document provides comprehensive API documentation for all endpoints in the Login and Registration Backend.

## Base URL

- **Development**: `http://localhost:5000` or `https://localhost:7001`
- **API Prefix**: `/api/auth`

## Authentication

Most endpoints require JWT authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

## Endpoints Overview

| Endpoint | Method | Auth Required | Purpose |
|----------|--------|---------------|---------|
| `/api/auth/test` | GET | No | Health check |
| `/api/auth/register` | POST | No | User registration |
| `/api/auth/login` | POST | No | User login |
| `/api/auth/profile` | GET | Yes | Get user profile |
| `/api/auth/google-login` | GET | No | Google OAuth login |
| `/api/auth/oauth-success` | GET | No | OAuth callback |

---

## 📍 Health Check

### GET `/api/auth/test`

Simple health check endpoint to verify the API is running.

**Request:**
```http
GET /api/auth/test
```

**Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json

"Server is running successfully!"
```

**Use Cases:**
- API health monitoring
- Connectivity testing
- Load balancer health checks

---

## 👤 User Registration

### POST `/api/auth/register`

Creates a new user account with username, email, and password.

**Request:**
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

**Request Body Schema:**
```json
{
  "username": {
    "type": "string",
    "required": true,
    "description": "Unique username for the account"
  },
  "email": {
    "type": "string",
    "required": true,
    "format": "email",
    "description": "Unique email address"
  },
  "password": {
    "type": "string",
    "required": true,
    "minLength": 8,
    "description": "Password meeting complexity requirements"
  }
}
```

**Success Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "message": "User registered successfully"
}
```

**Error Responses:**

**400 Bad Request - User Already Exists:**
```json
{
  "message": "Username or email already exists"
}
```

**400 Bad Request - Validation Errors:**
```json
{
  "message": "Registration failed",
  "errors": [
    "Password must contain at least one uppercase letter",
    "Password must contain at least one digit"
  ]
}
```

**400 Bad Request - General Error:**
```json
{
  "message": "Registration error: <error details>"
}
```

**Password Requirements:**
- Minimum 8 characters
- At least one uppercase letter (A-Z)
- At least one lowercase letter (a-z)
- At least one digit (0-9)

**Example Request:**
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "john_doe",
    "email": "john@example.com",
    "password": "SecurePass123!"
  }'
```

---

## 🔐 User Login

### POST `/api/auth/login`

Authenticates a user and returns a JWT token for subsequent requests.

**Request:**
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "string",
  "password": "string"
}
```

**Request Body Schema:**
```json
{
  "username": {
    "type": "string",
    "required": true,
    "description": "Username for authentication"
  },
  "password": {
    "type": "string",
    "required": true,
    "description": "User password"
  }
}
```

**Success Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "user": {
    "id": "string",
    "username": "string",
    "email": "string"
  },
  "token": "string"
}
```

**Error Responses:**

**401 Unauthorized:**
```json
{
  "message": "Invalid username or password"
}
```

**400 Bad Request:**
```json
{
  "message": "<error details>"
}
```

**Example Request:**
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "john_doe",
    "password": "SecurePass123!"
  }'
```

**Example Success Response:**
```json
{
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "username": "john_doe",
    "email": "john@example.com"
  },
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Token Details:**
- **Type**: JWT (JSON Web Token)
- **Algorithm**: HMAC SHA256
- **Expiration**: 7 days from issue
- **Claims**: User ID, Username, Email, Subject, JTI

---

## 👤 Get User Profile

### GET `/api/auth/profile`

Retrieves the current user's profile information. Requires valid JWT authentication.

**Request:**
```http
GET /api/auth/profile
Authorization: Bearer <jwt-token>
```

**Success Response:**
```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "user": {
    "id": "string",
    "username": "string", 
    "email": "string"
  }
}
```

**Error Responses:**

**401 Unauthorized - Invalid Token:**
```json
{
  "message": "Invalid token"
}
```

**404 Not Found - User Not Found:**
```json
{
  "message": "User not found"
}
```

**400 Bad Request:**
```json
{
  "message": "<error details>"
}
```

**Example Request:**
```bash
curl -X GET http://localhost:5000/api/auth/profile \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

**Example Response:**
```json
{
  "user": {
    "id": "123e4567-e89b-12d3-a456-426614174000",
    "username": "john_doe",
    "email": "john@example.com"
  }
}
```

---

## 🔗 OAuth Endpoints

### GET `/api/auth/google-login`

Initiates Google OAuth authentication flow.

**Request:**
```http
GET /api/auth/google-login?returnUrl=<optional-return-url>
```

**Query Parameters:**
- `returnUrl` (optional): URL to redirect after authentication

**Response:**
- **Type**: HTTP Redirect (302)
- **Location**: Google OAuth authorization URL

**Example Request:**
```bash
curl -X GET http://localhost:5000/api/auth/google-login
# This will redirect to Google's OAuth consent screen
```

**Note**: Currently configured but requires Google OAuth client credentials to be set up.

### GET `/api/auth/oauth-success`

OAuth callback endpoint that processes the authentication result from OAuth providers.

**Request:**
```http
GET /api/auth/oauth-success
```

**Success Response:**
- **Type**: HTTP Redirect (302)
- **Location**: `http://localhost:5174/oauth-success?token=<jwt-token>`

**Error Response:**
- **Type**: HTTP Redirect (302)  
- **Location**: `http://localhost:5174/oauth-success?error=<error-message>`

**Process Flow:**
1. User clicks Google/Microsoft login
2. Redirected to OAuth provider
3. User grants permission
4. Provider redirects back to `/oauth-success`
5. Backend processes authentication
6. User redirected to frontend with token or error

---

## 🔒 Authentication & Authorization

### JWT Token Structure

**Header:**
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

**Payload:**
```json
{
  "nameid": "user-id",
  "unique_name": "username",
  "email": "user@example.com",
  "sub": "user-id",
  "jti": "unique-token-id",
  "iss": "LoginRegistrationAPI",
  "aud": "LoginRegistrationUsers",
  "exp": 1234567890,
  "iat": 1234567890
}
```

### Using JWT Tokens

**In Headers:**
```http
Authorization: Bearer <jwt-token>
```

**In JavaScript:**
```javascript
fetch('/api/auth/profile', {
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
})
```

**Token Validation:**
- Tokens are validated on each request
- Expired tokens return 401 Unauthorized
- Invalid signatures return 401 Unauthorized
- Missing tokens return 401 Unauthorized

---

## 📝 Request/Response Examples

### Complete Registration Flow

**1. Register User:**
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "alice_smith",
    "email": "alice@example.com", 
    "password": "MySecure123!"
  }'
```

**2. Login User:**
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "alice_smith",
    "password": "MySecure123!"
  }'
```

**3. Access Protected Endpoint:**
```bash
curl -X GET http://localhost:5000/api/auth/profile \
  -H "Authorization: Bearer <token-from-login-response>"
```

### Error Handling Examples

**Invalid Registration:**
```bash
# Request with weak password
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "weak"
  }'

# Response
{
  "message": "Registration failed",
  "errors": [
    "Passwords must be at least 8 characters.",
    "Passwords must have at least one uppercase ('A'-'Z').",
    "Passwords must have at least one digit ('0'-'9')."
  ]
}
```

**Invalid Login:**
```bash
# Request with wrong password
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "alice_smith",
    "password": "wrongpassword"
  }'

# Response
{
  "message": "Invalid username or password"
}
```

---

## 🛠️ Testing the API

### Using curl

**Test Server Health:**
```bash
curl http://localhost:5000/api/auth/test
```

**Register and Login Flow:**
```bash
# Register
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","email":"test@example.com","password":"TestPass123!"}'

# Login  
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"TestPass123!"}'

# Use returned token for profile
curl -X GET http://localhost:5000/api/auth/profile \
  -H "Authorization: Bearer <your-token-here>"
```

### Using Postman

1. **Import Collection**: Create requests for each endpoint
2. **Set Environment Variables**: 
   - `baseUrl`: `http://localhost:5000`
   - `token`: `{{login-response.token}}`
3. **Chain Requests**: Use login response token in subsequent requests

### Testing Checklist

- [ ] Health check endpoint responds
- [ ] User registration with valid data succeeds
- [ ] User registration with invalid data fails appropriately
- [ ] User login with correct credentials succeeds
- [ ] User login with incorrect credentials fails
- [ ] Profile endpoint requires authentication
- [ ] Profile endpoint returns correct user data
- [ ] JWT tokens expire correctly
- [ ] Error responses include helpful messages

---

## 🚨 Common Issues & Solutions

### 401 Unauthorized
**Cause**: Missing, invalid, or expired JWT token
**Solution**: Check token format, expiration, and inclusion in Authorization header

### 400 Bad Request  
**Cause**: Invalid request data, validation errors
**Solution**: Verify request body format and required fields

### CORS Errors
**Cause**: Frontend origin not allowed
**Solution**: Update CORS policy in backend configuration

### Database Errors
**Cause**: Database connection or constraint issues
**Solution**: Check database file permissions and uniqueness constraints

---

## 📊 HTTP Status Codes

| Code | Description | When Used |
|------|-------------|-----------|
| 200 | OK | Successful requests |
| 400 | Bad Request | Invalid input, validation errors |
| 401 | Unauthorized | Authentication required or failed |
| 404 | Not Found | User not found |
| 500 | Internal Server Error | Unexpected server errors |

---

This API documentation covers all current endpoints and functionality. For OAuth setup and additional features, refer to the main README.md and setup documentation.
