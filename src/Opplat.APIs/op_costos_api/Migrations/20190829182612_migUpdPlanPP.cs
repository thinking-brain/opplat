using Microsoft.EntityFrameworkCore.Migrations;

namespace op_costos_api.Migrations
{
    public partial class migUpdPlanPP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ElementoValor",
                table: "Costos_PlanPronosticoProductivo",
                newName: "CuentaMN");

            migrationBuilder.RenameColumn(
                name: "Elemento",
                table: "Costos_PlanPronosticoProductivo",
                newName: "CuentaCUC");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CuentaMN",
                table: "Costos_PlanPronosticoProductivo",
                newName: "ElementoValor");

            migrationBuilder.RenameColumn(
                name: "CuentaCUC",
                table: "Costos_PlanPronosticoProductivo",
                newName: "Elemento");
        }
    }
}
