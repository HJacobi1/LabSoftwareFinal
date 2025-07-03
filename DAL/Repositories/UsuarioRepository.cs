using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class UsuarioRepository : GenericRepository<UsuarioEntidade>, IUsuarioRepository
    {
        public UsuarioRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<UsuarioEntidade?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<UsuarioEntidade?> AuthenticateAsync(string email, string senha)
        {
            var usuario = await GetByEmailAsync(email);
            if (usuario == null) return null;

            // Verificar se a senha está correta usando BCrypt
            if (BCrypt.Net.BCrypt.Verify(senha, usuario.Senha))
            {
                return usuario;
            }

            return null;
        }
    }
} 