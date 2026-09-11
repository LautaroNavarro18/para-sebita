using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SanSaludAPI.DataAccess;
using SanSaludAPI.Shared;

namespace SanSaludAPI.BusinessLogic
{
    public interface IMedicoService
    {
        Task<IEnumerable<MedicoResponseDTO>> GetAllMedicosAsync();
        Task<MedicoResponseDTO?> GetMedicoByIdAsync(Guid id);
        Task<MedicoResponseDTO> CreateMedicoAsync(MedicoCreateDTO medicoDto);
        Task DeleteMedicoAsync(Guid id);
    }
}
