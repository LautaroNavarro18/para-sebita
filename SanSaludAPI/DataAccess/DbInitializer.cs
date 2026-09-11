using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace SanSaludAPI.DataAccess
{
    /// <summary>
    /// Inicializa la base de datos al iniciar la aplicación:
    /// - Verifica si la base de datos existe; si no existe, la crea aplicando las migraciones.
    /// - Carga datos de ejemplo (3 registros por tabla) solo si las tablas están vacías.
    /// </summary>
    public static class DbInitializer
    {
        // Identificadores fijos para mantener la integridad referencial entre los datos de ejemplo
        private static readonly Guid MedicoCardiologiaId = new Guid("11111111-1111-1111-1111-111111111111");
        private static readonly Guid MedicoPediatriaId = new Guid("22222222-2222-2222-2222-222222222222");
        private static readonly Guid MedicoDermatologiaId = new Guid("33333333-3333-3333-3333-333333333333");

        private static readonly Guid PacienteJuanId = new Guid("44444444-4444-4444-4444-444444444444");
        private static readonly Guid PacienteMariaId = new Guid("55555555-5555-5555-5555-555555555555");
        private static readonly Guid PacientePedroId = new Guid("66666666-6666-6666-6666-666666666666");

        public static async Task InitializeAsync(SanSaludDbContext context)
        {
            // Si la base de datos no existe, crearla aplicando las migraciones pendientes
            var databaseCreator = context.Database.GetService<IRelationalDatabaseCreator>();
            if (!await databaseCreator.ExistsAsync())
            {
                await context.Database.MigrateAsync();
            }

            // Seed idempotente: solo se cargan datos si las tablas están vacías
            if (!await context.Medicos.AnyAsync())
            {
                await SeedMedicosAsync(context);
            }

            if (!await context.Pacientes.AnyAsync())
            {
                await SeedPacientesAsync(context);
            }

            if (!await context.Turnos.AnyAsync())
            {
                await SeedTurnosAsync(context);
            }
        }

        private static async Task SeedMedicosAsync(SanSaludDbContext context)
        {
            context.Medicos.AddRange(
                new Medico
                {
                    Id = MedicoCardiologiaId,
                    Nombre = "Dra. Ana Martínez",
                    Especialidad = "Cardiología",
                    Matricula = "MN12345"
                },
                new Medico
                {
                    Id = MedicoPediatriaId,
                    Nombre = "Dr. Luis Fernández",
                    Especialidad = "Pediatría",
                    Matricula = "MN23456"
                },
                new Medico
                {
                    Id = MedicoDermatologiaId,
                    Nombre = "Dra. Carla Sánchez",
                    Especialidad = "Dermatología",
                    Matricula = "MN34567"
                });

            await context.SaveChangesAsync();
        }

        private static async Task SeedPacientesAsync(SanSaludDbContext context)
        {
            context.Pacientes.AddRange(
                new Paciente
                {
                    Id = PacienteJuanId,
                    Nombre = "Juan Pérez",
                    DNI = "30123456",
                    Email = "juan.perez@mail.com"
                },
                new Paciente
                {
                    Id = PacienteMariaId,
                    Nombre = "María López",
                    DNI = "30987654",
                    Email = "maria.lopez@mail.com"
                },
                new Paciente
                {
                    Id = PacientePedroId,
                    Nombre = "Pedro Gómez",
                    DNI = "32456789",
                    Email = "pedro.gomez@mail.com"
                });

            await context.SaveChangesAsync();
        }

        private static async Task SeedTurnosAsync(SanSaludDbContext context)
        {
            var fechaBase = DateTime.UtcNow.Date;

            context.Turnos.AddRange(
                new Turno
                {
                    PacienteId = PacienteJuanId,
                    MedicoId = MedicoCardiologiaId,
                    FechaHora = fechaBase.AddDays(1).AddHours(13),
                    DuracionHoras = 2
                },
                new Turno
                {
                    PacienteId = PacienteMariaId,
                    MedicoId = MedicoPediatriaId,
                    FechaHora = fechaBase.AddDays(2).AddHours(13),
                    DuracionHoras = 2
                },
                new Turno
                {
                    PacienteId = PacientePedroId,
                    MedicoId = MedicoDermatologiaId,
                    FechaHora = fechaBase.AddDays(3).AddHours(13),
                    DuracionHoras = 2
                });

            await context.SaveChangesAsync();
        }
    }
}
