using BLL.Models;
using BLL.Services;
using DAL.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.BLL.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _mockRepo;
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _mockRepo = new Mock<IUsuarioRepository>();
            var config = new ConfigurationBuilder().Build();
            _service = new UsuarioService(_mockRepo.Object, config);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsUsuario_WhenFound()
        {
            var usuarioEntidade = new UsuarioEntidade
            {
                Id = 1,
                Email = "teste@teste.com",
                Senha = "hashedpass",
                IsAdmin = false,
                PessoaId = 5,
                Pessoa = new PessoaEntidade { LaboratorioId = 10 }
            };
            _mockRepo.Setup(r => r.AuthenticateAsync("teste@teste.com", "senha"))
                     .ReturnsAsync(usuarioEntidade);

            var result = await _service.AuthenticateAsync("teste@teste.com", "senha");

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal(10, result.LaboratorioId);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
                     .ReturnsAsync((UsuarioEntidade)null);

            var result = await _service.AuthenticateAsync("email", "senha");

            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterAsync_Throws_WhenEmailExists()
        {
            var usuario = new Usuario { Email = "existente@teste.com", Senha = "123", IsAdmin = false, PessoaId = 1 };
            _mockRepo.Setup(r => r.EmailExistsAsync(usuario.Email)).ReturnsAsync(true);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterAsync(usuario));
        }

        [Fact]
        public async Task RegisterAsync_Throws_WhenNonAdminWithoutPessoa()
        {
            var usuario = new Usuario { Email = "novo@teste.com", Senha = "123", IsAdmin = false, PessoaId = null };
            _mockRepo.Setup(r => r.EmailExistsAsync(usuario.Email)).ReturnsAsync(false);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RegisterAsync(usuario));
        }

        [Fact]
        public async Task RegisterAsync_HashesPassword_AndReturnsUsuario()
        {
            var usuario = new Usuario { Email = "novo@teste.com", Senha = "senha123", IsAdmin = true };

            _mockRepo.Setup(r => r.EmailExistsAsync(usuario.Email)).ReturnsAsync(false);
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<UsuarioEntidade>()))
                     .ReturnsAsync((UsuarioEntidade u) => u);

            var result = await _service.RegisterAsync(usuario);

            Assert.NotNull(result);
            Assert.Equal(usuario.Email, result.Email);
            Assert.NotEqual("senha123", result.Senha);
            Assert.True(result.Senha.StartsWith("$2")); // BCrypt hash usualmente começa assim
        }

        [Fact]
        public async Task GetAllAsync_ReturnsUsuarios()
        {
            var usuarios = new List<UsuarioEntidade>
            {
                new UsuarioEntidade { Id = 1, Email = "a@a.com", Senha = "h1" },
                new UsuarioEntidade { Id = 2, Email = "b@b.com", Senha = "h2" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(usuarios);

            var result = await _service.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetActiveUsersAsync_ReturnsOnlyNotDeleted()
        {
            var usuarios = new List<UsuarioEntidade>
            {
                new UsuarioEntidade { Id = 1, Email = "a@a.com", Senha = "h1", IsDeleted = false },
                new UsuarioEntidade { Id = 2, Email = "b@b.com", Senha = "h2", IsDeleted = true }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(usuarios);

            var result = await _service.GetActiveUsersAsync();

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsUsuario()
        {
            var usuario = new UsuarioEntidade { Id = 1, Email = "teste@teste.com", Senha = "h1" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(usuario);

            var result = await _service.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("teste@teste.com", result.Email);
        }

        [Fact]
        public async Task UpdateAsync_Throws_WhenUsuarioNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((UsuarioEntidade)null);
            var usuario = new Usuario { Email = "novo@teste.com", Senha = "123" };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(1, usuario));
        }

        [Fact]
        public async Task UpdateAsync_HashesPassword_AndReturnsUsuario()
        {
            var existing = new UsuarioEntidade
            {
                Id = 1,
                Email = "old@teste.com",
                Senha = BCrypt.Net.BCrypt.HashPassword("oldsenha")
            };

            var updated = new UsuarioEntidade
            {
                Id = 1,
                Email = "novo@teste.com",
                Senha = "hashednova"
            };

            var usuario = new Usuario { Email = "novo@teste.com", Senha = "novasenha" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<UsuarioEntidade>())).ReturnsAsync(updated);

            var result = await _service.UpdateAsync(1, usuario);

            Assert.Equal("novo@teste.com", result.Email);
            Assert.NotEqual(usuario.Senha, result.Senha);
            Assert.True(result.Senha.StartsWith("$2"));
        }

        [Fact]
        public async Task UpdateAsync_WithoutPassword_UpdatesEmailOnly()
        {
            var existing = new UsuarioEntidade
            {
                Id = 1,
                Email = "old@teste.com",
                Senha = BCrypt.Net.BCrypt.HashPassword("oldsenha")
            };

            var updated = new UsuarioEntidade
            {
                Id = 1,
                Email = "novo@teste.com",
                Senha = existing.Senha
            };

            var usuario = new Usuario { Email = "novo@teste.com", Senha = "" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<UsuarioEntidade>())).ReturnsAsync(updated);

            var result = await _service.UpdateAsync(1, usuario);

            Assert.Equal("novo@teste.com", result.Email);
            Assert.Equal(existing.Senha, result.Senha);
        }

        [Fact]
        public async Task DeleteAsync_CallsDelete()
        {
            _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            await _service.DeleteAsync(1);

            _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task EmailExistsAsync_ReturnsTrueOrFalse()
        {
            _mockRepo.Setup(r => r.EmailExistsAsync("email@teste.com")).ReturnsAsync(true);

            var result = await _service.EmailExistsAsync("email@teste.com");

            Assert.True(result);
        }
    }
}
