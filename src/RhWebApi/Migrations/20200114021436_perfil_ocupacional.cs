using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class perfil_ocupacional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Referencia",
                table: "Bolsa");

            migrationBuilder.AddColumn<string>(
                name: "Perfil_Ocupacional",
                table: "trabajadores",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Perfil_Ocupacional",
                table: "trabajadores");

            migrationBuilder.AddColumn<bool>(
                name: "Referencia",
                table: "Bolsa",
                nullable: false,
                defaultValue: false);
        }
    }
}
