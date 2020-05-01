using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class ArrayTelefonos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuentasBancarias_Entidades_EntidadId",
                table: "CuentasBancarias");

            migrationBuilder.AlterColumn<string>(
                name: "Numero",
                table: "Telefonos",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Extension",
                table: "Telefonos",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "EntidadId",
                table: "CuentasBancarias",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CuentasBancarias_Entidades_EntidadId",
                table: "CuentasBancarias",
                column: "EntidadId",
                principalTable: "Entidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuentasBancarias_Entidades_EntidadId",
                table: "CuentasBancarias");

            migrationBuilder.AlterColumn<int>(
                name: "Numero",
                table: "Telefonos",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Extension",
                table: "Telefonos",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EntidadId",
                table: "CuentasBancarias",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_CuentasBancarias_Entidades_EntidadId",
                table: "CuentasBancarias",
                column: "EntidadId",
                principalTable: "Entidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
