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
| `/api/v1/auth/refresh` | POST | Refresh access token |

**Target Implementation**: The frontend should authenticate directly with Keycloak using the browser authorization-code flow with PKCE. The backend should not broker interactive login or refresh; it should accept Keycloak access tokens and keep only business-profile synchronization where needed.

---

#### **Login Use Case**

**Location**: `SimpleECommerceBackend.Application/UseCases/Auth/Login/LoginCommandHandler.cs`

**Models**:

- Command: `SimpleECommerceBackend.Application.Models.Auth.Login.LoginCommand`
- Result: `SimpleECommerceBackend.Application.Models.Auth.Login.LoginResult`

**Target Flow**:

1. The SPA redirects the browser to Keycloak
2. Keycloak authenticates the user and returns tokens to the SPA
3. The SPA calls the backend with the access token in `Authorization: Bearer ...`
4. The backend validates the token and loads or creates the local `UserProfile` when needed
5. User identity is mapped by the Keycloak `sub` claim

**Security**: Authentication handled by Keycloak OAuth2/OIDC. Tokens are JWT format with configurable expiration times.

---

#### **Register Use Case**

**Location**: `SimpleECommerceBackend.Application/UseCases/Auth/Register/RegisterCommandHandler.cs`

**Models**:

- Command: `SimpleECommerceBackend.Application.Models.Auth.Register.RegisterCommand`
- Result: `SimpleECommerceBackend.Application.Models.Auth.Register.RegisterResult`

**Target Flow**:

1. Keycloak owns registration, password reset, email verification, and account management
2. The backend may still create or update a local `UserProfile` after the user authenticates, but it should not own the interactive registration experience
3. If the project keeps an onboarding endpoint, it should only bootstrap business data after a valid Keycloak session exists

**Post-Registration**: User accounts are immediately active in Keycloak. Email verification can be configured in Keycloak realm settings.

---

#### **Refresh Token Use Case**

**Location**: `SimpleECommerceBackend.Application/UseCases/Auth/RefreshToken/RefreshTokenCommandHandler.cs`

**Models**:

- Command: `SimpleECommerceBackend.Application.Models.Auth.RefreshToken.RefreshTokenCommand`
- Result: `SimpleECommerceBackend.Application.Models.Auth.RefreshToken.RefreshTokenResult`

**Target Flow**:

1. The SPA handles token renewal through Keycloak
2. The backend does not proxy refresh tokens for browser users
3. If a server-side client needs refresh or service-account access, that is a separate machine-to-machine concern

**Security**: Stateless token refresh handled entirely by Keycloak.

---

#### **Other Auth Use Cases**

Deprecated use cases from custom authentication system:

| Use Case       | Status     | Notes                                                  |
| -------------- | ---------- | ------------------------------------------------------ |
| VerifyAccount  | Deprecated | Handled by Keycloak email verification                 |
| ChangePassword | Deprecated | Users can change password via Keycloak account console |
| ResetPassword  | Deprecated | Handled by Keycloak forgot password flow               |

---

### 5. Security Infrastructure Components

#### **Keycloak Integration Services**

**Location**: `SimpleECommerceBackend.Infrastructure/Services/Keycloak/`

**Services**:

- **KeycloakTokenService**: Legacy helper for backend-mediated auth flows; not part of the preferred SPA login path
- **KeycloakAdminService**: User management operations for backend provisioning and admin tasks

**Configuration**: Configured via `appsettings.json` with Keycloak server URL, realm, and client credentials.

---

#### **UserProfile Entity**

**Location**: `SimpleECommerceBackend.Domain/Entities.Business/UserProfile.cs`

**Current Properties**:

- `Id` (Guid) - Maps to Keycloak user UUID
- `Email` (string)
- `FirstName`, `LastName` (string)
- `NickName` (string, nullable)
- `Sex` (enum)
- `BirthDate` (DateOnly)
- `AvatarUrl` (string, nullable)
- `CreatedAt`, `UpdatedAt`

**Business Logic**: Entity synchronized with Keycloak users. ID field stores Keycloak user UUID for mapping between systems.

---

### 6. Authentication Architecture

