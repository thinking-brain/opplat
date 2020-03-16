using Microsoft.EntityFrameworkCore.Migrations;

namespace FinanzasWebApi.Migrations
{
    public partial class ReportAddDiv : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Dividir",
                table: "ElementosDelReporteEFs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dividir",
                table: "ElementosDelReporteEFs");
        }
    }
}
