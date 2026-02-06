# Clean Architecture (Nguyên lý tổng quát, không phụ thuộc ngôn ngữ)

## 1. Clean Architecture là gì?
Clean Architecture là cách tổ chức hệ thống sao cho:
- Business rules không phụ thuộc kỹ thuật
- Thay đổi công nghệ không làm hỏng core
- Code dễ hiểu, dễ test, dễ tiến hóa

## 2. Nguyên lý cốt lõi

### 2.1 Dependency Rule
- Dependency luôn hướng vào trong
- Inner layer không biết outer layer tồn tại

### 2.2 Separation of Concerns
- Business ≠ Use case ≠ Technical detail

---

## 3. Các layer điển hình

### 3.1 Domain
- Chứa business rules cốt lõi
- Không biết database, framework, UI
- Là phần ít thay đổi nhất

### 3.2 Application
- Điều phối nghiệp vụ
- Xác định flow
- Quản lý transaction boundary

### 3.3 Infrastructure
- Kỹ thuật cụ thể
- Database, message queue, external service

### 3.4 Interface / Delivery
- API, UI, CLI
- Chỉ là cổng vào hệ thống

---

## 4. Entity & Business Modeling

### 4.1 Entity
- Có identity
- Có vòng đời
- Không phải DTO

### 4.2 Value Object
- Không identity
- Immutable
- Đại diện khái niệm nghiệp vụ

### 4.3 Status vs Delete
- Status = vòng đời nghiệp vụ
- Delete = quyết định kỹ thuật
- Entity lịch sử không bao giờ bị xóa

---

## 5. Transaction & Consistency

### 5.1 Unit of Work
- Một business operation = một Unit of Work
- Commit là quyết định của Application

### 5.2 Transaction
- Là cơ chế kỹ thuật
- Phục vụ Unit of Work
- Không phải business concept

---

## 6. Thời gian & Side Effect
- Thời gian là dependency
- Side-effect phải được kiểm soát
- Business không phụ thuộc system state

---

## 7. Những sai lầm phổ biến
- Dùng Clean Architecture như template folder
- Đưa mọi thứ vào Utils
- Domain phụ thuộc framework
- Lạm dụng soft delete

---

## 8. Khi nào KHÔNG cần Clean Architecture?
- Script nhỏ
- Tool tạm
- Prototype ngắn hạn

---

## 9. Tư duy chốt
Clean Architecture không nhằm mục tiêu:
- Viết code phức tạp
- Tăng số layer

Mà nhằm:
- Giữ business đúng
- Giữ hệ thống bền
- Giữ quyết định kỹ thuật có thể thay đổi
