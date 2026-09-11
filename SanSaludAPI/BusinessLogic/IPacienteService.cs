using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SanSaludAPI.Shared;

namespace SanSaludAPI.BusinessLogic
{
    public interface IPacienteService
    {
        Task<IEnumerable<PacienteResponseDTO>> GetAllPacientesAsync();
        Task<PacienteResponseDTO?> GetPacienteByIdAsync(Guid id);
        Task<PacienteResponseDTO> CreatePacienteAsync(PacienteCreateDTO pacienteDto);
    }
}
