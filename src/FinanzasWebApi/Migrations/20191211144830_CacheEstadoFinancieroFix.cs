using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FinanzasWebApi.Migrations
{
    public partial class CacheEstadoFinancieroFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CachesEstadosFinancieros",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Concepto = table.Column<string>(nullable: true),
                    Grupo = table.Column<string>(nullable: true),
                    EFE = table.Column<string>(nullable: true),
                    Mes = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    FechaActualizado = table.Column<DateTime>(nullable: false),
                    Saldo = table.Column<decimal>(nullable: false),
                    PlanAnual = table.Column<decimal>(nullable: false),
                    Apertura = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CachesEstadosFinancieros", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CachesEstadosFinancieros");
        }
    }
}
