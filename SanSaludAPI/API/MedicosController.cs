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
    public class MedicosController : ControllerBase
    {
        private readonly IMedicoService _medicoService;

        public MedicosController(IMedicoService medicoService)
        {
            _medicoService = medicoService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MedicoResponseDTO>>> GetMedicos()
        {
            var medicos = await _medicoService.GetAllMedicosAsync();
            return Ok(medicos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MedicoResponseDTO>> GetMedico(Guid id)
        {
            var medico = await _medicoService.GetMedicoByIdAsync(id);
            if (medico == null)
            {
                return NotFound(new { Message = $"No se encontró el médico con ID: {id}" });
            }
            return Ok(medico);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<MedicoResponseDTO>> CreateMedico([FromBody] MedicoCreateDTO medicoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var medico = await _medicoService.CreateMedicoAsync(medicoDto);
                return CreatedAtAction(nameof(GetMedico), new { id = medico.Id }, medico);
            }
            catch (MatriculaDuplicadaException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteMedico(Guid id)
        {
            try
            {
                await _medicoService.DeleteMedicoAsync(id);
                return NoContent();
            }
            catch (MedicoNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (MedicoHasTurnosException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }
    }
}
