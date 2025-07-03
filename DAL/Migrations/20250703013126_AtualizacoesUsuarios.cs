using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacoesUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipamentos_Laboratorios_LaboratorioId",
                table: "Equipamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipamentos_Metrologia_MetricaId",
                table: "Equipamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipamentos_ModeloEquipamento_ModeloId",
                table: "Equipamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_Laboratorios_LaboratorioId",
                table: "Pessoas");

            migrationBuilder.DropTable(
                name: "Metrologia");

            migrationBuilder.DropTable(
                name: "ModeloEquipamento");

            migrationBuilder.RenameColumn(
                name: "LaboratorioId",
                table: "Pessoas",
                newName: "LaboratorioEntidadeId");

            migrationBuilder.RenameIndex(
                name: "IX_Pessoas_LaboratorioId",
                table: "Pessoas",
                newName: "IX_Pessoas_LaboratorioEntidadeId");

            migrationBuilder.RenameColumn(
                name: "LaboratorioId",
                table: "Equipamentos",
                newName: "LaboratorioEntidadeId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipamentos_LaboratorioId",
                table: "Equipamentos",
                newName: "IX_Equipamentos_LaboratorioEntidadeId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Usuarios",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MetrologiaEntidade",
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
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetrologiaEntidade", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModeloEquipamentoEntidade",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Identificacao = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    TipoAD = table.Column<int>(type: "integer", nullable: false),
                    Marca = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeloEquipamentoEntidade", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Equipamentos_Laboratorios_LaboratorioEntidadeId",
                table: "Equipamentos",
                column: "LaboratorioEntidadeId",
                principalTable: "Laboratorios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipamentos_MetrologiaEntidade_MetricaId",
                table: "Equipamentos",
                column: "MetricaId",
                principalTable: "MetrologiaEntidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipamentos_ModeloEquipamentoEntidade_ModeloId",
                table: "Equipamentos",
                column: "ModeloId",
                principalTable: "ModeloEquipamentoEntidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_Laboratorios_LaboratorioEntidadeId",
                table: "Pessoas",
                column: "LaboratorioEntidadeId",
                principalTable: "Laboratorios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipamentos_Laboratorios_LaboratorioEntidadeId",
                table: "Equipamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipamentos_MetrologiaEntidade_MetricaId",
                table: "Equipamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipamentos_ModeloEquipamentoEntidade_ModeloId",
                table: "Equipamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_Laboratorios_LaboratorioEntidadeId",
                table: "Pessoas");

            migrationBuilder.DropTable(
                name: "MetrologiaEntidade");

            migrationBuilder.DropTable(
                name: "ModeloEquipamentoEntidade");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "LaboratorioEntidadeId",
                table: "Pessoas",
                newName: "LaboratorioId");

            migrationBuilder.RenameIndex(
                name: "IX_Pessoas_LaboratorioEntidadeId",
                table: "Pessoas",
                newName: "IX_Pessoas_LaboratorioId");

            migrationBuilder.RenameColumn(
                name: "LaboratorioEntidadeId",
                table: "Equipamentos",
                newName: "LaboratorioId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipamentos_LaboratorioEntidadeId",
                table: "Equipamentos",
                newName: "IX_Equipamentos_LaboratorioId");

            migrationBuilder.CreateTable(
                name: "Metrologia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CACalibracao = table.Column<string>(type: "text", nullable: false),
                    CAVerificacao = table.Column<string>(type: "text", nullable: false),
                    CapacidadeMedicao = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DivisaoEscala = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Periodicidade = table.Column<string>(type: "text", nullable: false),
                    TipoMC = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Metrologia", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModeloEquipamento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    Identificacao = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Marca = table.Column<string>(type: "text", nullable: false),
                    TipoAD = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeloEquipamento", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Equipamentos_Laboratorios_LaboratorioId",
                table: "Equipamentos",
                column: "LaboratorioId",
                principalTable: "Laboratorios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipamentos_Metrologia_MetricaId",
                table: "Equipamentos",
                column: "MetricaId",
                principalTable: "Metrologia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Equipamentos_ModeloEquipamento_ModeloId",
                table: "Equipamentos",
                column: "ModeloId",
                principalTable: "ModeloEquipamento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_Laboratorios_LaboratorioId",
                table: "Pessoas",
                column: "LaboratorioId",
                principalTable: "Laboratorios",
                principalColumn: "Id");
        }
    }
}
