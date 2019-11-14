using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace InventarioWebApi.Migrations
{
    public partial class unidadesmedida : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TiposUnidadDeMedida",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposUnidadDeMedida", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TiposUnidadDeMedida",
                columns: new[] { "Id", "Nombre" },
                values: new object[] { 1, "Masa" });

            migrationBuilder.InsertData(
                table: "UnidadesDeMedida",
                columns: new[] { "Id", "Descripcion", "FactorConversion", "Nombre", "TipoId" },
                values: new object[,]
                {
                    { 1, "g", 1m, "Gramo", 1 },
                    { 2, "kg", 1000m, "Kilogramo", 1 },
                    { 3, "q", 100000m, "Quintal", 1 },
                    { 4, "t", 1000000m, "Tonelada", 1 },
                    { 5, "lb", 453.59m, "Libra", 1 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TiposUnidadDeMedida");

            migrationBuilder.DeleteData(
                table: "UnidadesDeMedida",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UnidadesDeMedida",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UnidadesDeMedida",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "UnidadesDeMedida",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "UnidadesDeMedida",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
