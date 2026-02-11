# E-Commerce ER Diagram Analysis

## Entities from Diagram

### 1. **Credentials**
- PK: id
- Email (string)
- PasswordHash (string)
- RoleId (FK to Roles)
- CreatedAt, UpdatedAt (timestamps)

### 2. **Roles**
- PK: id
- Name (string)

### 3. **Permissions**
- PK: id
- Name (string)
- Description (string)

### 4. **RolePermissions** (Join table)
- PK: id
- RoleId (FK to Roles)
- PermissionId (FK to Permissions)

### 5. **UserProfiles**
- PK: id
- Email (string)
- FirstName (string)
- LastName (string)
- NickName (string)
- Sex (string/enum)
- BirthDate (datetime)
- AvatarUrl (string)
- Status (string/enum)
- CredentialId (FK to Credentials)
- CreatedAt, UpdatedAt (timestamps)

### 6. **UserShippingAddresses**
- PK: id
- ReceiverName (string)
- PhoneNumber (string)
- AddressLine (string)
- Ward (string)
- Province (string)
- IsDefault (bool)
- CustomerId (FK to UserProfiles)
- IsDeleted (bool)
- DeletedAt (nullable datetime)
- CreatedAt, UpdatedAt (timestamps)

### 7. **Categories**
- PK: id
- Name (string)
- Description (string)
- AdministratorId (FK to UserProfiles)
- CreatedAt, UpdatedAt (timestamps)

### 8. **Products**
- PK: id
- Name (string)
- Description (string)
- CurrentAmount (decimal)
- Currency (string/enum)
- Status (string/enum)
- SellerId (FK to UserProfiles)
- CategoryId (FK to Categories)
- CreatedAt, UpdatedAt (timestamps)

### 9. **ProductPrices** (Price history)
- PK: id
- Amount (decimal)
- Currency (string/enum)
- EffectiveFrom (datetime)
- ProductId (FK to Products)
- CreatedAt, UpdatedAt (timestamps)

### 10. **ProductImages**
- PK: id
- ImageUrl (string)
- DisplayOrder (int)
- Description (string)
- ProductId (FK to Products)
- CreatedAt (timestamp)

### 11. **Inventories**
- PK: id
- ProductId (FK to Products)
- QuantityOnHand (int)
- QuantityReserved (int)
- Version (int) - for optimistic locking
- CreatedAt, UpdatedAt (timestamps)

### 12. **Carts**
- PK: id
- CustomerId (FK to UserProfiles)
- CreatedAt, UpdatedAt (timestamps)

### 13. **CartItems**
- PK: id
- ProductId (FK to Products)
- CartId (FK to Carts)
- Quantity (int)

### 14. **Orders**
- PK: id
- Code (string) - Order number
- Note (string)
- TotalPrice (decimal)
- Currency (string/enum)
- Status (string/enum)
- OrderShippingAddressId (FK to OrderShippingAddresses)
- CustomerId (FK to UserProfiles)
- CreatedAt, UpdatedAt (timestamps)

### 15. **OrderItems**
- PK: id
- ProductId (FK to Products)
- OrderId (FK to Orders)
- CurrentAmount (decimal) - Price at time of order
- Quantity (int)
- CreatedAt, UpdatedAt (timestamps)

### 16. **OrderShippingAddresses** (Snapshot of shipping address)
- PK: id
- ReceiverName (string)
- PhoneNumber (string)
- AddressLine (string)
- Ward (string)
- Province (string)

### 17. **Payments**
- PK: id
- OrderId (FK to Orders)
- Amount (decimal)
- Currency (string/enum)
- Method (string/enum)
- Provider (string)
- Status (string/enum)
- ExternalTransactionId (string)
- CreatedAt, UpdatedAt (timestamps)

### 18. **Notifications**
- PK: id
- UserId (FK to UserProfiles)
- Message (string)
- IsRead (bool)
- CreatedAt (timestamp)

## Key Relationships

1. **Credentials** (1) -> (1) **UserProfiles**
2. **Roles** (1) -> (*) **Credentials**
3. **Roles** (*) <-> (*) **Permissions** via **RolePermissions**
4. **UserProfiles** (1) -> (*) **UserShippingAddresses**
5. **UserProfiles** (1) -> (*) **Categories** (as Administrator)
6. **UserProfiles** (1) -> (*) **Products** (as Seller)
7. **UserProfiles** (1) -> (1) **Carts**
8. **UserProfiles** (1) -> (*) **Orders** (as Customer)
9. **Categories** (1) -> (*) **Products**
10. **Products** (1) -> (*) **ProductPrices**
11. **Products** (1) -> (*) **ProductImages**
12. **Products** (1) -> (1) **Inventories**
13. **Products** (1) -> (*) **CartItems**
14. **Products** (1) -> (*) **OrderItems**
15. **Carts** (1) -> (*) **CartItems**
16. **Orders** (1) -> (*) **OrderItems**
17. **Orders** (1) -> (1) **OrderShippingAddresses**
18. **Orders** (1) -> (*) **Payments**

## Business Logic Patterns

### Soft Delete
- UserShippingAddresses uses soft delete (IsDeleted, DeletedAt)

### Audit Trail
- Most entities have CreatedAt, UpdatedAt timestamps

### Price History
- ProductPrices maintains historical pricing with EffectiveFrom

### Address Snapshot
- OrderShippingAddresses is a snapshot (not FK) to preserve historical data

### Inventory Management
- Inventories uses Version for optimistic concurrency control
- QuantityOnHand vs QuantityReserved for cart reservations

### Order Management
- Orders have unique Code for customer reference
- OrderItems capture CurrentAmount to preserve price at time of purchase
