using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SanSaludAPI.Shared;

namespace SanSaludAPI.BusinessLogic
{
    public interface ITurnoService
    {
        Task<TurnoResponseDTO> CreateTurnoAsync(TurnoCreateDTO turnoDto);
        Task<TurnoResponseDTO?> GetTurnoByIdAsync(Guid id);
        Task<IEnumerable<TurnoResponseDTO>> GetAllTurnosAsync();
        Task<IEnumerable<TurnoResponseDTO>> GetTurnosByPacienteIdAsync(Guid pacienteId);
        Task<IEnumerable<TurnoResponseDTO>> GetTurnosByMedicoIdAsync(Guid medicoId);
        Task<TurnoResponseDTO> UpdateTurnoAsync(TurnoUpdateDTO turnoDto);
        Task DeleteTurnoAsync(Guid id);
    }
}
