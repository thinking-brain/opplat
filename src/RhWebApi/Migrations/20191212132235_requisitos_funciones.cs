using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class requisitos_funciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cargo_grupos_escalas_GrupoEscalaId",
                table: "cargo");

            migrationBuilder.DropForeignKey(
                name: "FK_cargo_cargo_JefeId",
                table: "cargo");

            migrationBuilder.DropForeignKey(
                name: "FK_Entradas_cargo_CargoId",
                table: "Entradas");

            migrationBuilder.DropForeignKey(
                name: "FK_Plantilla_cargo_CargoId",
                table: "Plantilla");

            migrationBuilder.DropForeignKey(
                name: "FK_puestos_de_trabajos_cargo_CargoId",
                table: "puestos_de_trabajos");

            migrationBuilder.DropForeignKey(
                name: "FK_Traslados_cargo_CargoDestinoId",
                table: "Traslados");

            migrationBuilder.DropForeignKey(
                name: "FK_Traslados_cargo_CargoOrigenId",
                table: "Traslados");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cargo",
                table: "cargo");

            migrationBuilder.RenameTable(
                name: "cargo",
                newName: "Cargo");

            migrationBuilder.RenameIndex(
                name: "IX_cargo_JefeId",
                table: "Cargo",
                newName: "IX_Cargo_JefeId");

            migrationBuilder.RenameIndex(
                name: "IX_cargo_GrupoEscalaId",
                table: "Cargo",
                newName: "IX_Cargo_GrupoEscalaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cargo",
                table: "Cargo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cargo_grupos_escalas_GrupoEscalaId",
                table: "Cargo",
                column: "GrupoEscalaId",
                principalTable: "grupos_escalas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cargo_Cargo_JefeId",
                table: "Cargo",
                column: "JefeId",
                principalTable: "Cargo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entradas_Cargo_CargoId",
                table: "Entradas",
                column: "CargoId",
                principalTable: "Cargo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Plantilla_Cargo_CargoId",
                table: "Plantilla",
                column: "CargoId",
                principalTable: "Cargo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_puestos_de_trabajos_Cargo_CargoId",
                table: "puestos_de_trabajos",
                column: "CargoId",
                principalTable: "Cargo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Traslados_Cargo_CargoDestinoId",
                table: "Traslados",
                column: "CargoDestinoId",
                principalTable: "Cargo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Traslados_Cargo_CargoOrigenId",
                table: "Traslados",
                column: "CargoOrigenId",
                principalTable: "Cargo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cargo_grupos_escalas_GrupoEscalaId",
                table: "Cargo");

            migrationBuilder.DropForeignKey(
                name: "FK_Cargo_Cargo_JefeId",
                table: "Cargo");

            migrationBuilder.DropForeignKey(
                name: "FK_Entradas_Cargo_CargoId",
                table: "Entradas");

            migrationBuilder.DropForeignKey(
                name: "FK_Plantilla_Cargo_CargoId",
                table: "Plantilla");

            migrationBuilder.DropForeignKey(
                name: "FK_puestos_de_trabajos_Cargo_CargoId",
                table: "puestos_de_trabajos");

            migrationBuilder.DropForeignKey(
                name: "FK_Traslados_Cargo_CargoDestinoId",
                table: "Traslados");

            migrationBuilder.DropForeignKey(
                name: "FK_Traslados_Cargo_CargoOrigenId",
                table: "Traslados");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cargo",
                table: "Cargo");

            migrationBuilder.RenameTable(
                name: "Cargo",
                newName: "cargo");

            migrationBuilder.RenameIndex(
                name: "IX_Cargo_JefeId",
                table: "cargo",
                newName: "IX_cargo_JefeId");

            migrationBuilder.RenameIndex(
                name: "IX_Cargo_GrupoEscalaId",
                table: "cargo",
                newName: "IX_cargo_GrupoEscalaId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cargo",
                table: "cargo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_cargo_grupos_escalas_GrupoEscalaId",
                table: "cargo",
                column: "GrupoEscalaId",
                principalTable: "grupos_escalas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_cargo_cargo_JefeId",
                table: "cargo",
                column: "JefeId",
                principalTable: "cargo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Entradas_cargo_CargoId",
                table: "Entradas",
                column: "CargoId",
                principalTable: "cargo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Plantilla_cargo_CargoId",
                table: "Plantilla",
                column: "CargoId",
                principalTable: "cargo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_puestos_de_trabajos_cargo_CargoId",
                table: "puestos_de_trabajos",
                column: "CargoId",
                principalTable: "cargo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Traslados_cargo_CargoDestinoId",
                table: "Traslados",
                column: "CargoDestinoId",
                principalTable: "cargo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Traslados_cargo_CargoOrigenId",
                table: "Traslados",
                column: "CargoOrigenId",
                principalTable: "cargo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
