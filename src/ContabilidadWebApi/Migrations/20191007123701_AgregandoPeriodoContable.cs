using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContabilidadWebApi.Migrations
{
    public partial class AgregandoPeriodoContable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PeriodoContableId",
                table: "contb_dia_contable",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PeriodoContable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaInicio = table.Column<DateTime>(nullable: false),
                    FechaFin = table.Column<DateTime>(nullable: true),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodoContable", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contb_dia_contable_PeriodoContableId",
                table: "contb_dia_contable",
                column: "PeriodoContableId");

            migrationBuilder.AddForeignKey(
                name: "FK_contb_dia_contable_PeriodoContable_PeriodoContableId",
                table: "contb_dia_contable",
                column: "PeriodoContableId",
                principalTable: "PeriodoContable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_contb_dia_contable_PeriodoContable_PeriodoContableId",
                table: "contb_dia_contable");

            migrationBuilder.DropTable(
                name: "PeriodoContable");

            migrationBuilder.DropIndex(
                name: "IX_contb_dia_contable_PeriodoContableId",
                table: "contb_dia_contable");

            migrationBuilder.DropColumn(
                name: "PeriodoContableId",
                table: "contb_dia_contable");
        }
    }
}
