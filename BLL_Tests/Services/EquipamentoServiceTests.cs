using BLL.Enums;
using BLL.Models;
using BLL.Services;
using DAL.Models;
using DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.BLL.Services
{
    public class EquipamentoServiceTests
    {
        private readonly Mock<IGenericRepository<ModeloEquipamentoEntidade>> _mockRepo;
        private readonly EquipamentoService _service;

        public EquipamentoServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<ModeloEquipamentoEntidade>>();

            var config = new ConfigurationBuilder().Build();

            var repositoryMock = new Mock<Repository>(config);
            repositoryMock.Setup(r => r.GetRepository<ModeloEquipamentoEntidade>())
                          .Returns(_mockRepo.Object);

            _service = new EquipamentoServiceFake(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllEquipamentosAsync_ReturnsAll()
        {
            var equipamentos = new List<ModeloEquipamentoEntidade>
            {
                new ModeloEquipamentoEntidade { Id = 1, Descricao = "Equipamento 1" },
                new ModeloEquipamentoEntidade { Id = 2, Descricao = "Equipamento 2" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(equipamentos);

            var result = await _service.GetAllEquipamentosAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetActiveEquipamentosAsync_ReturnsOnlyNotDeleted()
        {
            var equipamentos = new List<ModeloEquipamentoEntidade>
            {
                new ModeloEquipamentoEntidade { Id = 1, IsDeleted = false },
                new ModeloEquipamentoEntidade { Id = 2, IsDeleted = true }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(equipamentos);

            var result = await _service.GetActiveEquipamentosAsync();

            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public async Task GetEquipamentoByIdAsync_ReturnsCorrect()
        {
            var equipamento = new ModeloEquipamentoEntidade { Id = 1, Descricao = "Equipamento" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(equipamento);

            var result = await _service.GetEquipamentoByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Equipamento", result.Descricao);
        }

        [Fact]
        public async Task CreateEquipamentoAsync_ReturnsCreated()
        {
            var input = new ModeloEquipamento
            {
                Id = 0,
                Descricao = "Novo",
                Marca = "MarcaX",
                Identificacao = "ID-01",
                TipoAD = AnalogicoDigital.Digital
            };

            var created = new ModeloEquipamentoEntidade
            {
                Id = 1,
                Descricao = "Novo",
                Marca = "MarcaX",
                Identificacao = "ID-01",
                TipoAD = (int)AnalogicoDigital.Digital
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<ModeloEquipamentoEntidade>())).ReturnsAsync(created);

            var result = await _service.CreateEquipamentoAsync(input);

            Assert.Equal(1, result.Id);
            Assert.Equal("MarcaX", result.Marca);
        }

        [Fact]
        public async Task UpdateEquipamentoAsync_UpdatesAndReturns()
        {
            var existing = new ModeloEquipamentoEntidade
            {
                Id = 1,
                Descricao = "Antigo",
                Identificacao = "ID-001",
                TipoAD = (int)AnalogicoDigital.Analógico
            };

            var updated = new ModeloEquipamentoEntidade
            {
                Id = 1,
                Descricao = "Novo",
                Identificacao = "ID-999",
                TipoAD = (int)AnalogicoDigital.Digital
            };

            var input = new ModeloEquipamento
            {
                Id = 1,
                Descricao = "Novo",
                Identificacao = "ID-999",
                TipoAD = AnalogicoDigital.Digital
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<ModeloEquipamentoEntidade>())).ReturnsAsync(updated);

            var result = await _service.UpdateEquipamentoAsync(1, input);

            Assert.Equal("Novo", result.Descricao);
            Assert.Equal("ID-999", result.Identificacao);
            Assert.Equal(AnalogicoDigital.Digital, result.TipoAD);
        }

        [Fact]
        public async Task DeleteEquipamentoAsync_DeletesCorrectly()
        {
            _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            await _service.DeleteEquipamentoAsync(1);

            _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        private class EquipamentoServiceFake : EquipamentoService
        {
            public EquipamentoServiceFake(IGenericRepository<ModeloEquipamentoEntidade> mockRepo)
                : base(new ConfigurationBuilder().Build())
            {
                typeof(EquipamentoService)
                    .GetField("_equipamentoRepository", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(this, mockRepo);
            }
        }
    }
}
