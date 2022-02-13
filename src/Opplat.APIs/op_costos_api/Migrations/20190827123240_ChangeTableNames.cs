using Microsoft.EntityFrameworkCore.Migrations;

namespace op_costos_api.Migrations
{
    public partial class ChangeTableNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubMayorCuenta",
                table: "SubMayorCuenta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubMayor",
                table: "SubMayor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExportExcel",
                table: "ExportExcel");

            migrationBuilder.RenameTable(
                name: "SubMayorCuenta",
                newName: "Costos_SubmayorDeCuentas");

            migrationBuilder.RenameTable(
                name: "SubMayor",
                newName: "Costos_Submayor");

            migrationBuilder.RenameTable(
                name: "ExportExcel",
                newName: "Costos_ExportExcel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Costos_SubmayorDeCuentas",
                table: "Costos_SubmayorDeCuentas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Costos_Submayor",
                table: "Costos_Submayor",
                columns: new[] { "Analisis", "Ano", "Cta", "Debe", "Epigrafe", "Fecha", "Haber", "Mes", "SubAnalisis", "SubCta" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Costos_ExportExcel",
                table: "Costos_ExportExcel",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Costos_SubmayorDeCuentas",
                table: "Costos_SubmayorDeCuentas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Costos_Submayor",
                table: "Costos_Submayor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Costos_ExportExcel",
                table: "Costos_ExportExcel");

            migrationBuilder.RenameTable(
                name: "Costos_SubmayorDeCuentas",
                newName: "SubMayorCuenta");

            migrationBuilder.RenameTable(
                name: "Costos_Submayor",
                newName: "SubMayor");

            migrationBuilder.RenameTable(
                name: "Costos_ExportExcel",
                newName: "ExportExcel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubMayorCuenta",
                table: "SubMayorCuenta",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubMayor",
                table: "SubMayor",
                columns: new[] { "Analisis", "Ano", "Cta", "Debe", "Epigrafe", "Fecha", "Haber", "Mes", "SubAnalisis", "SubCta" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExportExcel",
                table: "ExportExcel",
                column: "Id");
        }
    }
}
