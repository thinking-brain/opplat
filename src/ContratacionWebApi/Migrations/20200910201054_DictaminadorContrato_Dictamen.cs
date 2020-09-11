using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class DictaminadorContrato_Dictamen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dictamen_Especialista_EspecialistaId",
                table: "Dictamen");

            migrationBuilder.AlterColumn<int>(
                name: "EspecialistaId",
                table: "Dictamen",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "DictaminadorContratoId",
                table: "Dictamen",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Dictamen_DictaminadorContratoId",
                table: "Dictamen",
                column: "DictaminadorContratoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dictamen_DictaminadoresContratos_DictaminadorContratoId",
                table: "Dictamen",
                column: "DictaminadorContratoId",
                principalTable: "DictaminadoresContratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dictamen_Especialista_EspecialistaId",
                table: "Dictamen",
                column: "EspecialistaId",
                principalTable: "Especialista",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dictamen_DictaminadoresContratos_DictaminadorContratoId",
                table: "Dictamen");

            migrationBuilder.DropForeignKey(
                name: "FK_Dictamen_Especialista_EspecialistaId",
                table: "Dictamen");

            migrationBuilder.DropIndex(
                name: "IX_Dictamen_DictaminadorContratoId",
                table: "Dictamen");

            migrationBuilder.DropColumn(
                name: "DictaminadorContratoId",
                table: "Dictamen");

            migrationBuilder.AlterColumn<int>(
                name: "EspecialistaId",
                table: "Dictamen",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Dictamen_Especialista_EspecialistaId",
                table: "Dictamen",
                column: "EspecialistaId",
                principalTable: "Especialista",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
