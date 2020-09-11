using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class ContratoId_Dictamen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContratoId",
                table: "Dictamen",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "username",
                table: "Dictamen",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dictamen_ContratoId",
                table: "Dictamen",
                column: "ContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dictamen_Contratos_ContratoId",
                table: "Dictamen",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dictamen_Contratos_ContratoId",
                table: "Dictamen");

            migrationBuilder.DropIndex(
                name: "IX_Dictamen_ContratoId",
                table: "Dictamen");

            migrationBuilder.DropColumn(
                name: "ContratoId",
                table: "Dictamen");

            migrationBuilder.DropColumn(
                name: "username",
                table: "Dictamen");
        }
    }
}
