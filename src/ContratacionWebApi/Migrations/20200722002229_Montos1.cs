using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class Montos1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Monto_Contratos_ContratoId",
                table: "Monto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Monto",
                table: "Monto");

            migrationBuilder.RenameTable(
                name: "Monto",
                newName: "montos");

            migrationBuilder.RenameIndex(
                name: "IX_Monto_ContratoId",
                table: "montos",
                newName: "IX_montos_ContratoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_montos",
                table: "montos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_montos_Contratos_ContratoId",
                table: "montos",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_montos_Contratos_ContratoId",
                table: "montos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_montos",
                table: "montos");

            migrationBuilder.RenameTable(
                name: "montos",
                newName: "Monto");

            migrationBuilder.RenameIndex(
                name: "IX_montos_ContratoId",
                table: "Monto",
                newName: "IX_Monto_ContratoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Monto",
                table: "Monto",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Monto_Contratos_ContratoId",
                table: "Monto",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
