using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class DictaminadoresContratoModels1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DictaminadoresContrato_Contratos_ContratoId",
                table: "DictaminadoresContrato");

            migrationBuilder.DropIndex(
                name: "IX_DictaminadoresContrato_ContratoId",
                table: "DictaminadoresContrato");

            migrationBuilder.DropColumn(
                name: "ContratoId",
                table: "DictaminadoresContrato");

            migrationBuilder.DropColumn(
                name: "EconomicoId",
                table: "DictaminadoresContrato");

            migrationBuilder.RenameColumn(
                name: "JuridicoId",
                table: "DictaminadoresContrato",
                newName: "TrabajadorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TrabajadorId",
                table: "DictaminadoresContrato",
                newName: "JuridicoId");

            migrationBuilder.AddColumn<int>(
                name: "ContratoId",
                table: "DictaminadoresContrato",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EconomicoId",
                table: "DictaminadoresContrato",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DictaminadoresContrato_ContratoId",
                table: "DictaminadoresContrato",
                column: "ContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_DictaminadoresContrato_Contratos_ContratoId",
                table: "DictaminadoresContrato",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
