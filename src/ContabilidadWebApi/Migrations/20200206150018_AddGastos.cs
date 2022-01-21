using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContabilidadWebApi.Migrations
{
    public partial class AddGastos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ElementoDeGastos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    Codigo = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementoDeGastos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartidaDeGastos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Desripcion = table.Column<string>(nullable: true),
                    Codigo = table.Column<string>(nullable: true),
                    Activo = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartidaDeGastos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CuentaElementoDeGastos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CuentaId = table.Column<int>(nullable: false),
                    ElementoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentaElementoDeGastos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentaElementoDeGastos_contb_cuentas_CuentaId",
                        column: x => x.CuentaId,
                        principalTable: "contb_cuentas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CuentaElementoDeGastos_ElementoDeGastos_ElementoId",
                        column: x => x.ElementoId,
                        principalTable: "ElementoDeGastos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubElementoDeGastos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(nullable: true),
                    Codigo = table.Column<string>(nullable: true),
                    MonedaNacional = table.Column<bool>(nullable: false),
                    Activo = table.Column<bool>(nullable: false),
                    ElementoId = table.Column<int>(nullable: false),
                    PartidaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubElementoDeGastos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubElementoDeGastos_ElementoDeGastos_ElementoId",
                        column: x => x.ElementoId,
                        principalTable: "ElementoDeGastos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubElementoDeGastos_PartidaDeGastos_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "PartidaDeGastos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistroDeGastos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AsientoId = table.Column<int>(nullable: false),
                    SubElementoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroDeGastos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistroDeGastos_contb_asientos_AsientoId",
                        column: x => x.AsientoId,
                        principalTable: "contb_asientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistroDeGastos_SubElementoDeGastos_SubElementoId",
                        column: x => x.SubElementoId,
                        principalTable: "SubElementoDeGastos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CuentaElementoDeGastos_CuentaId",
                table: "CuentaElementoDeGastos",
                column: "CuentaId");

            migrationBuilder.CreateIndex(
                name: "IX_CuentaElementoDeGastos_ElementoId",
                table: "CuentaElementoDeGastos",
                column: "ElementoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroDeGastos_AsientoId",
                table: "RegistroDeGastos",
                column: "AsientoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroDeGastos_SubElementoId",
                table: "RegistroDeGastos",
                column: "SubElementoId");

            migrationBuilder.CreateIndex(
                name: "IX_SubElementoDeGastos_ElementoId",
                table: "SubElementoDeGastos",
                column: "ElementoId");

            migrationBuilder.CreateIndex(
                name: "IX_SubElementoDeGastos_PartidaId",
                table: "SubElementoDeGastos",
                column: "PartidaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CuentaElementoDeGastos");

            migrationBuilder.DropTable(
                name: "RegistroDeGastos");

            migrationBuilder.DropTable(
                name: "SubElementoDeGastos");

            migrationBuilder.DropTable(
                name: "ElementoDeGastos");

            migrationBuilder.DropTable(
                name: "PartidaDeGastos");
        }
    }
}
