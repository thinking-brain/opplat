using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class dictamen_Filepath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoricoDeDocumento_Documentos_DocumentoId",
                table: "HistoricoDeDocumento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HistoricoDeDocumento",
                table: "HistoricoDeDocumento");

            migrationBuilder.DropColumn(
                name: "Aprobado",
                table: "Dictamen");

            migrationBuilder.RenameTable(
                name: "HistoricoDeDocumento",
                newName: "HistoricosDeDocumentos");

            migrationBuilder.RenameIndex(
                name: "IX_HistoricoDeDocumento_DocumentoId",
                table: "HistoricosDeDocumentos",
                newName: "IX_HistoricosDeDocumentos_DocumentoId");

            migrationBuilder.AlterColumn<string>(
                name: "FundamentosDeDerecho",
                table: "Dictamen",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Dictamen",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_HistoricosDeDocumentos",
                table: "HistoricosDeDocumentos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricosDeDocumentos_Documentos_DocumentoId",
                table: "HistoricosDeDocumentos",
                column: "DocumentoId",
                principalTable: "Documentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoricosDeDocumentos_Documentos_DocumentoId",
                table: "HistoricosDeDocumentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HistoricosDeDocumentos",
                table: "HistoricosDeDocumentos");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Dictamen");

            migrationBuilder.RenameTable(
                name: "HistoricosDeDocumentos",
                newName: "HistoricoDeDocumento");

            migrationBuilder.RenameIndex(
                name: "IX_HistoricosDeDocumentos_DocumentoId",
                table: "HistoricoDeDocumento",
                newName: "IX_HistoricoDeDocumento_DocumentoId");

            migrationBuilder.AlterColumn<string>(
                name: "FundamentosDeDerecho",
                table: "Dictamen",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Aprobado",
                table: "Dictamen",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_HistoricoDeDocumento",
                table: "HistoricoDeDocumento",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricoDeDocumento_Documentos_DocumentoId",
                table: "HistoricoDeDocumento",
                column: "DocumentoId",
                principalTable: "Documentos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
