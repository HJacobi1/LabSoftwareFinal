using BLL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services
{
    public interface ISolicitacaoService
    {
        Task<IEnumerable<Solicitacao>> GetAllSolicitacoesAsync();
        Task<Solicitacao?> GetSolicitacaoByIdAsync(int id);
        Task<Solicitacao> CreateSolicitacaoAsync(Solicitacao solicitacao);
        Task<Solicitacao> UpdateSolicitacaoAsync(int id, Solicitacao solicitacao);
        Task DeleteSolicitacaoAsync(int id);
    }
} 