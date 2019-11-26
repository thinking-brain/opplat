using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InventarioWebApi.Migrations
{
    public partial class FechaEnMovimiento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "MovimientosDeProductos",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "MovimientosDeProductos");
        }
    }
}
