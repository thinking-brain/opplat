using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RhWebApi.Migrations
{
    public partial class plantillacontrollers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Plantilla",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    CargoId = table.Column<int>(nullable: false),
                    UnidadOrganizativaId = table.Column<int>(nullable: false),
                    PlantillaAprobada = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plantilla", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plantilla_cargo_CargoId",
                        column: x => x.CargoId,
                        principalTable: "cargo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Plantilla_unidades_organizativas_UnidadOrganizativaId",
                        column: x => x.UnidadOrganizativaId,
                        principalTable: "unidades_organizativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Plantilla_CargoId",
                table: "Plantilla",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_Plantilla_UnidadOrganizativaId",
                table: "Plantilla",
                column: "UnidadOrganizativaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plantilla");
        }
    }
}
