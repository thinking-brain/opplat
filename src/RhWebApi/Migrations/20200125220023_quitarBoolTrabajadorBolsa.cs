using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class quitarBoolTrabajadorBolsa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bolsa",
                table: "trabajadores");

            migrationBuilder.AlterColumn<string>(
                name: "CI",
                table: "trabajadores",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "TallaCalzado",
                table: "caracteristicas_del_trabjador",
                nullable: true,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CI",
                table: "trabajadores",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<bool>(
                name: "Bolsa",
                table: "trabajadores",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<double>(
                name: "TallaCalzado",
                table: "caracteristicas_del_trabjador",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);
        }
    }
}
