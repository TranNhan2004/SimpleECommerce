# Keycloak Implementation Plan

## Document Information

- **Project**: SimpleECommerce Backend
- **Date**: 2026-03-03
- **Target**: Migrate from Custom JWT Authentication to Keycloak
- **Strategy**: Full Keycloak Integration (Option 1)
- **Status**: Not Started

---

## Table of Contents

1. [Executive Summary](#executive-summary)
2. [Prerequisites](#prerequisites)
3. [Implementation Phases](#implementation-phases)
   - [Phase 1: Keycloak Setup](#phase-1-keycloak-setup)
   - [Phase 2: Backend Configuration](#phase-2-backend-configuration)
   - [Phase 3: Authentication Service Implementation](#phase-3-authentication-service-implementation)
   - [Phase 4: API Layer Updates](#phase-4-api-layer-updates)
   - [Phase 5: Use Case Layer Updates](#phase-5-use-case-layer-updates)
   - [Phase 6: Domain & Infrastructure Updates](#phase-6-domain--infrastructure-updates)
   - [Phase 7: Testing & Validation](#phase-7-testing--validation)
   - [Phase 8: Deployment & Migration](#phase-8-deployment--migration)
4. [Notifications & Alerts](#notifications--alerts)
5. [Rollback Plan](#rollback-plan)

---

## Executive Summary

### Goal

Replace the current custom JWT authentication system with Keycloak to leverage enterprise-grade authentication, authorization, and user management capabilities.

### Approach

**Full Keycloak Integration** - Delegate all authentication and user management to Keycloak while maintaining local UserProfile table for business-specific data.

### Timeline Estimate

| Phase     | Duration       | Status                                     | Description                      |
| --------- | -------------- | ------------------------------------------ | -------------------------------- |
| Phase 1-2 | 2-3 days       | ✅ Complete (Jan 2025)                     | Keycloak setup and configuration |
| Phase 3-5 | 5-7 days       | ✅ Complete (Jan 2025 + Mar 2026 refactor) | Core implementation              |
| Phase 6   | 3-4 days       | ⬜ Not Started                             | Domain updates and cleanup       |
| Phase 7   | 3-5 days       | ⬜ Not Started                             | Testing                          |
| Phase 8   | 2-3 days       | ⬜ Not Started                             | Deployment                       |
| **Total** | **15-22 days** | **~60% Complete**                          | Complete migration               |

### Key Benefits

- ✅ Industry-standard OAuth2/OIDC authentication
- ✅ Built-in user management UI
- ✅ MFA support out of the box
- ✅ Social login integration capabilities
- ✅ Reduced maintenance of authentication code
- ✅ Better security with regular updates

---

## Prerequisites

### Required Tools & Accounts

- [ ] Docker Desktop (for local Keycloak instance)
- [ ] Keycloak 24+ (latest stable version)
- [ ] .NET 10 SDK
- [ ] SQL Server Management Studio or Azure Data Studio
- [ ] Postman or similar API testing tool

### Required Knowledge

- OAuth2 and OpenID Connect fundamentals
- Keycloak administration basics
- ASP.NET Core authentication/authorization
- Entity Framework Core migrations

### NuGet Packages Required

- `Keycloak.AuthServices.Authentication` v2.5.2
- `Keycloak.AuthServices.Authorization` v2.5.2
- `Microsoft.AspNetCore.Authentication.JwtBearer` v8.0.0

---

## Implementation Phases

---

## Phase 1: Keycloak Setup

**Status**: ✅ Completed  
**Duration**: 1-2 days  
**Detailed Implementation**: [KEYCLOAK_IMPLEMENTATION_PHASE_1.md](./KEYCLOAK_IMPLEMENTATION_PHASE_1.md)

### Objectives

- Set up local Keycloak instance using Docker
- Configure realm, client, and authentication settings
- Create initial roles and test users
- Validate Keycloak installation

### Techniques & Approaches

1. **Containerization**: Use Docker Compose with PostgreSQL backend for persistent storage
2. **Realm Configuration**: Enable user registration, email as username, forgot password features
3. **Client Setup**: Configure confidential client with authentication flows and service accounts
4. **Scope Mapping**: Create custom client scopes for role claims in JWT tokens
5. **Role Management**: Define realm roles (customer, seller, admin) for authorization
6. **Testing**: Verify token endpoint with Postman and JWT decoder

### Key Files & Components

- `docker-compose.yml` - Keycloak services added to existing compose file
- Keycloak Admin Console - UI-based configuration
- Client credentials - Store securely for backend use

### Notifications

- ⚠️ **Important**: Save client secret immediately after creation
- ⚠️ **Warning**: Enable service accounts for admin operations
- ✅ **Success**: Validate JWT tokens contain role claims before proceeding

---

## Phase 2: Backend Configuration

**Status**: ✅ Completed  
**Duration**: 0.5-1 day  
**Detailed Implementation**: [KEYCLOAK_IMPLEMENTATION_PHASE_2.md](./KEYCLOAK_IMPLEMENTATION_PHASE_2.md)

### Objectives

- Install required NuGet packages
- Configure Keycloak settings in appsettings.json
- Create settings models for dependency injection
- Remove old JWT configuration

### Techniques & Approaches

1. **Package Management**: Install Keycloak authentication libraries via dotnet CLI
2. **Configuration Management**: Use IOptions pattern for Keycloak settings
3. **Settings Structure**: Separate credentials, endpoints, and validation options
4. **Environment Variables**: Prepare for production secrets management

### Key Files & Components

- `SimpleECommerceBackend.Api.csproj` - Package references
- `appsettings.json` / `appsettings.Development.json` - Configuration
- `KeycloakSettings.cs` - Strongly-typed settings model

### Notifications

- ⚠️ **Security**: Never commit client secrets to source control
- 🔔 **Next Step**: Update DependencyInjection after this phase

---

## Phase 3: Authentication Service Implementation

**Status**: ✅ Complete  
**Completed**: January 2025  
**Duration**: 2-3 days  
**Detailed Implementation**: [KEYCLOAK_IMPLEMENTATION_PHASE_3.md](./KEYCLOAK_IMPLEMENTATION_PHASE_3.md)

### Objectives

- Implement Keycloak token service for authentication operations
- Implement Keycloak admin service for user management
- Create DTOs for token responses and user info
- Configure HTTP clients with proper error handling

### Techniques & Approaches

1. **Service Abstraction**: Define interfaces in Application layer, implement in Infrastructure
2. **HTTP Communication**: Use HttpClient with FormUrlEncodedContent for OAuth2 flows
3. **Token Management**: Handle token acquisition, refresh, validation, and introspection
4. **Admin Operations**: Use service account credentials for user CRUD operations
5. **Error Handling**: Map Keycloak errors to domain exceptions
6. **Caching**: Implement token caching for admin operations to reduce API calls

### Key Files & Components

- `Application/Interfaces/Services/Keycloak/IKeycloakTokenService.cs` - Token operations interface
- `Infrastructure/Services/Keycloak/KeycloakTokenService.cs` - Token service implementation
- `Application/Interfaces/Services/Keycloak/IKeycloakAdminService.cs` - Admin operations interface
- `Infrastructure/Services/Keycloak/KeycloakAdminService.cs` - Admin service implementation
- `Application/Models/Keycloak/KeycloakTokenResponse.cs` - Token response model
- `Application/Models/Keycloak/KeycloakUserInfoResponse.cs` - User info response model
- `Application/Models/Keycloak/CreateKeycloakUserRequest.cs` - User creation request DTO
- `Application/Models/Keycloak/CreateKeycloakUserResponse.cs` - User creation response DTO
- `Infrastructure/Services/Keycloak/KeycloakSettings.cs` - Configuration model

### Notifications

- ⚠️ **Architecture**: Follow Clean Architecture dependency rules
- 🔥 **Performance**: Cache admin tokens to avoid frequent token requests
- ✅ **Testing**: Create mock implementations for unit tests

---

## Phase 4: API Layer Updates

**Status**: ✅ Complete  
**Completed**: January 2025  
**Duration**: 1-2 days  
**Detailed Implementation**: [KEYCLOAK_IMPLEMENTATION_PHASE_4.md](./KEYCLOAK_IMPLEMENTATION_PHASE_4.md)

### Objectives

- Update Program.cs with Keycloak authentication middleware
- Configure authorization policies for role-based access
- Update AuthController to use Keycloak services
- Modify DTOs to match new authentication flow
- Update Swagger configuration for Bearer tokens

### Techniques & Approaches

1. **Middleware Configuration**: Use Keycloak.AuthServices extensions for JWT validation
2. **Authorization Policies**: Create policy-based authorization for each role
3. **Controller Updates**: Remove password hashing, delegate authentication to Keycloak
4. **Response Mapping**: Update DTOs to include Keycloak tokens and user info
5. **OpenAPI Integration**: Configure Swagger UI to support Bearer token authentication

### Key Files & Components

- `Program.cs` - Authentication middleware configuration
- `AuthController.cs` - Updated endpoints
- DTOs in `SimpleECommerceBackend.Api/DTOs/Auth/` directory

### Notifications

- ⚠️ **Breaking Change**: API responses will change format
- 🔔 **Frontend Impact**: Frontend authentication flow must be updated
- ✅ **Validation**: Test all endpoints with Swagger before proceeding

---

## Phase 5: Use Case Layer Updates

**Status**: ✅ Complete  
**Completed**: January 2025  
**Duration**: 2-3 days  
**Detailed Implementation**: [KEYCLOAK_IMPLEMENTATION_PHASE_5.md](./KEYCLOAK_IMPLEMENTATION_PHASE_5.md)

### Objectives

- Rewrite RegisterCommandHandler to create users in Keycloak
- Rewrite LoginCommandHandler to authenticate via Keycloak
- Rewrite RefreshTokenCommandHandler to use Keycloak refresh flow
- Remove or deprecate password-related handlers
- Update UserProfile creation to use Keycloak user ID

### Techniques & Approaches

1. **User Registration**: Create Keycloak user first, then create local UserProfile
2. **Authentication Flow**: Validate credentials with Keycloak, fetch user info, sync profile
3. **Token Refresh**: Delegate token refresh to Keycloak service
4. **Error Handling**: Map Keycloak authentication errors to domain exceptions
5. **Transactional Integrity**: Rollback Keycloak user on local DB failure
6. **Deprecated Handlers**: Archive password change/reset handlers or reimplement via Admin API

### Key Files & Components

- `RegisterCommandHandler.cs` - User registration logic
- `LoginCommandHandler.cs` - Authentication logic
- `RefreshTokenCommandHandler.cs` - Token refresh logic

### Notifications

- ⚠️ **Data Integrity**: Ensure atomic operations between Keycloak and local DB
- 🔥 **Cleanup Required**: Remove deprecated handlers after migration
- ✅ **Testing**: Comprehensive integration tests required

---

## Phase 6: Domain & Infrastructure Updates

**Status**: ⬜ Not Started  
**Duration**: 2-3 days  
**Detailed Implementation**: [KEYCLOAK_IMPLEMENTATION_PHASE_6.md](./KEYCLOAK_IMPLEMENTATION_PHASE_6.md)

### Objectives

- Update UserProfile entity to reference Keycloak user ID
- Remove Credential entity and repository
- Remove password hasher implementations
- Remove or archive JWT generator
- Create database migration to drop Credentials table
- Update DependencyInjection configuration

### Techniques & Approaches

1. **Entity Updates**: UserProfile.Id now stores Keycloak user ID (sub claim)
2. **Database Migration**: Use EF Core migrations to drop Credentials table safely
3. **Cleanup**: Remove BCryptPasswordHasher and related interfaces
4. **DI Updates**: Remove Credential repository, add Keycloak services
5. **Backward Compatibility**: Consider keeping old code as archive for reference

### Key Files & Components

- `UserProfile.cs` - Entity definition
- `Credential.cs` - Remove or archive
- `DependencyInjection.cs` - Update service registrations
- EF Core migration files

### Notifications

- ⚠️ **Data Loss**: Old migrations and database will be dropped and recreated
- 🔥 **Breaking Change**: Old authentication completely removed
- ✅ **Validation**: Run database migration in development environment

---

## Phase 7: Testing & Validation

**Status**: ⬜ Not Started  
**Duration**: 3-5 days  
**Detailed Implementation**: [KEYCLOAK_IMPLEMENTATION_PHASE_7.md](./KEYCLOAK_IMPLEMENTATION_PHASE_7.md)

### Objectives

- Create mock Keycloak services for unit tests
- Write unit tests for all authentication use cases
- Write integration tests for end-to-end flows
- Perform manual API testing with Postman
- Test authorization policies with different roles
- Validate token expiration and refresh scenarios

### Techniques & Approaches

1. **Unit Testing**: Mock IKeycloakTokenService and IKeycloakAdminService
2. **Integration Testing**: Use test containers or test Keycloak instance
3. **Manual Testing**: Create comprehensive Postman collection
4. **Role Testing**: Verify each role can access appropriate endpoints
5. **Security Testing**: Test invalid tokens, expired tokens, wrong roles
6. **Performance Testing**: Measure response times and token refresh cycles

### Key Files & Components

- `MockKeycloakTokenService.cs` - Unit test mocks
- `MockKeycloakAdminService.cs` - Unit test mocks
- Integration test classes
- Postman collection

### Notifications

- ⚠️ **Coverage**: Aim for >80% code coverage on auth flows
- 🔥 **Security**: Test negative scenarios thoroughly
- ✅ **Sign-off**: All tests must pass before deployment

---

## Phase 8: Deployment & Migration

**Status**: ⬜ Not Started  
**Duration**: 2-3 days  
**Detailed Implementation**: [KEYCLOAK_IMPLEMENTATION_PHASE_8.md](./KEYCLOAK_IMPLEMENTATION_PHASE_8.md)

### Objectives

- Deploy production Keycloak instance
- Configure production settings with SSL
- Migrate existing users to Keycloak (if applicable)
- Deploy updated backend application
- Update frontend authentication
- Monitor and validate production deployment

### Techniques & Approaches

1. **Production Keycloak**: Use managed service or self-host with HA configuration
2. **SSL/TLS**: Configure HTTPS for all Keycloak communication
3. **User Migration**: Create migration scripts for existing users
4. **Deployment Strategy**: Blue-green or canary deployment
5. **Frontend Updates**: Update Angular app to use new token format
6. **Monitoring**: Set up logging, alerts, and dashboards

### Key Files & Components

- Keycloak production configuration
- User migration scripts
- Deployment manifests (Docker/Kubernetes)
- Frontend authentication service updates

### Notifications

- ⚠️ **Downtime**: Plan maintenance window for migration
- 🔥 **Rollback Ready**: Have rollback plan prepared
- ✅ **Monitoring**: Watch metrics closely post-deployment

---

## Notifications & Alerts

### Critical Warnings

- 🔴 **Database Reset**: Old migrations will be deleted and database recreated (Phase 6)
- 🔴 **Breaking Changes**: Frontend must be updated simultaneously (Phase 4)
- 🔴 **Secret Management**: Never commit Keycloak client secrets to Git

### Important Notifications

- 🟡 **Dependencies**: Backend depends on Keycloak availability
- 🟡 **Token Expiration**: Default 5-minute token lifetime (configure as needed)
- 🟡 **Testing Required**: Each phase requires validation before proceeding

### Information

- 🟢 **Documentation**: Each phase has detailed implementation guide
- 🟢 **Rollback**: Rollback plan available for each phase
- 🟢 **Support**: Keycloak community and documentation available

---

## Rollback Plan

### Immediate Rollback

If critical issues occur during deployment:

1. **Revert Deployment**
   - Roll back to previous application version using Git
   - Recreate database with old migration
   - Switch load balancer to old instance

2. **Restore Configuration**
   - Checkout previous Git commit with old auth system
   - Restore old `appsettings.json` with JWT settings
   - Restore old `Program.cs` authentication configuration
   - Restore old auth handlers and services

3. **Database Rollback**
   - Drop current database: `dotnet ef database drop --force`
   - Checkout old migrations from Git history
   - Recreate with old migrations: `dotnet ef database update`

### Gradual Rollback (Hybrid Mode)

Support both authentication methods temporarily:

- Keep old JWT authentication alongside Keycloak
- Use header inspection to route to appropriate auth method
- Migrate users gradually over time
- Remove old system once migration complete

### Phase-Specific Rollback

- **Phase 1-2**: Simple - just stop using Keycloak, no code changes yet
- **Phase 3-5**: Revert code changes via Git, services not yet integrated
- **Phase 6**: Git rollback to previous commit, recreate database
- **Phase 7-8**: Full rollback required via Git, redeploy old version

---

## Success Criteria

### Overall Project Success

- ✅ All users can authenticate via Keycloak
- ✅ Token-based authorization working correctly
- ✅ All roles and permissions properly configured
- ✅ Frontend successfully integrated
- ✅ Production deployment stable
- ✅ No security vulnerabilities introduced
- Performance meets or exceeds previous system

### Phase Completion Criteria

Each phase considered complete when:

- ✅ All tasks in detailed implementation completed
- ✅ Code reviewed and approved
- ✅ Tests passing (unit, integration, manual)
- ✅ Documentation updated
- ✅ Next phase prerequisites met

---

## Additional Resources

### Documentation

- [Keycloak Official Documentation](https://www.keycloak.org/documentation)
- [OAuth2 RFC 6749](https://tools.ietf.org/html/rfc6749)
- [OpenID Connect Core 1.0](https://openid.net/specs/openid-connect-core-1_0.html)
- [Keycloak.AuthServices NuGet](https://github.com/NikiforovAll/keycloak-authorization-services-dotnet)

### Support Channels

- Keycloak Community Slack
- Stack Overflow (keycloak tag)
- GitHub Issues for Keycloak.AuthServices

---

_Document Version: 2.0_  
_Last Updated: 2026-03-03_  
_Author: Development Team_
