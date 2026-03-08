# Phase 6: Domain & Infrastructure Updates

**Status**: ✅ Complete (March 6, 2026)  
**Duration**: 1-2 days  
**Phase Overview**: [KEYCLOAK_IMPLEMENTATION_PLAN.md](./KEYCLOAK_IMPLEMENTATION_PLAN.md)

---

## Table of Contents

1. [Objectives](#objectives)
2. [Prerequisites](#prerequisites)
3. [Implementation Steps](#implementation-steps)
   - [Step 6.1: Update UserProfile Entity](#step-61-update-userprofile-entity)
   - [Step 6.2: Remove or Archive Credential Entity](#step-62-remove-or-archive-credential-entity)
   - [Step 6.3: Remove Password Hasher](#step-63-remove-password-hasher)
   - [Step 6.4: Remove/Update JWT Generator](#step-64-removeupdate-jwt-generator)
   - [Step 6.5: Remove dbo Schema from Entity Configurations](#step-65-remove-dbo-schema-from-entity-configurations)
   - [Step 6.6: Update DependencyInjection](#step-66-update-dependencyinjection)
   - [Step 6.7: Clean Up Old Migrations](#step-67-clean-up-old-migrations)
   - [Step 6.8: Create New Migration](#step-68-create-new-migration)
4. [Verification Checklist](#verification-checklist)
5. [Troubleshooting](#troubleshooting)
6. [Next Steps](#next-steps)

---

## Objectives

- ✅ Update UserProfile entity to use Keycloak user ID
- ✅ Remove or archive Credential entity
- ✅ Remove password hashing infrastructure
- ✅ Remove custom JWT generation logic
- ✅ Remove dbo schema configuration from entity configurations
- ✅ Update dependency injection configuration
- ✅ Delete old migrations to start fresh
- ✅ Create and apply new clean migration
- ✅ Clean up authentication-related code

---

## Prerequisites

- [x] Phase 1 completed (Keycloak Setup)
  - Keycloak managing users with IDs (sub claim)
  - Roles managed in Keycloak realm
- [x] Phase 2-5 completed
- [x] Understanding of Entity Framework migrations
- [x] Understanding of Domain entity patterns (see Domain Analysis section below)
- [x] Git commit of current working state (for easy rollback)

### Domain Entity Pattern Analysis

Before making changes, understand the Domain layer entity structure:

**Entity Pattern**:

- All entities inherit from `Entity` abstract base class (provides `Guid Id` property and `SetId()` method)
- Optional interfaces: `ICreatedTrackable`, `IUpdatedTrackable`, `ISoftDeleteTrackable`
- Private parameterless constructor for EF Core
- Private parameterized constructor with validation logic
- Public static `Create()` factory method
- Public setter methods with business validation rules

**Example Entity Structure**:

```csharp
public class Cart : Entity, ICreatedTrackable, IUpdatedTrackable
{
    private Cart() { }  // For EF Core

    private Cart(Guid customerId)
    {
        SetCustomerId(customerId);
    }

    public Guid CustomerId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static Cart Create(Guid customerId)
    {
        return new Cart(customerId);
    }

    public void SetCustomerId(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new BusinessException("Customer ID is required");
        CustomerId = customerId;
    }
}
```

**UserProfile Updated Structure**:

- ✅ Inherits from `Entity` base class
- ✅ Uses `SetId(keycloakUserId)` to set Keycloak user ID as primary key
- ✅ No `CredentialId` foreign key or navigation property
- ✅ `Create()` method accepts `keycloakUserId` parameter
- ✅ Properly refactored to use Keycloak user ID directly as primary key

---

## Implementation Steps

### Step 6.1: Update UserProfile Entity

**File**: `SimpleECommerceBackend.Domain/Entities/UserProfile.cs`

#### 6.1.1: Review Current Entity Structure

Original UserProfile structure (before Phase 6):

```csharp
public class UserProfile : IEntity, ICreatedTrackable, IUpdatedTrackable
{
    public Guid Id { get; private set; }
    public Guid CredentialId { get; private set; }  // ❌ Removed
    public Credential? Credential { get; private set; }  // ❌ Removed
    public string Email { get; private set; }
    // ... other properties
}
```

**Changes Completed**:

1. ✅ Removed `CredentialId` property
2. ✅ Removed `Credential` navigation property
3. ✅ Updated `Create()` factory method to accept Keycloak user ID
4. ✅ Updated constructor to accept Keycloak user ID and use `SetId()`
5. ✅ Removed `SetCredentialId()` validation method
6. ✅ Updated using statements to remove Credential reference
7. ✅ Changed base class from `IEntity` to `Entity` abstract class

#### 6.1.2: Refactored UserProfile Entity

Current implementation (after Phase 6):

```csharp
using System.Text.RegularExpressions;
using SimpleECommerceBackend.Domain.Constants.Business;
using SimpleECommerceBackend.Domain.Entities.Abstracts;
using SimpleECommerceBackend.Domain.Enums;
using SimpleECommerceBackend.Domain.Exceptions;
using SimpleECommerceBackend.Domain.Utils;

namespace SimpleECommerceBackend.Domain.Entities;

/// <summary>
/// Represents a user's business profile in the application.
/// The Id property stores the Keycloak user ID (sub claim) for authentication correlation.
/// </summary>
public class UserProfile : Entity, ICreatedTrackable, IUpdatedTrackable
{
    private UserProfile()
    {
    }

    private UserProfile(
        Guid keycloakUserId,
        string email,
        string firstName,
        string lastName,
        string? nickName,
        Sex sex,
        DateOnly birthDate,
        string? avatarUrl
    )
    {
        SetId(keycloakUserId); // Set Keycloak user ID as primary key
        SetEmail(email);
        SetFirstName(firstName);
        SetLastName(lastName);
        SetNickName(nickName);
        SetSex(sex);
        SetBirthDate(birthDate);
        SetAvatarUrl(avatarUrl);
    }

    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string? NickName { get; private set; }
    public Sex Sex { get; private set; }
    public UserStatus Status { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public string? AvatarUrl { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public static UserProfile Create(
        Guid keycloakUserId,
        string email,
        string firstName,
        string lastName,
        string? nickName,
        Sex sex,
        DateOnly birthDate,
        string? avatarUrl
    )
    {
        return new UserProfile(
            keycloakUserId,
            email,
            firstName,
            lastName,
            nickName,
            sex,
            birthDate,
            avatarUrl
        );
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
            throw new BusinessException("Email is required");

        var trimmedEmail = email.Trim();

        // Note: Email validation is handled by Keycloak during user creation
        // Basic validation is sufficient here for business logic
        Email = trimmedEmail;
    }

    public void SetFirstName(string firstName)
    {
        if (string.IsNullOrEmpty(firstName))
            throw new BusinessException("First name is required");

        var trimmedFirstName = firstName.Trim();

        if (trimmedFirstName.Length > UserProfileConstants.FirstNameMaxLength)
            throw new BusinessException(
                $"First name cannot exceed {UserProfileConstants.FirstNameMaxLength} characters");

        FirstName = trimmedFirstName;
    }

    public void SetLastName(string lastName)
    {
        if (string.IsNullOrEmpty(lastName))
            throw new BusinessException("Last name is required");

        var trimmedLastName = lastName.Trim();

        if (trimmedLastName.Length > UserProfileConstants.LastNameMaxLength)
            throw new BusinessException(
                $"Last name cannot exceed {UserProfileConstants.LastNameMaxLength} characters");

        LastName = trimmedLastName;
    }

    public void SetNickName(string? nickName)
    {
        if (nickName is null)
        {
            NickName = null;
            return;
        }

        if (string.IsNullOrWhiteSpace(nickName))
            throw new BusinessException("Nick name is not blank");

        var trimmedNickName = nickName.Trim();

        if (trimmedNickName.Length > UserProfileConstants.NickNameMaxLength)
            throw new BusinessException($"Nick name cannot exceed {UserProfileConstants.NickNameMaxLength} characters");

        NickName = trimmedNickName;
    }

    public void SetSex(Sex sex)
    {
        Sex = sex;
    }

    public void SetBirthDate(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTimeOffset.UtcNow.UtcDateTime);

        if (birthDate > today)
            throw new BusinessException("Birth date cannot be in the future");

        if (AgeUtils.Calculate(birthDate, today) < UserProfileConstants.MinAge)
            throw new BusinessException($"Age cannot be less than {UserProfileConstants.MinAge} years");

        if (AgeUtils.Calculate(birthDate, today) > UserProfileConstants.MaxAge)
            throw new BusinessException($"Age cannot exceed than {UserProfileConstants.MaxAge} years");

        BirthDate = birthDate;
    }

    public void SetAvatarUrl(string? avatarUrl)
    {
        AvatarUrl = avatarUrl;
    }
}
```

#### 6.1.3: Key Changes Made

#### 6.1.3: Key Changes Made

- ✅ **Removed** `CredentialId` property
- ✅ **Removed** `Credential` navigation property
- ✅ **Removed** `SetCredentialId()` method
- ✅ **Removed** using statement for Auth-related constants (CredentialConstants)
- ✅ **Updated** constructor to accept `keycloakUserId` directly
- ✅ **Updated** `Create()` factory method signature
- ✅ **Set** `Id` directly from Keycloak user ID using `SetId()` method
- ✅ **Simplified** email validation (Keycloak handles comprehensive email validation)
- ✅ **Changed** base class from `IEntity` to `Entity` (abstract base class with SetId method)
- ✅ **Maintained** all validation logic in setter methods
- ✅ **Maintained** `UserStatus` property for business logic
- ✅ **Maintained** audit properties (`CreatedAt`, `UpdatedAt`)

---

### Step 6.2: Remove or Archive Credential Entity

#### 6.2.1: Option 1 - Remove Entirely (Recommended for New Projects)

**Files to remove:**

```bash
# Domain layer
rm SimpleECommerceBackend.Domain/Entities/Auth/Credential.cs

# Application layer interface
rm SimpleECommerceBackend.Application/Interfaces/Repositories/Auth/ICredentialRepository.cs

# Infrastructure layer implementation
rm SimpleECommerceBackend.Infrastructure/Repositories/Auth/CredentialRepository.cs
```

#### 6.2.2: Option 2 - Archive for Migration (Recommended for Existing Projects)

If you have existing users that need to be migrated:

**Rename and mark as obsolete:**

```csharp
namespace SimpleECommerceBackend.Domain.Entities.Auth;

/// <summary>
/// DEPRECATED: Legacy credential entity.
/// Authentication is now handled by Keycloak.
/// Kept for migration purposes only.
/// </summary>
[Obsolete("Use Keycloak for authentication. This entity is deprecated.")]
public class LegacyCredential
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public CredentialStatus Status { get; private set; }
    public Role Role { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    // Optional: Add Keycloak mapping
    public Guid? KeycloakUserId { get; set; }
}
```

#### 6.2.3: Remove from DbContext

**File**: `SimpleECommerceBackend.Infrastructure/Persistence/AppDbContext.cs`

Remove or comment out the Credentials DbSet:

```csharp
// Remove this line:
// public DbSet<Credential> Credentials { get; set; } = null!;

// Or if archiving:
// public DbSet<LegacyCredential> LegacyCredentials { get; set; } = null!;
```

---

### Step 6.3: Remove Password Hasher

#### 6.3.1: Remove Interface

**File to remove:**

```bash
rm SimpleECommerceBackend.Application/Interfaces/Security/IPasswordHasher.cs
```

#### 6.3.2: Remove Implementation

**File to remove:**

```bash
rm SimpleECommerceBackend.Infrastructure/Security/BCryptPasswordHasher.cs
```

#### 6.3.3: Remove from DependencyInjection

Search for any registration of `IPasswordHasher` in DependencyInjection files and remove those lines.

---

### Step 6.4: Remove/Update JWT Generator

#### 6.4.1: Option 1 - Remove Entirely (Recommended)

**Files to remove:**

```bash
# Interface
rm SimpleECommerceBackend.Application/Interfaces/Security/IJwtGenerator.cs

# Implementation
rm SimpleECommerceBackend.Infrastructure/Security/JwtGenerator.cs
```

#### 6.4.2: Option 2 - Keep for Token Validation

If you need custom token validation logic:

**File**: `SimpleECommerceBackend.Infrastructure/Security/KeycloakTokenValidator.cs`

```csharp
using SimpleECommerceBackend.Application.Interfaces.Security;

namespace SimpleECommerceBackend.Infrastructure.Security;

/// <summary>
/// Validates tokens issued by Keycloak using introspection endpoint.
/// </summary>
public class KeycloakTokenValidator
{
    private readonly IKeycloakTokenService _keycloakTokenService;

    public KeycloakTokenValidator(IKeycloakTokenService keycloakTokenService)
    {
        _keycloakTokenService = keycloakTokenService;
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _keycloakTokenService.ValidateTokenAsync(token, cancellationToken);
    }
}
```

---

### Step 6.5: Remove dbo Schema from Entity Configurations

Before creating new migrations, we need to remove the explicit `dbo` schema configuration from all entity type configurations to ensure proper database schema management.

#### 6.5.1: Locate Entity Configurations

Entity configurations are typically in:

```
SimpleECommerceBackend.Infrastructure/Persistence/Configurations/
```

#### 6.5.2: Remove dbo Schema References

Search for all `ToTable` calls that specify the `dbo` schema and remove the schema parameter.

**Example - Before:**

```csharp
builder.ToTable("UserProfiles", "dbo");
builder.ToTable("Credentials", "dbo");
builder.ToTable("Orders", "dbo");
```

**Example - After:**

```csharp
builder.ToTable("UserProfiles");
// builder.ToTable("Credentials"); // This line should be removed entirely
builder.ToTable("Orders");
```

#### 6.5.3: Files to Update

Update all entity configuration files in the Configurations folder:

**Auth Configurations** (if they still exist):

- `CredentialConfiguration.cs` - Remove entirely or comment out

**Business Configurations:**

- `UserProfileConfiguration.cs` - Remove `"dbo"` schema
- `OrderConfiguration.cs` - Remove `"dbo"` schema
- `CartConfiguration.cs` - Remove `"dbo"` schema
- `CustomerShippingAddressConfiguration.cs` - Remove `"dbo"` schema
- `SellerShopConfiguration.cs` - Remove `"dbo"` schema
- And any other entity configurations

**Command:**

```bash
# Use find and replace in your IDE to remove all instances of:
# From: .ToTable("TableName", "dbo")
# To: .ToTable("TableName")

# Or use command line (Linux/Mac):
cd SimpleECommerceBackend.Infrastructure/Persistence/Configurations
find . -name "*.cs" -exec sed -i 's/, "dbo"//g' {} +

# Or use command line (Windows PowerShell):
cd SimpleECommerceBackend.Infrastructure\Persistence\Configurations
Get-ChildItem -Filter *.cs -Recurse | ForEach-Object {
    (Get-Content $_.FullName) -replace ', "dbo"', '' | Set-Content $_.FullName
}
```

#### 6.5.4: Example Configuration File Update

**Before:**

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Business;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles", "dbo");

        builder.HasKey(x => x.Id);
        // ... other configurations
    }
}
```

**After:**

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleECommerceBackend.Domain.Entities.Business;

namespace SimpleECommerceBackend.Infrastructure.Persistence.Configurations.Business;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.HasKey(x => x.Id);
        // ... other configurations
    }
}
```

#### 6.5.5: Verify Changes

After making changes, build the project to ensure no errors:

```bash
dotnet build
```

---

### Step 6.6: Update DependencyInjection

**File**: `SimpleECommerceBackend.Infrastructure/DependencyInjection.cs`

#### 6.6.1: Update Infrastructure Registration

Replace the entire file content to remove old authentication services:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleECommerceBackend.Application.Interfaces;
using SimpleECommerceBackend.Application.Interfaces.Repositories;
using SimpleECommerceBackend.Application.Interfaces.Repositories.Business;
using SimpleECommerceBackend.Application.Interfaces.Security;
using SimpleECommerceBackend.Application.Interfaces.Services.Address;
using SimpleECommerceBackend.Application.Interfaces.Services.Email;
using SimpleECommerceBackend.Infrastructure.Persistence;
using SimpleECommerceBackend.Infrastructure.Persistence.Interceptors;
using SimpleECommerceBackend.Infrastructure.Repositories.Business;
using SimpleECommerceBackend.Infrastructure.Security;
using SimpleECommerceBackend.Infrastructure.Services;
using SimpleECommerceBackend.Infrastructure.Services.Address;
using SimpleECommerceBackend.Infrastructure.Services.Email;

namespace SimpleECommerceBackend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Email Services
        services.Configure<SmtpOptions>(configuration.GetSection("SmtpOptions"));
        services.AddScoped<IEmailProvider, EmailProvider>();
        services.AddSingleton<BackgroundEmailQueue>();
        services.AddSingleton<IEmailService, SmtpEmailService>();
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        services.AddHostedService<EmailBackgroundWorker>();

        // Keycloak Services (NEW)
        services.Configure<KeycloakSettings>(configuration.GetSection("Keycloak"));
        services.AddHttpClient<IKeycloakTokenService, KeycloakTokenService>();
        services.AddHttpClient<IKeycloakAdminService, KeycloakAdminService>();

        // Address Services
        services.AddSingleton<IAddressService, VnAddressService>();

        // Database
        services.AddScoped<AuditSaveChangesInterceptor>();

        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString);
            options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        // Business Repositories
        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICustomerShippingAddressRepository, CustomerShippingAddressRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISellerShopRepository, SellerShopRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();

        // REMOVED: ICredentialRepository (no longer needed)
        // REMOVED: IPasswordHasher (no longer needed)
        // REMOVED: IJwtGenerator (no longer needed)

        return services;
    }
}
```

#### 6.6.2: Key Changes

**Added:**

- `KeycloakSettings` configuration
- `IKeycloakTokenService` with `KeycloakTokenService` implementation
- `IKeycloakAdminService` with `KeycloakAdminService` implementation
- `AddHttpClient` for Keycloak HTTP communication

**Removed:**

- `ICredentialRepository` and `CredentialRepository`
- `IPasswordHasher` and `BCryptPasswordHasher`
- `IJwtGenerator` and `JwtGenerator`
- JwtSettings configuration

---

### Step 6.7: Clean Up Old Migrations

Before creating new migrations with the updated schema (without Credentials and without dbo schema), we should delete the old migrations to start fresh. This prevents conflicts and ensures a clean migration history.

⚠️ **Note**: Since we're starting fresh with no production data yet, we'll simply delete old migrations without backup.

#### 6.7.1: Delete Old Migration Files

**Location**: `SimpleECommerceBackend.Infrastructure/Persistence/Migrations/`

Delete all existing migration files:

```bash
cd SimpleECommerceBackend.Infrastructure/Persistence/Migrations

# Delete all migration files
rm -rf *

# On Windows PowerShell:
# Remove-Item * -Recurse -Force
```

#### 6.7.2: Drop and Recreate Database

The `__EFMigrationsHistory` table in your database tracks applied migrations. Since we're starting fresh, we need to drop and recreate the database.

**Drop the existing database:**

```bash
cd SimpleECommerceBackend.Api

# Drop the existing database
dotnet ef database drop --force

# The database will be recreated when we apply the new migration in Step 6.8
```

✅ **Development Note**: This approach is ideal for development environments where data loss is acceptable.

#### 6.7.3: Remove Migration Snapshot

Also delete the model snapshot file:

```bash
cd SimpleECommerceBackend.Infrastructure/Persistence/Migrations

# Delete the snapshot
rm *ModelSnapshot.cs

# On Windows PowerShell:
# Remove-Item *ModelSnapshot.cs
```

#### 6.7.4: Verify Cleanup

Ensure the Migrations folder is empty:

```bash
ls SimpleECommerceBackend.Infrastructure/Persistence/Migrations/

# Should show no files
```

---

### Step 6.8: Create New Migration

### Step 6.8: Create New Migration

Now that we've cleaned up old migrations and removed the Credential entity and dbo schema references, we can create a fresh initial migration.

#### 6.8.1: Create Initial Migration

Navigate to the Infrastructure project:

```bash
cd SimpleECommerceBackend.Infrastructure
```

Create a new initial migration:

```bash
dotnet ef migrations add InitialCreate --startup-project ../SimpleECommerceBackend.Api
```

This will create a clean migration without the Credentials table and without dbo schema references.

#### 6.8.2: Review Generated Migration

**File**: `SimpleECommerceBackend.Infrastructure/Persistence/Migrations/YYYYMMDDHHMMSS_InitialCreate.cs`

The migration should create tables for your business entities (UserProfiles, Orders, Carts, etc.) but **NOT** include a Credentials table.

Verify that:

- ✅ No Credentials table is created
- ✅ No dbo schema is specified in any table
- ✅ UserProfile table is created with Id as Guid (will store Keycloak user ID)
- ✅ All your business tables are included

**Example migration (should look similar):**

```csharp
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleECommerceBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create UserProfiles table (no Credentials table!)
            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    // ... other columns
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });

            // Create other tables (Orders, Carts, etc.)
            // ...
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserProfiles");
            // Drop other tables
        }
    }
}
```

#### 6.8.3: Apply Migration

**For Development:**

```bash
dotnet ef database update --startup-project ../SimpleECommerceBackend.Api
```

This will create your database with the new clean schema.

#### 6.8.4: Verify Database Schema

After applying the migration, verify in your database that:

- ✅ Credentials table does NOT exist
- ✅ UserProfiles table exists with Id column (uniqueidentifier)
- ✅ All tables are in the default dbo schema (or no explicit schema)
- ✅ All business tables are created correctly
- ✅ Foreign keys and indexes are properly set up

**Using Azure Data Studio or SQL Server Management Studio:**

```sql
-- Check if Credentials table exists (should return 0)
SELECT COUNT(*)
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = 'Credentials';

-- Check UserProfiles table structure
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'UserProfiles';

-- List all tables
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;
```

#### 6.8.5: For Production

Generate a SQL script for review before applying to production:

```bash
dotnet ef migrations script --startup-project ../SimpleECommerceBackend.Api --output InitialCreate.sql

# Review the SQL script before applying to production
# Apply using your preferred database deployment tool
```

---

## Verification Checklist

After completing this phase, verify the following:

- [x] UserProfile entity updated with documentation about Keycloak user ID
- [x] Credential entity removed or archived
- [x] IPasswordHasher interface removed
- [x] BCryptPasswordHasher implementation removed
- [x] IJwtGenerator interface removed (or kept for validation only)
- [x] JwtGenerator implementation removed (or updated)
- [x] dbo schema removed from all entity configurations
- [x] All `.ToTable("TableName", "dbo")` changed to `.ToTable("TableName")`
- [x] DependencyInjection.cs updated with Keycloak services
- [x] DependencyInjection.cs has no references to removed services
- [x] Old migrations folder cleaned up (deleted or archived)
- [x] Old database dropped or migration history cleared
- [x] New InitialCreate migration created successfully
- [x] Migration script reviewed (no Credentials table, no dbo schema)
- [x] Database migration applied to development database
- [x] Credentials table does NOT exist in database
- [x] All tables created without explicit dbo schema
- [x] UserProfiles.Id is uniqueidentifier (for Keycloak user ID)
- [x] No foreign key constraints errors
- [x] Application builds successfully
- [x] No compilation errors in any layer

---

## Troubleshooting

### Issue: "Cannot find entity configurations"

**Solution:**

- Check the correct path: `SimpleECommerceBackend.Infrastructure/Persistence/Configurations/`
- Entity configurations might be in subdirectories (Auth, Business, etc.)
- Use IDE's find in files feature to search for `.ToTable(`

### Issue: "Migration creates Credentials table"

**Solution:**

- Ensure Credential entity is removed from Domain layer
- Ensure CredentialConfiguration is removed or not applied
- Check DbContext doesn't have `DbSet<Credential>`
- Delete migration and recreate after fixing

### Issue: "Build errors after removing interfaces"

**Solution:**

- Search project for references to removed interfaces
- Remove all using statements for removed namespaces
- Check if any use cases still reference old services
- Phases 3-5 should have already updated these references

### Issue: "DbContext still references Credentials"

**Solution:**

- Remove `DbSet<Credential>` from AppDbContext
- Remove any configuration for Credentials in `OnModelCreating`
- Remove CredentialConfiguration.cs file
- Rebuild the solution

### Issue: "EF Core can't find migrations"

**Solution:**

- Ensure you're in the correct directory when running commands
- Use `--startup-project` flag to specify the API project
- Check that AppDbContext is properly registered in DI

### Issue: "Database still has Credentials table after migration"

**Solution:**

- You may have applied old migration before cleaning up
- Solution: Drop database and recreate with new migration
- Or manually drop Credentials table from database

---

## Next Steps

Once Phase 6 is complete and verified:

➡️ **Proceed to [Phase 7: Testing & Validation](./KEYCLOAK_IMPLEMENTATION_PHASE_7.md)**

Phase 7 will cover comprehensive testing including unit tests, integration tests, manual API testing, and authorization policy validation.
