using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContratacionWebApi.Migrations
{
    public partial class DictContratoModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContratoId",
                table: "EspecialistasExternos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DictContratoId",
                table: "EspecialistasExternos",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DictaminadoresContrato",
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
                    table.PrimaryKey("PK_DictaminadoresContrato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DictaminadoresContrato_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EspecialistasExternos_DictContratoId",
                table: "EspecialistasExternos",
                column: "DictContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_DictaminadoresContrato_ContratoId",
                table: "DictaminadoresContrato",
                column: "ContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_EspecialistasExternos_DictaminadoresContrato_DictContratoId",
                table: "EspecialistasExternos",
                column: "DictContratoId",
                principalTable: "DictaminadoresContrato",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EspecialistasExternos_DictaminadoresContrato_DictContratoId",
                table: "EspecialistasExternos");

            migrationBuilder.DropTable(
                name: "DictaminadoresContrato");

            migrationBuilder.DropIndex(
                name: "IX_EspecialistasExternos_DictContratoId",
                table: "EspecialistasExternos");

            migrationBuilder.DropColumn(
                name: "ContratoId",
                table: "EspecialistasExternos");

            migrationBuilder.DropColumn(
                name: "DictContratoId",
                table: "EspecialistasExternos");
        }
    }
}
