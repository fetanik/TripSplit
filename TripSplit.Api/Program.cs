using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TripSplit.Api.Data;
using TripSplit.Api.Models;

var builder =
    WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString =
    builder.Configuration
        .GetConnectionString(
            "DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string " +
        "'DefaultConnection' was not found.");

builder.Services.AddDbContext<AppDbContext>(
    options =>
        options.UseNpgsql(
            connectionString));

builder.Services
    .AddIdentityCore<ApplicationUser>(
        options =>
        {
            options.Password.RequiredLength = 6;

            options.Password.RequireDigit = true;

            options.Password.RequireLowercase = true;

            options.Password.RequireUppercase = false;

            options.Password
                .RequireNonAlphanumeric = false;

            options.User
                .RequireUniqueEmail = true;
        })
    .AddEntityFrameworkStores<AppDbContext>();

var jwtKey =
    builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT key was not configured.");

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"]
    ?? "TripSplit.Api";

var jwtAudience =
    builder.Configuration["Jwt:Audience"]
    ?? "TripSplit.Mobile";

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
        options =>
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,

                    ValidateAudience = true,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,

                    ValidIssuer =
                        jwtIssuer,

                    ValidAudience =
                        jwtAudience,

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Convert.FromBase64String(
                                jwtKey)),

                    ClockSkew =
                        TimeSpan.Zero
                };
        });

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();