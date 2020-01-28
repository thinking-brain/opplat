using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RhWebApi.Migrations
{
    public partial class bolsa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Bolsa",
                table: "trabajadores",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Bolsa",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    TrabajadorId = table.Column<int>(nullable: true),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Recomendado = table.Column<bool>(nullable: false),
                    Nombre_Recomendador = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bolsa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bolsa_trabajadores_TrabajadorId",
                        column: x => x.TrabajadorId,
                        principalTable: "trabajadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bolsa_TrabajadorId",
                table: "Bolsa",
                column: "TrabajadorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bolsa");

            migrationBuilder.DropColumn(
                name: "Bolsa",
                table: "trabajadores");
        }
    }
}
