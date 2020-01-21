using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class plantillaOcupada : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CantidadPorPlantilla",
                table: "puestos_de_trabajos",
                newName: "PlantillaOcupada");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlantillaOcupada",
                table: "puestos_de_trabajos",
                newName: "CantidadPorPlantilla");
        }
    }
}
