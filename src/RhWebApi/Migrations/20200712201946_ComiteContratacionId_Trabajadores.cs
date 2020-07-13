using Microsoft.EntityFrameworkCore.Migrations;

namespace RhWebApi.Migrations
{
    public partial class ComiteContratacionId_Trabajadores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComiteContratacionId",
                table: "trabajadores",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComiteContratacionId",
                table: "trabajadores");
        }
    }
}
