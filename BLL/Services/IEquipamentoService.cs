using BLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services
{
    public interface IEquipamentoService
    {
        Task<IEnumerable<ModeloEquipamento>> GetAllEquipamentosAsync();
        Task<IEnumerable<ModeloEquipamento>> GetActiveEquipamentosAsync();
        Task<ModeloEquipamento?> GetEquipamentoByIdAsync(int id);
        Task<ModeloEquipamento> CreateEquipamentoAsync(ModeloEquipamento equipamento);
        Task<ModeloEquipamento> UpdateEquipamentoAsync(int id, ModeloEquipamento equipamento);
        Task DeleteEquipamentoAsync(int id);
    }
} 