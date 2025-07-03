using BLL.Models;
using BLL.Services;
using DAL.Models;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.BLL.Services
{
    public class PessoaServiceTests
    {
        private readonly Mock<IGenericRepository<DAL.Models.Pessoa>> _mockRepo;
        private readonly PessoaService _service;

        public PessoaServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<DAL.Models.Pessoa>>();

            var config = new ConfigurationBuilder().Build();

            var repositoryMock = new Mock<Repository>(config);
            repositoryMock.Setup(r => r.GetRepository<DAL.Models.Pessoa>())
                          .Returns(_mockRepo.Object);

            _service = new PessoaServiceFake(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllPessoasAsync_ReturnsOrderedList()
        {
            var pessoas = new List<DAL.Models.Pessoa>
            {
                new DAL.Models.Pessoa { Id = 2, Nome = "Maria", Contato = "2222" },
                new DAL.Models.Pessoa { Id = 1, Nome = "João", Contato = "1111" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(pessoas);

            var result = await _service.GetAllPessoasAsync();

            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public async Task GetPessoaByIdAsync_ReturnsCorrectPessoa()
        {
            var pessoa = new DAL.Models.Pessoa { Id = 1, Nome = "João", Contato = "1111" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(pessoa);

            var result = await _service.GetPessoaByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("João", result.Nome);
        }

        [Fact]
        public async Task CreatePessoaAsync_CreatesAndReturnsPessoa()
        {
            var pessoaToCreate = new Pessoa { Id = 0, Nome = "Ana", Contato = "3333" };
            var createdPessoa = new DAL.Models.Pessoa { Id = 1, Nome = "Ana", Contato = "3333" };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<DAL.Models.Pessoa>()))
                     .ReturnsAsync(createdPessoa);

            var result = await _service.CreatePessoaAsync(pessoaToCreate);

            Assert.Equal(1, result.Id);
            Assert.Equal("Ana", result.Nome);
        }

        [Fact]
        public async Task UpdatePessoaAsync_UpdatesAndReturnsPessoa()
        {
            var existingPessoa = new DAL.Models.Pessoa { Id = 1, Nome = "Carlos", Contato = "9999" };
            var updatedPessoa = new DAL.Models.Pessoa { Id = 1, Nome = "Carlos Atualizado", Contato = "8888" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingPessoa);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<DAL.Models.Pessoa>())).ReturnsAsync(updatedPessoa);

            var updatedData = new Pessoa { Id = 1, Nome = "Carlos Atualizado", Contato = "8888" };

            var result = await _service.UpdatePessoaAsync(1, updatedData);

            Assert.Equal("Carlos Atualizado", result.Nome);
            Assert.Equal("8888", result.Contato);
        }

        [Fact]
        public async Task DeletePessoaAsync_CallsRepositoryDelete()
        {
            _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            await _service.DeletePessoaAsync(1);

            _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        private class PessoaServiceFake : PessoaService
        {
            public PessoaServiceFake(IGenericRepository<DAL.Models.Pessoa> mockRepo)
                : base(new ConfigurationBuilder().Build())
            {
                typeof(PessoaService)
                    .GetField("_pessoaRepository", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(this, mockRepo);
            }
        }
    }
}
