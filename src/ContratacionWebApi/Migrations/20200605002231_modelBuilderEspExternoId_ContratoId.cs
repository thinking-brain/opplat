using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContratacionWebApi.Migrations
{
    public partial class modelBuilderEspExternoId_ContratoId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EspecialistaExternoId_ContratoId",
                table: "EspecialistaExternoId_ContratoId");

            migrationBuilder.DropIndex(
                name: "IX_EspecialistaExternoId_ContratoId_ContratoId",
                table: "EspecialistaExternoId_ContratoId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "EspecialistaExternoId_ContratoId",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EspecialistaExternoId_ContratoId",
                table: "EspecialistaExternoId_ContratoId",
                columns: new[] { "ContratoId", "EspecialistaExternoId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EspecialistaExternoId_ContratoId",
                table: "EspecialistaExternoId_ContratoId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "EspecialistaExternoId_ContratoId",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EspecialistaExternoId_ContratoId",
                table: "EspecialistaExternoId_ContratoId",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EspecialistaExternoId_ContratoId_ContratoId",
                table: "EspecialistaExternoId_ContratoId",
                column: "ContratoId");
        }
    }
}
