using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SanSaludAPI.DataAccess;
using SanSaludAPI.Shared;

namespace SanSaludAPI.BusinessLogic
{
    public class TurnoService : ITurnoService
    {
        private readonly ITurnoRepository _turnoRepository;
        private readonly IMedicoRepository _medicoRepository;
        private readonly IPacienteRepository _pacienteRepository;

        public TurnoService(
            ITurnoRepository turnoRepository,
            IMedicoRepository medicoRepository,
            IPacienteRepository pacienteRepository)
        {
            _turnoRepository = turnoRepository;
            _medicoRepository = medicoRepository;
            _pacienteRepository = pacienteRepository;
        }

        public async Task<TurnoResponseDTO> CreateTurnoAsync(TurnoCreateDTO turnoDto)
        {
            await ValidateTurnoBusinessRules(turnoDto.PacienteId, turnoDto.MedicoId, turnoDto.FechaHora);
            await ValidateNoOverlappingTurnos(turnoDto.MedicoId, turnoDto.FechaHora, null);

            var turno = new Turno
            {
                PacienteId = turnoDto.PacienteId,
                MedicoId = turnoDto.MedicoId,
                FechaHora = turnoDto.FechaHora,
                DuracionHoras = 2
            };

            var createdTurno = await _turnoRepository.CreateAsync(turno);
            return MapToResponseDTO(createdTurno);
        }

        public async Task<TurnoResponseDTO?> GetTurnoByIdAsync(Guid id)
        {
            var turno = await _turnoRepository.GetByIdAsync(id);
            return turno == null ? null : MapToResponseDTO(turno);
        }

        public async Task<IEnumerable<TurnoResponseDTO>> GetAllTurnosAsync()
        {
            var turnos = await _turnoRepository.GetAllAsync();
            return turnos.Select(MapToResponseDTO).ToList();
        }

        public async Task<IEnumerable<TurnoResponseDTO>> GetTurnosByPacienteIdAsync(Guid pacienteId)
        {
            var paciente = await _pacienteRepository.GetByIdAsync(pacienteId);
            if (paciente == null)
                throw new PacienteNotFoundException(pacienteId);

            var turnos = await _turnoRepository.GetByPacienteIdAsync(pacienteId);
            return turnos.Select(MapToResponseDTO).ToList();
        }

        public async Task<IEnumerable<TurnoResponseDTO>> GetTurnosByMedicoIdAsync(Guid medicoId)
        {
            var medico = await _medicoRepository.GetByIdAsync(medicoId);
            if (medico == null)
                throw new MedicoNotFoundException(medicoId);

            var turnos = await _turnoRepository.GetByMedicoIdAsync(medicoId);
            return turnos.Select(MapToResponseDTO).ToList();
        }

        public async Task<TurnoResponseDTO> UpdateTurnoAsync(TurnoUpdateDTO turnoDto)
        {
            var existingTurno = await _turnoRepository.GetByIdAsync(turnoDto.Id);
            if (existingTurno == null)
                throw new TurnoNotFoundException(turnoDto.Id);

            await ValidateTurnoBusinessRules(turnoDto.PacienteId, turnoDto.MedicoId, turnoDto.FechaHora);
            await ValidateNoOverlappingTurnos(turnoDto.MedicoId, turnoDto.FechaHora, turnoDto.Id);

            existingTurno.PacienteId = turnoDto.PacienteId;
            existingTurno.MedicoId = turnoDto.MedicoId;
            existingTurno.FechaHora = turnoDto.FechaHora;

            var updatedTurno = await _turnoRepository.UpdateAsync(existingTurno);
            return MapToResponseDTO(updatedTurno);
        }

        public async Task DeleteTurnoAsync(Guid id)
        {
            if (!await _turnoRepository.ExistsAsync(id))
                throw new TurnoNotFoundException(id);

            await _turnoRepository.DeleteAsync(id);
        }

        private async Task ValidateTurnoBusinessRules(Guid pacienteId, Guid medicoId, DateTime fechaHora)
        {
            // Validar que la fecha sea futura
            if (fechaHora <= DateTime.Now)
                throw new InvalidTurnoDateException();

            // Validar que el médico exista
            var medico = await _medicoRepository.GetByIdAsync(medicoId);
            if (medico == null)
                throw new MedicoNotFoundException(medicoId);

            // Validar que el paciente exista
            var paciente = await _pacienteRepository.GetByIdAsync(pacienteId);
            if (paciente == null)
                throw new PacienteNotFoundException(pacienteId);
        }

        private async Task ValidateNoOverlappingTurnos(Guid medicoId, DateTime fechaHora, Guid? excludeTurnoId)
        {
            // Obtener turnos del médico en un rango de ±2 horas para optimizar
            var fechaInicio = fechaHora.AddHours(-4);
            var fechaFin = fechaHora.AddHours(4);
            var turnosExistentes = await _turnoRepository.GetByMedicoAndDateRangeAsync(medicoId, fechaInicio, fechaFin);

            var fechaFinNueva = fechaHora.AddHours(2);

            bool isOverlapping = turnosExistentes.Any(t =>
            {
                if (excludeTurnoId.HasValue && t.Id == excludeTurnoId.Value)
                    return false;

                return (fechaHora >= t.FechaHora && fechaHora < t.FechaHora.AddHours(t.DuracionHoras)) ||
                       (fechaFinNueva > t.FechaHora && fechaFinNueva <= t.FechaHora.AddHours(t.DuracionHoras)) ||
                       (fechaHora <= t.FechaHora && fechaFinNueva >= t.FechaHora.AddHours(t.DuracionHoras));
            });

            if (isOverlapping)
            {
                throw new OverlappingScheduleException("El turno solicitado se solapa en horario con otro existente para el mismo médico.");
            }
        }

        private TurnoResponseDTO MapToResponseDTO(Turno turno)
        {
            return new TurnoResponseDTO
            {
                Id = turno.Id,
                PacienteId = turno.PacienteId,
                PacienteNombre = turno.Paciente?.Nombre ?? "Desconocido",
                MedicoId = turno.MedicoId,
                MedicoNombre = turno.Medico?.Nombre ?? "Desconocido",
                MedicoEspecialidad = turno.Medico?.Especialidad ?? "Desconocida",
                FechaHora = turno.FechaHora,
                DuracionHoras = turno.DuracionHoras
            };
        }
    }
}
