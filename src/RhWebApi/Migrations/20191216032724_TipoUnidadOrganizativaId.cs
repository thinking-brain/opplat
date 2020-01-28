using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class TipoUnidadOrganizativaId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_unidades_organizativas_tipos_de_unidad_organizativa_TipoUni~",
                table: "unidades_organizativas");

            migrationBuilder.AlterColumn<int>(
                name: "TipoUnidadOrganizativaId",
                table: "unidades_organizativas",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_unidades_organizativas_tipos_de_unidad_organizativa_TipoUni~",
                table: "unidades_organizativas",
                column: "TipoUnidadOrganizativaId",
                principalTable: "tipos_de_unidad_organizativa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_unidades_organizativas_tipos_de_unidad_organizativa_TipoUni~",
                table: "unidades_organizativas");

            migrationBuilder.AlterColumn<int>(
                name: "TipoUnidadOrganizativaId",
                table: "unidades_organizativas",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_unidades_organizativas_tipos_de_unidad_organizativa_TipoUni~",
                table: "unidades_organizativas",
                column: "TipoUnidadOrganizativaId",
                principalTable: "tipos_de_unidad_organizativa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
