using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class VigenciaContrato : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaDeVencimiento",
                table: "Contratos",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Vigencia",
                table: "Contratos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Vigencia",
                table: "Contratos");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaDeVencimiento",
                table: "Contratos",
                nullable: true,
                oldClrType: typeof(DateTime));
        }
    }
}
