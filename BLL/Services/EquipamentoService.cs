using BLL.Enums;
using BLL.Models;
using DAL;
using DAL.Models;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class EquipamentoService : IEquipamentoService
    {
        private readonly Repository _repository;
        private readonly IGenericRepository<ModeloEquipamentoEntidade> _equipamentoRepository;

        public EquipamentoService(IConfiguration configuration)
        {
            _repository = new Repository(configuration);
            _equipamentoRepository = _repository.GetRepository<ModeloEquipamentoEntidade>();
        }

        public async Task<IEnumerable<ModeloEquipamento>> GetAllEquipamentosAsync()
        {
            var equipamentos = await _equipamentoRepository.GetAllAsync();
            return equipamentos.Select(MapToBLL);
        }
        public async Task<IEnumerable<ModeloEquipamento>> GetActiveEquipamentosAsync()
        {
            var equipamentos = await _equipamentoRepository.GetAllAsync();
            return equipamentos.Where(e => !e.IsDeleted).Select(MapToBLL);
        }

        public async Task<ModeloEquipamento?> GetEquipamentoByIdAsync(int id)
        {
            var equipamento = await _equipamentoRepository.GetByIdAsync(id);
            return equipamento != null ? MapToBLL(equipamento) : null;
        }

        public async Task<ModeloEquipamento> CreateEquipamentoAsync(ModeloEquipamento equipamento)
        {
            var dalEquipamento = MapToDAL(equipamento);
            var created = await _equipamentoRepository.AddAsync(dalEquipamento);
            return MapToBLL(created);
        }

        public async Task<ModeloEquipamento> UpdateEquipamentoAsync(int id, ModeloEquipamento equipamento)
        {
            var existing = await _equipamentoRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Equipamento with ID {id} not found.");

            // Atualizar campos
            existing.Descricao = equipamento.Descricao;
            existing.Identificacao = equipamento.Identificacao;
            existing.TipoAD = (int)equipamento.TipoAD;
            // Adicione outros campos conforme necessário

            var updated = await _equipamentoRepository.UpdateAsync(existing);
            return MapToBLL(updated);
        }

        public async Task DeleteEquipamentoAsync(int id)
        {
            await _equipamentoRepository.DeleteAsync(id);
        }

        private static ModeloEquipamento MapToBLL(ModeloEquipamentoEntidade entidade)
        {
            return new ModeloEquipamento
            {
                Id = entidade.Id,
                Descricao = entidade.Descricao,
                Marca = entidade.Marca,
                Identificacao = entidade.Identificacao,
                TipoAD = (AnalogicoDigital)entidade.TipoAD,
                CreatedAt = entidade.CreatedAt
            };
        }

        private static ModeloEquipamentoEntidade MapToDAL(ModeloEquipamento modelo)
        {
            return new ModeloEquipamentoEntidade
            {
                Id = modelo.Id,
                Descricao = modelo.Descricao,
                Marca = modelo.Marca,
                Identificacao = modelo.Identificacao,
                TipoAD = (int)modelo.TipoAD
            };
        }
    }
} 