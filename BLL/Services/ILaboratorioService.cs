using BLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services
{
    public interface ILaboratorioService
    {
        Task<IEnumerable<Laboratorio>> GetAllLaboratoriosAsync();
        Task<IEnumerable<Laboratorio>> GetActiveLaboratoriosAsync();
        Task<Laboratorio?> GetLaboratorioByIdAsync(int id);
        Task<Laboratorio> CreateLaboratorioAsync(Laboratorio laboratorio);
        Task<Laboratorio> UpdateLaboratorioAsync(int id, Laboratorio laboratorio);
        Task DeleteLaboratorioAsync(int id);
    }
} 