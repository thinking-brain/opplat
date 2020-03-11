using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ContratacionWebApi.Migrations
{
    public partial class _1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormasDePago_Contratos_ContratoId",
                table: "FormasDePago");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormasDePago",
                table: "FormasDePago");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Entidad",
                table: "Entidad");

            migrationBuilder.RenameTable(
                name: "FormasDePago",
                newName: "FormasDePagos");

            migrationBuilder.RenameTable(
                name: "Entidad",
                newName: "EspecialistasExternos");

            migrationBuilder.RenameIndex(
                name: "IX_FormasDePago_ContratoId",
                table: "FormasDePagos",
                newName: "IX_FormasDePagos_ContratoId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "HistoricosEstadoContratos",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Entidades",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Contratos",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "FormasDePagos",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "EspecialistasExternos",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormasDePagos",
                table: "FormasDePagos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EspecialistasExternos",
                table: "EspecialistasExternos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormasDePagos_Contratos_ContratoId",
                table: "FormasDePagos",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormasDePagos_Contratos_ContratoId",
                table: "FormasDePagos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormasDePagos",
                table: "FormasDePagos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EspecialistasExternos",
                table: "EspecialistasExternos");

            migrationBuilder.RenameTable(
                name: "FormasDePagos",
                newName: "FormasDePago");

            migrationBuilder.RenameTable(
                name: "EspecialistasExternos",
                newName: "Entidad");

            migrationBuilder.RenameIndex(
                name: "IX_FormasDePagos_ContratoId",
                table: "FormasDePago",
                newName: "IX_FormasDePago_ContratoId");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "HistoricosEstadoContratos",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Entidades",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Contratos",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "FormasDePago",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Entidad",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormasDePago",
                table: "FormasDePago",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Entidad",
                table: "Entidad",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormasDePago_Contratos_ContratoId",
                table: "FormasDePago",
                column: "ContratoId",
                principalTable: "Contratos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
