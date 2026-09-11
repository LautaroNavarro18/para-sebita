using System;

namespace SanSaludAPI.Shared
{
    public class PacienteResponseDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string DNI { get; set; } = string.Empty;
        public string? Email { get; set; }
    }
}
