using Microsoft.EntityFrameworkCore.Migrations;

namespace ContabilidadWebApi.Migrations
{
    public partial class BorradoEnCascadaCuenta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contb_cuentas_contb_cuentas_CuentaSuperiorId",
                table: "contb_cuentas");

            migrationBuilder.AddForeignKey(
                name: "FK_contb_cuentas_contb_cuentas_CuentaSuperiorId",
                table: "contb_cuentas",
                column: "CuentaSuperiorId",
                principalTable: "contb_cuentas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contb_cuentas_contb_cuentas_CuentaSuperiorId",
                table: "contb_cuentas");

            migrationBuilder.AddForeignKey(
                name: "FK_contb_cuentas_contb_cuentas_CuentaSuperiorId",
                table: "contb_cuentas",
                column: "CuentaSuperiorId",
                principalTable: "contb_cuentas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
