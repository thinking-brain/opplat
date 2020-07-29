using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContratacionWebApi.Migrations
{
    public partial class Montos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MontoCuc",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "MontoCup",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "MontoUsd",
                table: "Contratos");

            migrationBuilder.CreateTable(
                name: "Monto",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cantidad = table.Column<decimal>(nullable: false),
                    Moneda = table.Column<int>(nullable: false),
                    ContratoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Monto_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Monto_ContratoId",
                table: "Monto",
                column: "ContratoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Monto");

            migrationBuilder.AddColumn<decimal>(
                name: "MontoCuc",
                table: "Contratos",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoCup",
                table: "Contratos",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoUsd",
                table: "Contratos",
                nullable: true);
        }
    }
}
