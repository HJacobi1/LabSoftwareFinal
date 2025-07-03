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
    public class SolicitacaoService : ISolicitacaoService
    {
        private readonly Repository _repository;
        private readonly IGenericRepository<SolicitacaoEntidade> _solicitacaoRepository;

        public SolicitacaoService(IConfiguration configuration)
        {            
            _repository = new Repository(configuration);
            _solicitacaoRepository = _repository.GetRepository<SolicitacaoEntidade>();
        }

        public async Task<IEnumerable<Solicitacao>> GetAllSolicitacoesAsync()
        {
            var solicitacoes = await _solicitacaoRepository.GetAllAsync();
            return solicitacoes.Select(MapToBLL);
        }

        public async Task<Solicitacao?> GetSolicitacaoByIdAsync(int id)
        {
            var solicitacao = await _solicitacaoRepository.GetByIdAsync(id);
            return solicitacao != null ? MapToBLL(solicitacao) : null;
        }

        public async Task<Solicitacao> CreateSolicitacaoAsync(Solicitacao solicitacao)
        {
            var dalSolicitacao = MapToDAL(solicitacao);
            var created = await _solicitacaoRepository.AddAsync(dalSolicitacao);
            return MapToBLL(created);
        }

        public async Task<Solicitacao> UpdateSolicitacaoAsync(int id, Solicitacao solicitacao)
        {
            var existing = await _solicitacaoRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Solicitacao with ID {id} not found.");

            // Atualizar campos
            existing.Descricao = solicitacao.Descricao;
            // Adicione outros campos conforme necessário

            var updated = await _solicitacaoRepository.UpdateAsync(existing);
            return MapToBLL(updated);
        }

        public async Task DeleteSolicitacaoAsync(int id)
        {
            await _solicitacaoRepository.DeleteAsync(id);
        }

        private static Solicitacao MapToBLL(SolicitacaoEntidade entidade)
        {
            return new Solicitacao
            {
                Id = entidade.Id,
                Descricao = entidade.Descricao,
                Data = entidade.Data,
                TipoMC = (ManutencaoCalibracao)entidade.TipoMC,
            };
        }

        private static SolicitacaoEntidade MapToDAL(Solicitacao modelo)
        {
            return new SolicitacaoEntidade
            {
                Id = modelo.Id,
                Descricao = modelo.Descricao,
                Data = modelo.Data,
                TipoMC = (int)modelo.TipoMC,
            };
        }
    }
} 