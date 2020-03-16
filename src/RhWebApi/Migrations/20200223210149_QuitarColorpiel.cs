using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class QuitarColorpiel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorDePiel",
                table: "caracteristicas_de_los_socios");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ColorDePiel",
                table: "caracteristicas_de_los_socios",
                nullable: false,
                defaultValue: 0);
        }
    }
}
