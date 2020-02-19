using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class PerfilOcupacionalcomoClase2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trabajadores_PerfilOcupacional_PerfilOcupacionalId",
                table: "trabajadores");

            migrationBuilder.DropColumn(
                name: "Perfil_Ocupacional",
                table: "trabajadores");

            migrationBuilder.AlterColumn<int>(
                name: "PerfilOcupacionalId",
                table: "trabajadores",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_trabajadores_PerfilOcupacional_PerfilOcupacionalId",
                table: "trabajadores",
                column: "PerfilOcupacionalId",
                principalTable: "PerfilOcupacional",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trabajadores_PerfilOcupacional_PerfilOcupacionalId",
                table: "trabajadores");

            migrationBuilder.AlterColumn<int>(
                name: "PerfilOcupacionalId",
                table: "trabajadores",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "Perfil_Ocupacional",
                table: "trabajadores",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_trabajadores_PerfilOcupacional_PerfilOcupacionalId",
                table: "trabajadores",
                column: "PerfilOcupacionalId",
                principalTable: "PerfilOcupacional",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
