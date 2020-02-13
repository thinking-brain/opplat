using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class aperturaSocioListTrabstring : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ListaTrabajadoresId",
                table: "AperturaSocio",
                nullable: true,
                oldClrType: typeof(List<int>),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<int>>(
                name: "ListaTrabajadoresId",
                table: "AperturaSocio",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
