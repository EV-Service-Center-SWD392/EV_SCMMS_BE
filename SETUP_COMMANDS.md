# EV_SCMMS Solution Setup Commands

## Các lệnh CLI để tạo solution EV_SCMMS với Clean Architecture + PostgreSQL

### 1. Tạo Solution và cấu trúc thư mục

```bash
# Di chuyển đến thư mục dự án
cd /Users/huynh/Projects/EV_SCMMS

# Tạo solution file
dotnet new sln -n EV_SCMMS

# Tạo thư mục src
mkdir -p src
```

### 2. Tạo các Projects

```bash
# Di chuyển vào thư mục src
cd src

# Tạo Core project (Class Library)
dotnet new classlib -n EV_SCMMS.Core

# Tạo Infrastructure project (Class Library)
dotnet new classlib -n EV_SCMMS.Infrastructure

# Tạo WebAPI project (ASP.NET Core Web API)
dotnet new webapi -n EV_SCMMS.WebAPI
```

### 3. Thêm projects vào solution

```bash
# Quay lại thư mục root
cd ..

# Thêm các projects vào solution
dotnet sln add src/EV_SCMMS.Core/EV_SCMMS.Core.csproj
dotnet sln add src/EV_SCMMS.Infrastructure/EV_SCMMS.Infrastructure.csproj
dotnet sln add src/EV_SCMMS.WebAPI/EV_SCMMS.WebAPI.csproj
```

### 4. Thêm project references

```bash
# Infrastructure references Core
cd src/EV_SCMMS.Infrastructure
dotnet add reference ../EV_SCMMS.Core/EV_SCMMS.Core.csproj

# WebAPI references Core và Infrastructure
cd ../EV_SCMMS.WebAPI
dotnet add reference ../EV_SCMMS.Core/EV_SCMMS.Core.csproj
dotnet add reference ../EV_SCMMS.Infrastructure/EV_SCMMS.Infrastructure.csproj
```

### 5. Tạo cấu trúc thư mục cho Core project

```bash
cd ../EV_SCMMS.Core

# Tạo thư mục Domain
mkdir -p Domain/Entities
mkdir -p Domain/Enums
mkdir -p Domain/Services
mkdir -p Domain/ValueObjects

# Tạo thư mục Application
mkdir -p Application/Interfaces/Repositories
mkdir -p Application/Interfaces/Services
mkdir -p Application/DTOs
mkdir -p Application/Results
mkdir -p Application/Enums
mkdir -p Application/Base

# Xóa file mặc định
rm Class1.cs
```

### 6. Tạo cấu trúc thư mục cho Infrastructure project

```bash
cd ../EV_SCMMS.Infrastructure

# Tạo thư mục Persistence
mkdir -p Persistence/Repositories

# Tạo thư mục Services
mkdir -p Services

# Tạo thư mục Identity
mkdir -p Identity

# Tạo thư mục External
mkdir -p External

# Xóa file mặc định
rm Class1.cs
```

### 7. Tạo cấu trúc thư mục cho WebAPI project

```bash
cd ../EV_SCMMS.WebAPI

# Tạo thư mục Controllers (nếu chưa có)
mkdir -p Controllers
```

### 8. Thêm NuGet packages

#### EV_SCMMS.Core
```bash
cd src/EV_SCMMS.Core

# Thêm Identity Stores cho ApplicationUser (IdentityUser)
dotnet add package Microsoft.Extensions.Identity.Stores --version 8.0.11
```

#### EV_SCMMS.Infrastructure
```bash
cd ../EV_SCMMS.Infrastructure

# Thêm Entity Framework Core 8.0.11
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.11

# Thêm PostgreSQL provider 8.0.11
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.11

# Thêm EF Core Tools 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.11

# Thêm ASP.NET Core Identity với EF Core 8.0.11
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.11

# Thêm Identity Core 8.0.11 (cho UserManager, RoleManager)
dotnet add package Microsoft.Extensions.Identity.Core --version 8.0.11

# Thêm JWT tokens 8.0.2
dotnet add package System.IdentityModel.Tokens.Jwt --version 8.0.2
```

#### EV_SCMMS.WebAPI
```bash
cd ../EV_SCMMS.WebAPI

# Thêm JWT Bearer Authentication 8.0.11
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.11
```

### 9. Build solution

```bash
# Quay lại thư mục root
cd ../..

# Build toàn bộ solution
dotnet build
```

### 10. Chạy ứng dụng

```bash
# Chạy WebAPI
cd src/EV_SCMMS.WebAPI
dotnet run

# Hoặc với watch mode (tự động reload khi có thay đổi)
dotnet watch run
```

## Cấu trúc cuối cùng

