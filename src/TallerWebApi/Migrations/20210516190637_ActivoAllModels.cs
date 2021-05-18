using Microsoft.EntityFrameworkCore.Migrations;

namespace TallerWebApi.Migrations
{
    public partial class ActivoAllModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "TiposEquipos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Tecnicos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Talleres",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "RMAs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Repuestos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Proveedores",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Presupuestos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "OrdenesReparaciones_Repuestos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "OrdenesReparaciones",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Modelos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Marcas",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "HistoricoEquipos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Equipos",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "DocumentosEquipos",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "TiposEquipos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Tecnicos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Talleres");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "RMAs");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Repuestos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Presupuestos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "OrdenesReparaciones_Repuestos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "OrdenesReparaciones");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Modelos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Marcas");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "HistoricoEquipos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "DocumentosEquipos");
        }
    }
}
