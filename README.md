# EV Service Center Maintenance Management System (EV_SCMMS)

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15+-336791.svg)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

**Phần mềm quản lý bảo dưỡng xe điện cho trung tâm dịch vụ**

Hệ thống quản lý toàn diện cho trung tâm bảo dưỡng và sửa chữa xe điện, hỗ trợ đầy đủ quy trình từ đặt lịch, quản lý dịch vụ, phụ tùng, nhân sự đến tài chính và báo cáo.

---

## 📋 Mục lục

- [Tổng quan](#-tổng-quan)
- [Kiến trúc hệ thống](#-kiến-trúc-hệ-thống)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Tính năng chính](#-tính-năng-chính)
- [Cài đặt và triển khai](#-cài-đặt-và-triển-khai)
- [Cấu trúc dự án](#-cấu-trúc-dự-án)
- [Database Schema](#-database-schema)
- [API Documentation](#-api-documentation)
- [Đóng góp](#-đóng-góp)

---

## 🎯 Tổng quan

EV_SCMMS là hệ thống quản lý bảo dưỡng xe điện được xây dựng theo kiến trúc **Clean Architecture**, đảm bảo tính mở rộng, bảo trì và kiểm thử cao. Hệ thống phục vụ 4 nhóm người dùng chính:

- **Customer (Khách hàng)**: Theo dõi xe, đặt lịch dịch vụ, quản lý chi phí
- **Staff (Nhân viên)**: Tiếp nhận yêu cầu, quản lý lịch hẹn
- **Technician (Kỹ thuật viên)**: Thực hiện bảo dưỡng, ghi nhận tình trạng xe
- **Admin (Quản trị viên)**: Quản lý toàn bộ hệ thống, báo cáo, phân tích

---

## 🏗️ Kiến trúc hệ thống

Dự án được tổ chức theo **Clean Architecture** với 3 layer chính:

```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│      (EV_SCMMS.WebAPI)                  │
│   • Controllers                          │
│   • API Endpoints                        │
│   • Authentication & Authorization       │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│         Application Layer                │
│      (EV_SCMMS.Infrastructure)          │
│   • Services Implementation              │
│   • Repository Implementation            │
│   • External Services                    │
│   • Data Access (EF Core)                │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│           Domain Layer                   │
│       (EV_SCMMS.Core)                   │
│   • Entities                             │
│   • Interfaces                           │
│   • DTOs                                 │
│   • Business Rules                       │
└─────────────────────────────────────────┘
```

### Nguyên tắc thiết kế

- **Dependency Inversion**: Các layer phụ thuộc vào abstraction, không phụ thuộc vào implementation
- **Separation of Concerns**: Mỗi layer có trách nhiệm riêng biệt
- **Repository Pattern**: Abstraction cho data access
- **Unit of Work Pattern**: Quản lý transactions
- **SOLID Principles**: Đảm bảo code dễ bảo trì và mở rộng

---

## 🛠️ Công nghệ sử dụng

### Backend Framework
- **ASP.NET Core 8.0** - Web API framework
- **Entity Framework Core 8.0.11** - ORM
- **PostgreSQL 15+** - Relational database

### Authentication & Security
- **ASP.NET Core Identity 8.0.11** - User management
- **JWT Bearer Authentication** - Token-based auth
- **BCrypt** - Password hashing

### Development Tools
- **Swagger/OpenAPI** - API documentation
- **EF Core Migrations** - Database versioning
- **.NET CLI** - Build and development tools

### NuGet Packages

**EV_SCMMS.Core:**
```xml
<PackageReference Include="Microsoft.Extensions.Identity.Stores" Version="8.0.11" />
```

**EV_SCMMS.Infrastructure:**
```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.11" />
<PackageReference Include="Microsoft.Extensions.Identity.Core" Version="8.0.11" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.11" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.0.2" />
```

**EV_SCMMS.WebAPI:**
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.11" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.15" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
```

---

## ✨ Tính năng chính

### 1. Chức năng cho Khách hàng (Customer)

#### a. Theo dõi xe & Nhắc nhở
- ✅ Nhắc nhở bảo dưỡng định kỳ theo km hoặc thời gian
- ✅ Nhắc thanh toán gói bảo dưỡng định kỳ
- ✅ Thông báo gia hạn gói dịch vụ
- ✅ Dashboard tổng quan tình trạng xe

#### b. Đặt lịch dịch vụ
- ✅ Đặt lịch bảo dưỡng/sửa chữa trực tuyến
- ✅ Chọn trung tâm dịch vụ & loại dịch vụ
- ✅ Xem lịch trống của trung tâm
- ✅ Nhận xác nhận & thông báo trạng thái:
  - Chờ xác nhận
  - Đang bảo dưỡng
  - Hoàn tất
- ✅ Push notification real-time

#### c. Quản lý hồ sơ & Chi phí
- ✅ Lưu lịch sử bảo dưỡng xe điện đầy đủ
- ✅ Quản lý chi phí bảo dưỡng & sửa chữa theo từng lần
- ✅ Xem báo giá & hóa đơn
- ✅ Thanh toán online (e-wallet, banking, credit card)
- ✅ Thống kê chi phí theo thời gian

### 2. Chức năng cho Trung tâm dịch vụ (Staff, Technician, Admin)

#### a. Quản lý khách hàng & Xe
- ✅ Hồ sơ khách hàng chi tiết
- ✅ Thông tin xe (model, VIN, năm sản xuất)
- ✅ Lịch sử dịch vụ đầy đủ
- ✅ Chat trực tuyến với khách hàng
- ✅ Quản lý khách hàng thân thiết

#### b. Quản lý lịch hẹn & Dịch vụ
- ✅ Tiếp nhận yêu cầu đặt lịch của khách hàng
- ✅ Xác nhận/từ chối lịch hẹn
- ✅ Lập lịch cho kỹ thuật viên
- ✅ Quản lý hàng chờ thông minh
- ✅ Phiếu tiếp nhận dịch vụ điện tử
- ✅ Checklist EV chuẩn hóa
- ✅ Calendar view tổng quan

#### c. Quản lý quy trình bảo dưỡng
- ✅ Theo dõi tiến độ từng xe theo workflow:
  - Chờ tiếp nhận
  - Đang chẩn đoán
  - Đang bảo dưỡng
  - Kiểm tra chất lượng
  - Hoàn tất
- ✅ Ghi nhận tình trạng xe trước/sau bảo dưỡng
- ✅ Chụp ảnh/video minh chứng
- ✅ Báo cáo vấn đề phát hiện thêm
- ✅ Kanban board quản lý workflow

#### d. Quản lý phụ tùng
- ✅ Theo dõi số lượng phụ tùng EV tại trung tâm
- ✅ Kiểm soát lượng tồn kho tối thiểu
- ✅ Cảnh báo phụ tùng sắp hết
- ✅ **AI gợi ý nhu cầu phụ tùng thay thế**
- ✅ **Đề xuất lượng tồn kho tối ưu dựa trên ML**
- ✅ Lịch sử nhập/xuất phụ tùng
- ✅ Quản lý nhà cung cấp

#### e. Quản lý nhân sự
- ✅ Phân công kỹ thuật viên theo ca/lịch
- ✅ Theo dõi hiệu suất làm việc
- ✅ Thống kê thời gian xử lý công việc
- ✅ Quản lý chứng chỉ chuyên môn EV
- ✅ Nhắc nhở gia hạn chứng chỉ
- ✅ Đánh giá KPI kỹ thuật viên

#### f. Quản lý tài chính & Báo cáo
- ✅ Báo giá dịch vụ tự động/thủ công
- ✅ Tạo hóa đơn điện tử
- ✅ Thanh toán online/offline
- ✅ Quản lý doanh thu, chi phí, lợi nhuận
- ✅ Thống kê loại dịch vụ phổ biến
- ✅ Phân tích xu hướng hỏng hóc EV
- ✅ Dashboard tài chính real-time
- ✅ Export báo cáo Excel/PDF

---

## 🚀 Cài đặt và triển khai

### Yêu cầu hệ thống

- **.NET SDK 8.0** hoặc cao hơn
- **PostgreSQL 15+**
- **Git**

### 1. Clone repository

```bash
git clone https://github.com/EV-Service-Center-SWD392/EV_SCMMS_BE.git
cd EV_SCMMS_BE
```

### 2. Cài đặt PostgreSQL

#### Windows (sử dụng PostgreSQL installer)
```bash
# Download từ https://www.postgresql.org/download/windows/
# Cài đặt và nhớ password cho user postgres
```

#### macOS (sử dụng Homebrew)
```bash
brew install postgresql@15
brew services start postgresql@15
```

#### Linux (Ubuntu/Debian)
```bash
sudo apt update
sudo apt install postgresql-15 postgresql-contrib-15
sudo systemctl start postgresql
```

### 3. Tạo database

```bash
# Đăng nhập vào PostgreSQL
psql -U postgres

# Tạo database
CREATE DATABASE ev_scmms_db;

# Tạo user (optional)
CREATE USER ev_scmms_user WITH PASSWORD 'your_password';
GRANT ALL PRIVILEGES ON DATABASE ev_scmms_db TO ev_scmms_user;

# Thoát
\q
```

### 4. Cấu hình Connection String

Tạo file `appsettings.Development.json` trong `src/EV_SCMMS.WebAPI/`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ev_scmms_db;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "SecretKey": "oooooo!",
    "Issuer": "EV_SCMMS_API",
    "Audience": "EV_SCMMS_Client",
    "ExpiryInMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

**⚠️ Lưu ý**: File này đã được thêm vào `.gitignore` để bảo mật thông tin nhạy cảm.

### 5. Restore NuGet packages

```bash
cd src
dotnet restore
```

### 6. Chạy Entity Framework Migrations

```bash
# Di chuyển đến Infrastructure project
cd src/EV_SCMMS.Infrastructure

# Tạo migration đầu tiên
dotnet ef migrations add InitialCreate --startup-project ../EV_SCMMS.WebAPI

# Apply migration vào database
dotnet ef database update --startup-project ../EV_SCMMS.WebAPI
```

### 7. Build solution

```bash
# Quay lại root folder
cd ../..

# Build toàn bộ solution
dotnet build
```

### 8. Chạy ứng dụng

```bash
# Di chuyển đến WebAPI project
cd src/EV_SCMMS.WebAPI

# Chạy ứng dụng
dotnet run

# Hoặc với watch mode (tự động reload)
dotnet watch run
```

### 9. Truy cập ứng dụng

- **API Base URL**: `https://localhost:5001` hoặc `http://localhost:5000`
- **Swagger UI**: `https://localhost:5001/swagger` hoặc `http://localhost:5000`
- **Health Check**: `https://localhost:5001/health`

---

## 📁 Cấu trúc dự án

```
EV_SCMMS/
├── EV_SCMMS.sln                          # Solution file
├── README.md                              # Documentation
├── .gitignore                             # Git ignore rules
│
└── src/
    ├── EV_SCMMS.Core/                    # Domain & Application Layer
    │   ├── Domain/
    │   │   ├── Entities/                  # Domain entities
    │   │   │   ├── ApplicationUser.cs     # Identity user
    │   │   │   ├── User.cs                # Business user
    │   │   │   ├── Vehicle.cs             # Vehicle entity
    │   │   │   ├── ServiceAppointment.cs  # Appointment
    │   │   │   ├── MaintenanceRecord.cs   # Maintenance history
    │   │   │   ├── Part.cs                # Spare parts
    │   │   │   ├── Technician.cs          # Technician
    │   │   │   └── Invoice.cs             # Financial
    │   │   ├── Enums/                     # Domain enums
    │   │   │   ├── RoleEnum.cs
    │   │   │   ├── AppointmentStatus.cs
    │   │   │   └── MaintenanceStatus.cs
    │   │   └── Services/                  # Domain services
    │   │
    │   └── Application/
    │       ├── Interfaces/
    │       │   ├── Repositories/          # Repository contracts
    │       │   │   ├── IGenericRepository.cs
    │       │   │   ├── IUserRepository.cs
    │       │   │   ├── IVehicleRepository.cs
    │       │   │   ├── IAppointmentRepository.cs
    │       │   │   └── IPartRepository.cs
    │       │   ├── Services/              # Service contracts
    │       │   │   ├── IAuthService.cs
    │       │   │   ├── IUserService.cs
    │       │   │   ├── IVehicleService.cs
    │       │   │   ├── IAppointmentService.cs
    │       │   │   ├── IMaintenanceService.cs
    │       │   │   ├── IPartService.cs
    │       │   │   ├── ITechnicianService.cs
    │       │   │   ├── IInvoiceService.cs
    │       │   │   ├── INotificationService.cs
    │       │   │   └── ITokenService.cs
    │       │   └── IUnitOfWork.cs
    │       ├── DTOs/                      # Data Transfer Objects
    │       │   ├── Auth/
    │       │   │   ├── LoginDto.cs
    │       │   │   ├── RegisterDto.cs
    │       │   │   └── AuthResultDto.cs
    │       │   ├── User/
    │       │   ├── Vehicle/
    │       │   ├── Appointment/
    │       │   └── Maintenance/
    │       ├── Results/                   # Result patterns
    │       │   └── ServiceResult.cs
    │       ├── Enums/                     # Application enums
    │       │   └── StatusEnum.cs
    │       └── Base/                      # Base classes
    │           └── PagedResult.cs
    │
    ├── EV_SCMMS.Infrastructure/          # Infrastructure Layer
    │   ├── Persistence/
    │   │   ├── AppDbContext.cs           # EF Core DbContext
    │   │   ├── UnitOfWork.cs             # UoW implementation
    │   │   └── Repositories/             # Repository implementations
    │   │       ├── GenericRepository.cs
    │   │       ├── UserRepository.cs
    │   │       ├── VehicleRepository.cs
    │   │       ├── AppointmentRepository.cs
    │   │       └── PartRepository.cs
    │   ├── Services/                     # Service implementations
    │   │   ├── AuthService.cs
    │   │   ├── TokenService.cs
    │   │   ├── UserService.cs
    │   │   ├── VehicleService.cs
    │   │   ├── AppointmentService.cs
    │   │   ├── MaintenanceService.cs
    │   │   ├── PartService.cs
    │   │   └── NotificationService.cs
    │   ├── Identity/
    │   │   └── IdentityConfig.cs         # Identity configuration
    │   └── External/                     # External services
    │       ├── EmailService.cs
    │       ├── SmsService.cs
    │       └── PaymentService.cs
    │
    └── EV_SCMMS.WebAPI/                  # Presentation Layer
        ├── Controllers/                   # API Controllers
        │   ├── AuthController.cs          # Authentication
        │   ├── UserController.cs          # User management
        │   ├── VehicleController.cs       # Vehicle management
        │   ├── AppointmentController.cs   # Appointments
        │   ├── MaintenanceController.cs   # Maintenance
        │   ├── PartController.cs          # Parts inventory
        │   ├── TechnicianController.cs    # Technician
        │   └── InvoiceController.cs       # Financial
        ├── Program.cs                     # Application entry point
        ├── appsettings.json               # Configuration template
        ├── appsettings.Development.json   # Dev configuration (gitignored)
        └── EV_SCMMS.WebAPI.http          # HTTP test requests
```


### API Testing với HTTP Client

Sử dụng file `EV_SCMMS.WebAPI.http` trong Visual Studio/Rider hoặc REST Client extension trong VS Code.

---

## 🔒 Security Best Practices

### Authentication
- ✅ JWT Bearer token với expiry time
- ✅ Refresh token rotation
- ✅ Password hashing với Identity
- ✅ Role-based authorization

### Data Protection
- ✅ Connection string trong environment variables
- ✅ Secrets không commit vào Git
- ✅ HTTPS enforced
- ✅ CORS policy

### Database
- ✅ SQL injection prevention (EF Core parameterized queries)
- ✅ Input validation
- ✅ Data encryption for sensitive fields



### Coding Standards
- Follow C# coding conventions
- Write XML documentation comments
- Write unit tests for new features
- Keep PRs focused and small

---

## 📝 License

Distributed under the MIT License. See `LICENSE` for more information.

---

## 👥 Team

**EV Service Center SWD392**

- GitHub: [@EV-Service-Center-SWD392](https://github.com/EV-Service-Center-SWD392)

---

## 📞 Liên hệ

Project Link: [https://github.com/EV-Service-Center-SWD392/EV_SCMMS_BE](https://github.com/EV-Service-Center-SWD392/EV_SCMMS_BE)

---

## 🙏 Acknowledgments

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Clean Architecture by Uncle Bob](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

Made with ❤️ by EV Service Center Team
