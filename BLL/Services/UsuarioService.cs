using BLL;
using DAL.Models;
using BLL.Models;
using DAL;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfiguration _configuration;

        public UsuarioService(IUsuarioRepository usuarioRepository, IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _configuration = configuration;
        }

        public async Task<Usuario?> AuthenticateAsync(string email, string senha)
        {
            // Usar o repositório para autenticação
            var usuario = await _usuarioRepository.AuthenticateAsync(email, senha);
            
            return MapToBLL(usuario);
        }

        public async Task<Usuario> RegisterAsync(Usuario usuario)
        {
            // Verificar se o email já existe
            if (await _usuarioRepository.EmailExistsAsync(usuario.Email))
            {
                throw new InvalidOperationException("Email já cadastrado.");
            }

            // Hash da senha antes de salvar
            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);

            // Usar o repositório para salvar
            var salvo = await _usuarioRepository.AddAsync(MapToDAL(usuario));

            return MapToBLL(salvo);
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            var result = await _usuarioRepository.GetAllAsync();
            // Mapear entidades para modelos de negócio
            return result.Select(MapToBLL);
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            return MapToBLL(await _usuarioRepository.GetByIdAsync(id));
        }

        public async Task<Usuario> UpdateAsync(int id, Usuario usuario)
        {
            var existingUsuario = await _usuarioRepository.GetByIdAsync(id);
            if (existingUsuario == null)
            {
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado.");
            }

            // Atualizar propriedades
            existingUsuario.Email = usuario.Email;
            
            // Hash da senha apenas se foi fornecida
            if (!string.IsNullOrEmpty(usuario.Senha))
            {
                existingUsuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
            }

            return MapToBLL(await _usuarioRepository.UpdateAsync(existingUsuario));
        }

        public async Task DeleteAsync(int id)
        {
            await _usuarioRepository.DeleteAsync(id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _usuarioRepository.EmailExistsAsync(email);
        }

        private Usuario MapToBLL(UsuarioEntidade usuario)
        {
            return new Usuario()
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Senha = usuario.Senha,
                IsAdmin = usuario.IsAdmin
            };
        }
        private UsuarioEntidade MapToDAL(Usuario usuario)
        {
            return new UsuarioEntidade()
            {
                Id = usuario.Id,
                Email = usuario.Email,
                Senha = usuario.Senha,
                IsAdmin = usuario.IsAdmin
            };
        }
    }
}
