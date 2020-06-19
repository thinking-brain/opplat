using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContratacionWebApi.Migrations
{
    public partial class ContratoId_DepartamentoId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DictaminadoresContratos_Departamento_DepartamentoId",
                table: "DictaminadoresContratos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departamento",
                table: "Departamento");

            migrationBuilder.RenameTable(
                name: "Departamento",
                newName: "Departamentos");

            migrationBuilder.AddColumn<int>(
                name: "DepartamentoId",
                table: "AdminContratos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departamentos",
                table: "Departamentos",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ContratoId_DepartamentoId",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoId = table.Column<int>(nullable: false),
                    DepartamentoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContratoId_DepartamentoId", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContratoId_DepartamentoId_Contratos_ContratoId",
                        column: x => x.ContratoId,
                        principalTable: "Contratos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContratoId_DepartamentoId_Departamentos_DepartamentoId",
                        column: x => x.DepartamentoId,
                        principalTable: "Departamentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminContratos_DepartamentoId",
                table: "AdminContratos",
                column: "DepartamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratoId_DepartamentoId_ContratoId",
                table: "ContratoId_DepartamentoId",
                column: "ContratoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratoId_DepartamentoId_DepartamentoId",
                table: "ContratoId_DepartamentoId",
                column: "DepartamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminContratos_Departamentos_DepartamentoId",
                table: "AdminContratos",
                column: "DepartamentoId",
                principalTable: "Departamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DictaminadoresContratos_Departamentos_DepartamentoId",
                table: "DictaminadoresContratos",
                column: "DepartamentoId",
                principalTable: "Departamentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminContratos_Departamentos_DepartamentoId",
                table: "AdminContratos");

            migrationBuilder.DropForeignKey(
                name: "FK_DictaminadoresContratos_Departamentos_DepartamentoId",
                table: "DictaminadoresContratos");

            migrationBuilder.DropTable(
                name: "ContratoId_DepartamentoId");

            migrationBuilder.DropIndex(
                name: "IX_AdminContratos_DepartamentoId",
                table: "AdminContratos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departamentos",
                table: "Departamentos");

            migrationBuilder.DropColumn(
                name: "DepartamentoId",
                table: "AdminContratos");

            migrationBuilder.RenameTable(
                name: "Departamentos",
                newName: "Departamento");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departamento",
                table: "Departamento",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DictaminadoresContratos_Departamento_DepartamentoId",
                table: "DictaminadoresContratos",
                column: "DepartamentoId",
                principalTable: "Departamento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
