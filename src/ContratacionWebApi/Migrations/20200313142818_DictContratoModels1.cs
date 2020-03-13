using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class DictContratoModels1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DictaminadoresContrato_Contratos_ContratoId",
                table: "DictaminadoresContrato");

            migrationBuilder.DropForeignKey(
                name: "FK_EspecialistasExternos_DictaminadoresContrato_DictContratoId",
                table: "EspecialistasExternos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DictaminadoresContrato",
                table: "DictaminadoresContrato");

            migrationBuilder.RenameTable(
                name: "DictaminadoresContrato",
                newName: "DictId_ContratoId");

            migrationBuilder.RenameIndex(
                name: "IX_DictaminadoresContrato_ContratoId",
                table: "DictId_ContratoId",
                newName: "IX_DictId_ContratoId_ContratoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DictId_ContratoId",
                table: "DictId_ContratoId",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DictId_ContratoId_Contratos_ContratoId",
                table: "DictId_ContratoId",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EspecialistasExternos_DictId_ContratoId_DictContratoId",
                table: "EspecialistasExternos",
                column: "DictContratoId",
                principalTable: "DictId_ContratoId",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DictId_ContratoId_Contratos_ContratoId",
                table: "DictId_ContratoId");

            migrationBuilder.DropForeignKey(
                name: "FK_EspecialistasExternos_DictId_ContratoId_DictContratoId",
                table: "EspecialistasExternos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DictId_ContratoId",
                table: "DictId_ContratoId");

            migrationBuilder.RenameTable(
                name: "DictId_ContratoId",
                newName: "DictaminadoresContrato");

            migrationBuilder.RenameIndex(
                name: "IX_DictId_ContratoId_ContratoId",
                table: "DictaminadoresContrato",
                newName: "IX_DictaminadoresContrato_ContratoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DictaminadoresContrato",
                table: "DictaminadoresContrato",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DictaminadoresContrato_Contratos_ContratoId",
                table: "DictaminadoresContrato",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EspecialistasExternos_DictaminadoresContrato_DictContratoId",
                table: "EspecialistasExternos",
                column: "DictContratoId",
                principalTable: "DictaminadoresContrato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
