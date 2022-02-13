using Microsoft.EntityFrameworkCore.Migrations;

namespace InventarioWebApi.Migrations
{
    public partial class producto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecioCompraMlc",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "PrecioCompraMn",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "PrecioVentaMlc",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "PrecioVentaMn",
                table: "Productos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecioCompraMlc",
                table: "Productos",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioCompraMn",
                table: "Productos",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioVentaMlc",
                table: "Productos",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PrecioVentaMn",
                table: "Productos",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
