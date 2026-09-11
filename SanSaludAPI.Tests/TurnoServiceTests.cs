using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using SanSaludAPI.BusinessLogic;
using SanSaludAPI.DataAccess;
using SanSaludAPI.Shared;
using Xunit;

namespace SanSaludAPI.Tests
{
    public class TurnoServiceTests
    {
        private readonly Mock<ITurnoRepository> _turnoRepositoryMock;
        private readonly Mock<IMedicoRepository> _medicoRepositoryMock;
        private readonly Mock<IPacienteRepository> _pacienteRepositoryMock;
        private readonly TurnoService _turnoService;

        public TurnoServiceTests()
        {
            _turnoRepositoryMock = new Mock<ITurnoRepository>();
            _medicoRepositoryMock = new Mock<IMedicoRepository>();
            _pacienteRepositoryMock = new Mock<IPacienteRepository>();

            _turnoService = new TurnoService(
                _turnoRepositoryMock.Object,
                _medicoRepositoryMock.Object,
                _pacienteRepositoryMock.Object
            );
        }

        [Fact]
        public async Task CreateTurnoAsync_WithValidData_ReturnsCreatedTurnoResponseDTO()
        {
            // Arrange
            var medicoId = Guid.NewGuid();
            var pacienteId = Guid.NewGuid();
            var fechaHoraFutura = DateTime.Now.AddDays(2);

            var turnoCreateDTO = new TurnoCreateDTO
            {
                MedicoId = medicoId,
                PacienteId = pacienteId,
                FechaHora = fechaHoraFutura
            };

            var medicoMock = new Medico { Id = medicoId, Nombre = "Dr. Pedro Gómez", Especialidad = "Cardiología" };
            var pacienteMock = new Paciente { Id = pacienteId, Nombre = "Ana López" };

            _medicoRepositoryMock
                .Setup(repo => repo.GetByIdAsync(medicoId))
                .ReturnsAsync(medicoMock);

            _pacienteRepositoryMock
                .Setup(repo => repo.GetByIdAsync(pacienteId))
                .ReturnsAsync(pacienteMock);

            _turnoRepositoryMock
                .Setup(repo => repo.GetByMedicoAndDateRangeAsync(medicoId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Turno>());

            _turnoRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Turno>()))
                .ReturnsAsync((Turno t) =>
                {
                    t.Id = Guid.NewGuid();
                    t.Medico = medicoMock;
                    t.Paciente = pacienteMock;
                    return t;
                });

            // Act
            var result = await _turnoService.CreateTurnoAsync(turnoCreateDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(medicoId, result.MedicoId);
            Assert.Equal(pacienteId, result.PacienteId);
            Assert.Equal("Dr. Pedro Gómez", result.MedicoNombre);
            Assert.Equal("Ana López", result.PacienteNombre);
            Assert.Equal(2, result.DuracionHoras);
            _turnoRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Turno>()), Times.Once);
        }

        [Fact]
        public async Task CreateTurnoAsync_WithPastDate_ThrowsInvalidTurnoDateException()
        {
            // Arrange
            var turnoCreateDTO = new TurnoCreateDTO
            {
                MedicoId = Guid.NewGuid(),
                PacienteId = Guid.NewGuid(),
                FechaHora = DateTime.Now.AddHours(-1) // Fecha en el pasado
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidTurnoDateException>(() =>
                _turnoService.CreateTurnoAsync(turnoCreateDTO));
        }

        [Fact]
        public async Task CreateTurnoAsync_WithNonExistentMedico_ThrowsMedicoNotFoundException()
        {
            // Arrange
            var medicoId = Guid.NewGuid();
            var pacienteId = Guid.NewGuid();
            var turnoCreateDTO = new TurnoCreateDTO
            {
                MedicoId = medicoId,
                PacienteId = pacienteId,
                FechaHora = DateTime.Now.AddDays(1)
            };

            _medicoRepositoryMock
                .Setup(repo => repo.GetByIdAsync(medicoId))
                .ReturnsAsync((Medico?)null);

            // Act & Assert
            await Assert.ThrowsAsync<MedicoNotFoundException>(() =>
                _turnoService.CreateTurnoAsync(turnoCreateDTO));
        }

        [Fact]
        public async Task CreateTurnoAsync_WithNonExistentPaciente_ThrowsPacienteNotFoundException()
        {
            // Arrange
            var medicoId = Guid.NewGuid();
            var pacienteId = Guid.NewGuid();
            var turnoCreateDTO = new TurnoCreateDTO
            {
                MedicoId = medicoId,
                PacienteId = pacienteId,
                FechaHora = DateTime.Now.AddDays(1)
            };

            _medicoRepositoryMock
                .Setup(repo => repo.GetByIdAsync(medicoId))
                .ReturnsAsync(new Medico { Id = medicoId, Nombre = "Dr. House" });

            _pacienteRepositoryMock
                .Setup(repo => repo.GetByIdAsync(pacienteId))
                .ReturnsAsync((Paciente?)null);

            // Act & Assert
            await Assert.ThrowsAsync<PacienteNotFoundException>(() =>
                _turnoService.CreateTurnoAsync(turnoCreateDTO));
        }

        [Fact]
        public async Task CreateTurnoAsync_WithOverlappingSchedule_ThrowsOverlappingScheduleException()
        {
            // Arrange
            var medicoId = Guid.NewGuid();
            var pacienteId = Guid.NewGuid();
            var fechaHoraExistente = DateTime.Now.AddDays(1).Date.AddHours(10); // 10:00 AM mañana
            var fechaHoraNueva = fechaHoraExistente.AddHours(1); // 11:00 AM (Solapa con turno de 10:00 a 12:00)

            var turnoCreateDTO = new TurnoCreateDTO
            {
                MedicoId = medicoId,
                PacienteId = pacienteId,
                FechaHora = fechaHoraNueva
            };

            _medicoRepositoryMock
                .Setup(repo => repo.GetByIdAsync(medicoId))
                .ReturnsAsync(new Medico { Id = medicoId, Nombre = "Dr. House" });

            _pacienteRepositoryMock
                .Setup(repo => repo.GetByIdAsync(pacienteId))
                .ReturnsAsync(new Paciente { Id = pacienteId, Nombre = "Juan Pérez" });

            var turnosExistentes = new List<Turno>
            {
                new Turno
                {
                    Id = Guid.NewGuid(),
                    MedicoId = medicoId,
                    PacienteId = Guid.NewGuid(),
                    FechaHora = fechaHoraExistente,
                    DuracionHoras = 2
                }
            };

            _turnoRepositoryMock
                .Setup(repo => repo.GetByMedicoAndDateRangeAsync(medicoId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(turnosExistentes);

            // Act & Assert
            await Assert.ThrowsAsync<OverlappingScheduleException>(() =>
                _turnoService.CreateTurnoAsync(turnoCreateDTO));
        }

        [Fact]
        public async Task DeleteTurnoAsync_WithNonExistentId_ThrowsTurnoNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            _turnoRepositoryMock
                .Setup(repo => repo.ExistsAsync(nonExistentId))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<TurnoNotFoundException>(() =>
                _turnoService.DeleteTurnoAsync(nonExistentId));
        }
    }
}