**Deprecated System**:

- Custom JWT generation via `IJwtGenerator`
- BCrypt password hashing via `IPasswordHasher`
- Credential entity for storing passwords
- Backend-owned login and refresh flow

**Target System**:

- Keycloak handles interactive authentication in the browser
- The SPA receives tokens directly from Keycloak
- The backend validates Keycloak bearer tokens and uses the `sub` claim to map the local profile

**Current System** (Keycloak):

- OAuth2/OIDC standard protocol
- Keycloak token service for token operations
- Keycloak admin service for user management
- Role-based authorization with Keycloak roles
- No password storage in application database

**Token Flow**:

1. Client sends credentials to `/api/v1/auth/login`
2. Backend proxies request to Keycloak token endpoint
3. Keycloak validates credentials and returns JWT access token + refresh token
4. Backend syncs local UserProfile with Keycloak user data
5. Returns tokens and user profile to client
6. Client uses access token in Authorization header for protected endpoints
7. On token expiration, client uses refresh token at `/api/v1/auth/refresh`

**Token Claims** (managed by Keycloak):

- `sub` (Subject - Keycloak user UUID)
- `email` (User email)
- `given_name` (First name)
- `family_name` (Last name)
- `realm_access.roles` (Array of realm roles)
- `azp` (Authorized party - client ID)
- `exp` (Expiration time)
- `iat` (Issued at time)
- Additional custom claims can be configured in Keycloak

**Authorization Policies**:

Three policies defined in `Program.cs`:

- `CustomerPolicy` - Requires "customer" role
- `SellerPolicy` - Requires "seller" role
- `AdminPolicy` - Requires "admin" role

---

### 7. Application Models Structure

**Location**: `SimpleECommerceBackend.Application/Models/Auth/`

**Organization**: Commands and Results are organized in separate files by feature

```
Models/Auth/
├── Register/
│   ├── RegisterCommand.cs
│   └── RegisterResult.cs
├── Login/
│   ├── LoginCommand.cs
│   └── LoginResult.cs
└── RefreshToken/
    ├── RefreshTokenCommand.cs
    └── RefreshTokenResult.cs
```

**Benefits**:

- Clear separation of concerns
- Easy to locate and modify models
- Consistent with other model organization (Keycloak, Users)
- Supports MediatR CQRS pattern

---

### 8. Database Setup

**Location**: `SimpleECommerceBackend.Infrastructure/DependencyInjection.cs`

**Current Setup**:

- SQL Server database
- Entity Framework Core
- UserProfile table with personal information (linked to Keycloak user UUID)

**Data Storage**:

- **Authentication data** (credentials, passwords, roles) stored in Keycloak
- **User profile data** (personal info, preferences) stored locally in SQL Server
- **Mapping**: UserProfile.Id = Keycloak user UUID

**Removed Tables** (deprecated):

- Credentials table (moved to Keycloak)
- Token tables (Keycloak manages tokens)

---

### 9. Configuration & Dependency Injection

**Location**: `SimpleECommerceBackend.Infrastructure/DependencyInjection.cs`

**Current Registrations**:

```csharp
// Keycloak Services
services.AddScoped<IKeycloakTokenService, KeycloakTokenService>();
services.AddScoped<IKeycloakAdminService, KeycloakAdminService>();
services.AddHttpClient<IKeycloakTokenService, KeycloakTokenService>();
services.AddHttpClient<IKeycloakAdminService, KeycloakAdminService>();

// Keycloak Settings
services.Configure<KeycloakSettings>(configuration.GetSection("Keycloak"));
```

**API Layer** (`Program.cs`):

```csharp
// Keycloak Authentication
builder.Services
    .AddKeycloakWebApiAuthentication(builder.Configuration)
    .AddAuthorization(options =>
    {
        options.AddPolicy("CustomerPolicy", policy =>
            policy.RequireRole("customer"));
        options.AddPolicy("SellerPolicy", policy =>
            policy.RequireRole("seller"));
        options.AddPolicy("AdminPolicy", policy =>
            policy.RequireRole("admin"));
    });
```

**Architecture**: Keycloak services registered as scoped for per-request lifecycle with HttpClient integration for external API calls.

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

