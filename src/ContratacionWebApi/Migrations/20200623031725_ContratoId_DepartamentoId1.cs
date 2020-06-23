using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContratacionWebApi.Migrations
{
    public partial class ContratoId_DepartamentoId1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContratoId_DictaminadorId");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContratoId_DepartamentoId",
                table: "ContratoId_DepartamentoId");

            migrationBuilder.DropIndex(
                name: "IX_ContratoId_DepartamentoId_ContratoId",
                table: "ContratoId_DepartamentoId");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ContratoId_DepartamentoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContratoId_DepartamentoId",
                table: "ContratoId_DepartamentoId",
                columns: new[] { "ContratoId", "DepartamentoId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ContratoId_DepartamentoId",
                table: "ContratoId_DepartamentoId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ContratoId_DepartamentoId",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContratoId_DepartamentoId",
                table: "ContratoId_DepartamentoId",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ContratoId_DictaminadorId",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(nullable: false),
                    DictaminadorContratoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContratoId_DictaminadorId", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContratoId_DepartamentoId_ContratoId",
                table: "ContratoId_DepartamentoId",
                column: "ContratoId");
        }
    }
}
