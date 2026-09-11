using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SanSaludAPI.DataAccess
{
    public class Turno
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid PacienteId { get; set; }

        [Required]
        public Guid MedicoId { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        [Required]
        public int DuracionHoras { get; set; } = 2;

        // Navigation properties
        [ForeignKey("PacienteId")]
        public Paciente Paciente { get; set; } = null!;

        [ForeignKey("MedicoId")]
        public Medico Medico { get; set; } = null!;
    }
}
