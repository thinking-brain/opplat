using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace op_costos_api.Migrations
{
    public partial class migZwei : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubMayor",
                columns: table => new
                {
                    Ano = table.Column<short>(nullable: false),
                    Cta = table.Column<string>(maxLength: 20, nullable: false),
                    SubCta = table.Column<string>(maxLength: 20, nullable: false),
                    Analisis = table.Column<string>(maxLength: 20, nullable: false),
                    SubAnalisis = table.Column<string>(maxLength: 20, nullable: false),
                    Epigrafe = table.Column<string>(maxLength: 20, nullable: false),
                    Fecha = table.Column<DateTime>(type: "smalldatetime", nullable: false),
                    Debe = table.Column<decimal>(type: "money", nullable: false),
                    Haber = table.Column<decimal>(type: "money", nullable: false),
                    Mes = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubMayor", x => new { x.Analisis, x.Ano, x.Cta, x.Debe, x.Epigrafe, x.Fecha, x.Haber, x.Mes, x.SubAnalisis, x.SubCta });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubMayor");
        }
    }
}
