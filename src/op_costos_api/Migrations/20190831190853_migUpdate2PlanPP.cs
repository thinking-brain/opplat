using Microsoft.EntityFrameworkCore.Migrations;

namespace op_costos_api.Migrations
{
    public partial class migUpdate2PlanPP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConceptoCuentas_ConceptoPlan_ConceptoPlanId",
                table: "ConceptoCuentas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConceptoCuentas",
                table: "ConceptoCuentas");

            migrationBuilder.DropIndex(
                name: "IX_ConceptoCuentas_ConceptoPlanId",
                table: "ConceptoCuentas");

            migrationBuilder.DropColumn(
                name: "ConceptoId",
                table: "ConceptoCuentas");

            migrationBuilder.AlterColumn<int>(
                name: "ConceptoPlanId",
                table: "ConceptoCuentas",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConceptoCuentas",
                table: "ConceptoCuentas",
                columns: new[] { "ConceptoPlanId", "CuentaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ConceptoCuentas_ConceptoPlan_ConceptoPlanId",
                table: "ConceptoCuentas",
                column: "ConceptoPlanId",
                principalTable: "ConceptoPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConceptoCuentas_ConceptoPlan_ConceptoPlanId",
                table: "ConceptoCuentas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConceptoCuentas",
                table: "ConceptoCuentas");

            migrationBuilder.AlterColumn<int>(
                name: "ConceptoPlanId",
                table: "ConceptoCuentas",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ConceptoId",
                table: "ConceptoCuentas",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConceptoCuentas",
                table: "ConceptoCuentas",
                columns: new[] { "ConceptoId", "CuentaId" });

            migrationBuilder.CreateIndex(
                name: "IX_ConceptoCuentas_ConceptoPlanId",
                table: "ConceptoCuentas",
                column: "ConceptoPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConceptoCuentas_ConceptoPlan_ConceptoPlanId",
                table: "ConceptoCuentas",
                column: "ConceptoPlanId",
                principalTable: "ConceptoPlan",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
