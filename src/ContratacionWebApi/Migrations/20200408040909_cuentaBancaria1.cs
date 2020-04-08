using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContratacionWebApi.Migrations
{
    public partial class cuentaBancaria1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CuentasBancarias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumeroCuenta = table.Column<int>(nullable: false),
                    NumeroSucursal = table.Column<int>(nullable: false),
                    NombreSucursal = table.Column<int>(nullable: false),
                    Moneda = table.Column<int>(nullable: false),
                    EntidadId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasBancarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentasBancarias_Entidades_EntidadId",
                        column: x => x.EntidadId,
                        principalTable: "Entidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CuentasBancarias_EntidadId",
                table: "CuentasBancarias",
                column: "EntidadId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CuentasBancarias");
        }
    }
}
