using System;
using System.ComponentModel.DataAnnotations;

namespace SanSaludAPI.Shared
{
    public class TurnoUpdateDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid PacienteId { get; set; }

        [Required]
        public Guid MedicoId { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }
    }
}
