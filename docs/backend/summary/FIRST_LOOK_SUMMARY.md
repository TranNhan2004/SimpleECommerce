# SimpleECommerce Backend - First Look Summary

## Overview

This document provides a comprehensive summary of the current authentication implementation in the SimpleECommerceBackend project.

---

## Current Architecture

### Technology Stack

- **Framework**: ASP.NET Core (.NET 10)
- **Authentication Method**: JWT (JSON Web Tokens) with custom implementation
- **Password Hashing**: BCrypt
- **API Documentation**: Swagger/OpenAPI
- **API Versioning**: Versioned API endpoints (v1)
- **Mapping**: Mapster
- **Event Publishing**: MediatR

### Layered Architecture

```
SimpleECommerceBackend.Api/
├── Controllers
├── DTOs
├── Middleware
├── Extensions
├── Mapping
└── Program.cs

SimpleECommerceBackend.Application/
├── UseCases (Auth, Business)
├── Interfaces (Repositories, Services, Security)
└── Events

SimpleECommerceBackend.Infrastructure/
├── Security (JWT, Password Hashing)
├── Repositories
├── Services
├── Persistence (Database)
└── DependencyInjection

SimpleECommerceBackend.Domain/
├── Entities
├── Enums
├── Exceptions
├── Interfaces
└── ValueObjects
```

---

## Current Authentication Implementation

### 1. Authentication Entry Point: `Program.cs`

**Location**: `SimpleECommerceBackend.Api/Program.cs` (Lines 23-44)

**Current Implementation**:

```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Secret))
    };
});
```

**Authentication Method**: Custom JWT Bearer authentication using symmetric key signing.

---

### 2. JWT Settings Configuration

**Location**: `appsettings.json` & `appsettings.Development.json`

**Current Configuration**:

```json
"JwtSettings": {
    "Secret": "Q9c2vN4W5eYzXn7Yy8JkHq1F3rL6S0o8A2B5dZVxM4pCwTtE9uGmRFaK7DUsLh+",
    "Issuer": "SimpleECommerceBackend",
    "Audience": "SimpleECommerce.Client",
    "AccessTokenExpirationMinutes": 10,
    "RefreshTokenExpirationMinutes": 10800,
    "AccountVerificationTokenExpirationMinutes": 10,
    "PasswordResetTokenExpirationMinutes": 10
}
```

**Purpose**: Configuration for JWT token generation and validation. All tokens are generated and validated using a shared secret key.

---

### 3. JWT Generator & Token Validation

**Location**: `SimpleECommerceBackend.Infrastructure/Security/JwtGenerator.cs`

**Current Responsibilities**:

- Generate Access Token (10 min expiration)
- Generate Refresh Token (10800 min/7.5 day expiration)
- Generate Account Verification Token (10 min expiration)
- Generate Password Reset Token (10 min expiration)
- Validate JWT tokens using symmetric key validation

**Token Security**: All tokens are signed using HMAC-SHA256 with a symmetric secret key stored in configuration.

---

### 4. Authentication Controllers & Use Cases

#### **AuthController**

**Location**: `SimpleECommerceBackend.Api/Controllers/AuthController.cs`

