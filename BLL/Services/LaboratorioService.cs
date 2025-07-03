using BLL;
using BLL.Models;
using DAL;
using DAL.Models;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;

namespace BLL.Services
{
    public class LaboratorioService : ILaboratorioService
    {
        private readonly Repository _repository;
        private readonly IGenericRepository<DAL.Models.LaboratorioEntidade> _laboratorioRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public LaboratorioService(IConfiguration configuration, IUsuarioRepository usuarioRepository)
        {
            _repository = new Repository(configuration);
            _laboratorioRepository = _repository.GetRepository<DAL.Models.LaboratorioEntidade>();
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IEnumerable<Laboratorio>> GetAllLaboratoriosAsync()
        {
            var laboratorios = await _laboratorioRepository.GetAllAsync();
            foreach (var lab in laboratorios)
            {
                lab.Responsaveis = (await _usuarioRepository.GetByLaboratorioIdAsync(lab.Id)).Select(u => u.Pessoa).ToList();
            }
            laboratorios = laboratorios.OrderBy(p => p.Id);
            return laboratorios.Select(p => MapToBLL(p));
        }
        public async Task<IEnumerable<Laboratorio>> GetActiveLaboratoriosAsync()
        {
            var laboratorios = await _laboratorioRepository.GetAllAsync();
            foreach (var lab in laboratorios)
            {
                lab.Responsaveis = (await _usuarioRepository.GetByLaboratorioIdAsync(lab.Id)).Select(u => u.Pessoa).ToList();
            }
            laboratorios = laboratorios.OrderBy(p => p.Id);
            return laboratorios.Where(l => !l.IsDeleted).Select(p => MapToBLL(p));
        }

        public async Task<Laboratorio?> GetLaboratorioByIdAsync(int id)
        {
            var laboratorio = await _laboratorioRepository.GetByIdAsync(id);
            var responsaveis = (await _usuarioRepository.GetByLaboratorioIdAsync(laboratorio.Id)).Select(u => u.Pessoa).ToList();
            laboratorio.Responsaveis = responsaveis;

            return laboratorio != null ? MapToBLL(laboratorio) : null;
        }

        public async Task<Laboratorio> CreateLaboratorioAsync(Laboratorio Laboratorio)
        {
            var dalLaboratorio = MapToDAL(Laboratorio);
            var created = await _laboratorioRepository.AddAsync(dalLaboratorio);
            return MapToBLL(created);
        }

        public async Task<Laboratorio> UpdateLaboratorioAsync(int id, Laboratorio Laboratorio)
        {
            var existingLaboratorio = await _laboratorioRepository.GetByIdAsync(id);
            if (existingLaboratorio == null)
                throw new KeyNotFoundException($"Laboratorio with ID {id} not found.");

            existingLaboratorio.Nome = Laboratorio.Nome;
            existingLaboratorio.Endereco = Laboratorio.Endereco;

            var updated = await _laboratorioRepository.UpdateAsync(existingLaboratorio);
            return MapToBLL(updated);
        }

        public async Task DeleteLaboratorioAsync(int id)
        {
            await _laboratorioRepository.DeleteAsync(id);
        }

        private static Laboratorio MapToBLL(DAL.Models.LaboratorioEntidade Laboratorio)
        {
            return new Laboratorio
            {
                Id = Laboratorio.Id,
                Codigo = Laboratorio.Codigo,
                Nome = Laboratorio.Nome,
                Endereco = Laboratorio.Endereco,
                DataCriacao = Laboratorio.CreatedAt
            };
        }

        private static DAL.Models.LaboratorioEntidade MapToDAL(Laboratorio Laboratorio)
        {
            return new DAL.Models.LaboratorioEntidade
            {
                Id = Laboratorio.Id,
                Codigo = Laboratorio.Codigo,
                Nome = Laboratorio.Nome,
                Endereco = Laboratorio.Endereco
            };
        }
    }
}
