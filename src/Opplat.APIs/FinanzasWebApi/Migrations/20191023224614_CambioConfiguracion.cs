using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanzasWebApi.Migrations
{
    public partial class CambioConfiguracion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cargo",
                table: "ConfiguracionesFirmas");

            migrationBuilder.RenameColumn(
                name: "Titulo",
                table: "ConfiguracionesFirmas",
                newName: "Valor");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Valor",
                table: "ConfiguracionesFirmas",
                newName: "Titulo");

            migrationBuilder.AddColumn<string>(
                name: "Cargo",
                table: "ConfiguracionesFirmas",
                nullable: true);
        }
    }
}
