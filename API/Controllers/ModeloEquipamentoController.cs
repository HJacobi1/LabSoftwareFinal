using Microsoft.AspNetCore.Mvc;
using DAL.Models;
using DAL;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using DAL.Repositories;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModeloEquipamentoController : ControllerBase
    {
        private readonly Repository _repository;
        private readonly IGenericRepository<ModeloEquipamentoEntidade> _modeloRepository;

        public ModeloEquipamentoController(IConfiguration configuration)
        {
            _repository = new Repository(configuration);
            _modeloRepository = _repository.GetRepository<ModeloEquipamentoEntidade>();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModeloEquipamentoEntidade>>> GetModelos()
        {
            var modelos = await _modeloRepository.GetAllAsync();
            return Ok(modelos);
        }
    }
} 