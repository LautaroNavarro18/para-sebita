using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanSaludAPI.DataAccess
{
    public interface IPacienteRepository
    {
        Task<IEnumerable<Paciente>> GetAllAsync();
        Task<Paciente?> GetByIdAsync(Guid id);
        Task<Paciente> CreateAsync(Paciente paciente);
    }
}
