using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class Modelos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Pessoas",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LaboratorioId",
                table: "Pessoas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Laboratorios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<int>(type: "integer", nullable: false),
                    Endereco = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laboratorios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Solicitacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TipoMC = table.Column<int>(type: "integer", nullable: false),
                    IdEquipamento = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solicitacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Equipamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NroPatrimonio = table.Column<string>(type: "text", nullable: false),
                    Identificacao = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    TipoAD = table.Column<int>(type: "integer", nullable: false),
                    Marca = table.Column<string>(type: "text", nullable: false),
                    CertificadoCalibracao = table.Column<string>(type: "text", nullable: false),
                    NroSerie = table.Column<string>(type: "text", nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CodLaboratorio = table.Column<int>(type: "integer", nullable: false),
                    LaboratorioId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipamentos_Laboratorios_LaboratorioId",
                        column: x => x.LaboratorioId,
                        principalTable: "Laboratorios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Metrologia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TipoMC = table.Column<int>(type: "integer", nullable: false),
                    CACalibracao = table.Column<string>(type: "text", nullable: false),
                    CAVerificacao = table.Column<string>(type: "text", nullable: false),
                    CapacidadeMedicao = table.Column<string>(type: "text", nullable: false),
                    Periodicidade = table.Column<string>(type: "text", nullable: false),
                    DivisaoEscala = table.Column<string>(type: "text", nullable: false),
                    EquipamentoId = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metrologia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Metrologia_Equipamentos_EquipamentoId",
                        column: x => x.EquipamentoId,
                        principalTable: "Equipamentos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_LaboratorioId",
                table: "Pessoas",
                column: "LaboratorioId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipamentos_LaboratorioId",
                table: "Equipamentos",
                column: "LaboratorioId");

            migrationBuilder.CreateIndex(
                name: "IX_Metrologia_EquipamentoId",
                table: "Metrologia",
                column: "EquipamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_Laboratorios_LaboratorioId",
                table: "Pessoas",
                column: "LaboratorioId",
                principalTable: "Laboratorios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_Laboratorios_LaboratorioId",
                table: "Pessoas");

            migrationBuilder.DropTable(
                name: "Metrologia");

            migrationBuilder.DropTable(
                name: "Solicitacoes");

            migrationBuilder.DropTable(
                name: "Equipamentos");

            migrationBuilder.DropTable(
                name: "Laboratorios");

            migrationBuilder.DropIndex(
                name: "IX_Pessoas_LaboratorioId",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "LaboratorioId",
                table: "Pessoas");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Pessoas",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }
    }
}
