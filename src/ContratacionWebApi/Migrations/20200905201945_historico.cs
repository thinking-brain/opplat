using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class historico : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "HistoricoDeDocumento");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "HistoricoDeDocumento",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "HistoricoDeDocumento");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "HistoricoDeDocumento",
                nullable: false,
                defaultValue: 0);
        }
    }
}
