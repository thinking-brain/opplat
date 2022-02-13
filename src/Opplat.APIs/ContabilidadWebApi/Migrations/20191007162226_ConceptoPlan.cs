using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContabilidadWebApi.Migrations
{
    public partial class ConceptoPlan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConceptoPlan",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Concepto = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConceptoPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanesIngresosGastos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Year = table.Column<string>(nullable: true),
                    Titulo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanesIngresosGastos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConceptoCuentas",
                columns: table => new
                {
                    ConceptoPlanId = table.Column<int>(nullable: false),
                    CuentaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConceptoCuentas", x => new { x.ConceptoPlanId, x.CuentaId });
                    table.ForeignKey(
                        name: "FK_ConceptoCuentas_ConceptoPlan_ConceptoPlanId",
                        column: x => x.ConceptoPlanId,
                        principalTable: "ConceptoPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConceptoCuentas_contb_cuentas_CuentaId",
                        column: x => x.CuentaId,
                        principalTable: "contb_cuentas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallePlanGI",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlanId = table.Column<int>(nullable: false),
                    ConceptoId = table.Column<int>(nullable: false),
                    Enero = table.Column<decimal>(nullable: false),
                    Febrero = table.Column<decimal>(nullable: false),
                    Marzo = table.Column<decimal>(nullable: false),
                    Abril = table.Column<decimal>(nullable: false),
                    Mayo = table.Column<decimal>(nullable: false),
                    Junio = table.Column<decimal>(nullable: false),
                    Julio = table.Column<decimal>(nullable: false),
                    Agosto = table.Column<decimal>(nullable: false),
                    Septiembre = table.Column<decimal>(nullable: false),
                    Octubre = table.Column<decimal>(nullable: false),
                    Noviembre = table.Column<decimal>(nullable: false),
                    Diciembre = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallePlanGI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallePlanGI_ConceptoPlan_ConceptoId",
                        column: x => x.ConceptoId,
                        principalTable: "ConceptoPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallePlanGI_PlanesIngresosGastos_PlanId",
                        column: x => x.PlanId,
                        principalTable: "PlanesIngresosGastos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConceptoCuentas_CuentaId",
                table: "ConceptoCuentas",
                column: "CuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallePlanGI_ConceptoId",
                table: "DetallePlanGI",
                column: "ConceptoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallePlanGI_PlanId",
                table: "DetallePlanGI",
                column: "PlanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConceptoCuentas");

            migrationBuilder.DropTable(
                name: "DetallePlanGI");

            migrationBuilder.DropTable(
                name: "ConceptoPlan");

            migrationBuilder.DropTable(
                name: "PlanesIngresosGastos");
        }
    }
}
