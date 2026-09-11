using System;

namespace SanSaludAPI.Shared
{
    public class TurnoResponseDTO
    {
        public Guid Id { get; set; }
        public Guid PacienteId { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public Guid MedicoId { get; set; }
        public string MedicoNombre { get; set; } = string.Empty;
        public string MedicoEspecialidad { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public int DuracionHoras { get; set; }
    }
}
