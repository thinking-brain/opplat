using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContratacionWebApi.Migrations
{
    public partial class DictaminadoresContratoDbContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContratoId_DictaminadorId",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(nullable: false),
                    JuridicoId = table.Column<int>(nullable: false),
                    EconomicoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContratoId_DictaminadorId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContratoId_DictaminadorId_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EspecialistaExternoId_ContratoId",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EspecialistaExternoId = table.Column<int>(nullable: false),
                    ContratoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EspecialistaExternoId_ContratoId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EspecialistaExternoId_ContratoId_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EspecialistaExternoId_ContratoId_EspecialistasExternos_Espe~",
                        column: x => x.EspecialistaExternoId,
                        principalTable: "EspecialistasExternos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContratoId_DictaminadorId_ContratoId",
                table: "ContratoId_DictaminadorId",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_EspecialistaExternoId_ContratoId_ContratoId",
                table: "EspecialistaExternoId_ContratoId",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_EspecialistaExternoId_ContratoId_EspecialistaExternoId",
                table: "EspecialistaExternoId_ContratoId",
                column: "EspecialistaExternoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContratoId_DictaminadorId");

            migrationBuilder.DropTable(
                name: "EspecialistaExternoId_ContratoId");
        }
    }
}
