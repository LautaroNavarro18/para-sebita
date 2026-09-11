using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanSaludAPI.DataAccess
{
    public interface IMedicoRepository
    {
        Task<IEnumerable<Medico>> GetAllAsync();
        Task<Medico?> GetByIdAsync(Guid id);
        Task<Medico> CreateAsync(Medico medico);
        Task<bool> HasTurnosAsync(Guid medicoId);
        Task<bool> ExistsByMatriculaAsync(string matricula);
        Task DeleteAsync(Guid id);
    }
}
