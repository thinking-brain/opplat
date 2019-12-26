using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class sexoNuleable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Sexo",
                table: "trabajadores",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Sexo",
                table: "trabajadores",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
