using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class ObjetoSocial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ObjetoSocial",
                table: "Entidades",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NumeroSucursal",
                table: "CuentasBancarias",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "NumeroCuenta",
                table: "CuentasBancarias",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObjetoSocial",
                table: "Entidades");

            migrationBuilder.AlterColumn<string>(
                name: "NumeroSucursal",
                table: "CuentasBancarias",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NumeroCuenta",
                table: "CuentasBancarias",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
