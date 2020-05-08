using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class espe_ContId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "EspecialistaExternoId_ContratoId");

            migrationBuilder.AddColumn<int>(
                name: "FormaDePagoId",
                table: "ContratoId_FormaPagoId",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ContratoId_FormaPagoId_ContratoId",
                table: "ContratoId_FormaPagoId",
                column: "ContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContratoId_FormaPagoId_Contratos_ContratoId",
                table: "ContratoId_FormaPagoId",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContratoId_FormaPagoId_Contratos_ContratoId",
                table: "ContratoId_FormaPagoId");

            migrationBuilder.DropIndex(
                name: "IX_ContratoId_FormaPagoId_ContratoId",
                table: "ContratoId_FormaPagoId");

            migrationBuilder.DropColumn(
                name: "FormaDePagoId",
                table: "ContratoId_FormaPagoId");

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "EspecialistaExternoId_ContratoId",
                nullable: false,
                defaultValue: 0);
        }
    }
}
