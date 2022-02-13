using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContabilidadWebApi.Migrations
{
    public partial class Inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contb_cuentas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumeroParcial = table.Column<string>(nullable: false),
                    Nombre = table.Column<string>(nullable: false),
                    CuentaSuperiorId = table.Column<int>(nullable: true),
                    Naturaleza = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contb_cuentas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contb_cuentas_contb_cuentas_CuentaSuperiorId",
                        column: x => x.CuentaSuperiorId,
                        principalTable: "contb_cuentas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contb_dia_contable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Abierto = table.Column<bool>(nullable: false),
                    HoraEnQueCerro = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contb_dia_contable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "contb_asientos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DiaContableId = table.Column<int>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    UsuarioId = table.Column<string>(nullable: false),
                    Detalle = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contb_asientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contb_asientos_contb_dia_contable_DiaContableId",
                        column: x => x.DiaContableId,
                        principalTable: "contb_dia_contable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contb_movimientos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AsientoId = table.Column<int>(nullable: false),
                    CuentaId = table.Column<int>(nullable: false),
                    Importe = table.Column<decimal>(nullable: false),
                    TipoDeOperacion = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contb_movimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_contb_movimientos_contb_asientos_AsientoId",
                        column: x => x.AsientoId,
                        principalTable: "contb_asientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_contb_movimientos_contb_cuentas_CuentaId",
                        column: x => x.CuentaId,
                        principalTable: "contb_cuentas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contb_asientos_DiaContableId",
                table: "contb_asientos",
                column: "DiaContableId");

            migrationBuilder.CreateIndex(
                name: "IX_contb_cuentas_CuentaSuperiorId",
                table: "contb_cuentas",
                column: "CuentaSuperiorId");

            migrationBuilder.CreateIndex(
                name: "IX_contb_movimientos_AsientoId",
                table: "contb_movimientos",
                column: "AsientoId");

            migrationBuilder.CreateIndex(
                name: "IX_contb_movimientos_CuentaId",
                table: "contb_movimientos",
                column: "CuentaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contb_movimientos");

            migrationBuilder.DropTable(
                name: "contb_asientos");

            migrationBuilder.DropTable(
                name: "contb_cuentas");

            migrationBuilder.DropTable(
                name: "contb_dia_contable");
        }
    }
}
