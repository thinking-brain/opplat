using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ContabilidadWebApi.Migrations
{
    public partial class FechaFinNoNullPeriodo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaFin",
                table: "PeriodoContable",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaFin",
                table: "PeriodoContable",
                nullable: true,
                oldClrType: typeof(DateTime));
        }
    }
}
