using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class adminContratoId1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_AdminContratos_AdminContratoId",
                table: "Contratos");

            migrationBuilder.DropIndex(
                name: "IX_Contratos_AdminContratoId",
                table: "Contratos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Contratos_AdminContratoId",
                table: "Contratos",
                column: "AdminContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_AdminContratos_AdminContratoId",
                table: "Contratos",
                column: "AdminContratoId",
                principalTable: "AdminContratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
