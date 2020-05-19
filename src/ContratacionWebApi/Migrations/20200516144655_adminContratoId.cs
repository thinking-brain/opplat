using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class adminContratoId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TrabajadorId",
                table: "Dictaminadores",
                newName: "DictaminadorId");

            migrationBuilder.RenameColumn(
                name: "TrabajadorId",
                table: "AdminContratos",
                newName: "AdminContratoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DictaminadorId",
                table: "Dictaminadores",
                newName: "TrabajadorId");

            migrationBuilder.RenameColumn(
                name: "AdminContratoId",
                table: "AdminContratos",
                newName: "TrabajadorId");
        }
    }
}
