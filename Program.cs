using Bank.Api.Infrastructure;
using Bank.Api.Middleware;
using Bank.Api.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; // 👈 importante para SwaggerDoc

var builder = WebApplication.CreateBuilder(args);

// Db
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Services
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IMovementService, MovementService>();

// Validators
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Bank API",
        Version = "v1"
    });
});

var app = builder.Build();

// Endpoint de salud
app.MapGet("/health", () => Results.Ok(new { ok = true, time = DateTime.UtcNow }));

// Security headers simples
app.Use(async (ctx, next) =>
{
    ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
    ctx.Response.Headers["X-Frame-Options"] = "DENY";
    ctx.Response.Headers["Referrer-Policy"] = "no-referrer";
    await next();
});

app.UseMiddleware<ErrorHandlingMiddleware>();

// Swagger SIEMPRE (útil en Docker)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bank API v1");
    c.RoutePrefix = "swagger"; // para /swagger
});

// Rutas
app.MapControllers();

// Migraciones + seed (si quieres, rodea con try/catch o condiciona por entorno)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbInitializer.SeedAsync(db);
}

app.Run();
