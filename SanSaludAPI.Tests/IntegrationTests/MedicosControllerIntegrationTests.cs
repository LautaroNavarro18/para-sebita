using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using SanSaludAPI.Shared;
using Xunit;

namespace SanSaludAPI.Tests.IntegrationTests
{
    public class MedicosControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public MedicosControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetMedicos_ReturnsSuccessAndJsonArray()
        {
            // Act
            var response = await _client.GetAsync("/api/medicos");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var medicos = await response.Content.ReadFromJsonAsync<List<MedicoResponseDTO>>();
            Assert.NotNull(medicos);
        }

        [Fact]
        public async Task CreateMedico_ReturnsSuccessAndCreatedMedico()
        {
            // Arrange
            var newMedico = new MedicoCreateDTO
            {
                Nombre = "Dr. Roberto Gómez",
                Especialidad = "Neurología",
                Matricula = "MN9988"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/medicos", newMedico);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created);
            var createdMedico = await response.Content.ReadFromJsonAsync<MedicoResponseDTO>();
            Assert.NotNull(createdMedico);
            Assert.NotEqual(Guid.Empty, createdMedico.Id);
            Assert.Equal("Dr. Roberto Gómez", createdMedico.Nombre);
            Assert.Equal("Neurología", createdMedico.Especialidad);
            Assert.Equal("MN9988", createdMedico.Matricula);
        }

        [Fact]
        public async Task CreateMedico_WithInvalidJson_Returns400BadRequest()
        {
            // Arrange
            using var content = new StringContent("esto no es un json valido", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/medicos", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateMedico_WithMissingRequiredField_Returns400BadRequest()
        {
            // Arrange
            var payload = new { Nombre = "Dr. Juan Pérez", Especialidad = "Pediatría" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/medicos", payload);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateMedico_WhenDuplicateMatricula_Returns409Conflict()
        {
            // Arrange
            var primerMedico = new MedicoCreateDTO
            {
                Nombre = "Dr. Roberto Gómez",
                Especialidad = "Neurología",
                Matricula = "MN-UNICO-777"
            };
            var medicoRepetido = new MedicoCreateDTO
            {
                Nombre = "Dra. Lucía Ríos",
                Especialidad = "Cardiología",
                Matricula = "MN-UNICO-777"
            };

            // Act
            var crearRespuesta = await _client.PostAsJsonAsync("/api/medicos", primerMedico);
            var respuestaDuplicada = await _client.PostAsJsonAsync("/api/medicos", medicoRepetido);

            // Assert
            Assert.True(crearRespuesta.StatusCode == HttpStatusCode.OK || crearRespuesta.StatusCode == HttpStatusCode.Created);
            Assert.Equal(HttpStatusCode.Conflict, respuestaDuplicada.StatusCode);
        }

        [Fact]
        public async Task CreateMedico_WithWhitespaceOnlyField_Returns400BadRequest()
        {
            // Arrange
            var medicoInvalido = new MedicoCreateDTO
            {
                Nombre = "   ",
                Especialidad = "Neurología",
                Matricula = "MN9999"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/medicos", medicoInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateMedico_WithOversizedField_Returns400BadRequest()
        {
            // Arrange
            var medicoInvalido = new MedicoCreateDTO
            {
                Nombre = new string('a', 300),
                Especialidad = "Neurología",
                Matricula = "MN9998"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/medicos", medicoInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetMedicoById_WhenNonExistent_ReturnsNotFound()
        {
            // Arrange
            var randomId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/medicos/{randomId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
