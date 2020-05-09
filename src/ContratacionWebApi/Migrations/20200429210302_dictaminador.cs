using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class dictaminador : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContratoId_DictaminadorId_Contratos_ContratoId",
                table: "ContratoId_DictaminadorId");

            migrationBuilder.DropForeignKey(
                name: "FK_ContratoId_DictaminadorId_Dictaminadores_DictaminadorId",
                table: "ContratoId_DictaminadorId");

            migrationBuilder.DropIndex(
                name: "IX_ContratoId_DictaminadorId_ContratoId",
                table: "ContratoId_DictaminadorId");

            migrationBuilder.DropIndex(
                name: "IX_ContratoId_DictaminadorId_DictaminadorId",
                table: "ContratoId_DictaminadorId");

            migrationBuilder.DropColumn(
                name: "DictaminadorId",
                table: "ContratoId_DictaminadorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DictaminadorId",
                table: "ContratoId_DictaminadorId",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContratoId_DictaminadorId_ContratoId",
                table: "ContratoId_DictaminadorId",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratoId_DictaminadorId_DictaminadorId",
                table: "ContratoId_DictaminadorId",
                column: "DictaminadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContratoId_DictaminadorId_Contratos_ContratoId",
                table: "ContratoId_DictaminadorId",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContratoId_DictaminadorId_Dictaminadores_DictaminadorId",
                table: "ContratoId_DictaminadorId",
                column: "DictaminadorId",
                principalTable: "Dictaminadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
