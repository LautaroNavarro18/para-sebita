using System;
using System.ComponentModel.DataAnnotations;

namespace SanSaludAPI.DataAccess
{
    public class Medico
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Especialidad { get; set; } = string.Empty;

        [Required]
        public string Matricula { get; set; } = string.Empty;
    }
}
