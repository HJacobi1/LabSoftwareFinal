using Microsoft.AspNetCore.Mvc;
using BLL.Models;
using BLL.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitacaoController : ControllerBase
    {
        private readonly ISolicitacaoService _solicitacaoService;

        public SolicitacaoController(ISolicitacaoService solicitacaoService)
        {
            _solicitacaoService = solicitacaoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Solicitacao>>> GetSolicitacoes()
        {
            try
            {
                var solicitacoes = await _solicitacaoService.GetAllSolicitacoesAsync();
                return Ok(solicitacoes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Solicitacao>> GetSolicitacaoPorId(int id)
        {
            try
            {
                var solicitacao = await _solicitacaoService.GetSolicitacaoByIdAsync(id);
                if (solicitacao == null)
                    return NotFound($"Solicitacao with ID {id} not found.");

                return Ok(solicitacao);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Solicitacao>> PostSolicitacao([FromBody] Solicitacao solicitacao)
        {
            try
            {
                var createdSolicitacao = await _solicitacaoService.CreateSolicitacaoAsync(solicitacao);
                return CreatedAtAction(nameof(GetSolicitacaoPorId), new { id = createdSolicitacao.Id }, createdSolicitacao);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Solicitacao>> PutSolicitacao(int id, [FromBody] Solicitacao solicitacao)
        {
            try
            {
                if (id != solicitacao.Id)
                    return BadRequest("ID mismatch");

                var updatedSolicitacao = await _solicitacaoService.UpdateSolicitacaoAsync(id, solicitacao);
                return Ok(updatedSolicitacao);
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
        public async Task<ActionResult> DeleteSolicitacao(int id)
        {
            try
            {
                await _solicitacaoService.DeleteSolicitacaoAsync(id);
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
