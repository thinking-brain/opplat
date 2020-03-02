using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class caractRangoEdad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Perfil_Ocupacional",
                table: "caracteristicas_de_los_socios");

            migrationBuilder.AddColumn<int>(
                name: "PerfilOcupacionalId",
                table: "caracteristicas_de_los_socios",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_caracteristicas_de_los_socios_PerfilOcupacionalId",
                table: "caracteristicas_de_los_socios",
                column: "PerfilOcupacionalId");

            migrationBuilder.AddForeignKey(
                name: "FK_caracteristicas_de_los_socios_PerfilOcupacional_PerfilOcupa~",
                table: "caracteristicas_de_los_socios",
                column: "PerfilOcupacionalId",
                principalTable: "PerfilOcupacional",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_caracteristicas_de_los_socios_PerfilOcupacional_PerfilOcupa~",
                table: "caracteristicas_de_los_socios");

            migrationBuilder.DropIndex(
                name: "IX_caracteristicas_de_los_socios_PerfilOcupacionalId",
                table: "caracteristicas_de_los_socios");

            migrationBuilder.DropColumn(
                name: "PerfilOcupacionalId",
                table: "caracteristicas_de_los_socios");

            migrationBuilder.AddColumn<string>(
                name: "Perfil_Ocupacional",
                table: "caracteristicas_de_los_socios",
                nullable: true);
        }
    }
}
