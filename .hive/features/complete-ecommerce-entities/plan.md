# Complete E-Commerce Backend Implementation

## Overview
Implement all entities, repositories, use cases, and endpoints based on the provided ER diagram following Clean Architecture principles.

## Architecture Layers
- **Domain**: Entities, Value Objects, Enums, Domain Exceptions
- **Application**: Use Cases, Interfaces (IUnitOfWork, Repositories)
- **Infrastructure**: DbContext, Entity Configurations, Repository Implementations
- **Api**: Controllers, DTOs, Mapping Configurations

---

## Tasks

### 1. Create Domain Enums
Create all enum types needed across entities:
- UserStatus (Active, Inactive, Suspended, Deleted)
- UserSex (Male, Female, Other, PreferNotToSay)
- ProductStatus (Draft, Active, OutOfStock, Discontinued)
- OrderStatus (Pending, Confirmed, Processing, Shipped, Delivered, Cancelled, Refunded)
- PaymentStatus (Pending, Processing, Completed, Failed, Refunded)
- PaymentMethod (CreditCard, DebitCard, BankTransfer, EWallet, CashOnDelivery)
- Currency (USD, VND, EUR, etc.)

**Files**: SimpleECommerceBackend.Domain/Enums/*.cs

### 2. Create Domain Value Objects
Create value objects for better domain modeling:
- Money (Amount, Currency) - for prices and payments
- Address (AddressLine, Ward, Province) - for shipping addresses
- PhoneNumber - with validation
- OrderCode - unique order identifier

**Files**: SimpleECommerceBackend.Domain/ValueObjects/*.cs

### 3. Create Core Domain Entities - Authentication & Authorization
Implement entities:
- Credential (Id, Email, PasswordHash, RoleId, Role navigation, UserProfile navigation, CreatedAt, UpdatedAt)
- Role (Id, Name, Credentials collection, RolePermissions collection)
- Permission (Id, Name, Description, RolePermissions collection)
- RolePermission (Id, RoleId, PermissionId, Role navigation, Permission navigation)

**Files**: SimpleECommerceBackend.Domain/Entities/*.cs

### 4. Create Domain Entities - User Management
Implement entities:
- UserProfile (Id, Email, FirstName, LastName, NickName, Sex, BirthDate, AvatarUrl, Status, CredentialId, Credential navigation, collections for ShippingAddresses, Products, Orders, Cart, CreatedAt, UpdatedAt)
- UserShippingAddress (Id, ReceiverName, PhoneNumber, AddressLine, Ward, Province, IsDefault, CustomerId, Customer navigation, IsDeleted, DeletedAt, CreatedAt, UpdatedAt)

Include soft delete logic in UserShippingAddress.

**Files**: SimpleECommerceBackend.Domain/Entities/*.cs

### 5. Create Domain Entities - Product Catalog
Implement entities:
- Category (Id, Name, Description, AdministratorId, Administrator navigation, Products collection, CreatedAt, UpdatedAt)
- Product (Id, Name, Description, CurrentAmount, Currency, Status, SellerId, Seller navigation, CategoryId, Category navigation, ProductPrices collection, ProductImages collection, Inventory navigation, CreatedAt, UpdatedAt)
- ProductPrice (Id, Amount, Currency, EffectiveFrom, ProductId, Product navigation, CreatedAt, UpdatedAt)
- ProductImage (Id, ImageUrl, DisplayOrder, Description, ProductId, Product navigation, CreatedAt)
- Inventory (Id, ProductId, Product navigation, QuantityOnHand, QuantityReserved, Version, CreatedAt, UpdatedAt)

Add domain methods for inventory management (Reserve, Release, Adjust).

**Files**: SimpleECommerceBackend.Domain/Entities/*.cs

### 6. Create Domain Entities - Shopping Cart
Implement entities:
- Cart (Id, CustomerId, Customer navigation, CartItems collection, CreatedAt, UpdatedAt)
- CartItem (Id, ProductId, Product navigation, CartId, Cart navigation, Quantity)

Add domain methods for cart operations (AddItem, UpdateQuantity, RemoveItem, Clear, CalculateTotal).

**Files**: SimpleECommerceBackend.Domain/Entities/*.cs

### 7. Create Domain Entities - Order Management
Implement entities:
- Order (Id, Code, Note, TotalPrice, Currency, Status, OrderShippingAddressId, ShippingAddress navigation, CustomerId, Customer navigation, OrderItems collection, Payments collection, CreatedAt, UpdatedAt)
- OrderItem (Id, ProductId, Product navigation, OrderId, Order navigation, CurrentAmount, Quantity, CreatedAt, UpdatedAt)
- OrderShippingAddress (Id, ReceiverName, PhoneNumber, AddressLine, Ward, Province)

Add domain methods for order operations (CalculateTotal, CancelOrder, UpdateStatus).

**Files**: SimpleECommerceBackend.Domain/Entities/*.cs

### 8. Create Domain Entities - Payment & Notifications
Implement entities:
- Payment (Id, OrderId, Order navigation, Amount, Currency, Method, Provider, Status, ExternalTransactionId, CreatedAt, UpdatedAt)
- Notification (Id, UserId, User navigation, Message, IsRead, CreatedAt)

**Files**: SimpleECommerceBackend.Domain/Entities/*.cs

### 9. Create Domain Repository Interfaces
Create repository interfaces following Repository pattern:
- ICredentialRepository
- IRoleRepository
- IPermissionRepository
- IUserProfileRepository
- IUserShippingAddressRepository
- ICategoryRepository
- IProductRepository
- IInventoryRepository
- ICartRepository
- IOrderRepository
- IPaymentRepository
- INotificationRepository

Each with standard CRUD + specific query methods (e.g., GetByEmailAsync, GetActiveProductsAsync, etc.).

**Files**: SimpleECommerceBackend.Domain/Interfaces/Repositories/*.cs

### 10. Update IUnitOfWork Interface
Add all repository properties to IUnitOfWork interface and ensure SaveChangesAsync method.

**Files**: SimpleECommerceBackend.Application/Interfaces/IUnitOfWork.cs

### 11. Create ApplicationDbContext
Create EF Core DbContext with:
- DbSet properties for all entities
- Override OnModelCreating for entity configurations
- Implement audit trail (CreatedAt, UpdatedAt) via SaveChangesAsync override
- Configure soft delete query filters

**Files**: SimpleECommerceBackend.Infrastructure/Persistence/ApplicationDbContext.cs

### 12. Create Entity Configurations
Create IEntityTypeConfiguration for each entity:
- Configure primary keys, indexes, constraints
- Configure relationships and foreign keys
- Configure value conversions (enums, value objects)
- Configure decimal precision for money fields
- Configure required/optional fields, max lengths
- Configure query filters for soft delete
- Configure cascade delete behaviors

**Files**: SimpleECommerceBackend.Infrastructure/Persistence/Configurations/*.cs

### 13. Implement Repository Classes
Implement concrete repository classes:
- Base GenericRepository with common CRUD operations
- Specific repositories inheriting from GenericRepository
- Use EF Core for data access
- Implement specific query methods

**Files**: SimpleECommerceBackend.Infrastructure/Repositories/*.cs

### 14. Implement UnitOfWork Class
Create UnitOfWork implementation:
- Initialize all repositories
- Implement SaveChangesAsync with transaction support
- Implement IDisposable pattern

**Files**: SimpleECommerceBackend.Infrastructure/Persistence/UnitOfWork.cs

### 15. Update Infrastructure DependencyInjection
Register all services:
- ApplicationDbContext with connection string
- UnitOfWork and repositories
- Configure EF Core options (query filters, etc.)

**Files**: SimpleECommerceBackend.Infrastructure/DependencyInjection.cs

### 16. Create Database Migration
Generate EF Core migration for all entities and apply to database.

**Command**: `dotnet ef migrations add InitialECommerceSchema`

### 17. Create Use Cases - User Management
Implement use cases:
- RegisterUser
- UpdateUserProfile
- GetUserProfile
- ManageShippingAddresses (Add, Update, Delete, SetDefault, List)

**Files**: SimpleECommerceBackend.Application/UseCases/Users/*.cs

### 18. Create Use Cases - Role & Permission Management
Implement use cases:
- CreateRole, UpdateRole, DeleteRole, GetRoles
- CreatePermission, GetPermissions
- AssignPermissionToRole, RemovePermissionFromRole

**Files**: SimpleECommerceBackend.Application/UseCases/Roles/*.cs

### 19. Create Use Cases - Product Catalog Management
Implement use cases:
- CreateCategory, UpdateCategory, DeleteCategory, GetCategories
- CreateProduct, UpdateProduct, DeleteProduct, GetProducts, GetProductById, SearchProducts
- AddProductImage, RemoveProductImage, UpdateProductImageOrder
- UpdateProductPrice (adds new ProductPrice entry)
- AdjustInventory

**Files**: SimpleECommerceBackend.Application/UseCases/Products/*.cs

### 20. Create Use Cases - Shopping Cart
Implement use cases:
- GetCart (create if not exists)
- AddToCart
- UpdateCartItemQuantity
- RemoveFromCart
- ClearCart
- GetCartSummary (with total calculation)

**Files**: SimpleECommerceBackend.Application/UseCases/Cart/*.cs

### 21. Create Use Cases - Order Management
Implement use cases:
- CreateOrder (from cart, with inventory reservation)
- GetOrders (with filtering, pagination)
- GetOrderById
- CancelOrder (release inventory)
- UpdateOrderStatus

**Files**: SimpleECommerceBackend.Application/UseCases/Orders/*.cs

### 22. Create Use Cases - Payment Processing
Implement use cases:
- InitiatePayment
- ProcessPaymentCallback
- GetPaymentStatus
- RequestRefund

**Files**: SimpleECommerceBackend.Application/UseCases/Payments/*.cs

### 23. Create Use Cases - Notifications
Implement use cases:
- SendNotification
- GetUserNotifications
- MarkNotificationAsRead
- MarkAllNotificationsAsRead

**Files**: SimpleECommerceBackend.Application/UseCases/Notifications/*.cs

### 24. Create DTOs - Request Models
Create request DTOs for all use cases:
- User DTOs (RegisterUserRequest, UpdateUserProfileRequest, etc.)
- Product DTOs (CreateProductRequest, UpdateProductRequest, etc.)
- Cart DTOs (AddToCartRequest, UpdateCartItemRequest, etc.)
- Order DTOs (CreateOrderRequest, UpdateOrderStatusRequest, etc.)
- Payment DTOs
- Address DTOs

**Files**: SimpleECommerceBackend.Api/DTOs/Requests/*.cs

### 25. Create DTOs - Response Models
Create response DTOs for all use cases:
- User DTOs (UserProfileResponse, ShippingAddressResponse, etc.)
- Product DTOs (ProductResponse, ProductListResponse, CategoryResponse, etc.)
- Cart DTOs (CartResponse, CartSummaryResponse, etc.)
- Order DTOs (OrderResponse, OrderListResponse, etc.)
- Payment DTOs
- Pagination DTOs (PagedResponse<T>)

**Files**: SimpleECommerceBackend.Api/DTOs/Responses/*.cs

### 26. Create Mapping Configurations
Create AutoMapper profiles for entity-DTO mappings:
- UserMappingProfile
- ProductMappingProfile
- CartMappingProfile
- OrderMappingProfile
- PaymentMappingProfile

**Files**: SimpleECommerceBackend.Api/Mapping/*.cs

### 27. Create API Controllers - User Management
Implement controllers:
- UserProfileController (GET profile, PUT update, GET shipping addresses, POST/PUT/DELETE shipping address)

Use proper HTTP methods, status codes, and authorization attributes.

**Files**: SimpleECommerceBackend.Api/Controllers/UserProfileController.cs

### 28. Create API Controllers - Product Catalog
Implement controllers:
- CategoryController (CRUD operations)
- ProductController (CRUD, search, filter by category, pagination)
- ProductImageController (upload, delete, reorder)
- InventoryController (adjust, check availability)

**Files**: SimpleECommerceBackend.Api/Controllers/Category*.cs, Product*.cs, Inventory*.cs

### 29. Create API Controllers - Shopping & Orders
Implement controllers:
- CartController (GET cart, POST add item, PUT update item, DELETE remove item, DELETE clear)
- OrderController (POST create, GET list orders, GET order by id, PUT cancel, PUT update status)

**Files**: SimpleECommerceBackend.Api/Controllers/CartController.cs, OrderController.cs

### 30. Create API Controllers - Payments & Notifications
Implement controllers:
- PaymentController (POST initiate, POST callback/webhook, GET status, POST refund)
- NotificationController (GET list, PUT mark as read, PUT mark all as read)

**Files**: SimpleECommerceBackend.Api/Controllers/PaymentController.cs, NotificationController.cs

### 31. Add Validation & Business Rules
Implement:
- FluentValidation validators for all request DTOs
- Domain validation in entity constructors
- Business rule validation in use cases (e.g., sufficient inventory, valid order status transitions)

**Files**: Throughout Application layer and Domain entities

### 32. Add Error Handling & Logging
Enhance:
- Domain exceptions for business rule violations
- Global exception handler to map exceptions to appropriate HTTP responses
- Logging in use cases and controllers

**Files**: Domain/Exceptions/*.cs, Api/Middleware/GlobalExceptionHandlingMiddleware.cs

### 33. Configure Authentication & Authorization
Implement:
- JWT authentication setup
- Role-based authorization
- Permission-based authorization (custom policy provider)
- Protect endpoints with [Authorize] attributes

**Files**: Infrastructure/Security/*.cs, Api/Program.cs

### 34. Add Seeding Data
Create database seeder for:
- Default roles (Admin, Seller, Customer)
- Default permissions
- Role-permission assignments
- Test categories and products

**Files**: Infrastructure/Persistence/Seeders/*.cs

### 35. Add API Documentation
Configure Swagger/OpenAPI:
- Add XML documentation comments
- Configure Swagger UI
- Add authentication to Swagger
- Group endpoints by feature

**Files**: Api/Program.cs, XML comments in controllers

### 36. Add Integration Tests
Create integration tests:
- Test database operations
- Test use cases end-to-end
- Test API endpoints
- Test authentication flows

**Files**: SimpleECommerceBackend.IntegrationTest/*/*.cs

### 37. Add Unit Tests
Create unit tests:
- Test domain entities and business logic
- Test use cases
- Test repository logic
- Test value objects

**Files**: SimpleECommerceBackend.UnitTests/*/*.cs

### 38. Update Configuration Files
Update:
- appsettings.json with connection strings, JWT settings, etc.
- .env files for environment-specific settings
- Docker configuration if needed

**Files**: Api/appsettings*.json, .env.*

### 39. Create README Documentation
Document:
- Project structure
- Setup instructions
- API endpoints overview
- Development guidelines
- Testing instructions

**Files**: README.md

### 40. Final Review & Testing
- Run all unit tests
- Run all integration tests
- Test all API endpoints manually or with Postman
- Verify database schema
- Code review and cleanup
