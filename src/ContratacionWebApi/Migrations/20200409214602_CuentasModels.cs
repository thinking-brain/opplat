using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class CuentasModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CuentasBancarias_Entidades_EntidadId",
                table: "CuentasBancarias");

            migrationBuilder.DropIndex(
                name: "IX_CuentasBancarias_EntidadId",
                table: "CuentasBancarias");

            migrationBuilder.DropColumn(
                name: "EntidadId",
                table: "CuentasBancarias");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EntidadId",
                table: "CuentasBancarias",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CuentasBancarias_EntidadId",
                table: "CuentasBancarias",
                column: "EntidadId");

            migrationBuilder.AddForeignKey(
                name: "FK_CuentasBancarias_Entidades_EntidadId",
                table: "CuentasBancarias",
                column: "EntidadId",
                principalTable: "Entidades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
