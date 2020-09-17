using Microsoft.EntityFrameworkCore.Migrations;

namespace ContratacionWebApi.Migrations
{
    public partial class numero : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroDeDictamen",
                table: "Dictamenes");

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "Dictamenes",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Dictamenes");

            migrationBuilder.AddColumn<string>(
                name: "NumeroDeDictamen",
                table: "Dictamenes",
                nullable: true);
        }
    }
}
