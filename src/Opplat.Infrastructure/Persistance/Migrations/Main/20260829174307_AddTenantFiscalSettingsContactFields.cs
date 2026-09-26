using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Opplat.Infrastructure.Persistance.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddTenantFiscalSettingsContactFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "TenantFiscalSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "TenantFiscalSettings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "TenantFiscalSettings",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "TenantFiscalSettings");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "TenantFiscalSettings");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "TenantFiscalSettings");
        }
    }
}
