using BLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services
{
    public interface IPessoaService
    {
        Task<IEnumerable<Pessoa>> GetAllPessoasAsync();
        Task<Pessoa?> GetPessoaByIdAsync(int id);
        Task<Pessoa> CreatePessoaAsync(Pessoa pessoa);
        Task<Pessoa> UpdatePessoaAsync(int id, Pessoa pessoa);
        Task DeletePessoaAsync(int id);
    }
} 