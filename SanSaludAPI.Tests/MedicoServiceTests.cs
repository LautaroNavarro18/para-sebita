using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using SanSaludAPI.BusinessLogic;
using SanSaludAPI.DataAccess;
using SanSaludAPI.Shared;
using Xunit;

namespace SanSaludAPI.Tests
{
    public class MedicoServiceTests
    {
        private readonly Mock<IMedicoRepository> _medicoRepositoryMock;
        private readonly MedicoService _medicoService;

        public MedicoServiceTests()
        {
            _medicoRepositoryMock = new Mock<IMedicoRepository>();
            _medicoService = new MedicoService(_medicoRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllMedicosAsync_ReturnsAllMedicosAsDTOs()
        {
            // Arrange
            var medicosList = new List<Medico>
            {
                new Medico { Id = Guid.NewGuid(), Nombre = "Dr. Carlos Rossi", Especialidad = "Pediatría", Matricula = "MP1234" },
                new Medico { Id = Guid.NewGuid(), Nombre = "Dra. Elena Silva", Especialidad = "Dermatología", Matricula = "MP5678" }
            };

            _medicoRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(medicosList);

            // Act
            var result = await _medicoService.GetAllMedicosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, m => m.Nombre == "Dr. Carlos Rossi");
            Assert.Contains(result, m => m.Nombre == "Dra. Elena Silva");
        }

        [Fact]
        public async Task GetMedicoByIdAsync_WhenExists_ReturnsMedicoResponseDTO()
        {
            // Arrange
            var medicoId = Guid.NewGuid();
            var medicoMock = new Medico
            {
                Id = medicoId,
                Nombre = "Dr. Mario Bros",
                Especialidad = "Traumatología",
                Matricula = "MP9999"
            };

            _medicoRepositoryMock
                .Setup(repo => repo.GetByIdAsync(medicoId))
                .ReturnsAsync(medicoMock);

            // Act
            var result = await _medicoService.GetMedicoByIdAsync(medicoId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(medicoId, result.Id);
            Assert.Equal("Dr. Mario Bros", result.Nombre);
            Assert.Equal("Traumatología", result.Especialidad);
        }

        [Fact]
        public async Task GetMedicoByIdAsync_WhenDoesNotExist_ReturnsNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            _medicoRepositoryMock
                .Setup(repo => repo.GetByIdAsync(nonExistentId))
                .ReturnsAsync((Medico?)null);

            // Act
            var result = await _medicoService.GetMedicoByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateMedicoAsync_SavesAndReturnsCreatedMedico()
        {
            // Arrange
            var medicoCreateDto = new MedicoCreateDTO
            {
                Nombre = "Dr. Gregory House",
                Especialidad = "Diagnóstico Médico",
                Matricula = "MP7777"
            };

            _medicoRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<Medico>()))
                .ReturnsAsync((Medico m) =>
                {
                    m.Id = Guid.NewGuid();
                    return m;
                });

            // Act
            var result = await _medicoService.CreateMedicoAsync(medicoCreateDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("Dr. Gregory House", result.Nombre);
            Assert.Equal("Diagnóstico Médico", result.Especialidad);
            _medicoRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Medico>()), Times.Once);
        }

