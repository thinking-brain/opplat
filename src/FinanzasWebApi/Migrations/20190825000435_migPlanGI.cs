using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanzasWebApi.Migrations
{
    public partial class migPlanGI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Costos_PlanGI",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
                    table.PrimaryKey("PK_Costos_PlanGI", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Costos_PlanGI");
        }
    }
}
