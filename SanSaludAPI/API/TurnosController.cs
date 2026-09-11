using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanSaludAPI.BusinessLogic;
using SanSaludAPI.Shared;

namespace SanSaludAPI.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class TurnosController : ControllerBase
    {
        private readonly ITurnoService _turnoService;

        public TurnosController(ITurnoService turnoService)
        {
            _turnoService = turnoService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetTurnos()
        {
            var turnos = await _turnoService.GetAllTurnosAsync();
            return Ok(turnos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetTurno(Guid id)
        {
            var turno = await _turnoService.GetTurnoByIdAsync(id);
            if (turno == null)
            {
                return NotFound(new { Message = $"No se encontró el turno con ID: {id}" });
            }
            return Ok(turno);
        }

        [HttpGet("paciente/{pacienteId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetTurnosByPaciente(Guid pacienteId)
        {
            try
            {
                var turnos = await _turnoService.GetTurnosByPacienteIdAsync(pacienteId);
                return Ok(turnos);
            }
            catch (PacienteNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet("medico/{medicoId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetTurnosByMedico(Guid medicoId)
        {
            try
            {
                var turnos = await _turnoService.GetTurnosByMedicoIdAsync(medicoId);
                return Ok(turnos);
            }
            catch (MedicoNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateTurno([FromBody] TurnoCreateDTO turnoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var turno = await _turnoService.CreateTurnoAsync(turnoDto);
                return CreatedAtAction(nameof(GetTurno), new { id = turno.Id }, turno);
            }
            catch (MedicoNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (PacienteNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidTurnoDateException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (OverlappingScheduleException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateTurno(Guid id, [FromBody] TurnoUpdateDTO turnoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != turnoDto.Id)
            {
                return BadRequest(new { Message = "El ID de la ruta no coincide con el ID del cuerpo" });
            }

            try
            {
                var turno = await _turnoService.UpdateTurnoAsync(turnoDto);
                return Ok(turno);
            }
            catch (TurnoNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (MedicoNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (PacienteNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidTurnoDateException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (OverlappingScheduleException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTurno(Guid id)
        {
            try
            {
                await _turnoService.DeleteTurnoAsync(id);
                return NoContent();
            }
            catch (TurnoNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
