using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class contraId_DictaminId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ContratoId_DictaminadorId_EconomicoId",
                table: "ContratoId_DictaminadorId",
                column: "EconomicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratoId_DictaminadorId_JuridicoId",
                table: "ContratoId_DictaminadorId",
                column: "JuridicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContratoId_DictaminadorId_DictaminadoresContrato_EconomicoId",
                table: "ContratoId_DictaminadorId",
                column: "EconomicoId",
                principalTable: "DictaminadoresContrato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContratoId_DictaminadorId_DictaminadoresContrato_JuridicoId",
                table: "ContratoId_DictaminadorId",
                column: "JuridicoId",
                principalTable: "DictaminadoresContrato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContratoId_DictaminadorId_DictaminadoresContrato_EconomicoId",
                table: "ContratoId_DictaminadorId");

            migrationBuilder.DropForeignKey(
                name: "FK_ContratoId_DictaminadorId_DictaminadoresContrato_JuridicoId",
                table: "ContratoId_DictaminadorId");

            migrationBuilder.DropIndex(
                name: "IX_ContratoId_DictaminadorId_EconomicoId",
                table: "ContratoId_DictaminadorId");

            migrationBuilder.DropIndex(
                name: "IX_ContratoId_DictaminadorId_JuridicoId",
                table: "ContratoId_DictaminadorId");
        }
    }
}
