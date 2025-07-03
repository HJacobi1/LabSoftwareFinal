using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AppDbContext : DbContext
    {        
        public DbSet<PessoaEntidade> Pessoas { get; set; }
        public DbSet<LaboratorioEntidade> Laboratorios { get; set; }
        public DbSet<EquipamentoEntidade> Equipamentos { get; set; }
        public DbSet<SolicitacaoEntidade> Solicitacoes { get; set; }
        public DbSet<UsuarioEntidade> Usuarios { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {            
            modelBuilder.Entity<PessoaEntidade>()
                .HasOne(p => p.Laboratorio)
                .WithMany(l => l.Responsaveis)
                .HasForeignKey(p => p.LaboratorioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}