| Component                       | Location                                                               | Purpose                                   |
| ------------------------------- | ---------------------------------------------------------------------- | ----------------------------------------- |
| **Authentication Setup**        | `Api/Program.cs`                                                       | Configure Keycloak OAuth2 authentication  |
| **Keycloak Token Service**      | `Infrastructure/Services/Keycloak/KeycloakTokenService.cs`             | Token operations with Keycloak            |
| **Keycloak Admin Service**      | `Infrastructure/Services/Keycloak/KeycloakAdminService.cs`             | User management via Keycloak Admin API    |
| **Auth Controller**             | `Api/Controllers/AuthController.cs`                                    | Auth endpoints (register, login, refresh) |
| **Login Handler**               | `Application/UseCases/Auth/Login/LoginCommandHandler.cs`               | Business logic for login                  |
| **Login Command/Result**        | `Application/Models/Auth/Login/`                                       | Login request/response models             |
| **Register Handler**            | `Application/UseCases/Auth/Register/RegisterCommandHandler.cs`         | Business logic for registration           |
| **Register Command/Result**     | `Application/Models/Auth/Register/`                                    | Register request/response models          |
| **RefreshToken Handler**        | `Application/UseCases/Auth/RefreshToken/RefreshTokenCommandHandler.cs` | Token refresh logic                       |
| **RefreshToken Command/Result** | `Application/Models/Auth/RefreshToken/`                                | Refresh token request/response models     |
| **UserProfile Entity**          | `Domain/Entities/Business/UserProfile.cs`                              | User profile domain model                 |
| **UserProfile Repository**      | `Infrastructure/Repositories/Business/`                                | Data access for user profiles             |
| **Keycloak Settings**           | `appsettings.json`                                                     | Keycloak configuration                    |
| **Database Context**            | `Infrastructure/Persistence/AppDbContext.cs`                           | EF Core database context                  |

**Deprecated/Removed Components**:

- ~~JWT Generator~~ (Replaced by Keycloak)
- ~~Password Hasher~~ (Replaced by Keycloak)
- ~~Credential Entity~~ (Moved to Keycloak)
- ~~Credential Repository~~ (No longer needed)

---

## Authentication Flow Summary

### Registration Flow:

1. User submits email, password, role, first name, last name
2. Backend checks if user exists in Keycloak
3. User created in Keycloak with hashed password
4. Role assigned to user in Keycloak
5. Local UserProfile created with Keycloak user UUID
6. Success response returned

**Note**: Email verification can be configured in Keycloak realm settings.

### Login Flow:

1. User submits email and password
2. Backend sends authentication request to Keycloak
3. Keycloak validates credentials
4. Keycloak returns access token and refresh token
5. Backend retrieves user info from Keycloak token
6. Backend finds or creates local UserProfile
7. Tokens and user profile returned to client

### Token Refresh Flow:

1. Client submits refresh token
2. Backend forwards request to Keycloak
3. Keycloak validates refresh token
4. Keycloak returns new access token and refresh token
5. New tokens returned to client

**Token Management**: All tokens are JWTs issued and managed by Keycloak. Backend validates tokens using Keycloak's public key.

---

## Database Schema (Authentication-Related)

### UserProfiles Table (Current)

- `Id` (Guid, Primary Key) - **Maps to Keycloak user UUID**
- `Email` (string)
- `FirstName` (string)
- `LastName` (string)
- `NickName` (string, nullable)
- `Sex` (enum)
- `BirthDate` (DateOnly)
- `AvatarUrl` (string, nullable)
- `CreatedAt` (DateTimeOffset)
- `UpdatedAt` (DateTimeOffset, nullable)

**Key Change**: UserProfile.Id now stores the Keycloak user UUID instead of a separate credential FK.

### Credentials Table (Deprecated/Removed)

This table has been removed as all authentication data is now managed by Keycloak:

- ~~Email~~ (moved to Keycloak)
- ~~PasswordHash~~ (managed by Keycloak)
- ~~Status~~ (managed by Keycloak)
- ~~Role~~ (managed by Keycloak realm roles)

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
