using Microsoft.AspNetCore.Mvc;
using BLL.Models;
using BLL.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipamentoController : ControllerBase
    {
        private readonly IEquipamentoService _equipamentoService;

        public EquipamentoController(IEquipamentoService equipamentoService)
        {
            _equipamentoService = equipamentoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModeloEquipamento>>> GetEquipamentos()
        {
            try
            {
                var equipamentos = await _equipamentoService.GetActiveEquipamentosAsync();
                return Ok(equipamentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ModeloEquipamento>> GetEquipamentoPorId(int id)
        {
            try
            {
                var equipamento = await _equipamentoService.GetEquipamentoByIdAsync(id);
                if (equipamento == null)
                    return NotFound($"Equipamento with ID {id} not found.");

                return Ok(equipamento);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ModeloEquipamento>> PostEquipamento([FromBody] ModeloEquipamento equipamento)
        {
            try
            {
                var createdEquipamento = await _equipamentoService.CreateEquipamentoAsync(equipamento);
                return CreatedAtAction(nameof(GetEquipamentoPorId), new { id = createdEquipamento.Id }, createdEquipamento);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ModeloEquipamento>> PutEquipamento(int id, [FromBody] ModeloEquipamento equipamento)
        {
            try
            {
                if (id != equipamento.Id)
                    return BadRequest("ID mismatch");

                var updatedEquipamento = await _equipamentoService.UpdateEquipamentoAsync(id, equipamento);
                return Ok(updatedEquipamento);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteEquipamento(int id)
        {
            try
            {
                await _equipamentoService.DeleteEquipamentoAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
