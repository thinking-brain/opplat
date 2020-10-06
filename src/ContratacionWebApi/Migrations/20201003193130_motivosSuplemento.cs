using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class motivosSuplemento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MotivoSuplemento",
                table: "Contratos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_ContratoId",
                table: "Contratos",
                column: "ContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contratos_Contratos_ContratoId",
                table: "Contratos",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contratos_Contratos_ContratoId",
                table: "Contratos");

            migrationBuilder.DropIndex(
                name: "IX_Contratos_ContratoId",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "MotivoSuplemento",
                table: "Contratos");
        }
    }
}
