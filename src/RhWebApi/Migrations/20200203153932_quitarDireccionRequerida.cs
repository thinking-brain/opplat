using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class quitarDireccionRequerida : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "trabajadores",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Direccion",
                table: "trabajadores",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
