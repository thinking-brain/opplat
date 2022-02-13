using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContabilidadWebApi.Migrations
{
    public partial class RemoveVariableEstadoFinanciero : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VariablesEstadoFinancieroCuentas");

            migrationBuilder.DropTable(
                name: "VariablesEstadoFinanciero");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VariablesEstadoFinanciero",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariablesEstadoFinanciero", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VariablesEstadoFinancieroCuentas",
                columns: table => new
                {
                    VariablesEstadoFinancieroId = table.Column<int>(nullable: false),
                    CuentaId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariablesEstadoFinancieroCuentas", x => new { x.VariablesEstadoFinancieroId, x.CuentaId });
                    table.ForeignKey(
                        name: "FK_VariablesEstadoFinancieroCuentas_contb_cuentas_CuentaId",
                        column: x => x.CuentaId,
                        principalTable: "contb_cuentas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VariablesEstadoFinancieroCuentas_VariablesEstadoFinanciero_~",
                        column: x => x.VariablesEstadoFinancieroId,
                        principalTable: "VariablesEstadoFinanciero",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VariablesEstadoFinancieroCuentas_CuentaId",
                table: "VariablesEstadoFinancieroCuentas",
                column: "CuentaId");
        }
    }
}
