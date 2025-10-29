using System.Text;
using EV_SCMMS.Core.Application.Interfaces;
using EV_SCMMS.Core.Application.Interfaces.Services;
using EV_SCMMS.Infrastructure.Persistence;
using EV_SCMMS.Infrastructure.Services;
using EV_SCMMS.WebAPI.Authorization;
using EV_SCMMS.WebAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container
builder.Services.AddControllers();

// Configure Database
builder.Services.AddDbContext<AppDbContext>(
    options =>
    {
        // Cấu hình PostgreSQL
        options.UseNpgsql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly("EV_SCMMS.Infrastructure");
                npgsqlOptions.CommandTimeout(60); // timeout 60s để tránh request treo lâu
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 1,
                    maxRetryDelay: TimeSpan.FromSeconds(15),
                    errorCodesToAdd: null);
            });
        
        // Fix timezone issue
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        // Logging & debugging — chỉ bật khi đang ở môi trường dev
        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging(); // hiển thị parameter trong query
            options.EnableDetailedErrors();       // hiển thị lỗi chi tiết
            options.LogTo(Console.WriteLine, LogLevel.Information);
        }
        else
        {
            // Trong production chỉ log cảnh báo trở lên để tiết kiệm hiệu năng
            options.LogTo(Console.WriteLine, LogLevel.Warning);
        }
    },
    ServiceLifetime.Scoped // 🔒 Scoped để mỗi request có 1 DbContext riêng
);


// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    // Default policy: JWT validation + refresh token validation (ensures token not revoked)
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new ValidRefreshTokenRequirement())
        .Build();

    // JwtOnly policy for token management endpoints (login, refresh, revoke)
    options.AddPolicy("JwtOnly", new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

    // Role-based policies (still include refresh token validation by default)
    options.AddPolicy("AdminOnly", new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole("ADMIN")
        .AddRequirements(new ValidRefreshTokenRequirement())
        .Build());

    options.AddPolicy("TechnicianAndStaff", new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole("TECHNICIAN", "STAFF")
        .AddRequirements(new ValidRefreshTokenRequirement())
        .Build());
});


// Register Application Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Authorization Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthorizationHandler, ValidRefreshTokenHandler>();

// Register Authentication Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();

// Register Business Services for Spare Parts Management
builder.Services.AddScoped<ICenterService, CenterService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<ISparepartService, SparepartService>();
builder.Services.AddScoped<ISparepartTypeService, SparepartTypeService>();
builder.Services.AddScoped<ISparepartForecastService, SparepartForecastService>();
builder.Services.AddScoped<ISparepartReplenishmentRequestService, SparepartReplenishmentRequestService>();
builder.Services.AddScoped<ISparepartUsageHistoryService, SparepartUsageHistoryService>();
builder.Services.AddScoped<IWorkScheduleService, WorkScheduleService>();
builder.Services.AddScoped<IUserWorkScheduleService, UserWorkScheduleService>();
builder.Services.AddScoped<IUserAccountService, UserAccountService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IBookingApprovalService, BookingApprovalService>();
builder.Services.AddScoped<IServiceIntakeService, ServiceIntakeService>();
builder.Services.AddScoped<IChecklistService, ChecklistService>();
builder.Services.AddScoped<IChecklistItemService, ChecklistItemService>();
builder.Services.AddScoped<IWorkOrderService, WorkOrderService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<IUserCertificateService, UserCertificateService>();

// Register ChatBot AI Service
builder.Services.AddHttpClient<IChatBotService, ChatBotService>();
builder.Services.AddScoped<IChatBotService, ChatBotService>();

// Add Basic Health Checks
builder.Services.AddHealthChecks();

// Configure Swagger/OpenAPI with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EV_SCMMS API",
        Version = "v1.0.0",
        Description = "EV Service Center Maintenance Management System API",
        Contact = new OpenApiContact
        {
            Name = "EV_SCMMS SWD392 Group 7"
        }
    });

    // Enable XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVercel",
        policy => policy
            .WithOrigins("https://ev-web-fe.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();
// Configure the HTTP request pipeline

// Use custom middleware in correct order (ExceptionHandling -> Logging -> Performance)
// app.UseMiddleware<ExceptionHandlingMiddleware>();
// app.UseMiddleware<RequestLoggingMiddleware>();
// app.UseMiddleware<PerformanceMonitoringMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EV_SCMMS API V1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at app's root
});

app.UseHttpsRedirection();

app.UseCors("AllowVercel");

app.UseAuthentication();
app.UseAuthorization();

// Add Health Check endpoints
app.MapHealthChecks("/health");

app.MapControllers();


app.Run();
