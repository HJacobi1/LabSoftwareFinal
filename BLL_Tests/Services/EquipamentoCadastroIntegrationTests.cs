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
    /// <summary>
    /// Testes de integração para o cadastro de equipamentos
    /// Seguindo o padrão Given-When-Then (Dado-Quando-Então)
    /// </summary>
    public class EquipamentoCadastroIntegrationTests
    {
        private readonly Mock<IGenericRepository<ModeloEquipamentoEntidade>> _mockRepo;
        private readonly EquipamentoService _service;

        public EquipamentoCadastroIntegrationTests()
        {
            _mockRepo = new Mock<IGenericRepository<ModeloEquipamentoEntidade>>();
            var config = new ConfigurationBuilder().Build();
            _service = new EquipamentoServiceFake(_mockRepo.Object);
        }

        /// <summary>
        /// Cenário: Cadastro bem-sucedido de equipamento
        /// Given: Admin preenche dados válidos
        /// When: Submete o formulário
        /// Then: Equipamento é salvo com sucesso
        /// </summary>
        [Fact]
        public async Task CadastroEquipamento_AdminPreencheDadosValidos_EquipamentoSalvoComSucesso()
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

        /// <summary>
        /// Cenário: Cadastro com dados inválidos
        /// Given: Admin preenche dados inválidos
        /// When: Submete o formulário
        /// Then: Retorna erro de validação
        /// </summary>
        [Fact]
        public async Task CadastroEquipamento_DadosInvalidos_RetornaErroValidacao()
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

        /// <summary>
        /// Cenário: Cadastro com identificação duplicada
        /// Given: Já existe equipamento com mesma identificação
        /// When: Admin tenta cadastrar equipamento duplicado
        /// Then: Retorna erro de duplicação
        /// </summary>
        [Fact]
        public async Task CadastroEquipamento_IdentificacaoDuplicada_RetornaErroDuplicacao()
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

        /// <summary>
        /// Cenário: Cadastro de diferentes tipos de equipamentos
        /// Given: Admin preenche diferentes tipos de equipamentos
        /// When: Submete cada formulário
        /// Then: Todos os equipamentos são salvos corretamente
        /// </summary>
        [Theory]
        [InlineData("MIC-001", "Microscópio Eletrônico", "Zeiss", AnalogicoDigital.Digital)]
        [InlineData("BAL-002", "Balança Analítica", "Mettler Toledo", AnalogicoDigital.Analógico)]
        [InlineData("PHM-003", "pHmetro", "Hanna Instruments", AnalogicoDigital.Digital)]
        [InlineData("CEN-004", "Centrífuga", "Eppendorf", AnalogicoDigital.Digital)]
        [InlineData("TER-005", "Termômetro", "Fluke", AnalogicoDigital.Analógico)]
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

        /// <summary>
        /// Cenário: Cadastro em lote de equipamentos
        /// Given: Admin preenche múltiplos equipamentos
        /// When: Submete todos os formulários
        /// Then: Todos são salvos e listados corretamente
        /// </summary>
        [Fact]
        public async Task CadastroEquipamento_LoteEquipamentos_TodosSalvosEListados()
        {
            // Given (Dado que o admin preenche múltiplos equipamentos)
            var equipamentos = new List<ModeloEquipamento>
            {
                new ModeloEquipamento { Id = 0, Identificacao = "MIC-001", Descricao = "Microscópio", Marca = "Zeiss", TipoAD = AnalogicoDigital.Digital },
                new ModeloEquipamento { Id = 0, Identificacao = "BAL-002", Descricao = "Balança", Marca = "Mettler", TipoAD = AnalogicoDigital.Analógico },
                new ModeloEquipamento { Id = 0, Identificacao = "PHM-003", Descricao = "pHmetro", Marca = "Hanna", TipoAD = AnalogicoDigital.Digital }
            };

            var equipamentosSalvos = new List<ModeloEquipamentoEntidade>
            {
                new ModeloEquipamentoEntidade { Id = 1, Identificacao = "MIC-001", Descricao = "Microscópio", Marca = "Zeiss", TipoAD = (int)AnalogicoDigital.Digital, IsDeleted = false },
                new ModeloEquipamentoEntidade { Id = 2, Identificacao = "BAL-002", Descricao = "Balança", Marca = "Mettler", TipoAD = (int)AnalogicoDigital.Analógico, IsDeleted = false },
                new ModeloEquipamentoEntidade { Id = 3, Identificacao = "PHM-003", Descricao = "pHmetro", Marca = "Hanna", TipoAD = (int)AnalogicoDigital.Digital, IsDeleted = false }
            };

            // Configurar mocks
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<ModeloEquipamentoEntidade>()))
                    .ReturnsAsync((ModeloEquipamentoEntidade e) => 
                    {
                        var index = equipamentos.FindIndex(x => x.Identificacao == e.Identificacao);
                        return equipamentosSalvos[index];
                    });

            _mockRepo.Setup(r => r.GetAllAsync())
                    .ReturnsAsync(equipamentosSalvos);

            // When (quando submete todos os formulários)
            var resultados = new List<ModeloEquipamento>();
            foreach (var equipamento in equipamentos)
            {
                var resultado = await _service.CreateEquipamentoAsync(equipamento);
                resultados.Add(resultado);
            }

            var todosEquipamentos = await _service.GetAllEquipamentosAsync();

            // Then (então todos são salvos e listados corretamente)
            Assert.Equal(3, resultados.Count);
            Assert.Equal(3, todosEquipamentos.Count());

            foreach (var resultado in resultados)
            {
                Assert.True(resultado.Id > 0);
                Assert.False(string.IsNullOrEmpty(resultado.Identificacao));
                Assert.False(string.IsNullOrEmpty(resultado.Descricao));
                Assert.False(string.IsNullOrEmpty(resultado.Marca));
            }
        }

        /// <summary>
        /// Cenário: Validação de campos obrigatórios
        /// Given: Admin deixa campos obrigatórios vazios
        /// When: Tenta submeter o formulário
        /// Then: Retorna erro específico para cada campo
        /// </summary>
        [Theory]
        [InlineData("", "Descrição válida", "Marca válida", "Identificação é obrigatória")]
        [InlineData("ID-001", "", "Marca válida", "Descrição é obrigatória")]
        [InlineData("ID-001", "Descrição válida", "", "Marca é obrigatória")]
        public async Task CadastroEquipamento_CamposObrigatoriosVazios_RetornaErroEspecifico(
            string identificacao, string descricao, string marca, string mensagemEsperada)
        {
            // Given (Dado que o admin deixa campos obrigatórios vazios)
            var dadosIncompletos = new ModeloEquipamento
            {
                Id = 0,
                Identificacao = identificacao,
                Descricao = descricao,
                TipoAD = AnalogicoDigital.Digital,
                Marca = marca,
                CreatedAt = DateTime.Now
            };

            // Configurar o mock para simular erro de validação
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<ModeloEquipamentoEntidade>()))
                    .ThrowsAsync(new ArgumentException(mensagemEsperada));

            // When & Then (quando submete, então retorna erro específico)
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateEquipamentoAsync(dadosIncompletos)
            );

            Assert.Contains(mensagemEsperada, exception.Message);
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