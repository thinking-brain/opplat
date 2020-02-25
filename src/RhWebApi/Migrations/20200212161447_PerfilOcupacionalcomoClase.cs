using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RhWebApi.Migrations
{
    public partial class PerfilOcupacionalcomoClase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PerfilOcupacionalId",
                table: "trabajadores",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PerfilOcupacional",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilOcupacional", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trabajadores_PerfilOcupacionalId",
                table: "trabajadores",
                column: "PerfilOcupacionalId");

            migrationBuilder.AddForeignKey(
                name: "FK_trabajadores_PerfilOcupacional_PerfilOcupacionalId",
                table: "trabajadores",
                column: "PerfilOcupacionalId",
                principalTable: "PerfilOcupacional",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_trabajadores_PerfilOcupacional_PerfilOcupacionalId",
                table: "trabajadores");

            migrationBuilder.DropTable(
                name: "PerfilOcupacional");

            migrationBuilder.DropIndex(
                name: "IX_trabajadores_PerfilOcupacionalId",
                table: "trabajadores");

            migrationBuilder.DropColumn(
                name: "PerfilOcupacionalId",
                table: "trabajadores");
        }
    }
}
