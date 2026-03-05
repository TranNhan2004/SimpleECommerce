# Phase 2: Backend Configuration

**Status**: ✅ Completed  
**Duration**: 0.5-1 day  
**Phase Overview**: [KEYCLOAK_IMPLEMENTATION_PLAN.md](./KEYCLOAK_IMPLEMENTATION_PLAN.md)

---

## Table of Contents

1. [Objectives](#objectives)
2. [Prerequisites](#prerequisites)
3. [Implementation Steps](#implementation-steps)
   - [Step 2.1: Update appsettings.json](#step-21-update-appsettingsjson)
   - [Step 2.2: Create Keycloak Settings Model](#step-22-create-keycloak-settings-model)
   - [Step 2.3: Install NuGet Packages](#step-23-install-nuget-packages)
4. [Verification Checklist](#verification-checklist)
5. [Next Steps](#next-steps)

---

## Objectives

- ✅ Install required Keycloak NuGet packages
- ✅ Configure Keycloak settings in appsettings.json
- ✅ Create strongly-typed settings model
- ✅ Remove old JWT configuration

---

## Prerequisites

- [x] Phase 1 completed successfully
- [x] Keycloak client secret available from Phase 1, Step 1.4
  - Client ID: `simple-e-commerce-backend`
  - Realm: `SimpleECommerce`
  - Keycloak URL: `http://localhost:8080`
- [x] .NET 10 SDK installed
- [x] Access to the backend code

---

## Implementation Steps

### Step 2.1: Update appsettings.json

#### Remove Old JWT Settings

Open `SimpleECommerceBackend.Api/appsettings.json` and `appsettings.Development.json`.

Remove the old `JwtSettings` section (if it exists).

#### Add Keycloak Configuration

Add the following `Keycloak` section to your appsettings.json:

**File**: `SimpleECommerceBackend.Api/appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_DB_CONNECTION_STRING"
  },
  "Keycloak": {
    "realm": "SimpleECommerce",
    "auth-server-url": "http://localhost:8080/",
    "ssl-required": "external",
    "resource": "simple-e-commerce-backend",
    "credentials": {
      "secret": "<YOUR_CLIENT_SECRET>"
    },
    "confidential-port": 0,
    "verify-token-audience": true,
    "token-endpoint": "http://localhost:8080/realms/SimpleECommerce/protocol/openid-connect/token",
    "userinfo-endpoint": "http://localhost:8080/realms/SimpleECommerce/protocol/openid-connect/userinfo",
    "introspection-endpoint": "http://localhost:8080/realms/SimpleECommerce/protocol/openid-connect/token/introspect",
    "admin-url": "http://localhost:8080/admin/realms/SimpleECommerce",
    "timeout-seconds": 30
  },
  "SmtpOptions": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "YOUR_SMTP_USERNAME",
    "Password": "YOUR_SMTP_PASSWORD",
    "UseSsl": false,
    "From": "YOUR_SMTP_USERNAME"
  }
}
```

**⚠️ Important**: Replace `<YOUR_CLIENT_SECRET>` with the actual client secret from Phase 1.

**💡 For Production**: Use environment variables or Azure Key Vault for secrets:

```json
"Keycloak": {
  "credentials": {
    "secret": "${KEYCLOAK_CLIENT_SECRET}"
  }
}
```

---

### Step 2.2: Create Keycloak Settings Model

Create a strongly-typed configuration model for dependency injection.

**File**: `SimpleECommerceBackend.Infrastructure/Services/Keycloak/KeycloakSettings.cs`

```csharp
namespace SimpleECommerceBackend.Infrastructure.Services.Keycloak;

public class KeycloakSettings
{
    public string Realm { get; init; } = null!;
    public string AuthServerUrl { get; init; } = null!;
    public string Resource { get; init; } = null!;
    public KeycloakCredentials Credentials { get; init; } = null!;
    public string TokenEndpoint { get; init; } = null!;
    public string UserInfoEndpoint { get; init; } = null!;
    public string IntrospectionEndpoint { get; init; } = null!;
    public string AdminUrl { get; init; } = null!;
    public bool VerifyTokenAudience { get; init; }
    public int TimeoutSeconds { get; init; }
}

public class KeycloakCredentials
{
    public string Secret { get; init; } = null!;
}
```

**File Location**: Create the `Services/Keycloak` folder if it doesn't exist in the Infrastructure project.

```bash
cd SimpleECommerceBackend.Infrastructure
mkdir -p Services/Keycloak
```

---

### Step 2.3: Install NuGet Packages

Install the required Keycloak authentication packages.

**Command**:

```bash
cd SimpleECommerceBackend.Api

# Install Keycloak authentication services
dotnet add package Keycloak.AuthServices.Authentication --version 2.5.2

# Install Keycloak authorization services
dotnet add package Keycloak.AuthServices.Authorization --version 2.5.2

# Restore all packages
dotnet restore
```

**Verify Installation**:

Check `SimpleECommerceBackend.Api.csproj` to ensure the packages are added:

```xml
<ItemGroup>
  <PackageReference Include="Keycloak.AuthServices.Authentication" Version="2.5.2" />
  <PackageReference Include="Keycloak.AuthServices.Authorization" Version="2.5.2" />
  <!-- Other packages... -->
</ItemGroup>
```

**Expected Output**:

```
info : Adding PackageReference for package 'Keycloak.AuthServices.Authentication' into project
info : Restoring packages for SimpleECommerceBackend.Api.csproj...
info : Package 'Keycloak.AuthServices.Authentication' is compatible with all the specified frameworks
info : PackageReference for package 'Keycloak.AuthServices.Authentication' version '2.5.2' added to file 'SimpleECommerceBackend.Api.csproj'
```

---

## Verification Checklist

Complete this checklist before proceeding to Phase 3:

- [x] Old JWT settings removed from appsettings.json
- [x] Keycloak configuration added to appsettings.json
- [ ] Client secret correctly set in configuration (⚠️ Replace `<YOUR_CLIENT_SECRET>` with actual secret)
- [x] KeycloakSettings.cs created in Infrastructure/Security folder
- [x] Both classes (KeycloakSettings and KeycloakCredentials) defined
- [x] Keycloak.AuthServices.Authentication package installed (v2.5.2)
- [x] Keycloak.AuthServices.Authorization package installed (v2.5.2)
- [x] `dotnet restore` completed successfully
- [x] Project still builds: `dotnet build`

**Test Build**:

```bash
cd SimpleECommerceBackend.Api
dotnet build
```

Expected: Build succeeds with no errors.

**🎉 Phase 2 Complete**: If all checkboxes are checked, you're ready for Phase 3!

---

## Configuration Notes

### Settings Explanation

| Setting                  | Purpose                                      |
| ------------------------ | -------------------------------------------- |
| `realm`                  | The Keycloak realm name created in Phase 1   |
| `auth-server-url`        | Base URL of your Keycloak server             |
| `resource`               | Client ID from Phase 1                       |
| `credentials.secret`     | Client secret from Phase 1                   |
| `token-endpoint`         | Endpoint for obtaining tokens                |
| `userinfo-endpoint`      | Endpoint for user information                |
| `introspection-endpoint` | Endpoint for token validation                |
| `admin-url`              | Admin API base URL for user management       |
| `verify-token-audience`  | Validate token audience claim                |
| `timeout-seconds`        | HTTP client timeout in seconds (default: 30) |

### Production Considerations

- **Never commit secrets to Git**: Use environment variables or secret managers
- **Enable SSL**: Change `ssl-required` to `"all"` in production
- **Use HTTPS**: Update all URLs to use `https://` in production
- **Secret Management**: Consider Azure Key Vault, AWS Secrets Manager, or similar

---

## Next Steps

After completing Phase 2, proceed to:

**[Phase 3: Authentication Service Implementation](./KEYCLOAK_IMPLEMENTATION_PHASE_3.md)**

In Phase 3, you will:

- Create IKeycloakTokenService interface
- Implement KeycloakTokenService for authentication
- Create IKeycloakAdminService interface
- Implement KeycloakAdminService for user management

---

_Phase 2 Last Updated: 2026-03-03_  
_Author: Development Team_
