using Microsoft.EntityFrameworkCore;
using SanSaludAPI.DataAccess;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// ==========================================
// 1. DataAccess Layer Configuration
// ==========================================
// Para cambiar de motor de base de datos (e.g. a SQL Server, PostgreSQL, MySQL),
// simplemente debes:
// 1. Instalar el paquete NuGet del proveedor correspondiente (ej. Npgsql.EntityFrameworkCore.PostgreSQL)
// 2. Cambiar ".UseSqlite" por el método correspondiente (ej. ".UseNpgsql")
// 3. Actualizar la cadena de conexión en appsettings.json.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SanSaludDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<SanSaludAPI.DataAccess.ITurnoRepository, SanSaludAPI.DataAccess.TurnoRepository>();
builder.Services.AddScoped<SanSaludAPI.DataAccess.IMedicoRepository, SanSaludAPI.DataAccess.MedicoRepository>();
builder.Services.AddScoped<SanSaludAPI.DataAccess.IPacienteRepository, SanSaludAPI.DataAccess.PacienteRepository>();

// ==========================================
// 2. BusinessLogic Layer Configuration
// ==========================================
builder.Services.AddScoped<SanSaludAPI.BusinessLogic.ITurnoService, SanSaludAPI.BusinessLogic.TurnoService>();
builder.Services.AddScoped<SanSaludAPI.BusinessLogic.IMedicoService, SanSaludAPI.BusinessLogic.MedicoService>();
builder.Services.AddScoped<SanSaludAPI.BusinessLogic.IPacienteService, SanSaludAPI.BusinessLogic.PacienteService>();

var app = builder.Build();

// ==========================================
// 3. Database Initialization
// ==========================================
// Al iniciar, verifica si la base de datos existe.
// Si no existe, se crea aplicando las migraciones y se cargan datos de ejemplo.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SanSaludDbContext>();
    await DbInitializer.InitializeAsync(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }

