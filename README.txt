****README


REST API para gestión de clientes, cuentas, movimientos y reportes (JSON y PDF).
Stack: .NET 8, EF Core, SQL Server, QuestPDF, FluentValidation.
Arquitectura: MVC + Services + DTOs + Validations.
Contenedores: Docker + docker-compose (API + SQL Server).

Requirements

.NET SDK 8.0 (para compilar local)

Docker Desktop (para levantar con contenedores)

(Opcional) SQL Server local si no usas Docker

Quick start (Docker)
# desde la carpeta Bank.Api/
docker compose up --build
# API: http://localhost:8080/swagger
# SQL Server: localhost,1433 (sa / <tu_password>)

Local run (sin Docker)

Ajusta appsettings.json → "ConnectionStrings:Default" a tu SQL Server local.

Ejecuta migraciones y seed:

dotnet ef database update
# el seed corre en Program.cs al iniciar la app
dotnet run


API en http://localhost:8080/swagger.

Configuration

appsettings.json:

{
  "ConnectionStrings": { "Default": "Server=localhost,1433;Database=BankDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True" },
  "DailyDebitLimit": 1000
}


DailyDebitLimit: tope diario de débitos.

El seeding crea clientes/cuentas base si la DB está vacía.

Endpoints (resumen)

GET /api/clients, POST /api/clients, PUT /api/clients/{id}, DELETE /api/clients/{id}

GET /api/accounts, POST /api/accounts, PUT /api/accounts/{id}, DELETE /api/accounts/{id}

GET /api/movements (filtros: accountId, from, to)

POST /api/movements

DELETE /api/movements/{id}

GET /api/reports?clientId=&from=&to=&format=json|pdf

Errores de negocio (p.ej. “Saldo no disponible”, “Cupo diario excedido”) retornan 400 con { "error": "<mensaje>" }.

Project structure
Bank.Api/
 ├─ Controllers/
 ├─ Domain/
 ├─ Infrastructure/         # AppDbContext, Migrations, DbInitializer
 ├─ Middleware/             # ErrorHandlingMiddleware
 ├─ Services/               # ClientService, AccountService, MovementService, ReportService
 ├─ Validation/             # DTOs + FluentValidators
 ├─ docker-compose.yml
 ├─ Dockerfile
 ├─ appsettings.json
 ├─ docs/
 │   ├─ postman/BankApp.postman_collection.json
 │   └─ sql/BankDb.sql
 └─ README.md

Testing with Postman

Importa docs/postman/BankApp.postman_collection.json.
Variables:

baseUrl = http://localhost:8080/api

Colección incluye:

CRUD Clientes/Cuentas

Casos de Movimientos (saldo insuficiente, tope diario)

Reporte JSON/PDF

PDF Reports

Generados con QuestPDF (Community).

Archivo PDF devuelto como base64 si format=pdf.

Notes

Valida en backend con FluentValidation + DTOs.

Manejo de errores: ErrorHandlingMiddleware.

CORS habilitado para el frontend AngularJS.