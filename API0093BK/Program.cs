using API0093BK.Data;
using API0093BK.Helpers;
using API0093BK.Middleware;
using API0093BK.Repositories;
using API0093BK.Repositories.Interfaces;
using API0093BK.Services;
using API0093BK.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов в контейнер
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Настройка Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API0093BK",
        Version = "v1",
        Description = "Burger King Employee Management System"
    });

    // Настройка авторизации через JWT в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Пример: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new List<string>()
        }
    });
});

// Настройка подключения к базе данных
builder.Services.AddDbContext<API0093DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Настройка JWT аутентификации
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "my-super-secret-key-that-is-at-least-32-characters-long-2024";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Регистрация вспомогательных классов
builder.Services.AddSingleton(new JwtHelper(
    secretKey,
    jwtSettings["Issuer"] ?? "API0093BK",
    jwtSettings["Audience"] ?? "API0093BKClient"
));

// Регистрация репозиториев
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWishRepository, WishRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();

// Регистрация сервисов
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IWishService, WishService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Настройка конвейера HTTP запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();

// Создание администратора по умолчанию при первом запуске
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<API0093DbContext>();
    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

    // Создание базы данных, если её нет
    context.Database.EnsureCreated();

    // Создание администратора, если его нет
    var admin = await userRepository.GetUserByUsernameAsync("admin");
    if (admin == null)
    {
        var adminUser = new API0093BK.Models.User
        {
            Username = "admin",
            PasswordHash = PasswordHelper.HashPassword("Admin123!"),
            Email = "admin@burgerking.ru",
            FirstName = "System",
            LastName = "Administrator",
            Role = API0093BK.Models.UserRole.Administrator,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = 0,
            IsActive = true
        };

        await userRepository.AddAsync(adminUser);
        Console.WriteLine("Администратор создан: admin / Admin123!");
    }
}

app.Run();