# Phase 6: Domain & Infrastructure Updates

**Status**: ⬜ Not Started  
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
   - [Step 6.5: Update DependencyInjection](#step-65-update-dependencyinjection)
   - [Step 6.6: Database Migration](#step-66-database-migration)
4. [Verification Checklist](#verification-checklist)
5. [Troubleshooting](#troubleshooting)
6. [Next Steps](#next-steps)

---

## Objectives

- ✅ Update UserProfile entity to use Keycloak user ID
- ✅ Remove or archive Credential entity
- ✅ Remove password hashing infrastructure
- ✅ Remove custom JWT generation logic
- ✅ Update dependency injection configuration
- ✅ Create and apply database migration
- ✅ Clean up authentication-related code

---

## Prerequisites

- [ ] Phase 1-5 completed
- [ ] Backup of current database
- [ ] Understanding of Entity Framework migrations
- [ ] Git commit of current working state (for easy rollback)

---

## Implementation Steps

### Step 6.1: Update UserProfile Entity

**File**: `SimpleECommerceBackend.Domain/Entities/Business/UserProfile.cs`

#### 6.1.1: Review Current Entity Structure

The UserProfile entity should already be using `Id` as the primary key. Ensure it's documented that this `Id` now represents the Keycloak user ID.

#### 6.1.2: Add Documentation Comment

Update the entity with clear documentation:

```csharp
namespace SimpleECommerceBackend.Domain.Entities.Business;

/// <summary>
/// Represents a user's business profile in the application.
/// The Id field stores the Keycloak user ID (sub claim) for authentication correlation.
/// </summary>
public class UserProfile : Entity
{
    // Id inherited from Entity - represents Keycloak user ID
    public string Email { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string? NickName { get; private set; }
    public Sex Sex { get; private set; }
    public DateOnly BirthDate { get; private set; }
    public string? AvatarUrl { get; private set; }

    // Navigation properties
    public ICollection<Order> Orders { get; private set; } = null!;
    public ICollection<CustomerShippingAddress> ShippingAddresses { get; private set; } = null!;
    public Cart? Cart { get; private set; }
    public SellerShop? SellerShop { get; private set; }

    private UserProfile() { } // For EF Core

    public static UserProfile Create(
        Guid keycloakUserId,
        string email,
        string firstName,
        string lastName,
        string? nickName,
        Sex sex,
        DateOnly birthDate,
        string? avatarUrl)
    {
        return new UserProfile
        {
            Id = keycloakUserId, // Set Keycloak user ID as primary key
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            NickName = nickName,
            Sex = sex,
            BirthDate = birthDate,
            AvatarUrl = avatarUrl
        };
    }

    // Update methods remain the same
    public void UpdateProfile(
        string firstName,
        string lastName,
        string? nickName,
        Sex sex,
        DateOnly birthDate,
        string? avatarUrl)
    {
        FirstName = firstName;
        LastName = lastName;
        NickName = nickName;
        Sex = sex;
        BirthDate = birthDate;
        AvatarUrl = avatarUrl;
    }
}
```

#### 6.1.3: Key Points

- **No structural changes needed** - The entity already uses `Id` as the primary key
- **Semantic change only** - `Id` now represents Keycloak user ID instead of custom generated ID
- **Documentation added** - Clear comments about the Keycloak relationship

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

### Step 6.5: Update DependencyInjection

**File**: `SimpleECommerceBackend.Infrastructure/DependencyInjection.cs`

#### 6.5.1: Update Infrastructure Registration

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

#### 6.5.2: Key Changes

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

### Step 6.6: Database Migration

#### 6.6.1: Create Migration to Remove Credentials Table

Navigate to the Infrastructure project:

```bash
cd SimpleECommerceBackend.Infrastructure
```

Create a new migration:

```bash
dotnet ef migrations add RemoveCredentialsTable --startup-project ../SimpleECommerceBackend.Api
```

#### 6.6.2: Review Generated Migration

**File**: `SimpleECommerceBackend.Infrastructure/Persistence/Migrations/YYYYMMDDHHMMSS_RemoveCredentialsTable.cs`

The migration should look similar to this:

```csharp
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleECommerceBackend.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCredentialsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key from UserProfiles if exists
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Credentials_CredentialId",
                table: "UserProfiles");

            // Drop index if exists
            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_CredentialId",
                table: "UserProfiles");

            // Drop CredentialId column from UserProfiles if exists
            migrationBuilder.DropColumn(
                name: "CredentialId",
                table: "UserProfiles");

            // Drop Credentials table
            migrationBuilder.DropTable(
                name: "Credentials");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate Credentials table for rollback
            migrationBuilder.CreateTable(
                name: "Credentials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credentials", x => x.Id);
                });

            // Recreate unique index on Email
            migrationBuilder.CreateIndex(
                name: "IX_Credentials_Email",
                table: "Credentials",
                column: "Email",
                unique: true);

            // Recreate CredentialId column in UserProfiles if needed
            migrationBuilder.AddColumn<Guid>(
                name: "CredentialId",
                table: "UserProfiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty);

            // Recreate foreign key
            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_CredentialId",
                table: "UserProfiles",
                column: "CredentialId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Credentials_CredentialId",
                table: "UserProfiles",
                column: "CredentialId",
                principalTable: "Credentials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
```

#### 6.6.3: Apply Migration

**For Development:**

```bash
dotnet ef database update --startup-project ../SimpleECommerceBackend.Api
```

**For Production:**

```bash
# Generate SQL script for review
dotnet ef migrations script --startup-project ../SimpleECommerceBackend.Api --output migration.sql

# Review the SQL script before applying to production
# Apply using your preferred database deployment tool
```

#### 6.6.4: Alternative - Manual Migration

If you prefer manual control, create the migration file manually:

```csharp
using Microsoft.EntityFrameworkCore.Migrations;

public partial class RemoveCredentialsForKeycloak : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // IMPORTANT: Backup data before dropping tables

        // Drop Credentials table
        migrationBuilder.Sql(@"
            IF OBJECT_ID('dbo.Credentials', 'U') IS NOT NULL
                DROP TABLE dbo.Credentials;
        ");

        // Add comment to UserProfiles.Id column
        migrationBuilder.Sql(@"
            EXEC sp_addextendedproperty
                @name = N'MS_Description',
                @value = N'Keycloak User ID (sub claim)',
                @level0type = N'SCHEMA', @level0name = N'dbo',
                @level1type = N'TABLE', @level1name = N'UserProfiles',
                @level2type = N'COLUMN', @level2name = N'Id';
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Cannot easily rollback - would need to recreate entire Credentials infrastructure
        throw new NotSupportedException("Rolling back from Keycloak to custom authentication is not supported.");
    }
}
```

---

## Verification Checklist

After completing this phase, verify the following:

- [ ] UserProfile entity uses Keycloak user ID
- [ ] Credential entity removed or archived
- [ ] IPasswordHasher interface removed
- [ ] BCryptPasswordHasher implementation removed
- [ ] IJwtGenerator interface removed (or kept for validation only)
- [ ] JwtGenerator implementation removed (or updated)
- [ ] DependencyInjection.cs updated with Keycloak services
- [ ] DependencyInjection.cs has no references to removed services
- [ ] Database migration created successfully
- [ ] Migration script reviewed
- [ ] Database migration applied to development database
- [ ] No foreign key constraints errors
- [ ] Application builds successfully
- [ ] No compilation errors in any layer

---

## Troubleshooting

### Issue: "Cannot drop table because it is referenced by a foreign key"

**Solution:**

- Ensure you drop foreign keys before dropping the table
- Check if any other tables reference Credentials
- Update migration to drop all foreign keys first

### Issue: "Credentials table does not exist"

**Solution:**

- Check if table was already removed
- Review previous migrations
- This is not an error if the table was never created

### Issue: "Build errors after removing interfaces"

**Solution:**

- Search project for references to removed interfaces
- Remove all using statements for removed namespaces
- Check if any use cases still reference old services

### Issue: "DbContext still references Credentials"

**Solution:**

- Remove `DbSet<Credential>` from AppDbContext
- Remove any configuration for Credentials in `OnModelCreating`
- Rebuild the solution

### Issue: "Migration fails with constraint errors"

**Solution:**

- Backup your database first
- Drop constraints manually before running migration
- Review migration SQL script for correct order of operations

---

## Next Steps

Once Phase 6 is complete and verified:

➡️ **Proceed to [Phase 7: Testing & Validation](./KEYCLOAK_IMPLEMENTATION_PHASE_7.md)**

Phase 7 will cover comprehensive testing including unit tests, integration tests, manual API testing, and authorization policy validation.
