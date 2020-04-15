using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class CuentasModels1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EntidadId",
                table: "CuentasBancarias",
                nullable: true);

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
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
