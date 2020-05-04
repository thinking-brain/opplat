using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class formadePago1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormaDePagoId",
                table: "ContratoId_FormaPagoId");

            migrationBuilder.AddColumn<int>(
                name: "FormaDePago",
                table: "ContratoId_FormaPagoId",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormaDePago",
                table: "ContratoId_FormaPagoId");

            migrationBuilder.AddColumn<int>(
                name: "FormaDePagoId",
                table: "ContratoId_FormaPagoId",
                nullable: false,
                defaultValue: 0);
        }
    }
}
