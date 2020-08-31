using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class Aprob_por_Estado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AprobComitContratacion",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "AprobEconomico",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "AprobJuridico",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Contratos");

            migrationBuilder.AddColumn<int>(
                name: "EstadoComitContratacion",
                table: "Contratos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstadoContrato",
                table: "Contratos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstadoEconomico",
                table: "Contratos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstadoJuridico",
                table: "Contratos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Departamentos",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 1, "Económico" });

            migrationBuilder.InsertData(
                table: "Departamentos",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 2, "Jurídico" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Departamentos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Departamentos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "EstadoComitContratacion",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "EstadoContrato",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "EstadoEconomico",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "EstadoJuridico",
                table: "Contratos");

            migrationBuilder.AddColumn<bool>(
                name: "AprobComitContratacion",
                table: "Contratos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AprobEconomico",
                table: "Contratos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AprobJuridico",
                table: "Contratos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Contratos",
                nullable: false,
                defaultValue: 0);
        }
    }
}
