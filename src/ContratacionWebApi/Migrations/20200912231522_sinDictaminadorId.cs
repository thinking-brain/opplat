using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class sinDictaminadorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dictamenes_DictaminadoresContratos_DictaminadorContratoId",
                table: "Dictamenes");

            migrationBuilder.DropIndex(
                name: "IX_Dictamenes_DictaminadorContratoId",
                table: "Dictamenes");

            migrationBuilder.DropColumn(
                name: "DictaminadorContratoId",
                table: "Dictamenes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DictaminadorContratoId",
                table: "Dictamenes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Dictamenes_DictaminadorContratoId",
                table: "Dictamenes",
                column: "DictaminadorContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dictamenes_DictaminadoresContratos_DictaminadorContratoId",
                table: "Dictamenes",
                column: "DictaminadorContratoId",
                principalTable: "DictaminadoresContratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
