using Microsoft.EntityFrameworkCore;
using TripSplit.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Додаємо підтримку контролерів
builder.Services.AddControllers();

// Отримуємо рядок підключення до PostgreSQL
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

// Підключаємо Entity Framework Core та PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// OpenAPI доступний у режимі розробки
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Перенаправлення HTTP -> HTTPS
app.UseHttpsRedirection();

// Підключаємо маршрути контролерів
app.MapControllers();

app.Run();