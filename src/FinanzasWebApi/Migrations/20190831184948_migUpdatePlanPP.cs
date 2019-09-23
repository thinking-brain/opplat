using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanzasWebApi.Migrations
{
    public partial class migUpdatePlanPP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CuentaCUC",
                table: "Costos_PlanPronosticoProductivo");

            migrationBuilder.DropColumn(
                name: "CuentaMN",
                table: "Costos_PlanPronosticoProductivo");

            migrationBuilder.AddColumn<int>(
                name: "ConceptoPlanId",
                table: "Costos_PlanPronosticoProductivo",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ConceptoPlan",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Concepto = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConceptoPlan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConceptoCuentas",
                columns: table => new
                {
                    ConceptoId = table.Column<int>(nullable: false),
                    CuentaId = table.Column<int>(nullable: false),
                    ConceptoPlanId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConceptoCuentas", x => new { x.ConceptoId, x.CuentaId });
                    table.ForeignKey(
                        name: "FK_ConceptoCuentas_ConceptoPlan_ConceptoPlanId",
                        column: x => x.ConceptoPlanId,
                        principalTable: "ConceptoPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Costos_PlanPronosticoProductivo_ConceptoPlanId",
                table: "Costos_PlanPronosticoProductivo",
                column: "ConceptoPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ConceptoCuentas_ConceptoPlanId",
                table: "ConceptoCuentas",
                column: "ConceptoPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Costos_PlanPronosticoProductivo_ConceptoPlan_ConceptoPlanId",
                table: "Costos_PlanPronosticoProductivo",
                column: "ConceptoPlanId",
                principalTable: "ConceptoPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Costos_PlanPronosticoProductivo_ConceptoPlan_ConceptoPlanId",
                table: "Costos_PlanPronosticoProductivo");

            migrationBuilder.DropTable(
                name: "ConceptoCuentas");

            migrationBuilder.DropTable(
                name: "ConceptoPlan");

            migrationBuilder.DropIndex(
                name: "IX_Costos_PlanPronosticoProductivo_ConceptoPlanId",
                table: "Costos_PlanPronosticoProductivo");

            migrationBuilder.DropColumn(
                name: "ConceptoPlanId",
                table: "Costos_PlanPronosticoProductivo");

            migrationBuilder.AddColumn<string>(
                name: "CuentaCUC",
                table: "Costos_PlanPronosticoProductivo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CuentaMN",
                table: "Costos_PlanPronosticoProductivo",
                nullable: true);
        }
    }
}
