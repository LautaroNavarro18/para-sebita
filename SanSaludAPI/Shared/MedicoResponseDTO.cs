using System;

namespace SanSaludAPI.Shared
{
    public class MedicoResponseDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Especialidad { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
    }
}
