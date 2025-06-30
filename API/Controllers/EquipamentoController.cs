using Microsoft.AspNetCore.Mvc;
using BLL.Models;
using BLL.Services;
using Microsoft.Extensions.Configuration;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipamentoController : ControllerBase
    {
        private readonly PessoaService _pessoaService;

        public EquipamentoController(IConfiguration configuration)
        {
            _pessoaService = new PessoaService(configuration);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pessoa>>> GetPessoas()
        {
            try
            {
                var pessoas = await _pessoaService.GetAllPessoasAsync();
                return Ok(pessoas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pessoa>> GetPessoaPorId(int id)
        {
            try
            {
                var pessoa = await _pessoaService.GetPessoaByIdAsync(id);
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
        public async Task<ActionResult<Pessoa>> PostPessoa([FromBody] Pessoa pessoa)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var createdPessoa = await _pessoaService.CreatePessoaAsync(pessoa);
                return CreatedAtAction(nameof(GetPessoaPorId), new { id = createdPessoa.Id }, createdPessoa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Pessoa>> PutPessoa(int id, [FromBody] Pessoa pessoa)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (id != pessoa.Id)
                    return BadRequest("ID mismatch");

                var updatedPessoa = await _pessoaService.UpdatePessoaAsync(id, pessoa);
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
                await _pessoaService.DeletePessoaAsync(id);
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
