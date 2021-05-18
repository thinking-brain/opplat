using Microsoft.EntityFrameworkCore.Migrations;

namespace TallerWebApi.Migrations
{
    public partial class Modelo_por_Marca : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipos_Marcas_ModeloId",
                table: "Equipos");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipos_Modelos_ModeloId",
                table: "Equipos",
                column: "ModeloId",
                principalTable: "Modelos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipos_Modelos_ModeloId",
                table: "Equipos");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipos_Marcas_ModeloId",
                table: "Equipos",
                column: "ModeloId",
                principalTable: "Marcas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
