using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class adminContrato : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_AdminContratos_TrabajadorId",
                table: "Contratos");

            migrationBuilder.RenameColumn(
                name: "TrabajadorId",
                table: "Contratos",
                newName: "AdminContratoId");

            migrationBuilder.RenameIndex(
                name: "IX_Contratos_TrabajadorId",
                table: "Contratos",
                newName: "IX_Contratos_AdminContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_AdminContratos_AdminContratoId",
                table: "Contratos",
                column: "AdminContratoId",
                principalTable: "AdminContratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_AdminContratos_AdminContratoId",
                table: "Contratos");

            migrationBuilder.RenameColumn(
                name: "AdminContratoId",
                table: "Contratos",
                newName: "TrabajadorId");

            migrationBuilder.RenameIndex(
                name: "IX_Contratos_AdminContratoId",
                table: "Contratos",
                newName: "IX_Contratos_TrabajadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_AdminContratos_TrabajadorId",
                table: "Contratos",
                column: "TrabajadorId",
                principalTable: "AdminContratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
