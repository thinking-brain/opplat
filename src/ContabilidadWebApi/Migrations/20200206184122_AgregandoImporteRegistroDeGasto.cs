using Microsoft.EntityFrameworkCore.Migrations;

namespace ContabilidadWebApi.Migrations
{
    public partial class AgregandoImporteRegistroDeGasto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Importe",
                table: "RegistroDeGastos",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Importe",
                table: "RegistroDeGastos");
        }
    }
}
