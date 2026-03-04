# Phase 8: Deployment & Migration

**Status**: ⬜ Not Started  
**Duration**: 2-3 days  
**Phase Overview**: [KEYCLOAK_IMPLEMENTATION_PLAN.md](./KEYCLOAK_IMPLEMENTATION_PLAN.md)

---

## Table of Contents

1. [Objectives](#objectives)
2. [Prerequisites](#prerequisites)
3. [Implementation Steps](#implementation-steps)
   - [Step 8.1: Production Keycloak Setup](#step-81-production-keycloak-setup)
   - [Step 8.2: Environment Configuration](#step-82-environment-configuration)
   - [Step 8.3: User Migration Strategy](#step-83-user-migration-strategy)
   - [Step 8.4: Backend Deployment](#step-84-backend-deployment)
   - [Step 8.5: Frontend Integration](#step-85-frontend-integration)
   - [Step 8.6: Post-Deployment Validation](#step-86-post-deployment-validation)
4. [Verification Checklist](#verification-checklist)
5. [Rollback Plan](#rollback-plan)
6. [Troubleshooting](#troubleshooting)
7. [Project Completion](#project-completion)

---

## Objectives

- ✅ Set up production Keycloak instance
- ✅ Configure production environment variables
- ✅ Migrate existing users to Keycloak (if applicable)
- ✅ Deploy backend with Keycloak integration
- ✅ Update frontend authentication flow
- ✅ Validate production deployment
- ✅ Configure monitoring and logging
- ✅ Complete documentation

---

## Prerequisites

- [ ] Phase 1-7 completed successfully
- [ ] All tests passing
- [ ] Production environment prepared
- [ ] Keycloak hosting solution selected
- [ ] SSL certificates obtained
- [ ] Database backup completed
- [ ] Deployment pipeline ready

---

## Implementation Steps

### Step 8.1: Production Keycloak Setup

#### 8.1.1: Choose Keycloak Hosting Option

**Option 1: Self-Hosted on Kubernetes**

Advantages:

- Full control over configuration
- Cost-effective for high usage
- Scalable

**Option 2: Self-Hosted on Docker/VM**

Advantages:

- Simpler setup than Kubernetes
- Good for smaller deployments

**Option 3: Managed Keycloak Service**

Advantages:

- No infrastructure management
- Built-in high availability
- Professional support

Options include:

- Red Hat Single Sign-On (enterprise)
- Cloud IAM providers with OIDC support

#### 8.1.2: Setup Production Keycloak (Docker Example)

**File**: `docker-compose.keycloak-production.yml`

```yaml
version: "3.8"

services:
  keycloak-db:
    image: postgres:16-alpine
    container_name: keycloak-postgres-prod
    environment:
      POSTGRES_DB: keycloak_prod
      POSTGRES_USER: ${KEYCLOAK_DB_USER}
      POSTGRES_PASSWORD: ${KEYCLOAK_DB_PASSWORD}
    volumes:
      - keycloak_prod_data:/var/lib/postgresql/data
    networks:
      - keycloak-network
    restart: unless-stopped
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U ${KEYCLOAK_DB_USER}"]
      interval: 10s
      timeout: 5s
      retries: 5

  keycloak:
    image: quay.io/keycloak/keycloak:24.0
    container_name: keycloak-prod
    environment:
      KC_DB: postgres
      KC_DB_URL: jdbc:postgresql://keycloak-db:5432/keycloak_prod
      KC_DB_USERNAME: ${KEYCLOAK_DB_USER}
      KC_DB_PASSWORD: ${KEYCLOAK_DB_PASSWORD}
      KEYCLOAK_ADMIN: ${KEYCLOAK_ADMIN_USER}
      KEYCLOAK_ADMIN_PASSWORD: ${KEYCLOAK_ADMIN_PASSWORD}
      KC_HOSTNAME: ${KEYCLOAK_HOSTNAME}
      KC_HOSTNAME_STRICT: true
      KC_HOSTNAME_STRICT_HTTPS: true
      KC_HTTP_ENABLED: false
      KC_HTTPS_ENABLED: true
      KC_HTTPS_CERTIFICATE_FILE: /opt/keycloak/conf/tls.crt
      KC_HTTPS_CERTIFICATE_KEY_FILE: /opt/keycloak/conf/tls.key
      KC_PROXY: edge
      KC_LOG_LEVEL: INFO
    command: start
    ports:
      - "8443:8443"
    depends_on:
      keycloak-db:
        condition: service_healthy
    networks:
      - keycloak-network
    restart: unless-stopped
    volumes:
      - ./certs:/opt/keycloak/conf
    healthcheck:
      test:
        ["CMD-SHELL", "curl -f https://localhost:8443/health/ready || exit 1"]
      interval: 30s
      timeout: 10s
      retries: 3

volumes:
  keycloak_prod_data:

networks:
  keycloak-network:
    driver: bridge
```

#### 8.1.3: Environment Variables for Production

**File**: `.env.production` (DO NOT commit to git)

```bash
# Keycloak Database
KEYCLOAK_DB_USER=keycloak_admin
KEYCLOAK_DB_PASSWORD=<STRONG_PASSWORD>

# Keycloak Admin
KEYCLOAK_ADMIN_USER=admin
KEYCLOAK_ADMIN_PASSWORD=<STRONG_ADMIN_PASSWORD>

# Keycloak Hostname
KEYCLOAK_HOSTNAME=auth.yourdomain.com

# Backend Configuration
KEYCLOAK_CLIENT_SECRET=<CLIENT_SECRET_FROM_KEYCLOAK>
```

#### 8.1.4: Configure SSL/TLS

**Generate SSL Certificate (Let's Encrypt example):**

```bash
# Install certbot
sudo apt-get install certbot

# Obtain certificate
sudo certbot certonly --standalone -d auth.yourdomain.com

# Copy certificates
sudo cp /etc/letsencrypt/live/auth.yourdomain.com/fullchain.pem ./certs/tls.crt
sudo cp /etc/letsencrypt/live/auth.yourdomain.com/privkey.pem ./certs/tls.key
```

#### 8.1.5: Start Production Keycloak

```bash
# Load environment variables
export $(cat .env.production | xargs)

# Start Keycloak
docker-compose -f docker-compose.keycloak-production.yml up -d

# Check logs
docker logs -f keycloak-prod

# Verify health
curl https://auth.yourdomain.com/health/ready
```

#### 8.1.6: Configure Production Realm

Follow the same steps as Phase 1, but with production settings:

1. Create `SimpleECommerce` realm
2. Configure email settings (SMTP for production)
3. Enable email verification
4. Create `simple-ecommerce-backend` client
5. Generate and save client secret
6. Create roles: customer, seller, admin
7. Configure token lifespans (shorter for production)
8. Enable security features (brute force detection, etc.)

---

### Step 8.2: Environment Configuration

#### 8.2.1: Update Production appsettings

**File**: `SimpleECommerceBackend.Api/appsettings.Production.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=${DB_SERVER};Database=${DB_NAME};User Id=${DB_USER};Password=${DB_PASSWORD};TrustServerCertificate=True;MultipleActiveResultSets=true"
  },
  "Keycloak": {
    "realm": "SimpleECommerce",
    "auth-server-url": "https://auth.yourdomain.com/",
    "ssl-required": "all",
    "resource": "simple-ecommerce-backend",
    "verify-token-audience": true,
    "credentials": {
      "secret": "${KEYCLOAK_CLIENT_SECRET}"
    },
    "confidential-port": 0,
    "token-endpoint": "https://auth.yourdomain.com/realms/SimpleECommerce/protocol/openid-connect/token",
    "userinfo-endpoint": "https://auth.yourdomain.com/realms/SimpleECommerce/protocol/openid-connect/userinfo",
    "introspection-endpoint": "https://auth.yourdomain.com/realms/SimpleECommerce/protocol/openid-connect/token/introspect",
    "admin-url": "https://auth.yourdomain.com/admin/realms/SimpleECommerce",
    "admin-username": "${KEYCLOAK_ADMIN_USER}",
    "admin-password": "${KEYCLOAK_ADMIN_PASSWORD}"
  },
  "AllowedHosts": "*",
  "Cors": {
    "Origins": ["https://yourdomain.com", "https://www.yourdomain.com"]
  }
}
```

#### 8.2.2: Update CORS Configuration

**File**: `SimpleECommerceBackend.Api/Program.cs`

Add CORS configuration for production:

```csharp
// Add before builder.Build()
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", builder =>
    {
        builder
            .WithOrigins(
                "https://yourdomain.com",
                "https://www.yourdomain.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add after app instantiation
if (app.Environment.IsProduction())
{
    app.UseCors("ProductionPolicy");
}
```

#### 8.2.3: Update Authentication Configuration for Production

**File**: `SimpleECommerceBackend.Api/Program.cs`

Ensure `RequireHttpsMetadata` is enabled in production:

```csharp
builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration, options =>
{
    options.Audience = builder.Configuration["Keycloak:resource"];
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment(); // true in production
});
```

---

### Step 8.3: User Migration Strategy

#### 8.3.1: Option 1 - Bulk Migration Script

**For projects with existing users in Credentials table:**

**File**: `SimpleECommerceBackend.Api/Services/UserMigrationService.cs`

```csharp
using SimpleECommerceBackend.Application.Interfaces.Repositories.Auth;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Security;

namespace SimpleECommerceBackend.Api.Services;

public class UserMigrationService
{
    private readonly ICredentialRepository _credentialRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IKeycloakAdminService _keycloakAdminService;
    private readonly ILogger<UserMigrationService> _logger;

    public UserMigrationService(
        ICredentialRepository credentialRepository,
        IUserProfileRepository userProfileRepository,
        IKeycloakAdminService keycloakAdminService,
        ILogger<UserMigrationService> logger)
    {
        _credentialRepository = credentialRepository;
        _userProfileRepository = userProfileRepository;
        _keycloakAdminService = keycloakAdminService;
        _logger = logger;
    }

    public async Task<MigrationResult> MigrateAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var result = new MigrationResult();
        var existingCredentials = await _credentialRepository.GetAllAsync(cancellationToken);

        _logger.LogInformation($"Starting migration of {existingCredentials.Count} users");

        foreach (var credential in existingCredentials)
        {
            try
            {
                // Check if user already exists in Keycloak
                var existsInKeycloak = await _keycloakAdminService.UserExistsAsync(
                    credential.Email,
                    cancellationToken);

                if (existsInKeycloak)
                {
                    _logger.LogWarning($"User {credential.Email} already exists in Keycloak, skipping");
                    result.SkippedCount++;
                    continue;
                }

                // Get user profile
                var userProfile = await _userProfileRepository.FindByCredentialIdAsync(credential.Id);
                if (userProfile == null)
                {
                    _logger.LogError($"No user profile found for credential {credential.Email}");
                    result.FailedCount++;
                    result.FailedEmails.Add(credential.Email);
                    continue;
                }

                // Generate temporary password
                var tempPassword = GenerateTemporaryPassword();

                // Create user in Keycloak
                var keycloakUser = await _keycloakAdminService.CreateUserAsync(new CreateKeycloakUserRequest
                {
                    Email = credential.Email,
                    Password = tempPassword,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    Role = credential.Role.ToString().ToLower()
                }, cancellationToken);

                // Send password reset email via Keycloak
                await _keycloakAdminService.SendVerificationEmailAsync(
                    keycloakUser.KeycloakUserId,
                    cancellationToken);

                // Update UserProfile with Keycloak ID
                // This depends on your entity structure
                // You may need to update the Id or add a new KeycloakUserId field

                _logger.LogInformation($"Successfully migrated user: {credential.Email}");
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to migrate user {credential.Email}");
                result.FailedCount++;
                result.FailedEmails.Add(credential.Email);
            }
        }

        _logger.LogInformation($"Migration completed: {result.SuccessCount} successful, {result.FailedCount} failed, {result.SkippedCount} skipped");
        return result;
    }

    private string GenerateTemporaryPassword()
    {
        // Generate a secure random password
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 16)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

public class MigrationResult
{
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public int SkippedCount { get; set; }
    public List<string> FailedEmails { get; set; } = new();
}
```

**Create Migration Endpoint:**

**File**: `SimpleECommerceBackend.Api/Controllers/MigrationController.cs`

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleECommerceBackend.Api.Services;

namespace SimpleECommerceBackend.Api.Controllers;

[Route("api/v{version:apiVersion}/migration")]
[ApiVersion("1.0")]
[ApiController]
[Authorize(Policy = "RequireAdminRole")]
public class MigrationController : ControllerBase
{
    private readonly UserMigrationService _migrationService;
    private readonly ILogger<MigrationController> _logger;

    public MigrationController(
        UserMigrationService migrationService,
        ILogger<MigrationController> logger)
    {
        _migrationService = migrationService;
        _logger = logger;
    }

    [HttpPost("migrate-users")]
    public async Task<IActionResult> MigrateUsers()
    {
        _logger.LogWarning("User migration initiated");

        var result = await _migrationService.MigrateAllUsersAsync();

        return Ok(new
        {
            message = "Migration completed",
            successCount = result.SuccessCount,
            failedCount = result.FailedCount,
            skippedCount = result.SkippedCount,
            failedEmails = result.FailedEmails
        });
    }
}
```

#### 8.3.2: Option 2 - Lazy Migration on Login

Keep both authentication methods temporarily and migrate users as they log in:

```csharp
public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
{
    // Try Keycloak first
    try
    {
        return await LoginWithKeycloakAsync(request, cancellationToken);
    }
    catch (UnauthorizedAccessException)
    {
        // Fall back to legacy authentication
        var credential = await _credentialRepository.FindByEmailAsync(request.Email);
        if (credential != null && _passwordHasher.VerifyPassword(request.Password, credential.PasswordHash))
        {
            // Migrate user to Keycloak
            await MigrateLegacyUserAsync(credential, request.Password, cancellationToken);

            // Now authenticate with Keycloak
            return await LoginWithKeycloakAsync(request, cancellationToken);
        }

        throw new UnauthorizedAccessException("Invalid credentials");
    }
}
```

#### 8.3.3: Run Migration

**Execute migration:**

```bash
# Using migration endpoint (requires admin token)
curl -X POST https://api.yourdomain.com/api/v1/migration/migrate-users \
  -H "Authorization: Bearer ADMIN_TOKEN"

# Or run as background job
# Add job to your deployment scripts
```

---

### Step 8.4: Backend Deployment

#### 8.4.1: Build Docker Image

**File**: `SimpleECommerceBackend/Dockerfile`

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SimpleECommerceBackend.Api/SimpleECommerceBackend.Api.csproj", "SimpleECommerceBackend.Api/"]
COPY ["SimpleECommerceBackend.Application/SimpleECommerceBackend.Application.csproj", "SimpleECommerceBackend.Application/"]
COPY ["SimpleECommerceBackend.Domain/SimpleECommerceBackend.Domain.csproj", "SimpleECommerceBackend.Domain/"]
COPY ["SimpleECommerceBackend.Infrastructure/SimpleECommerceBackend.Infrastructure.csproj", "SimpleECommerceBackend.Infrastructure/"]
RUN dotnet restore "SimpleECommerceBackend.Api/SimpleECommerceBackend.Api.csproj"
COPY . .
WORKDIR "/src/SimpleECommerceBackend.Api"
RUN dotnet build "SimpleECommerceBackend.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleECommerceBackend.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleECommerceBackend.Api.dll"]
```

**Build and push:**

```bash
# Build
docker build -t yourdockerhub/simple-ecommerce-backend:keycloak .

# Push
docker push yourdockerhub/simple-ecommerce-backend:keycloak
```

#### 8.4.2: Deploy to Production

**Using Docker Compose:**

```yaml
version: "3.8"

services:
  backend:
    image: yourdockerhub/simple-ecommerce-backend:keycloak
    container_name: backend-prod
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      DB_SERVER: ${DB_SERVER}
      DB_NAME: ${DB_NAME}
      DB_USER: ${DB_USER}
      DB_PASSWORD: ${DB_PASSWORD}
      KEYCLOAK_CLIENT_SECRET: ${KEYCLOAK_CLIENT_SECRET}
      KEYCLOAK_ADMIN_USER: ${KEYCLOAK_ADMIN_USER}
      KEYCLOAK_ADMIN_PASSWORD: ${KEYCLOAK_ADMIN_PASSWORD}
    ports:
      - "5000:80"
      - "5001:443"
    networks:
      - app-network
    restart: unless-stopped

networks:
  app-network:
    external: true
```

**Deploy:**

```bash
# Load environment variables
export $(cat .env.production | xargs)

# Deploy
docker-compose -f docker-compose.production.yml up -d

# Check logs
docker logs -f backend-prod
```

#### 8.4.3: Run Database Migrations

```bash
# Connect to backend container
docker exec -it backend-prod bash

# Run migrations
dotnet ef database update

# Or run migrations before deployment
dotnet ef migrations script --idempotent --output migration.sql
# Apply migration.sql to production database
```

---

### Step 8.5: Frontend Integration

#### 8.5.1: Update Angular Authentication Service

**File**: `simple-e-commerce-frontend/src/app/services/auth.service.ts`

```typescript
import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Observable, BehaviorSubject } from "rxjs";
import { tap, catchError } from "rxjs/operators";
import { environment } from "../../environments/environment";

export interface RegisterRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  role: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface RefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/api/v1/auth`;
  private currentUserSubject = new BehaviorSubject<LoginResponse | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
    // Load user from localStorage on init
    const storedUser = localStorage.getItem("currentUser");
    if (storedUser) {
      this.currentUserSubject.next(JSON.parse(storedUser));
    }
  }

  register(request: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, request);
  }

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${this.apiUrl}/login`, {
        email,
        password,
      })
      .pipe(
        tap((response) => {
          // Store tokens
          localStorage.setItem("accessToken", response.accessToken);
          localStorage.setItem("refreshToken", response.refreshToken);
          localStorage.setItem("currentUser", JSON.stringify(response));
          this.currentUserSubject.next(response);
        }),
      );
  }

  refreshToken(): Observable<RefreshTokenResponse> {
    const refreshToken = localStorage.getItem("refreshToken");
    if (!refreshToken) {
      throw new Error("No refresh token available");
    }

    return this.http
      .post<RefreshTokenResponse>(`${this.apiUrl}/refresh`, {
        refreshToken,
      })
      .pipe(
        tap((response) => {
          localStorage.setItem("accessToken", response.accessToken);
          localStorage.setItem("refreshToken", response.refreshToken);
        }),
      );
  }

  logout(): void {
    localStorage.removeItem("accessToken");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("currentUser");
    this.currentUserSubject.next(null);
  }

  getAccessToken(): string | null {
    return localStorage.getItem("accessToken");
  }

  isAuthenticated(): boolean {
    return !!this.getAccessToken();
  }

  getCurrentUser(): LoginResponse | null {
    return this.currentUserSubject.value;
  }
}
```

#### 8.5.2: Create HTTP Interceptor for Token Refresh

**File**: `simple-e-commerce-frontend/src/app/interceptors/auth.interceptor.ts`

```typescript
import { Injectable } from "@angular/core";
import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse,
} from "@angular/common/http";
import { Observable, throwError, BehaviorSubject } from "rxjs";
import { catchError, filter, take, switchMap } from "rxjs/operators";
import { AuthService } from "../services/auth.service";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(
    null,
  );

  constructor(private authService: AuthService) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    const accessToken = this.authService.getAccessToken();

    if (accessToken) {
      request = this.addToken(request, accessToken);
    }

    return next.handle(request).pipe(
      catchError((error) => {
        if (error instanceof HttpErrorResponse && error.status === 401) {
          return this.handle401Error(request, next);
        }
        return throwError(() => error);
      }),
    );
  }

  private addToken(request: HttpRequest<any>, token: string): HttpRequest<any> {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
  }

  private handle401Error(
    request: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return this.authService.refreshToken().pipe(
        switchMap((response: any) => {
          this.isRefreshing = false;
          this.refreshTokenSubject.next(response.accessToken);
          return next.handle(this.addToken(request, response.accessToken));
        }),
        catchError((err) => {
          this.isRefreshing = false;
          this.authService.logout();
          return throwError(() => err);
        }),
      );
    }

    return this.refreshTokenSubject.pipe(
      filter((token) => token !== null),
      take(1),
      switchMap((token) => next.handle(this.addToken(request, token))),
    );
  }
}
```

#### 8.5.3: Register Interceptor

**File**: `simple-e-commerce-frontend/src/app/app.config.ts`

```typescript
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import { AuthInterceptor } from "./interceptors/auth.interceptor";

export const appConfig: ApplicationConfig = {
  providers: [
    // ... other providers
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
  ],
};
```

---

### Step 8.6: Post-Deployment Validation

#### 8.6.1: Health Check Endpoints

Create health check endpoints to verify deployment:

```bash
# Check Keycloak
curl https://auth.yourdomain.com/health/ready

# Check Backend
curl https://api.yourdomain.com/health

# Check Backend with authentication
curl https://api.yourdomain.com/api/v1/test-auth/public
```

#### 8.6.2: End-to-End Testing

1. Register a new user via frontend
2. Verify email (if enabled)
3. Login with new user
4. Access protected resources
5. Refresh token
6. Logout

#### 8.6.3: Monitoring Setup

**Configure Application Insights / ELK Stack:**

```csharp
// Program.cs
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["ApplicationInsights:ConnectionString"]);
```

**Monitor Key Metrics:**

- Authentication success/failure rates
- Token refresh rates
- API response times
- Error rates
- User registration counts

---

## Verification Checklist

### Production Keycloak

- [ ] Keycloak running on HTTPS
- [ ] SSL certificate valid
- [ ] Realm configured correctly
- [ ] Client secrets secured
- [ ] Email service configured
- [ ] Backup strategy in place

### Backend Deployment

- [ ] Application deployed successfully
- [ ] Database migrations applied
- [ ] Environment variables configured
- [ ] HTTPS enabled
- [ ] CORS configured correctly
- [ ] Health checks passing

### User Migration

- [ ] Existing users migrated (if applicable)
- [ ] Password reset emails sent
- [ ] UserProfile IDs updated
- [ ] Legacy data backed up

### Frontend Integration

- [ ] Authentication service updated
- [ ] HTTP interceptor configured
- [ ] Token refresh working
- [ ] Login/logout functional
- [ ] Protected routes working

### Testing

- [ ] Registration works end-to-end
- [ ] Login returns valid tokens
- [ ] Token refresh successful
- [ ] Authorization policies enforced
- [ ] Error handling works correctly

### Monitoring

- [ ] Logging configured
- [ ] Metrics collection enabled
- [ ] Alerts configured
- [ ] Dashboard created

---

## Rollback Plan

### If Critical Issues Occur

#### Immediate Rollback Steps

1. **Switch traffic back to old version:**

```bash
# Revert to previous Docker image
docker-compose down
docker-compose -f docker-compose.old.yml up -d
```

2. **Restore database:**

```bash
# Restore from backup
sqlcmd -S localhost -U sa -P password -Q "RESTORE DATABASE SimpleECommerce FROM DISK = '/backup/SimpleECommerce_backup.bak'"
```

3. **Revert DNS/Load balancer**

4. **Communicate to users** about temporary rollback

#### Gradual Rollback (Hybrid Mode)

Keep both authentication methods active:

```csharp
// Support both custom JWT and Keycloak
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "MultiAuth";
})
.AddPolicyScheme("MultiAuth", "Multiple Auth Schemes", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        var token = context.Request.Headers["Authorization"].ToString();
        return IsKeycloakToken(token) ? "Keycloak" : "CustomJwt";
    };
})
.AddJwtBearer("Keycloak", options => { /* Keycloak config */ })
.AddJwtBearer("CustomJwt", options => { /* Old JWT config */ });
```

---

## Troubleshooting

### Issue: "SSL certificate errors"

**Solution:**

- Verify certificate is valid and not expired
- Check certificate chain
- Ensure proper certificate format
- Verify certificate matches hostname

### Issue: "Cannot connect to Keycloak"

**Solution:**

- Check Keycloak container is running
- Verify network connectivity
- Check firewall rules
- Verify DNS resolution

### Issue: "Database migration fails"

**Solution:**

- Ensure database backup exists
- Check SQL Server compatibility
- Review migration script for errors
- Apply migrations manually if needed

### Issue: "Users can't login after migration"

**Solution:**

- Verify users exist in Keycloak
- Check password reset emails were sent
- Verify UserProfile IDs match Keycloak IDs
- Check Keycloak user status (enabled/disabled)

### Issue: "Frontend can't reach backend"

**Solution:**

- Verify CORS configuration
- Check API endpoint URLs
- Verify SSL certificates
- Check browser console for errors

---

## Project Completion

### Documentation Checklist

- [x] Phase 1-8 implementation guides created
- [ ] API documentation updated
- [ ] Architecture diagrams updated
- [ ] Deployment runbooks created
- [ ] Troubleshooting guide completed
- [ ] User migration guide (if applicable)
- [ ] Security best practices documented

### Post-Implementation Tasks

1. **Security Audit**
   - Review token expiration times
   - Enable MFA
   - Configure password policies
   - Set up brute force protection

2. **Performance Optimization**
   - Enable caching for tokens
   - Configure connection pooling
   - Optimize database queries

3. **Future Enhancements**
   - Social login (Google, Facebook, GitHub)
   - Single Sign-On (SSO)
   - Custom Keycloak themes
   - Advanced role hierarchies
   - User federation (LDAP, Active Directory)

4. **Monitoring & Alerts**
   - Authentication failure alerts
   - Token expiration monitoring
   - Keycloak health alerts
   - Database performance monitoring

### Success Metrics

Track these metrics post-deployment:

- ✅ Authentication success rate > 99%
- ✅ Average login time < 2 seconds
- ✅ Token refresh success rate > 99%
- ✅ Zero critical security incidents
- ✅ User satisfaction maintained or improved

---

## Congratulations! 🎉

You have successfully completed the Keycloak implementation for the SimpleECommerce backend!

**What You've Achieved:**

- ✅ Replaced custom JWT authentication with enterprise-grade Keycloak
- ✅ Implemented OAuth2/OIDC standards
- ✅ Set up role-based authorization
- ✅ Deployed to production
- ✅ Migrated existing users (if applicable)
- ✅ Updated frontend integration

**Next Steps:**

- Monitor system performance
- Gather user feedback
- Plan future enhancements
- Continue security hardening

---

_Document Version: 1.0_  
_Last Updated: 2026-03-03_  
_Phase Status: Complete_
