using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace op_finanzas_api.Migrations
{
    public partial class migOne : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Costos_Area",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Codigo = table.Column<string>(nullable: true),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Costos_Area", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Costos_GrupoSubelemento",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Costos_GrupoSubelemento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Costos_Plan",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
                    table.PrimaryKey("PK_Costos_Plan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Costos_CentroCostoArea",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AreaId = table.Column<int>(nullable: false),
                    CentroCostoId = table.Column<string>(nullable: true),
                    Detalles = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Costos_CentroCostoArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Costos_CentroCostoArea_Costos_Area_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Costos_Area",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Costos_GrupoSubElemento_SubElemento",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GrupoSubelementoId = table.Column<int>(nullable: false),
                    SubElementoGastoId = table.Column<string>(nullable: true),
                    Descripcion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Costos_GrupoSubElemento_SubElemento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Costos_GrupoSubElemento_SubElemento_Costos_GrupoSubelemento_GrupoSubelementoId",
                        column: x => x.GrupoSubelementoId,
                        principalTable: "Costos_GrupoSubelemento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Costos_CentroCostoArea_AreaId",
                table: "Costos_CentroCostoArea",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Costos_GrupoSubElemento_SubElemento_GrupoSubelementoId",
                table: "Costos_GrupoSubElemento_SubElemento",
                column: "GrupoSubelementoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Costos_CentroCostoArea");

            migrationBuilder.DropTable(
                name: "Costos_GrupoSubElemento_SubElemento");

            migrationBuilder.DropTable(
                name: "Costos_Plan");

            migrationBuilder.DropTable(
                name: "Costos_Area");

            migrationBuilder.DropTable(
                name: "Costos_GrupoSubelemento");
        }
    }
}
