using BLL;
using BLL.Models;
using DAL;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;

namespace BLL.Services
{
    public class LaboratorioService
    {
        private readonly Repository _repository;
        private readonly IGenericRepository<DAL.Models.LaboratorioEntidade> _laboratorioRepository;

        public LaboratorioService(IConfiguration configuration)
        {
            _repository = new Repository(configuration);
            _laboratorioRepository = _repository.GetRepository<DAL.Models.LaboratorioEntidade>();
        }

        public async Task<IEnumerable<Laboratorio>> GetAllLaboratoriosAsync()
        {
            var Laboratorios = await _laboratorioRepository.GetAllAsync();
            Laboratorios = Laboratorios.OrderBy(p => p.Id);
            return Laboratorios.Select(p => MapToBLL(p));
        }

        public async Task<Laboratorio?> GetLaboratorioByIdAsync(int id)
        {
            var Laboratorio = await _laboratorioRepository.GetByIdAsync(id);
            return Laboratorio != null ? MapToBLL(Laboratorio) : null;
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
                Nome = Laboratorio.Nome,
                Endereco = Laboratorio.Endereco
            };
        }

        private static DAL.Models.LaboratorioEntidade MapToDAL(Laboratorio Laboratorio)
        {
            return new DAL.Models.LaboratorioEntidade
            {
                Id = Laboratorio.Id,
                Nome = Laboratorio.Nome,
                Endereco = Laboratorio.Endereco
            };
        }
    }
}
