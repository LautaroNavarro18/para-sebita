using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanSaludAPI.BusinessLogic;
using SanSaludAPI.Shared;

namespace SanSaludAPI.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly IPacienteService _pacienteService;

        public PacientesController(IPacienteService pacienteService)
        {
            _pacienteService = pacienteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PacienteResponseDTO>>> GetPacientes()
        {
            var pacientes = await _pacienteService.GetAllPacientesAsync();
            return Ok(pacientes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PacienteResponseDTO>> GetPaciente(Guid id)
        {
            var paciente = await _pacienteService.GetPacienteByIdAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }
            return Ok(paciente);
        }

        [HttpPost]
        public async Task<ActionResult<PacienteResponseDTO>> CreatePaciente([FromBody] PacienteCreateDTO pacienteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var paciente = await _pacienteService.CreatePacienteAsync(pacienteDto);
            return CreatedAtAction(nameof(GetPaciente), new { id = paciente.Id }, paciente);
        }
    }
}
