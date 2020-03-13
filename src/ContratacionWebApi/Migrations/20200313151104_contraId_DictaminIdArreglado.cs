using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class contraId_DictaminIdArreglado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "EconomicoId",
                table: "ContratoId_DictaminadorId");

            migrationBuilder.RenameColumn(
                name: "JuridicoId",
                table: "ContratoId_DictaminadorId",
                newName: "DictaminadorContratoId");

            migrationBuilder.RenameIndex(
                name: "IX_ContratoId_DictaminadorId_JuridicoId",
                table: "ContratoId_DictaminadorId",
                newName: "IX_ContratoId_DictaminadorId_DictaminadorContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContratoId_DictaminadorId_DictaminadoresContrato_Dictaminad~",
                table: "ContratoId_DictaminadorId",
                column: "DictaminadorContratoId",
                principalTable: "DictaminadoresContrato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContratoId_DictaminadorId_DictaminadoresContrato_Dictaminad~",
                table: "ContratoId_DictaminadorId");

            migrationBuilder.RenameColumn(
                name: "DictaminadorContratoId",
                table: "ContratoId_DictaminadorId",
                newName: "JuridicoId");

            migrationBuilder.RenameIndex(
                name: "IX_ContratoId_DictaminadorId_DictaminadorContratoId",
                table: "ContratoId_DictaminadorId",
                newName: "IX_ContratoId_DictaminadorId_JuridicoId");

            migrationBuilder.AddColumn<int>(
                name: "EconomicoId",
                table: "ContratoId_DictaminadorId",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ContratoId_DictaminadorId_EconomicoId",
                table: "ContratoId_DictaminadorId",
                column: "EconomicoId");

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
    }
}
