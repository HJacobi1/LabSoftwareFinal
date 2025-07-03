using DAL.Models;

namespace DAL.Repositories
{
    public interface IUsuarioRepository : IGenericRepository< UsuarioEntidade>
    {
        Task<UsuarioEntidade?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<UsuarioEntidade?> AuthenticateAsync(string email, string senha);
    }
} 