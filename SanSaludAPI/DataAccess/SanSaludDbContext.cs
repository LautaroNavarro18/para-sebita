using Microsoft.EntityFrameworkCore;

namespace SanSaludAPI.DataAccess
{
    public class SanSaludDbContext : DbContext
    {
        public SanSaludDbContext(DbContextOptions<SanSaludDbContext> options) : base(options)
        {
        }

        public DbSet<Paciente> Pacientes { get; set; } = null!;
        public DbSet<Medico> Medicos { get; set; } = null!;
        public DbSet<Turno> Turnos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar relaciones con Fluent API
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Paciente)
                .WithMany()
                .HasForeignKey(t => t.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Medico)
                .WithMany()
                .HasForeignKey(t => t.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
