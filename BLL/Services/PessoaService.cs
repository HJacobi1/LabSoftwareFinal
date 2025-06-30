using BLL;
using BLL.Models;
using DAL;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;

namespace BLL.Services
{
    public class PessoaService
    {
        private readonly Repository _repository;
        private readonly IGenericRepository<DAL.Models.Pessoa> _pessoaRepository;

        public PessoaService(IConfiguration configuration)
        {
            _repository = new Repository(configuration);
            _pessoaRepository = _repository.GetRepository<DAL.Models.Pessoa>();
        }

        public async Task<IEnumerable<Pessoa>> GetAllPessoasAsync()
        {
            var pessoas = await _pessoaRepository.GetAllAsync();
            pessoas = pessoas.OrderBy(p => p.Id);
            return pessoas.Select(p => MapToBLL(p));
        }

        public async Task<Pessoa?> GetPessoaByIdAsync(int id)
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(id);
            return pessoa != null ? MapToBLL(pessoa) : null;
        }

        public async Task<Pessoa> CreatePessoaAsync(Pessoa pessoa)
        {
            var dalPessoa = MapToDAL(pessoa);
            var created = await _pessoaRepository.AddAsync(dalPessoa);
            return MapToBLL(created);
        }

        public async Task<Pessoa> UpdatePessoaAsync(int id, Pessoa pessoa)
        {
            var existingPessoa = await _pessoaRepository.GetByIdAsync(id);
            if (existingPessoa == null)
                throw new KeyNotFoundException($"Pessoa with ID {id} not found.");

            existingPessoa.Nome = pessoa.Nome;
            existingPessoa.Contato = pessoa.Contato;

            var updated = await _pessoaRepository.UpdateAsync(existingPessoa);
            return MapToBLL(updated);
        }

        public async Task DeletePessoaAsync(int id)
        {
            await _pessoaRepository.DeleteAsync(id);
        }

        private static Pessoa MapToBLL(DAL.Models.Pessoa pessoa)
        {
            return new Pessoa
            {
                Id = pessoa.Id,
                Nome = pessoa.Nome,
                Contato = pessoa.Contato
            };
        }

        private static DAL.Models.Pessoa MapToDAL(Pessoa pessoa)
        {
            return new DAL.Models.Pessoa
            {
                Id = pessoa.Id,
                Nome = pessoa.Nome,
                Contato = pessoa.Contato
            };
        }
    }
}
