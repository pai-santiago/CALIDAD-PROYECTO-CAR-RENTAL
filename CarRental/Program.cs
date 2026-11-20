using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CarRental.Security;
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using CarRentalServices.Repositories;
using CarRentalServices.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IMySqlDataService, MySqlDataService>();
builder.Services.AddScoped<IAuthUserRepository, MySqlAuthUserRepository>();
builder.Services.AddScoped<IPasswordHasher<AuthUser>, PasswordHasher<AuthUser>>();
builder.Services.AddKeyedScoped<IBrandRepository, BrandRepository>("MySql");
builder.Services.AddKeyedScoped<ICategoryRepository, MySqlCategoryRepository>("MySql");
builder.Services.AddKeyedScoped<ICarRepository, CarRepository>("MySql");
builder.Services.AddKeyedScoped<ICustomerRepository, CustomerRepository>("MySql");
builder.Services.AddKeyedScoped<ICountryRepository, CountryRepository>("MySql");
builder.Services.AddKeyedScoped<ILocationRepository, LocationRepository>("MySql");
builder.Services.AddKeyedScoped<IColorRepository, ColorRepository>("MySql");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Car Rental API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Token obtenido en POST /api/auth/login"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        }] = Array.Empty<string>()
    });
});

var connectionString = builder.Configuration.GetConnectionString("MysqlConnection");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Falta ConnectionStrings__MysqlConnection.");

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) && builder.Environment.IsDevelopment())
    jwtKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("Jwt:Key debe tener al menos 32 bytes.");

var issuer = builder.Configuration["Jwt:Issuer"] ?? "CarRental.Api";
var audience = builder.Configuration["Jwt:Audience"] ?? "CarRental.Client";
var expirationMinutes = builder.Configuration.GetValue("Jwt:ExpireMinutes", 60);
builder.Services.AddSingleton(new JwtTokenService(jwtKey, issuer, audience, expirationMinutes));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            NameClaimType = "sub",
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var mysql = scope.ServiceProvider.GetRequiredService<IMySqlDataService>();
    mysql.ExecuteNonQuery("""
        CREATE TABLE IF NOT EXISTS auth_roles (
            Id TINYINT UNSIGNED NOT NULL AUTO_INCREMENT,
            Name VARCHAR(32) NOT NULL,
            PRIMARY KEY (Id),
            UNIQUE KEY UX_auth_roles_Name (Name)
        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
        """);
    mysql.ExecuteNonQuery("INSERT IGNORE INTO auth_roles (Id, Name) VALUES (1, 'User'), (2, 'Admin');");
    mysql.ExecuteNonQuery("""
        CREATE TABLE IF NOT EXISTS auth_users (
            Id BIGINT NOT NULL AUTO_INCREMENT,
            Username VARCHAR(60) NOT NULL,
            NormalizedUsername VARCHAR(60) NOT NULL,
            Email VARCHAR(254) NOT NULL,
            NormalizedEmail VARCHAR(254) NOT NULL,
            PasswordHash VARCHAR(512) NOT NULL,
            RoleId TINYINT UNSIGNED NOT NULL DEFAULT 1,
            CreatedAtUtc DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
            PRIMARY KEY (Id),
            UNIQUE KEY UX_auth_users_NormalizedUsername (NormalizedUsername),
            UNIQUE KEY UX_auth_users_NormalizedEmail (NormalizedEmail),
            CONSTRAINT FK_auth_users_auth_roles FOREIGN KEY (RoleId) REFERENCES auth_roles (Id)
        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
        """);
}

app.Run();
