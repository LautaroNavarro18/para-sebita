using System;
using System.ComponentModel.DataAnnotations;

namespace SanSaludAPI.DataAccess
{
    public class Paciente
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(8)]
        public string DNI { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }
    }
}