```
EV_SCMMS/
├── EV_SCMMS.sln
└── src/
    ├── EV_SCMMS.Core/ (Class Library)
    │   ├── Domain/
    │   │   ├── Entities/
    │   │   ├── Enums/
    │   │   ├── Services/
    │   │   └── ValueObjects/
    │   └── Application/
    │       ├── Interfaces/
    │       │   ├── Repositories/
    │       │   │   ├── IGenericRepository.cs
    │       │   │   ├── IUserRepository.cs
    │       │   ├── Services/
    │       │   │   ├── IUserService.cs
    │       │   │   └── IVehicleService.cs
    │       │   ├── IUnitOfWork.cs
    │       ├── DTOs/
    │       │   ├── UserDto.cs
    │       ├── Results/
    │       │   └── ServiceResult.cs
    │       ├── Enums/
    │       │   └── StatusEnum.cs
    │       └── Base/
    │           └── PagedResult.cs
    │
    ├── EV_SCMMS.Infrastructure/ (Class Library)
    │   ├── Persistence/
    │   │   ├── Repositories/
    │   │   │   ├── GenericRepository.cs
    │   │   │   ├── UserRepository.cs
    │   │   ├── UnitOfWork.cs
    │   │   └── AppDbContext.cs
    │   ├── Services/
    │   │   ├── UserService.cs
    │   ├── Identity/
    │   │   └── IdentityConfig.cs
    │   └── External/
    │       └── EmailService.cs
    │
    └── EV_SCMMS.WebAPI/ (ASP.NET Core Web API)
        ├── Controllers/
        │   ├── UserController.cs
        └── Program.cs
```

## Dependencies giữa các projects

- **EV_SCMMS.Core**: Không phụ thuộc vào project nào (Domain Layer)
- **EV_SCMMS.Infrastructure**: Phụ thuộc vào Core (Implementation Layer)
- **EV_SCMMS.WebAPI**: Phụ thuộc vào Core và Infrastructure (Presentation Layer)

## NuGet Packages đã cài đặt

### EV_SCMMS.Core
- `Microsoft.Extensions.Identity.Stores` (8.0.11) - Cho ApplicationUser kế thừa IdentityUser

### EV_SCMMS.Infrastructure
- `Microsoft.EntityFrameworkCore` (8.0.11) - ORM framework
- `Npgsql.EntityFrameworkCore.PostgreSQL` (8.0.11) - PostgreSQL database provider
- `Microsoft.EntityFrameworkCore.Tools` (8.0.11) - EF Core CLI tools cho migrations
- `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (8.0.11) - ASP.NET Core Identity với EF Core
- `Microsoft.Extensions.Identity.Core` (8.0.11) - Identity core (UserManager, RoleManager, SignInManager)
- `System.IdentityModel.Tokens.Jwt` (8.0.2) - JWT token generation và validation

### EV_SCMMS.WebAPI
- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.11) - JWT Bearer authentication middleware
- `Microsoft.AspNetCore.OpenApi` (8.0.15) - OpenAPI/Swagger support (mặc định)
- `Swashbuckle.AspNetCore` (6.6.2) - Swagger UI (mặc định)

**Tất cả packages đều sử dụng phiên bản 8.x để tương thích hoàn toàn với .NET 8.0**

## Cấu hình PostgreSQL Connection String

Thêm vào `appsettings.json` trong project WebAPI:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ev_scmms_db;Username=postgres;Password=your_password"
  }
}
```

## Entity Framework Migrations

```bash
# Tạo migration đầu tiên
cd src/EV_SCMMS.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../EV_SCMMS.WebAPI

# Apply migration vào database
dotnet ef database update --startup-project ../EV_SCMMS.WebAPI
```

## Cài lại/Cập nhật NuGet Packages

### Cách 1: Xóa và cài lại tất cả packages (khuyến nghị)

#### EV_SCMMS.Core
```bash
cd src/EV_SCMMS.Core

# Xóa packages
dotnet remove package Microsoft.Extensions.Identity.Stores

# Cài lại với phiên bản 8.0.11
dotnet add package Microsoft.Extensions.Identity.Stores --version 8.0.11
```

#### EV_SCMMS.Infrastructure
```bash
cd ../EV_SCMMS.Infrastructure

# Xóa tất cả packages
dotnet remove package Microsoft.EntityFrameworkCore
dotnet remove package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet remove package Microsoft.EntityFrameworkCore.Tools
dotnet remove package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet remove package Microsoft.Extensions.Identity.Core
dotnet remove package System.IdentityModel.Tokens.Jwt

# Cài lại với phiên bản 8.x
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.11
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 8.0.11
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.11
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 8.0.11
dotnet add package Microsoft.Extensions.Identity.Core --version 8.0.11
dotnet add package System.IdentityModel.Tokens.Jwt --version 8.0.2
```

#### EV_SCMMS.WebAPI
```bash
cd ../EV_SCMMS.WebAPI

# Xóa packages
dotnet remove package Microsoft.AspNetCore.Authentication.JwtBearer

# Cài lại với phiên bản 8.0.11
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.11
```

### Cách 2: Cập nhật trực tiếp trong file .csproj

#### EV_SCMMS.Core.csproj
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Extensions.Identity.Stores" Version="8.0.11" />
</ItemGroup>
```

#### EV_SCMMS.Infrastructure.csproj
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.11" />
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.11" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.11">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
  </PackageReference>
  <PackageReference Include="Microsoft.Extensions.Identity.Core" Version="8.0.11" />
  <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.11" />
  <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.0.2" />
</ItemGroup>
```

#### EV_SCMMS.WebAPI.csproj
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.11" />
  <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.15" />
  <PackageReference Include="Swashbuckle.AspNetCore" Version="6.6.2" />
</ItemGroup>
```

Sau đó chạy:
```bash
dotnet restore
dotnet build
```

## Notes

- Solution sử dụng **.NET 8.0**
- Database: **PostgreSQL** (qua Npgsql provider)
- Architecture: **Clean Architecture** (Domain-Driven Design)
- Pattern: **Repository Pattern + Unit of Work**
- Result Pattern: **ServiceResult** cho error handling
