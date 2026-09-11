using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SanSaludAPI.DataAccess;
using SanSaludAPI.Shared;

namespace SanSaludAPI.BusinessLogic
{
    public class MedicoService : IMedicoService
    {
        private readonly IMedicoRepository _medicoRepository;

        public MedicoService(IMedicoRepository medicoRepository)
        {
            _medicoRepository = medicoRepository;
        }

        public async Task<IEnumerable<MedicoResponseDTO>> GetAllMedicosAsync()
        {
            var medicos = await _medicoRepository.GetAllAsync();
            return medicos.Select(m => new MedicoResponseDTO
            {
                Id = m.Id,
                Nombre = m.Nombre,
                Especialidad = m.Especialidad,
                Matricula = m.Matricula
            });
        }

        public async Task<MedicoResponseDTO?> GetMedicoByIdAsync(Guid id)
        {
            var medico = await _medicoRepository.GetByIdAsync(id);
            if (medico == null) return null;

            return new MedicoResponseDTO
            {
                Id = medico.Id,
                Nombre = medico.Nombre,
                Especialidad = medico.Especialidad,
                Matricula = medico.Matricula
            };
        }

        public async Task<MedicoResponseDTO> CreateMedicoAsync(MedicoCreateDTO medicoDto)
        {
            var nombre = medicoDto.Nombre?.Trim() ?? string.Empty;
            var especialidad = medicoDto.Especialidad?.Trim() ?? string.Empty;
            var matricula = medicoDto.Matricula?.Trim() ?? string.Empty;

            if (string.IsNullOrEmpty(nombre)) throw new ValidationException("El nombre es obligatorio.");
            if (string.IsNullOrEmpty(especialidad)) throw new ValidationException("La especialidad es obligatoria.");
            if (string.IsNullOrEmpty(matricula)) throw new ValidationException("La matrícula es obligatoria.");

            if (await _medicoRepository.ExistsByMatriculaAsync(matricula))
            {
                throw new MatriculaDuplicadaException(matricula);
            }

            var medico = new Medico
            {
                Nombre = nombre,
                Especialidad = especialidad,
                Matricula = matricula
            };

            var createdMedico = await _medicoRepository.CreateAsync(medico);

            return new MedicoResponseDTO
            {
                Id = createdMedico.Id,
                Nombre = createdMedico.Nombre,
                Especialidad = createdMedico.Especialidad,
                Matricula = createdMedico.Matricula
            };
        }

        public async Task DeleteMedicoAsync(Guid id)
        {
            var medico = await _medicoRepository.GetByIdAsync(id);
            if (medico == null)
            {
                throw new MedicoNotFoundException(id);
            }

            if (await _medicoRepository.HasTurnosAsync(id))
            {
                throw new MedicoHasTurnosException(id);
            }

            await _medicoRepository.DeleteAsync(id);
        }
    }
}
