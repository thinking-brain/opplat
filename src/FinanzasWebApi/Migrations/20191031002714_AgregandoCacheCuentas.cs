using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FinanzasWebApi.Migrations
{
    public partial class AgregandoCacheCuentas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CachesCuentasEnPeriodos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cuenta = table.Column<string>(nullable: true),
                    Mes = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    FechaActualizado = table.Column<DateTime>(nullable: false),
                    Saldo = table.Column<decimal>(nullable: false),
                    Acumulado = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CachesCuentasEnPeriodos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CachesCuentasEnPeriodos");
        }
    }
}
