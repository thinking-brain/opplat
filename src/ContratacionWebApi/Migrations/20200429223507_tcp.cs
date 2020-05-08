using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class tcp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CI",
                table: "Entidades",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarnetTCP",
                table: "Entidades",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CI",
                table: "Entidades");

            migrationBuilder.DropColumn(
                name: "CarnetTCP",
                table: "Entidades");
        }
    }
}
