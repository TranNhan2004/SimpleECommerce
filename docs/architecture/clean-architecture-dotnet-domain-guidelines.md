# Clean Architecture với .NET (Hướng dẫn thực chiến)

## 1. Tổng quan kiến trúc
Clean Architecture chia hệ thống thành các vòng đồng tâm, trong đó **dependency luôn hướng vào trong**.

Thứ tự từ trong ra ngoài:
- Domain
- Application
- Infrastructure
- Presentation (API / UI)

## 2. Domain Layer (Quan trọng nhất)

### 2.1 Trách nhiệm
- Chứa **business rules cốt lõi**
- Không phụ thuộc framework, database, UI
- Là phần **ổn định nhất** của hệ thống

### 2.2 Những thứ nên có trong Domain
- Entity (Aggregate Root, Entity phụ)
- Value Object
- Domain Exception
- Domain Service (logic thuần, không state)
- Enum thể hiện vòng đời (Status)

### 2.3 Những thứ KHÔNG nên có trong Domain
- DbContext, EF Core
- HTTP, Controller, DTO
- Transaction, SaveChanges
- Logger, Cache
- Clock hệ thống trực tiếp (DateTime.Now)

### 2.4 Quy tắc thiết kế Entity
- Không public setter
- Mọi thay đổi state phải qua method có nghĩa
- Không expose setter trực tiếp cho Status vòng đời
- Constructor / Create không nhận status tự do
- Status dùng cho lifecycle, không dùng soft delete cho entity lịch sử

### 2.5 Soft Delete vs Status
- Soft delete + TTL: chỉ dùng cho ownership data (UserShippingAddress)
- Status (Active / Inactive / Archived): dùng cho Category, Product, User
- Entity xuất hiện trong Order: **không bao giờ hard delete**

### 2.6 Thời gian trong Domain
- Không dùng DateTime.Now
- Dùng abstraction (IClock)
- Clock chỉ xuất hiện khi ảnh hưởng invariant

---

## 3. Application Layer

### 3.1 Trách nhiệm
- Điều phối use case
- Xác định transaction boundary
- Gọi repository
- Gọi SaveChanges

### 3.2 Những thứ nên có
- Use case / Command / Query handler
- DTO cho input/output
- Validation orchestration
- IUnitOfWork

### 3.3 Những thứ không nên có
- Business rule chi tiết
- EF Core logic
- SQL

### 3.4 Unit of Work
- Đặt ở Application
- Một use case = một Unit of Work
- SaveChangesAsync là commit point
- Transaction explicit chỉ khi có side-effect hoặc nhiều commit

---

## 4. Infrastructure Layer

### 4.1 Trách nhiệm
- Triển khai kỹ thuật
- Không chứa business logic

### 4.2 Những thứ nên có
- DbContext (EF Core)
- Repository implementation
- Migration
- External services
- Background job

### 4.3 Những thứ không nên có
- Business rule
- Validation nghiệp vụ

### 4.4 Database & Delete strategy
- Financial / historical data: không delete
- FK có thể SET NULL nếu đã snapshot dữ liệu
- Unique constraint nên dùng filtered index (active-only)

---

## 5. Những lỗi phổ biến cần tránh
- Nhét logic vào Utils
- Repository gọi SaveChanges
- Domain biết transaction
- Soft delete cho mọi entity
- Status setter public

---

## 6. Tư duy chốt
Clean Architecture không phải để code nhiều layer hơn, mà để:
- Business rõ ràng
- Thay đổi không lan
- Hệ thống sống lâu
