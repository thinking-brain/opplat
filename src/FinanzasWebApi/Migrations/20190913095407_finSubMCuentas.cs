using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanzasWebApi.Migrations
{
    public partial class finSubMCuentas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdPaseDeCuenta",
                table: "Costos_SubmayorDeCuentas",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdPaseDeCuenta",
                table: "Costos_SubmayorDeCuentas");
        }
    }
}
