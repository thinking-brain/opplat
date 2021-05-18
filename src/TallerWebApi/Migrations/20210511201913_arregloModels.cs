using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TallerWebApi.Migrations
{
    public partial class arregloModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipos_Tipos_TipoId",
                table: "Equipos");

            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropTable(
                name: "Tipos");

            migrationBuilder.RenameColumn(
                name: "TipoId",
                table: "Equipos",
                newName: "TipoEquipoId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipos_TipoId",
                table: "Equipos",
                newName: "IX_Equipos_TipoEquipoId");

            migrationBuilder.CreateTable(
                name: "DocumentosEquipos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true),
                    FilePath = table.Column<string>(nullable: true),
                    EquipoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosEquipos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentosEquipos_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TiposEquipos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Nombre = table.Column<string>(nullable: true),
                    Codigo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposEquipos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosEquipos_EquipoId",
                table: "DocumentosEquipos",
                column: "EquipoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipos_TiposEquipos_TipoEquipoId",
                table: "Equipos",
                column: "TipoEquipoId",
                principalTable: "TiposEquipos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipos_TiposEquipos_TipoEquipoId",
                table: "Equipos");

            migrationBuilder.DropTable(
                name: "DocumentosEquipos");

            migrationBuilder.DropTable(
                name: "TiposEquipos");

            migrationBuilder.RenameColumn(
                name: "TipoEquipoId",
                table: "Equipos",
                newName: "TipoId");

            migrationBuilder.RenameIndex(
                name: "IX_Equipos_TipoEquipoId",
                table: "Equipos",
                newName: "IX_Equipos_TipoId");

            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    EquipoId = table.Column<int>(nullable: true),
                    FilePath = table.Column<string>(nullable: true),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documentos_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tipos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Codigo = table.Column<string>(nullable: true),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tipos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_EquipoId",
                table: "Documentos",
                column: "EquipoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipos_Tipos_TipoId",
                table: "Equipos",
                column: "TipoId",
                principalTable: "Tipos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
