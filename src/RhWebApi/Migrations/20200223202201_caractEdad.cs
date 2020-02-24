using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class caractEdad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EdadDesde",
                table: "AperturaSocio");

            migrationBuilder.DropColumn(
                name: "EdadHasta",
                table: "AperturaSocio");

            migrationBuilder.AddColumn<int>(
                name: "EdadDesde",
                table: "caracteristicas_de_los_socios",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EdadHasta",
                table: "caracteristicas_de_los_socios",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EdadDesde",
                table: "caracteristicas_de_los_socios");

            migrationBuilder.DropColumn(
                name: "EdadHasta",
                table: "caracteristicas_de_los_socios");

            migrationBuilder.AddColumn<int>(
                name: "EdadDesde",
                table: "AperturaSocio",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EdadHasta",
                table: "AperturaSocio",
                nullable: false,
                defaultValue: 0);
        }
    }
}
