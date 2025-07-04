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
            return await _dbSet
                .Include(u => u.Pessoa)
                .ThenInclude(p => p.Laboratorio)
                .FirstOrDefaultAsync(u => u.Email == email);
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

        public async Task<List<UsuarioEntidade>> GetByLaboratorioIdAsync(int id)
        {
            var usuarios = _dbSet
                .Include(u => u.Pessoa)
                .Where(u => u.Pessoa.LaboratorioId == id).ToList();

            return usuarios;
        }

        public async Task<UsuarioEntidade?> GetByPessoaIdAsync(int id)
        {
            var usuarios = await GetAllAsync();
            return usuarios.Where(u => u.Pessoa != null && u.Pessoa.Id == id).FirstOrDefault();
        }
    }
} 