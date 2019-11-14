using Microsoft.EntityFrameworkCore.Migrations;

namespace InventarioWebApi.Migrations
{
    public partial class tipo_movimiento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "TiposDeMovimiento",
                newName: "Descripcion");

            migrationBuilder.InsertData(
                table: "TiposDeMovimiento",
                columns: new[] { "Id", "Descripcion", "Factor" },
                values: new object[,]
                {
                    { 1, "Entrada", 1 },
                    { 2, "Entrada traslado interno", 1 },
                    { 3, "Merma", -1 },
                    { 4, "Salida a producción", -1 },
                    { 5, "Salida traslado interno", -1 },
                    { 6, "Venta independiente", -1 },
                    { 7, "Entrada por error en salida", 1 },
                    { 8, "Salida por error en entrada", -1 },
                    { 9, "Entrada por ajuste", 1 },
                    { 10, "Salida", -1 },
                    { 11, "Entrada por conversión de producto", 1 },
                    { 12, "Salida por conversion de producto", -1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TiposDeMovimiento",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TiposDeMovimiento",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TiposDeMovimiento",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TiposDeMovimiento",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TiposDeMovimiento",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TiposDeMovimiento",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TiposDeMovimiento",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TiposDeMovimiento",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TiposDeMovimiento",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "TiposDeMovimiento",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "TiposDeMovimiento",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "TiposDeMovimiento",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "TiposDeMovimiento",
                newName: "Nombre");
        }
    }
}
