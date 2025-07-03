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
    public class LaboratorioServiceTests
    {
        private readonly Mock<IGenericRepository<DAL.Models.Laboratorio>> _mockRepo;
        private readonly LaboratorioService _service;

        public LaboratorioServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<DAL.Models.Laboratorio>>();

            var config = new ConfigurationBuilder().Build();

            var repositoryMock = new Mock<Repository>(config);
            repositoryMock.Setup(r => r.GetRepository<DAL.Models.Laboratorio>())
                          .Returns(_mockRepo.Object);

            _service = new LaboratorioServiceFake(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllLaboratoriosAsync_ReturnsOrderedList()
        {
            var labs = new List<DAL.Models.Laboratorio>
            {
                new DAL.Models.Laboratorio { Id = 2, Nome = "Lab B", Endereco = "Rua B" },
                new DAL.Models.Laboratorio { Id = 1, Nome = "Lab A", Endereco = "Rua A" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(labs);

            var result = await _service.GetAllLaboratoriosAsync();

            Assert.Equal(2, result.Count());
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public async Task GetLaboratorioByIdAsync_ReturnsCorrectLaboratorio()
        {
            var lab = new DAL.Models.Laboratorio { Id = 1, Nome = "Lab A", Endereco = "Rua A" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(lab);

            var result = await _service.GetLaboratorioByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Lab A", result.Nome);
        }

        [Fact]
        public async Task CreateLaboratorioAsync_CreatesAndReturnsLaboratorio()
        {
            var labToCreate = new Laboratorio { Id = 0, Nome = "Lab Novo", Endereco = "Endereço" };
            var createdLab = new DAL.Models.Laboratorio { Id = 1, Nome = "Lab Novo", Endereco = "Endereço" };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<DAL.Models.Laboratorio>()))
                     .ReturnsAsync(createdLab);

            var result = await _service.CreateLaboratorioAsync(labToCreate);

            Assert.Equal(1, result.Id);
            Assert.Equal("Lab Novo", result.Nome);
        }

        [Fact]
        public async Task UpdateLaboratorioAsync_UpdatesAndReturnsLaboratorio()
        {
            var existingLab = new DAL.Models.Laboratorio { Id = 1, Nome = "Antigo", Endereco = "Antiga" };
            var updatedLab = new DAL.Models.Laboratorio { Id = 1, Nome = "Novo", Endereco = "Nova" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingLab);
            _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<DAL.Models.Laboratorio>())).ReturnsAsync(updatedLab);

            var updatedData = new Laboratorio { Id = 1, Nome = "Novo", Endereco = "Nova" };

            var result = await _service.UpdateLaboratorioAsync(1, updatedData);

            Assert.Equal("Novo", result.Nome);
            Assert.Equal("Nova", result.Endereco);
        }

        [Fact]
        public async Task DeleteLaboratorioAsync_CallsRepositoryDelete()
        {
            _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            await _service.DeleteLaboratorioAsync(1);

            _mockRepo.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        private class LaboratorioServiceFake : LaboratorioService
        {
            public LaboratorioServiceFake(IGenericRepository<DAL.Models.Laboratorio> mockRepo)
                : base(new ConfigurationBuilder().Build())
            {
                typeof(LaboratorioService)
                    .GetField("_laboratorioRepository", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(this, mockRepo);
            }
        }
    }
}
