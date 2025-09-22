using Bank.Api.Infrastructure;
using Bank.Api.Middleware;
using Bank.Api.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;



QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IMovementService, MovementService>();


if (!builder.Environment.IsEnvironment("Testing"))
{
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
}


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bank API", Version = "v1" });
});


builder.Services.AddCors(o =>
{
o.AddPolicy("allow-front", p => p
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod());
});

var app = builder.Build();


app.MapGet("/health", () => Results.Ok(new { ok = true, time = DateTime.UtcNow }));


if (!app.Environment.IsEnvironment("Testing"))
{
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await db.Database.MigrateAsync();
await DbInitializer.SeedAsync(db);
}


app.UseCors("allow-front");

app.Use(async (ctx, next) =>
{
ctx.Response.Headers["X-Content-Type-Options"] = "nosniff";
ctx.Response.Headers["X-Frame-Options"] = "DENY";
ctx.Response.Headers["Referrer-Policy"] = "no-referrer";
await next();
});


app.UseMiddleware<ErrorHandlingMiddleware>();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bank API v1");
c.RoutePrefix = "swagger";
});

app.MapControllers();

app.Run();
public partial class Program { }
