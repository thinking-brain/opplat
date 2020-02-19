using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class apertura : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cerrada",
                table: "AperturaSocio");

            migrationBuilder.AddColumn<int>(
                name: "EstadosApertura",
                table: "AperturaSocio",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadosApertura",
                table: "AperturaSocio");

            migrationBuilder.AddColumn<bool>(
                name: "Cerrada",
                table: "AperturaSocio",
                nullable: false,
                defaultValue: false);
        }
    }
}
