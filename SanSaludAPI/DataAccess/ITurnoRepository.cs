using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanSaludAPI.DataAccess
{
    public interface ITurnoRepository
    {
        Task<Turno> CreateAsync(Turno turno);
        Task<Turno?> GetByIdAsync(Guid id);
        Task<IEnumerable<Turno>> GetAllAsync();
        Task<IEnumerable<Turno>> GetByPacienteIdAsync(Guid pacienteId);
        Task<IEnumerable<Turno>> GetByMedicoIdAsync(Guid medicoId);
        Task<IEnumerable<Turno>> GetByMedicoAndDateRangeAsync(Guid medicoId, DateTime fechaInicio, DateTime fechaFin);
        Task<Turno> UpdateAsync(Turno turno);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
