using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class trasladoFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Traslados_unidades_organizativas_UnidadOrganizativaId",
                table: "Traslados");

            migrationBuilder.RenameColumn(
                name: "UnidadOrganizativaId",
                table: "Traslados",
                newName: "UnidOrgOrigenId");

            migrationBuilder.RenameIndex(
                name: "IX_Traslados_UnidadOrganizativaId",
                table: "Traslados",
                newName: "IX_Traslados_UnidOrgOrigenId");

            migrationBuilder.AddColumn<int>(
                name: "UnidOrgDestinoId",
                table: "Traslados",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Traslados_UnidOrgDestinoId",
                table: "Traslados",
                column: "UnidOrgDestinoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Traslados_unidades_organizativas_UnidOrgDestinoId",
                table: "Traslados",
                column: "UnidOrgDestinoId",
                principalTable: "unidades_organizativas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Traslados_unidades_organizativas_UnidOrgOrigenId",
                table: "Traslados",
                column: "UnidOrgOrigenId",
                principalTable: "unidades_organizativas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Traslados_unidades_organizativas_UnidOrgDestinoId",
                table: "Traslados");

            migrationBuilder.DropForeignKey(
                name: "FK_Traslados_unidades_organizativas_UnidOrgOrigenId",
                table: "Traslados");

            migrationBuilder.DropIndex(
                name: "IX_Traslados_UnidOrgDestinoId",
                table: "Traslados");

            migrationBuilder.DropColumn(
                name: "UnidOrgDestinoId",
                table: "Traslados");

            migrationBuilder.RenameColumn(
                name: "UnidOrgOrigenId",
                table: "Traslados",
                newName: "UnidadOrganizativaId");

            migrationBuilder.RenameIndex(
                name: "IX_Traslados_UnidOrgOrigenId",
                table: "Traslados",
                newName: "IX_Traslados_UnidadOrganizativaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Traslados_unidades_organizativas_UnidadOrganizativaId",
                table: "Traslados",
                column: "UnidadOrganizativaId",
                principalTable: "unidades_organizativas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
