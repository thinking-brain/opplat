using Microsoft.EntityFrameworkCore.Migrations;

namespace op_costos_api.Migrations
{
    public partial class migCodePartida : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Costos_GrupoSubelemento",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Costos_GrupoSubelemento");
        }
    }
}
