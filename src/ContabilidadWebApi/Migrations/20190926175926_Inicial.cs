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
                name: "Area",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Codigo = table.Column<string>(nullable: true),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConceptoPlan",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Concepto = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConceptoPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrupoSubelemento",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Codigo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoSubelemento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plan",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CentroCostoAreaId = table.Column<int>(nullable: false),
                    Cuenta = table.Column<string>(nullable: true),
                    SubCuenta = table.Column<string>(nullable: true),
                    Analisis = table.Column<string>(nullable: true),
                    SubAnalisis = table.Column<string>(nullable: true),
                    CentroCosto = table.Column<string>(nullable: true),
                    Elemento = table.Column<string>(nullable: true),
                    SubElemento = table.Column<string>(nullable: true),
                    SubElementoTope = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true),
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
                    Diciembre = table.Column<decimal>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanGI",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Año = table.Column<string>(nullable: true),
                    Elemento = table.Column<string>(nullable: true),
                    ElementoValor = table.Column<string>(nullable: true),
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
                    Diciembre = table.Column<decimal>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanGI", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubMayor",
                columns: table => new
                {
                    Ano = table.Column<short>(nullable: false),
                    Mes = table.Column<byte>(nullable: false),
                    Cta = table.Column<string>(maxLength: 20, nullable: false),
                    Epigrafe = table.Column<string>(maxLength: 20, nullable: false),
                    Analisis = table.Column<string>(maxLength: 20, nullable: false),
                    SubAnalisis = table.Column<string>(maxLength: 20, nullable: false),
                    SubCta = table.Column<string>(maxLength: 20, nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Debe = table.Column<decimal>(type: "money", nullable: false),
                    Haber = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubMayor", x => new { x.Analisis, x.Ano, x.Cta, x.Debe, x.Epigrafe, x.Fecha, x.Haber, x.Mes, x.SubAnalisis, x.SubCta });
                });

            migrationBuilder.CreateTable(
                name: "SubMayorCuenta",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    IdCuenta = table.Column<int>(nullable: false),
                    IdPaseDeCuenta = table.Column<int>(nullable: true),
                    ClaveCuenta = table.Column<string>(nullable: true),
                    Mes = table.Column<int>(nullable: false),
                    Año = table.Column<int>(nullable: false),
                    Importe = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubMayorCuenta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CentroCostoArea",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AreaId = table.Column<int>(nullable: false),
                    CentroCostoId = table.Column<string>(nullable: true),
                    Detalles = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentroCostoArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CentroCostoArea_Area_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                });

            migrationBuilder.CreateTable(
                name: "PlanPronosticoProductivo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Año = table.Column<string>(nullable: true),
                    ConceptoPlanId = table.Column<int>(nullable: false),
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
                    Diciembre = table.Column<decimal>(nullable: false),
                    Fecha = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanPronosticoProductivo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanPronosticoProductivo_ConceptoPlan_ConceptoPlanId",
                        column: x => x.ConceptoPlanId,
                        principalTable: "ConceptoPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrupoSubElemento_SubElemento",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    GrupoSubelementoId = table.Column<int>(nullable: false),
                    SubElementoGastoId = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoSubElemento_SubElemento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrupoSubElemento_SubElemento_GrupoSubelemento_GrupoSubeleme~",
                        column: x => x.GrupoSubelementoId,
                        principalTable: "GrupoSubelemento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CentroCostoArea_AreaId",
                table: "CentroCostoArea",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoSubElemento_SubElemento_GrupoSubelementoId",
                table: "GrupoSubElemento_SubElemento",
                column: "GrupoSubelementoId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanPronosticoProductivo_ConceptoPlanId",
                table: "PlanPronosticoProductivo",
                column: "ConceptoPlanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CentroCostoArea");

            migrationBuilder.DropTable(
                name: "ConceptoCuentas");

            migrationBuilder.DropTable(
                name: "GrupoSubElemento_SubElemento");

            migrationBuilder.DropTable(
                name: "Plan");

            migrationBuilder.DropTable(
                name: "PlanGI");

            migrationBuilder.DropTable(
                name: "PlanPronosticoProductivo");

            migrationBuilder.DropTable(
                name: "SubMayor");

            migrationBuilder.DropTable(
                name: "SubMayorCuenta");

            migrationBuilder.DropTable(
                name: "Area");

            migrationBuilder.DropTable(
                name: "GrupoSubelemento");

            migrationBuilder.DropTable(
                name: "ConceptoPlan");
        }
    }
}
