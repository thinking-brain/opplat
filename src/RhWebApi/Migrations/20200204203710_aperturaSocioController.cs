using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RhWebApi.Migrations
{
    public partial class aperturaSocioController : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AperturaSocioId",
                table: "trabajadores",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "caracteristicas_de_los_socios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Direccion = table.Column<string>(nullable: true),
                    Sexo = table.Column<int>(nullable: true),
                    ColorDePiel = table.Column<int>(nullable: false),
                    MunicipioId = table.Column<int>(nullable: true),
                    Perfil_Ocupacional = table.Column<string>(nullable: true),
                    NivelDeEscolaridad = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_caracteristicas_de_los_socios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_caracteristicas_de_los_socios_municipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "municipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AperturaSocio",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Fecha = table.Column<DateTime>(nullable: false),
                    CantTrabajadores = table.Column<int>(nullable: false),
                    NumeroAcuerdo = table.Column<int>(nullable: false),
                    CaracteristicasSocioId = table.Column<int>(nullable: true),
                    Cerrada = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AperturaSocio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AperturaSocio_caracteristicas_de_los_socios_Caracteristicas~",
                        column: x => x.CaracteristicasSocioId,
                        principalTable: "caracteristicas_de_los_socios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trabajadores_AperturaSocioId",
                table: "trabajadores",
                column: "AperturaSocioId");

            migrationBuilder.CreateIndex(
                name: "IX_AperturaSocio_CaracteristicasSocioId",
                table: "AperturaSocio",
                column: "CaracteristicasSocioId");

            migrationBuilder.CreateIndex(
                name: "IX_caracteristicas_de_los_socios_MunicipioId",
                table: "caracteristicas_de_los_socios",
                column: "MunicipioId");

            migrationBuilder.AddForeignKey(
                name: "FK_trabajadores_AperturaSocio_AperturaSocioId",
                table: "trabajadores",
                column: "AperturaSocioId",
                principalTable: "AperturaSocio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trabajadores_AperturaSocio_AperturaSocioId",
                table: "trabajadores");

            migrationBuilder.DropTable(
                name: "AperturaSocio");

            migrationBuilder.DropTable(
                name: "caracteristicas_de_los_socios");

            migrationBuilder.DropIndex(
                name: "IX_trabajadores_AperturaSocioId",
                table: "trabajadores");

            migrationBuilder.DropColumn(
                name: "AperturaSocioId",
                table: "trabajadores");
        }
    }
}
