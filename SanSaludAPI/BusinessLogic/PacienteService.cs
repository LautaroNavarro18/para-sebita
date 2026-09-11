using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SanSaludAPI.DataAccess;
using SanSaludAPI.Shared;

namespace SanSaludAPI.BusinessLogic
{
    public class PacienteService : IPacienteService
    {
        private readonly IPacienteRepository _pacienteRepository;

        public PacienteService(IPacienteRepository pacienteRepository)
        {
            _pacienteRepository = pacienteRepository;
        }

        public async Task<IEnumerable<PacienteResponseDTO>> GetAllPacientesAsync()
        {
            var pacientes = await _pacienteRepository.GetAllAsync();
            return pacientes.Select(p => new PacienteResponseDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                DNI = p.DNI,
                Email = p.Email
            });
        }

        public async Task<PacienteResponseDTO?> GetPacienteByIdAsync(Guid id)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(id);
            if (paciente == null) return null;

            return new PacienteResponseDTO
            {
                Id = paciente.Id,
                Nombre = paciente.Nombre,
                DNI = paciente.DNI,
                Email = paciente.Email
            };
        }

        public async Task<PacienteResponseDTO> CreatePacienteAsync(PacienteCreateDTO pacienteDto)
        {
            var paciente = new Paciente
            {
                Nombre = pacienteDto.Nombre,
                DNI = pacienteDto.DNI,
                Email = pacienteDto.Email
            };

            var createdPaciente = await _pacienteRepository.CreateAsync(paciente);

            return new PacienteResponseDTO
            {
                Id = createdPaciente.Id,
                Nombre = createdPaciente.Nombre,
                DNI = createdPaciente.DNI,
                Email = createdPaciente.Email
            };
        }
    }
}