**Current Endpoints**:
| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/api/v1/auth/register` | POST | Register new user |
| `/api/v1/auth/login` | POST | Login user |

**Implementation**: Both endpoints handle full authentication flow internally using custom JWT generation and BCrypt password hashing.

---

#### **Login Use Case**

**Location**: `SimpleECommerceBackend.Application/UseCases/Auth/Login/LoginCommandHandler.cs`

**Current Flow**:

1. Find credential by email
2. Check credential status (must be Active)
3. Verify password using BCrypt
4. Find associated user profile
5. Generate JWT access token and refresh token
6. Return user profile with tokens

**Security**: Password verification using BCrypt, JWT tokens generated with 10-minute access token and 7.5-day refresh token expiration.

---

#### **Register Use Case**

**Location**: `SimpleECommerceBackend.Application/UseCases/Auth/Register/RegisterCommandHandler.cs`

**Current Flow**:

1. Create Credential entity with email and hashed password
2. Save to database
3. Generate account verification token
4. Publish UserRegisteredEvent (triggers verification email)

**Post-Registration**: User accounts start as Inactive and must be verified via email verification token before becoming Active.

---

#### **Other Auth Use Cases**

Located in `SimpleECommerceBackend.Application/UseCases/Auth/`:

| Use Case       | Location          | Purpose                                              |
| -------------- | ----------------- | ---------------------------------------------------- |
| VerifyAccount  | `VerifyAccount/`  | Activate registered account using verification token |
| RefreshToken   | `RefreshToken/`   | Generate new access token from refresh token         |
| ChangePassword | `ChangePassword/` | Change user password with old password verification  |
| ResetPassword  | `ResetPassword/`  | Reset forgotten password using reset token           |

---

### 5. Security Infrastructure Components

#### **BCryptPasswordHasher**

**Location**: `SimpleECommerceBackend.Infrastructure/Security/BCryptPasswordHasher.cs`

**Current Responsibility**: Hash and verify passwords using BCrypt

**Configuration**: Uses work factor of 12 for BCrypt hashing algorithm.

---

#### **Credential Entity**

**Location**: `SimpleECommerceBackend.Domain/Entities/Auth/Credential.cs`

**Current Properties**:

- `Id` (Guid)
- `Email` (string)
- `PasswordHash` (string)
- `Status` (CredentialStatus: Inactive, Active, Archived)
- `Role` (enum)
- `CreatedAt`, `UpdatedAt`

**Business Logic**: Entity provides methods to Activate(), Deactivate(), and Archive() credentials. Email validation uses regex pattern validation.

---

### 6. Token Types & Claims

**Location**: `SimpleECommerceBackend.Application/Interfaces/Security/IJwtGenerator.cs`

**Current Token Types**:

```csharp
enum TokenType
{
    AccessToken = 0,
    RefreshToken = 1,
    AccountVerificationToken = 2,
    PasswordResetToken = 3
}
```

**Current Claims Structure**:

- `email` (Email)
- `jti` (JWT ID)
- `role` (Role)
- `token_type` (Custom claim)

**Usage**: Token types are used to validate that the correct token is being used for each operation (e.g., can't use refresh token for account verification).

---

### 7. Password Hashing Interface

**Location**: `SimpleECommerceBackend.Application/Interfaces/Security/IPasswordHasher.cs`

**Current Interface**:

```csharp
public interface IPasswordHasher
{
    string Hash(string plainPassword);
    bool Verify(string plainPassword, string passwordHash);
}
```

**Implementation**: BCryptPasswordHasher implements this interface with work factor 12.

---

### 8. Database Setup

**Location**: `SimpleECommerceBackend.Infrastructure/DependencyInjection.cs`

**Current Setup**:

- SQL Server database
- Entity Framework Core
- Credentials table with email, password hash, status, role
- UserProfile table with personal information

**Data Storage**: All user credentials (email, password hash, role, status) are stored locally in SQL Server. UserProfile table stores additional personal information linked to credentials.

---

### 9. Configuration & Dependency Injection

**Location**: `SimpleECommerceBackend.Infrastructure/DependencyInjection.cs`

**Current Registrations**:

```csharp
services.AddSingleton<IJwtGenerator, JwtGenerator>();
services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
```

**Architecture**: All authentication services are registered as singletons for performance, repositories as scoped for per-request lifecycle.

---

## Migration to Keycloak - Options

### Option 1: Full Keycloak Integration

- Delegate all authentication and user management to Keycloak
- Remove Credentials table and password hashing
- Keep UserProfile table for business data
- Backend validates Keycloak tokens only
- **Pros**: Simplest, most standard approach
- **Cons**: Requires significant refactoring

### Option 2: Hybrid Approach

- Use Keycloak for authentication (token issuance)
- Keep local database for user data synchronization
- Map Keycloak users to local UserProfile entries
- **Pros**: More control over user data
- **Cons**: Complexity in keeping data synchronized

### Option 3: Gradual Migration

- Support both authentication methods during transition
- Migrate users gradually to Keycloak
- **Pros**: Zero downtime migration
- **Cons**: Increased code complexity during migration period

---

## Key Components Summary

| Component                 | Location                                          | Purpose                             |
| ------------------------- | ------------------------------------------------- | ----------------------------------- |
| **Authentication Setup**  | `Program.cs`                                      | Configure JWT Bearer authentication |
| **JWT Generator**         | `Infrastructure/Security/JwtGenerator.cs`         | Generate and validate JWT tokens    |
| **Password Hasher**       | `Infrastructure/Security/BCryptPasswordHasher.cs` | Hash and verify passwords           |
| **Auth Controller**       | `Api/Controllers/AuthController.cs`               | Login and Register endpoints        |
| **Login Handler**         | `Application/UseCases/Auth/Login/`                | Business logic for login            |
| **Register Handler**      | `Application/UseCases/Auth/Register/`             | Business logic for registration     |
| **Credential Entity**     | `Domain/Entities/Auth/Credential.cs`              | User credential domain model        |
| **Credential Repository** | `Infrastructure/Repositories/Auth/`               | Data access for credentials         |
| **JWT Settings**          | `appsettings.json`                                | Configuration for JWT               |
| **Database Context**      | `Infrastructure/Persistence/AppDbContext.cs`      | EF Core database context            |

---

## Authentication Flow Summary

### Registration Flow:

1. User submits email, password, and role
2. Password is hashed using BCrypt (work factor 12)
3. Credential created with Inactive status
4. Saved to database
5. Verification email sent with JWT token
6. User clicks verification link
7. Account activated to Active status
8. UserProfile created with default values

### Login Flow:

1. User submits email and password
2. Credential retrieved from database
3. Status checked (must be Active)
4. Password verified using BCrypt
5. UserProfile retrieved
6. Access token generated (10 min expiration)
7. Refresh token generated (7.5 day expiration)
8. Tokens and user profile returned

### Token Refresh Flow:

1. Client submits refresh token
2. Token validated (must be RefreshToken type)
3. Claims extracted from token
4. New access token generated
5. New access token returned

---

## Database Schema (Authentication-Related)

### Credentials Table

- `Id` (Guid, Primary Key)
- `Email` (string, unique)
- `PasswordHash` (string)
- `Status` (enum: Inactive, Active, Archived)
- `Role` (enum: Customer, Seller, Admin, etc.)
- `CreatedAt` (DateTimeOffset)
- `UpdatedAt` (DateTimeOffset, nullable)

### UserProfiles Table

- `Id` (Guid, Primary Key)
- `CredentialId` (Guid, Foreign Key to Credentials)
- `Email` (string)
- `FirstName` (string)
- `LastName` (string)
- `NickName` (string, nullable)
- `Sex` (enum)
- `BirthDate` (DateOnly)
- `AvatarUrl` (string, nullable)
- `CreatedAt` (DateTimeOffset)
- `UpdatedAt` (DateTimeOffset, nullable)

---

## Security Features

### Password Security

- **Algorithm**: BCrypt
- **Work Factor**: 12
- **Storage**: Only password hashes stored, never plain text
- **Validation**: Regex pattern for email format

### Token Security

- **Algorithm**: HMAC-SHA256
- **Key Type**: Symmetric secret key
- **Access Token Lifetime**: 10 minutes
- **Refresh Token Lifetime**: 10,800 minutes (7.5 days)
- **Token Types**: Access, Refresh, Account Verification, Password Reset
- **Validation**: Issuer, Audience, Lifetime, Signature validation

### Status Management

- **Inactive**: New registrations, requires email verification
- **Active**: Verified accounts, can login
- **Archived**: Disabled accounts, cannot login

---

_Document Generated: 2026-03-03_
_Project: SimpleECommerce Backend_
_Authentication: Custom JWT-based Authentication_
