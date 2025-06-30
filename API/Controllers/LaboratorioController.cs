using Microsoft.AspNetCore.Mvc;
using BLL.Models;
using BLL.Services;
using Microsoft.Extensions.Configuration;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LaboratorioController : ControllerBase
    {
        private readonly LaboratorioService _laboratorioService;

        public LaboratorioController(IConfiguration configuration)
        {
            _laboratorioService = new LaboratorioService(configuration);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Laboratorio>>> GetLaboratorios()
        {
            try
            {
                var pessoas = await _laboratorioService.GetAllLaboratoriosAsync();
                return Ok(pessoas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Laboratorio>> GetLaboratorioPorId(int id)
        {
            try
            {
                var pessoa = await _laboratorioService.GetLaboratorioByIdAsync(id);
                if (pessoa == null)
                    return NotFound($"Pessoa with ID {id} not found.");

                return Ok(pessoa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Laboratorio>> PostLaboratorio([FromBody] Laboratorio laboratorio)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdPessoa = await _laboratorioService.CreateLaboratorioAsync(laboratorio);
                return CreatedAtAction(nameof(GetLaboratorioPorId), new { id = createdPessoa.Id }, createdPessoa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Laboratorio>> PutPessoa(int id, [FromBody] Laboratorio laboratorio)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (id != laboratorio.Id)
                    return BadRequest("ID mismatch");

                var updatedPessoa = await _laboratorioService.UpdateLaboratorioAsync(id, laboratorio);
                return Ok(updatedPessoa);
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
        public async Task<ActionResult> DeletePessoa(int id)
        {
            try
            {
                await _laboratorioService.DeleteLaboratorioAsync(id);
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