        [Fact]
        public async Task CreateMedicoAsync_WhenMatriculaAlreadyExists_ThrowsMatriculaDuplicadaException()
        {
            // Arrange
            var medicoCreateDto = new MedicoCreateDTO
            {
                Nombre = "Dr. Gregory House",
                Especialidad = "Diagnóstico Médico",
                Matricula = "MP7777"
            };

            _medicoRepositoryMock
                .Setup(repo => repo.ExistsByMatriculaAsync("MP7777"))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<MatriculaDuplicadaException>(() => _medicoService.CreateMedicoAsync(medicoCreateDto));
            _medicoRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Medico>()), Times.Never);
        }

        [Fact]
        public async Task CreateMedicoAsync_WithWhitespaceOnlyField_ThrowsValidationException()
        {
            // Arrange
            var medicoCreateDto = new MedicoCreateDTO
            {
                Nombre = "   ",
                Especialidad = "Diagnóstico Médico",
                Matricula = "MP7777"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _medicoService.CreateMedicoAsync(medicoCreateDto));
            _medicoRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Medico>()), Times.Never);
        }

        [Fact]
        public async Task CreateMedicoAsync_WithEmptyRequiredField_ThrowsValidationException()
        {
            // Arrange
            var medicoCreateDto = new MedicoCreateDTO
            {
                Nombre = "",
                Especialidad = "Diagnóstico Médico",
                Matricula = "MP7777"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _medicoService.CreateMedicoAsync(medicoCreateDto));
            _medicoRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Medico>()), Times.Never);
        }

        [Fact]
        public async Task CreateMedicoAsync_WithValidData_TrimsAndSavesMedico()
        {
            // Arrange
            var medicoCreateDto = new MedicoCreateDTO
            {
                Nombre = "  Dr. Gregory House  ",
                Especialidad = " Diagnóstico Médico ",
                Matricula = " MP7777 "
            };

            _medicoRepositoryMock
                .Setup(repo => repo.ExistsByMatriculaAsync("MP7777"))
                .ReturnsAsync(false);

            _medicoRepositoryMock
                .Setup(repo => repo.CreateAsync(It.Is<Medico>(m =>
                    m.Matricula == "MP7777" &&
                    m.Nombre == "Dr. Gregory House" &&
                    m.Especialidad == "Diagnóstico Médico")))
                .ReturnsAsync((Medico m) =>
                {
                    m.Id = Guid.NewGuid();
                    return m;
                });

            // Act
            var result = await _medicoService.CreateMedicoAsync(medicoCreateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("MP7777", result.Matricula);
            Assert.Equal("Dr. Gregory House", result.Nombre);
            Assert.Equal("Diagnóstico Médico", result.Especialidad);
            _medicoRepositoryMock.Verify(repo => repo.ExistsByMatriculaAsync("MP7777"), Times.Once);
        }

        [Fact]
        public async Task DeleteMedicoAsync_WhenExistsAndHasNoTurnos_DeletesMedico()
        {
            // Arrange
            var medicoId = Guid.NewGuid();
            var medicoMock = new Medico
            {
                Id = medicoId,
                Nombre = "Dr. Mario Bros",
                Especialidad = "Traumatología",
                Matricula = "MP9999"
            };

            _medicoRepositoryMock.Setup(repo => repo.GetByIdAsync(medicoId)).ReturnsAsync(medicoMock);
            _medicoRepositoryMock.Setup(repo => repo.HasTurnosAsync(medicoId)).ReturnsAsync(false);

            // Act
            await _medicoService.DeleteMedicoAsync(medicoId);

            // Assert
            _medicoRepositoryMock.Verify(repo => repo.DeleteAsync(medicoId), Times.Once);
        }

        [Fact]
        public async Task DeleteMedicoAsync_WhenDoesNotExist_ThrowsMedicoNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            _medicoRepositoryMock.Setup(repo => repo.GetByIdAsync(nonExistentId)).ReturnsAsync((Medico?)null);

            // Act & Assert
            await Assert.ThrowsAsync<MedicoNotFoundException>(() => _medicoService.DeleteMedicoAsync(nonExistentId));
            _medicoRepositoryMock.Verify(repo => repo.DeleteAsync(nonExistentId), Times.Never);
        }

        [Fact]
        public async Task DeleteMedicoAsync_WhenHasTurnos_ThrowsMedicoHasTurnosException()
        {
            // Arrange
            var medicoId = Guid.NewGuid();
            var medicoMock = new Medico
            {
                Id = medicoId,
                Nombre = "Dr. Mario Bros",
                Especialidad = "Traumatología",
                Matricula = "MP9999"
            };

            _medicoRepositoryMock.Setup(repo => repo.GetByIdAsync(medicoId)).ReturnsAsync(medicoMock);
            _medicoRepositoryMock.Setup(repo => repo.HasTurnosAsync(medicoId)).ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<MedicoHasTurnosException>(() => _medicoService.DeleteMedicoAsync(medicoId));
            _medicoRepositoryMock.Verify(repo => repo.DeleteAsync(medicoId), Times.Never);
        }
    }
}
