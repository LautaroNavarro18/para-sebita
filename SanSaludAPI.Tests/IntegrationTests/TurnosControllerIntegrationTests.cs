using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using SanSaludAPI.Shared;
using Xunit;

namespace SanSaludAPI.Tests.IntegrationTests
{
    public class TurnosControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public TurnosControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateTurno_WithValidData_Returns201Created()
        {
            // Arrange - Crear Médico
            var medicoResp = await _client.PostAsJsonAsync("/api/medicos", new MedicoCreateDTO
            {
                Nombre = "Dr. Esteban Quito",
                Especialidad = "Traumatología",
                Matricula = "MN1122"
            });
            var medico = await medicoResp.Content.ReadFromJsonAsync<MedicoResponseDTO>();

            // Arrange - Crear Paciente
            var pacienteResp = await _client.PostAsJsonAsync("/api/pacientes", new PacienteCreateDTO
            {
                Nombre = "María Becerra",
                DNI = "38999888",
                Email = "maria@example.com"
            });
            var paciente = await pacienteResp.Content.ReadFromJsonAsync<PacienteResponseDTO>();

            var fechaTurno = DateTime.Now.AddDays(5).Date.AddHours(14); // 14:00 dentro de 5 días

            var turnoDto = new TurnoCreateDTO
            {
                MedicoId = medico!.Id,
                PacienteId = paciente!.Id,
                FechaHora = fechaTurno
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/turnos", turnoDto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdTurno = await response.Content.ReadFromJsonAsync<TurnoResponseDTO>();
            Assert.NotNull(createdTurno);
            Assert.Equal(medico.Id, createdTurno.MedicoId);
            Assert.Equal(paciente.Id, createdTurno.PacienteId);
            Assert.Equal("Dr. Esteban Quito", createdTurno.MedicoNombre);
            Assert.Equal("María Becerra", createdTurno.PacienteNombre);
        }

        [Fact]
        public async Task CreateTurno_WhenOverlappingSchedule_Returns409Conflict()
        {
            // Arrange - Crear Médico
            var medicoResp = await _client.PostAsJsonAsync("/api/medicos", new MedicoCreateDTO
            {
                Nombre = "Dra. Juana Azurduy",
                Especialidad = "Oftalmología",
                Matricula = "MN5566"
            });
            var medico = await medicoResp.Content.ReadFromJsonAsync<MedicoResponseDTO>();

            // Arrange - Crear Pacientes
            var paciente1Resp = await _client.PostAsJsonAsync("/api/pacientes", new PacienteCreateDTO
            {
                Nombre = "Paciente Uno",
                DNI = "11111111",
                Email = "paciente1@example.com"
            });
            var paciente1 = await paciente1Resp.Content.ReadFromJsonAsync<PacienteResponseDTO>();

            var paciente2Resp = await _client.PostAsJsonAsync("/api/pacientes", new PacienteCreateDTO
            {
                Nombre = "Paciente Dos",
                DNI = "22222222",
                Email = "paciente2@example.com"
            });
            var paciente2 = await paciente2Resp.Content.ReadFromJsonAsync<PacienteResponseDTO>();

            var fechaTurno1 = DateTime.Now.AddDays(3).Date.AddHours(10); // 10:00 AM (duración 2hs => hasta las 12:00)
            var fechaTurnoSolapado = fechaTurno1.AddHours(1); // 11:00 AM (Solapa con el primer turno)

            // Act - Crear primer turno (10:00)
            var primerTurnoResponse = await _client.PostAsJsonAsync("/api/turnos", new TurnoCreateDTO
            {
                MedicoId = medico!.Id,
                PacienteId = paciente1!.Id,
                FechaHora = fechaTurno1
            });
            Assert.Equal(HttpStatusCode.Created, primerTurnoResponse.StatusCode);

            // Act - Intentar crear segundo turno solapado (11:00)
            var segundoTurnoResponse = await _client.PostAsJsonAsync("/api/turnos", new TurnoCreateDTO
            {
                MedicoId = medico.Id,
                PacienteId = paciente2!.Id,
                FechaHora = fechaTurnoSolapado
            });

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, segundoTurnoResponse.StatusCode);
        }

        [Fact]
        public async Task CreateTurno_WithPastDate_Returns400BadRequest()
        {
            // Arrange
            var turnoPastDto = new TurnoCreateDTO
            {
                MedicoId = Guid.NewGuid(),
                PacienteId = Guid.NewGuid(),
                FechaHora = DateTime.Now.AddDays(-1)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/turnos", turnoPastDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
