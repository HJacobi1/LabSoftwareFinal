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
    public class SolicitacaoServiceTests
    {
        private readonly Mock<IGenericRepository<SolicitacaoEntidade>> _mockRepo;
        private readonly SolicitacaoService _service;

        public SolicitacaoServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<SolicitacaoEntidade>>();

            var config = new ConfigurationBuilder().Build();

            var repositoryMock = new Mock<Repository>(config);
            repositoryMock.Setup(r => r.GetRepository<SolicitacaoEntidade>())
                          .Returns(_mockRepo.Object);

            _service = new SolicitacaoServiceFake(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllSolicitacoesAsync_ReturnsAll()
        {
            var solicitacoes = new List<SolicitacaoEntidade>
            {
                new SolicitacaoEntidade { Id = 1, Descricao = "Solicitação 1" },
                new SolicitacaoEntidade { Id = 2, Descricao = "Solicitação 2" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(solicitacoes);

            var result = await _service.GetAllSolicitacoesAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetSolicitacaoByIdAsync_ReturnsCorrect()
        {
            var solicitacao = new SolicitacaoEntidade { Id = 1, Descricao = "Solicitação" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(solicitacao);

            var result = await _service.GetSolicitacaoByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Solicitação", result.Descricao);
        }

        [Fact]
        public async Task CreateSolicitacaoAsync_ReturnsCreated()
        {
            var input = new Solicitacao
            {
                Id = 0,
                Descricao = "Nova",
                Data = DateTime.Today,
                TipoMC = ManutencaoCalibracao.Manutencao
            };

            var created = new SolicitacaoEntidade
            {
                Id = 1,
                Descricao = "Nova",
                Data = DateTime.Today,
                TipoMC = (int)ManutencaoCalibracao.Manutencao
            };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<SolicitacaoEntidade>())).ReturnsAsync(created);

            var result = await _service.CreateSolicitacaoAsync(input);

            Assert.Equal(1, result.Id);
            Assert.Equal("Nova", result.Descricao);
        }

        [Fact]
        public async Task UpdateSolicitacaoAsync_UpdatesAndReturns()
        {
            var existing = new SolicitacaoEntidade
            {
                Id = 1,
                Descricao = "Antiga",
                Data = DateTime.Today.AddDays(-1),
                TipoMC = (int)ManutencaoCalibracao.Calibracao
            };

            var updated = new SolicitacaoEntidade
            {
                Id = 1,
                Descricao = "Atualizada",
                Data = DateTime.Today,
                TipoMC = (int)ManutencaoCalibracao.Manutencao
            };

            var input = new Solicitacao
            {
                Id = 1,
                Descricao = "Atualizada",
                Data = DateTime.Today,
                TipoMC = ManutencaoCalibracao.Manutencao
            };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<SolicitacaoEntidade>())).ReturnsAsync(updated);

            var result = await _service.UpdateSolicitacaoAsync(1, input);

            Assert.Equal("Atualizada", result.Descricao);
            Assert.Equal(DateTime.Today, result.Data);
        }

        [Fact]
        public async Task DeleteSolicitacaoAsync_CallsDelete()
        {
            _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            await _service.DeleteSolicitacaoAsync(1);

            _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        private class SolicitacaoServiceFake : SolicitacaoService
        {
            public SolicitacaoServiceFake(IGenericRepository<SolicitacaoEntidade> mockRepo)
                : base(new ConfigurationBuilder().Build())
            {
                typeof(SolicitacaoService)
                    .GetField("_solicitacaoRepository", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(this, mockRepo);
            }
        }
    }
}
