using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class LaboratorioPessoaRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_Laboratorios_LaboratorioEntidadeId",
                table: "Pessoas");

            migrationBuilder.DropIndex(
                name: "IX_Pessoas_LaboratorioEntidadeId",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "LaboratorioEntidadeId",
                table: "Pessoas");

            migrationBuilder.AddColumn<int>(
                name: "LaboratorioId",
                table: "Pessoas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_LaboratorioId",
                table: "Pessoas",
                column: "LaboratorioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_Laboratorios_LaboratorioId",
                table: "Pessoas",
                column: "LaboratorioId",
                principalTable: "Laboratorios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pessoas_Laboratorios_LaboratorioId",
                table: "Pessoas");

            migrationBuilder.DropIndex(
                name: "IX_Pessoas_LaboratorioId",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "LaboratorioId",
                table: "Pessoas");

            migrationBuilder.AddColumn<int>(
                name: "LaboratorioEntidadeId",
                table: "Pessoas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_LaboratorioEntidadeId",
                table: "Pessoas",
                column: "LaboratorioEntidadeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pessoas_Laboratorios_LaboratorioEntidadeId",
                table: "Pessoas",
                column: "LaboratorioEntidadeId",
                principalTable: "Laboratorios",
                principalColumn: "Id");
        }
    }
}
