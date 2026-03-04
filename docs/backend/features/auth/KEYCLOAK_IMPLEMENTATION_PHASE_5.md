# Phase 5: Use Case Layer Updates

**Status**: ⬜ Not Started  
**Duration**: 2-3 days  
**Phase Overview**: [KEYCLOAK_IMPLEMENTATION_PLAN.md](./KEYCLOAK_IMPLEMENTATION_PLAN.md)

---

## Table of Contents

1. [Objectives](#objectives)
2. [Prerequisites](#prerequisites)
3. [Implementation Steps](#implementation-steps)
   - [Step 5.1: Update Register Command Handler](#step-51-update-register-command-handler)
   - [Step 5.2: Update Login Command Handler](#step-52-update-login-command-handler)
   - [Step 5.3: Update Refresh Token Command Handler](#step-53-update-refresh-token-command-handler)
   - [Step 5.4: Remove or Update Other Auth Use Cases](#step-54-remove-or-update-other-auth-use-cases)
4. [Verification Checklist](#verification-checklist)
5. [Troubleshooting](#troubleshooting)
6. [Next Steps](#next-steps)

---

## Objectives

- ✅ Update Register handler to create users in Keycloak
- ✅ Update Login handler to authenticate via Keycloak
- ✅ Update RefreshToken handler to use Keycloak token refresh
- ✅ Remove or deprecate custom authentication use cases
- ✅ Ensure all handlers follow Clean Architecture principles
- ✅ Maintain UserProfile synchronization with Keycloak users

---

## Prerequisites

- [ ] Phase 1 (Keycloak Setup) completed
- [ ] Phase 2 (Backend Configuration) completed
- [ ] Phase 3 (Authentication Service Implementation) completed
- [ ] Phase 4 (API Layer Updates) completed
- [ ] Keycloak services available via DI
- [ ] Understanding of MediatR pattern

---

## Implementation Steps

### Step 5.1: Update Register Command Handler

**File**: `SimpleECommerceBackend.Application/UseCases/Auth/Register/RegisterCommandHandler.cs`

#### 5.1.1: Update Command and Result Classes

Replace the entire file with the following implementation:

```csharp
using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Application.UseCases.Auth.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Role
) : IRequest<RegisterResult>;

public class RegisterResult
{
    public string Email { get; init; } = null!;
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly IKeycloakAdminService _keycloakAdminService;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(
        IKeycloakAdminService keycloakAdminService,
        IUserProfileRepository userProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _keycloakAdminService = keycloakAdminService;
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken = default)
    {
        // Check if user already exists in Keycloak
        var userExists = await _keycloakAdminService.UserExistsAsync(request.Email, cancellationToken);
        if (userExists)
            throw new BusinessException("User with this email already exists");

        // Validate role
        var validRoles = new[] { "customer", "seller", "admin" };
        if (!validRoles.Contains(request.Role.ToLower()))
            throw new BusinessException($"Invalid role. Must be one of: {string.Join(", ", validRoles)}");

        // Create user in Keycloak
        var keycloakUser = await _keycloakAdminService.CreateUserAsync(new CreateKeycloakUserRequest
        {
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role.ToLower()
        }, cancellationToken);

        // Create local UserProfile with Keycloak user ID
        var userProfile = UserProfile.Create(
            Guid.Parse(keycloakUser.KeycloakUserId),
            request.Email,
            request.FirstName,
            request.LastName,
            null, // nickname
            Sex.Other,
            AgeUtils.CreateRandomBirthDate(UserProfileConstants.MinAge, UserProfileConstants.MaxAge),
            null // avatar URL
        );

        _userProfileRepository.Add(userProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterResult
        {
            Email = request.Email
        };
    }
}
```

#### 5.1.2: Key Changes Explained

**What Changed:**

- **Keycloak Integration**: Uses `IKeycloakAdminService` to create users in Keycloak
- **Dual Storage**: Creates user in both Keycloak (auth) and local DB (business profile)
- **ID Mapping**: UserProfile.Id now stores the Keycloak user ID for future correlation
- **Role Validation**: Validates that the role is one of: customer, seller, or admin
- **Error Handling**: Checks if user already exists before attempting creation

**Flow:**

1. Check if email already exists in Keycloak
2. Validate the requested role
3. Create user in Keycloak with credentials
4. Create local UserProfile with Keycloak user ID
5. Save to database

---

### Step 5.2: Update Login Command Handler

**File**: `SimpleECommerceBackend.Application/UseCases/Auth/Login/LoginCommandHandler.cs`

#### 5.2.1: Update Command and Result Classes

Replace the entire file with the following implementation:

```csharp
using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Domain.Entities.Business;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;

namespace SimpleECommerceBackend.Application.UseCases.Auth.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<LoginResult>;

public class LoginResult
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? NickName { get; init; }
    public Sex Sex { get; init; }
    public DateOnly BirthDate { get; init; }
    public string? AvatarUrl { get; init; }
    public string Role { get; init; } = null!;
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public int ExpiresIn { get; init; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IKeycloakTokenService _keycloakTokenService;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IKeycloakTokenService keycloakTokenService,
        IUserProfileRepository userProfileRepository,
        IUnitOfWork unitOfWork)
    {
        _keycloakTokenService = keycloakTokenService;
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken = default)
    {
        // Authenticate with Keycloak
        var tokenResponse = await _keycloakTokenService.GetTokenAsync(
            request.Email,
            request.Password,
            cancellationToken);

        // Get user info from Keycloak
        var userInfo = await _keycloakTokenService.GetUserInfoAsync(
            tokenResponse.AccessToken,
            cancellationToken);

        // Find local user profile by Keycloak user ID
        var keycloakUserId = Guid.Parse(userInfo.Sub);
        var userProfile = await _userProfileRepository.FindByIdAsync(keycloakUserId);

        // If user profile doesn't exist, create it (for existing Keycloak users)
        if (userProfile == null)
        {
            userProfile = UserProfile.Create(
                keycloakUserId,
                userInfo.Email,
                userInfo.GivenName ?? "User",
                userInfo.FamilyName ?? "Name",
                null,
                Sex.Other,
                DateOnly.FromDateTime(DateTime.Now.AddYears(-25)),
                null
            );
            _userProfileRepository.Add(userProfile);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Extract role from Keycloak
        var role = userInfo.Roles.FirstOrDefault() ?? "customer";

        return new LoginResult
        {
            UserId = userProfile.Id,
            Email = userProfile.Email,
            FirstName = userProfile.FirstName,
            LastName = userProfile.LastName,
            NickName = userProfile.NickName,
            Sex = userProfile.Sex,
            BirthDate = userProfile.BirthDate,
            AvatarUrl = userProfile.AvatarUrl,
            Role = role,
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiresIn = tokenResponse.ExpiresIn
        };
    }
}
```

#### 5.2.2: Key Changes Explained

**What Changed:**

- **Keycloak Authentication**: Uses `IKeycloakTokenService` to authenticate with Keycloak
- **Token Response**: Returns Keycloak-issued access token and refresh token
- **User Profile Sync**: Automatically creates local UserProfile if it doesn't exist
- **Role Extraction**: Extracts user role from Keycloak token claims
- **No Password Verification**: Password verification is handled by Keycloak

**Flow:**

1. Request token from Keycloak with email/password
2. Get user information from Keycloak using access token
3. Find or create local UserProfile
4. Extract role from Keycloak claims
5. Return combined user data with tokens

---

### Step 5.3: Update Refresh Token Command Handler

**File**: `SimpleECommerceBackend.Application/UseCases/Auth/RefreshToken/RefreshTokenCommandHandler.cs`

#### 5.3.1: Create or Update Handler

Replace the entire file with the following implementation:

```csharp
using MediatR;
using SimpleECommerceBackend.Application.Interfaces.Security;

namespace SimpleECommerceBackend.Application.UseCases.Auth.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken
) : IRequest<RefreshTokenResult>;

public class RefreshTokenResult
{
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
    public int ExpiresIn { get; init; }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    private readonly IKeycloakTokenService _keycloakTokenService;

    public RefreshTokenCommandHandler(IKeycloakTokenService keycloakTokenService)
    {
        _keycloakTokenService = keycloakTokenService;
    }

    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken = default)
    {
        var tokenResponse = await _keycloakTokenService.RefreshTokenAsync(
            request.RefreshToken,
            cancellationToken);

        return new RefreshTokenResult
        {
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            ExpiresIn = tokenResponse.ExpiresIn
        };
    }
}
```

#### 5.3.2: Key Changes Explained

**What Changed:**

- **Direct Keycloak Integration**: Simply proxies the refresh request to Keycloak
- **Stateless**: No database queries needed for token refresh
- **Simple Implementation**: All logic handled by Keycloak

**Flow:**

1. Receive refresh token from client
2. Send refresh request to Keycloak
3. Return new access token and refresh token

---

### Step 5.4: Remove or Update Other Auth Use Cases

#### 5.4.1: Identify Files to Remove/Update

The following use cases were part of the custom authentication system and should be handled:

**Files to Consider Removing:**

1. **`VerifyAccount/VerifyAccountCommandHandler.cs`**
   - Keycloak handles email verification
   - Can be removed if using Keycloak's email verification flow

2. **`ChangePassword/ChangePasswordCommandHandler.cs`**
   - Should redirect to Keycloak account management
   - Or implement as proxy to Keycloak Admin API

3. **`ResetPassword/ResetPasswordCommandHandler.cs`**
   - Use Keycloak's forgot password flow
   - Can be removed or reimplemented as Keycloak proxy

#### 5.4.2: Option 1 - Remove Files (Recommended)

If you want to fully rely on Keycloak:

```bash
# Navigate to Application layer
cd SimpleECommerceBackend.Application/UseCases/Auth

# Remove deprecated folders
rm -rf VerifyAccount
rm -rf ChangePassword
rm -rf ResetPassword
```

#### 5.4.3: Option 2 - Keep as Keycloak Proxies

If you want to maintain the same API endpoints:

**Example: Update ChangePassword Handler**

```csharp
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IKeycloakAdminService _keycloakAdminService;

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // Proxy to Keycloak Admin API
        await _keycloakAdminService.ResetPasswordAsync(
            request.UserId.ToString(),
            request.NewPassword,
            cancellationToken);

        return Unit.Value;
    }
}
```

#### 5.4.4: Update API Documentation

Document that the following operations should be done through Keycloak:

- **Email Verification**: Configured in Keycloak realm settings
- **Password Reset**: Use Keycloak's forgot password page
- **Account Management**: Direct users to Keycloak account console

---

## Verification Checklist

After completing this phase, verify the following:

- [ ] RegisterCommandHandler creates users in Keycloak
- [ ] RegisterCommandHandler creates local UserProfile with Keycloak ID
- [ ] LoginCommandHandler authenticates via Keycloak
- [ ] LoginCommandHandler returns Keycloak tokens
- [ ] LoginCommandHandler creates UserProfile if missing
- [ ] RefreshTokenCommandHandler refreshes tokens via Keycloak
- [ ] Deprecated use cases are removed or updated
- [ ] All handlers compile without errors
- [ ] No references to IPasswordHasher remain
- [ ] No references to IJwtGenerator remain

---

## Troubleshooting

### Issue: "IKeycloakAdminService not found"

**Solution:**

- Ensure Phase 3 is completed
- Verify the service is registered in DependencyInjection
- Add proper using statement

### Issue: "UserProfile.Create requires CredentialId"

**Solution:**

- Update UserProfile entity to accept Keycloak user ID
- The `Id` field should now represent the Keycloak user ID
- This will be fully addressed in Phase 6

### Issue: "Cannot parse Keycloak user ID to Guid"

**Solution:**

- Keycloak `sub` claim is a string representation of UUID
- Use `Guid.Parse(userInfo.Sub)` to convert
- Ensure Keycloak is configured with UUID user IDs

### Issue: "Login creates duplicate UserProfiles"

**Solution:**

- Ensure `FindByIdAsync` looks up by Keycloak user ID
- Use the correct ID field (should be the Keycloak sub claim)
- Check that database constraints prevent duplicate IDs

### Issue: "Role not found in token"

**Solution:**

- Verify realm roles are assigned to users in Keycloak
- Ensure client scopes include realm roles
- Check token claims using JWT debugger (jwt.io)

---

## Next Steps

Once Phase 5 is complete and verified:

➡️ **Proceed to [Phase 6: Domain & Infrastructure Updates](./KEYCLOAK_IMPLEMENTATION_PHASE_6.md)**

Phase 6 will update the domain entities, remove legacy authentication infrastructure, update dependency injection, and create database migrations.
