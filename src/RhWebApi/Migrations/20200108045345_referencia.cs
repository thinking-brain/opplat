using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class referencia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bolsa_trabajadores_TrabajadorId",
                table: "Bolsa");

            migrationBuilder.RenameColumn(
                name: "Recomendado",
                table: "Bolsa",
                newName: "Referencia");

            migrationBuilder.RenameColumn(
                name: "Nombre_Recomendador",
                table: "Bolsa",
                newName: "Nombre_Referencia");

            migrationBuilder.AlterColumn<int>(
                name: "TrabajadorId",
                table: "Bolsa",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bolsa_trabajadores_TrabajadorId",
                table: "Bolsa",
                column: "TrabajadorId",
                principalTable: "trabajadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bolsa_trabajadores_TrabajadorId",
                table: "Bolsa");

            migrationBuilder.RenameColumn(
                name: "Referencia",
                table: "Bolsa",
                newName: "Recomendado");

            migrationBuilder.RenameColumn(
                name: "Nombre_Referencia",
                table: "Bolsa",
                newName: "Nombre_Recomendador");

            migrationBuilder.AlterColumn<int>(
                name: "TrabajadorId",
                table: "Bolsa",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Bolsa_trabajadores_TrabajadorId",
                table: "Bolsa",
                column: "TrabajadorId",
                principalTable: "trabajadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
