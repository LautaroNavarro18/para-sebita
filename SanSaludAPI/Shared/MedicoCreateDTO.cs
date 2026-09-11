using System.ComponentModel.DataAnnotations;

namespace SanSaludAPI.Shared
{
    public class MedicoCreateDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(200, ErrorMessage = "El nombre no puede superar los 200 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especialidad es obligatoria.")]
        [MaxLength(200, ErrorMessage = "La especialidad no puede superar los 200 caracteres.")]
        public string Especialidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "La matrícula es obligatoria.")]
        [MaxLength(50, ErrorMessage = "La matrícula no puede superar los 50 caracteres.")]
        public string Matricula { get; set; } = string.Empty;
    }
}
