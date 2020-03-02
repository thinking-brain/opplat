using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class aperturaEdad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EdadDesde",
                table: "AperturaSocio");

            migrationBuilder.DropColumn(
                name: "EdadHasta",
                table: "AperturaSocio");
        }
    }
}
