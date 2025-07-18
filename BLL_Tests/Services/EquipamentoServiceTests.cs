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
        public async Task CadastroEquipamento_AdminPreencheDados_EquipamentoSalvoComSucesso()
        {
            // Given (Dado que o admin preenche os dados)
            var dadosEquipamento = new ModeloEquipamento
            {
                Id = 0, // ID zero indica novo equipamento
                Identificacao = "MIC-001",
                Descricao = "Microscópio Eletrônico de Varredura",
                TipoAD = AnalogicoDigital.Digital,
                Marca = "Zeiss",
                CreatedAt = DateTime.Now
            };

            var equipamentoSalvo = new ModeloEquipamentoEntidade
            {
                Id = 1, // ID gerado pelo banco
                Identificacao = "MIC-001",
                Descricao = "Microscópio Eletrônico de Varredura",
                TipoAD = (int)AnalogicoDigital.Digital,
                Marca = "Zeiss",
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            // Configurar o mock para simular o salvamento
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<ModeloEquipamentoEntidade>()))
                    .ReturnsAsync(equipamentoSalvo);

            // When (quando submete)
            var resultado = await _service.CreateEquipamentoAsync(dadosEquipamento);

            // Then (então o equipamento é salvo)
            Assert.NotNull(resultado);
            Assert.Equal(1, resultado.Id); // Verifica se foi gerado um ID
            Assert.Equal("MIC-001", resultado.Identificacao);
            Assert.Equal("Microscópio Eletrônico de Varredura", resultado.Descricao);
            Assert.Equal(AnalogicoDigital.Digital, resultado.TipoAD);
            Assert.Equal("Zeiss", resultado.Marca);
            Assert.True(resultado.CreatedAt > DateTime.MinValue);

            // Verificar se o método AddAsync foi chamado
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<ModeloEquipamentoEntidade>()), Times.Once);
        }

        [Fact]
        public async Task CadastroEquipamento_DadosInvalidos_RetornaErro()
        {
            // Given (Dado que o admin preenche dados inválidos)
            var dadosInvalidos = new ModeloEquipamento
            {
                Id = 0,
                Identificacao = "", // Identificação vazia
                Descricao = "", // Descrição vazia
                TipoAD = AnalogicoDigital.Digital,
                Marca = "", // Marca vazia
                CreatedAt = DateTime.Now
            };

            // Configurar o mock para simular erro de validação
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<ModeloEquipamentoEntidade>()))
                    .ThrowsAsync(new ArgumentException("Dados inválidos"));

            // When & Then (quando submete, então retorna erro)
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateEquipamentoAsync(dadosInvalidos)
            );

            Assert.Contains("Dados inválidos", exception.Message);
        }

        [Fact]
        public async Task CadastroEquipamento_IdentificacaoDuplicada_RetornaErro()
        {
            // Given (Dado que já existe um equipamento com a mesma identificação)
            var dadosEquipamento = new ModeloEquipamento
            {
                Id = 0,
                Identificacao = "MIC-001",
                Descricao = "Microscópio Eletrônico",
                TipoAD = AnalogicoDigital.Digital,
                Marca = "Zeiss",
                CreatedAt = DateTime.Now
            };

            // Configurar o mock para simular erro de duplicação
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<ModeloEquipamentoEntidade>()))
                    .ThrowsAsync(new InvalidOperationException("Identificação já existe"));

            // When & Then (quando submete, então retorna erro de duplicação)
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CreateEquipamentoAsync(dadosEquipamento)
            );

            Assert.Contains("Identificação já existe", exception.Message);
        }

        [Theory]
        [InlineData("MIC-001", "Microscópio Eletrônico", "Zeiss", AnalogicoDigital.Digital)]
        [InlineData("BAL-002", "Balança Analítica", "Mettler Toledo", AnalogicoDigital.Analógico)]
        [InlineData("PHM-003", "pHmetro", "Hanna Instruments", AnalogicoDigital.Digital)]
        public async Task CadastroEquipamento_DiferentesTipos_EquipamentosSalvosComSucesso(
            string identificacao, string descricao, string marca, AnalogicoDigital tipoAD)
        {
            // Given (Dado que o admin preenche diferentes tipos de equipamentos)
            var dadosEquipamento = new ModeloEquipamento
            {
                Id = 0,
                Identificacao = identificacao,
                Descricao = descricao,
                TipoAD = tipoAD,
                Marca = marca,
                CreatedAt = DateTime.Now
            };

            var equipamentoSalvo = new ModeloEquipamentoEntidade
            {
                Id = 1,
                Identificacao = identificacao,
                Descricao = descricao,
                TipoAD = (int)tipoAD,
                Marca = marca,
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<ModeloEquipamentoEntidade>()))
                    .ReturnsAsync(equipamentoSalvo);

            // When (quando submete)
            var resultado = await _service.CreateEquipamentoAsync(dadosEquipamento);

            // Then (então o equipamento é salvo corretamente)
            Assert.NotNull(resultado);
            Assert.Equal(identificacao, resultado.Identificacao);
            Assert.Equal(descricao, resultado.Descricao);
            Assert.Equal(marca, resultado.Marca);
            Assert.Equal(tipoAD, resultado.TipoAD);
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
