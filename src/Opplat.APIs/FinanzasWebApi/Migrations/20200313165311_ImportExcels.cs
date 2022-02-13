using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace FinanzasWebApi.Migrations
{
    public partial class ImportExcels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CachesSubElementosPeriodos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    subElemento = table.Column<string>(nullable: true),
                    elemento = table.Column<string>(nullable: true),
                    Mes = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    FechaActualizado = table.Column<DateTime>(nullable: false),
                    Saldo = table.Column<decimal>(nullable: false),
                    Acumulado = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CachesSubElementosPeriodos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReporteEstadoFinancieros",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReporteEstadoFinancieros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ElementosDelReporteEFs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    ColeccionSubElementos = table.Column<bool>(nullable: false),
                    Celda = table.Column<string>(nullable: true),
                    Tipo = table.Column<string>(nullable: true),
                    Sumar = table.Column<string>(nullable: true),
                    Restar = table.Column<string>(nullable: true),
                    ReporteEstadoFinancieroDescripcion = table.Column<string>(nullable: true),
                    ReporteEstadoFinancieroId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementosDelReporteEFs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElementosDelReporteEFs_ReporteEstadoFinancieros_ReporteEsta~",
                        column: x => x.ReporteEstadoFinancieroId,
                        principalTable: "ReporteEstadoFinancieros",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubElementosEfReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    ElementosDelReporteEFDescripcion = table.Column<string>(nullable: true),
                    ElementosDelReporteEFId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubElementosEfReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubElementosEfReports_ElementosDelReporteEFs_ElementosDelRe~",
                        column: x => x.ElementosDelReporteEFId,
                        principalTable: "ElementosDelReporteEFs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElementosDelReporteEFs_ReporteEstadoFinancieroId",
                table: "ElementosDelReporteEFs",
                column: "ReporteEstadoFinancieroId");

            migrationBuilder.CreateIndex(
                name: "IX_SubElementosEfReports_ElementosDelReporteEFId",
                table: "SubElementosEfReports",
                column: "ElementosDelReporteEFId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CachesSubElementosPeriodos");

            migrationBuilder.DropTable(
                name: "SubElementosEfReports");

            migrationBuilder.DropTable(
                name: "ElementosDelReporteEFs");

            migrationBuilder.DropTable(
                name: "ReporteEstadoFinancieros");
        }
    }
}
