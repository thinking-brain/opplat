using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace InventarioWebApi.Migrations
{
    public partial class nuevosmodelos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_UnidadDeMedida_UnidadId",
                table: "Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Submayores_UnidadDeMedida_UnidadId",
                table: "Submayores");

            migrationBuilder.DropIndex(
                name: "IX_Submayores_UnidadId",
                table: "Submayores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnidadDeMedida",
                table: "UnidadDeMedida");

            migrationBuilder.DropColumn(
                name: "UnidadId",
                table: "Submayores");

            migrationBuilder.RenameTable(
                name: "UnidadDeMedida",
                newName: "UnidadesDeMedida");

            migrationBuilder.RenameColumn(
                name: "Unidad",
                table: "UnidadesDeMedida",
                newName: "Nombre");

            migrationBuilder.AddColumn<string>(
                name: "Localizacion",
                table: "Almacenes",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FactorConversion",
                table: "UnidadesDeMedida",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TipoId",
                table: "UnidadesDeMedida",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnidadesDeMedida",
                table: "UnidadesDeMedida",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TiposDeMovimiento",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: false),
                    Factor = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDeMovimiento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosDeProductos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    AlmacenId = table.Column<int>(nullable: false),
                    ProductoId = table.Column<int>(nullable: false),
                    Cantidad = table.Column<decimal>(nullable: false),
                    UnidadDeMedidaId = table.Column<int>(nullable: false),
                    TipoMovimientoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosDeProductos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosDeProductos_Almacenes_AlmacenId",
                        column: x => x.AlmacenId,
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovimientosDeProductos_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovimientosDeProductos_TiposDeMovimiento_TipoMovimientoId",
                        column: x => x.TipoMovimientoId,
                        principalTable: "TiposDeMovimiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovimientosDeProductos_UnidadesDeMedida_UnidadDeMedidaId",
                        column: x => x.UnidadDeMedidaId,
                        principalTable: "UnidadesDeMedida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosDeProductos_AlmacenId",
                table: "MovimientosDeProductos",
                column: "AlmacenId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosDeProductos_ProductoId",
                table: "MovimientosDeProductos",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosDeProductos_TipoMovimientoId",
                table: "MovimientosDeProductos",
                column: "TipoMovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosDeProductos_UnidadDeMedidaId",
                table: "MovimientosDeProductos",
                column: "UnidadDeMedidaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_UnidadesDeMedida_UnidadId",
                table: "Productos",
                column: "UnidadId",
                principalTable: "UnidadesDeMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Productos_UnidadesDeMedida_UnidadId",
                table: "Productos");

            migrationBuilder.DropTable(
                name: "MovimientosDeProductos");

            migrationBuilder.DropTable(
                name: "TiposDeMovimiento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnidadesDeMedida",
                table: "UnidadesDeMedida");

            migrationBuilder.DropColumn(
                name: "Localizacion",
                table: "Almacenes");

            migrationBuilder.DropColumn(
                name: "FactorConversion",
                table: "UnidadesDeMedida");

            migrationBuilder.DropColumn(
                name: "TipoId",
                table: "UnidadesDeMedida");

            migrationBuilder.RenameTable(
                name: "UnidadesDeMedida",
                newName: "UnidadDeMedida");

            migrationBuilder.RenameColumn(
                name: "Nombre",
                table: "UnidadDeMedida",
                newName: "Unidad");

            migrationBuilder.AddColumn<int>(
                name: "UnidadId",
                table: "Submayores",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnidadDeMedida",
                table: "UnidadDeMedida",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Submayores_UnidadId",
                table: "Submayores",
                column: "UnidadId");

            migrationBuilder.AddForeignKey(
                name: "FK_Productos_UnidadDeMedida_UnidadId",
                table: "Productos",
                column: "UnidadId",
                principalTable: "UnidadDeMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submayores_UnidadDeMedida_UnidadId",
                table: "Submayores",
                column: "UnidadId",
                principalTable: "UnidadDeMedida",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
