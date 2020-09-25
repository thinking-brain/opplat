using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class contratoId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContratoId",
                table: "Contratos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Departamentos",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nombre",
                value: "ECONÓMICO");

            migrationBuilder.UpdateData(
                table: "Departamentos",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nombre",
                value: "JURÍDICO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContratoId",
                table: "Contratos");

            migrationBuilder.UpdateData(
                table: "Departamentos",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nombre",
                value: "Económico");

            migrationBuilder.UpdateData(
                table: "Departamentos",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nombre",
                value: "Jurídico");
        }
    }
}
