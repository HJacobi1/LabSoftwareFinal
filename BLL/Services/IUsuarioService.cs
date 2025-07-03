using BLL.Models;

namespace BLL.Services
{
    public interface IUsuarioService
    {
        Task<Usuario?> AuthenticateAsync(string email, string senha);
        Task<Usuario> RegisterAsync(Usuario usuario);
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario?> GetByIdAsync(int id);
        Task<Usuario> UpdateAsync(int id, Usuario usuario);
        Task DeleteAsync(int id);
        Task<bool> EmailExistsAsync(string email);
    }
} 