using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class VencimientoOferta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaDeVenOferta",
                table: "Contratos",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaDeVenOferta",
                table: "Contratos");
        }
    }
}
