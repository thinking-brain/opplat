using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Opplat.Infrastructure.Persistance.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddInvoiceFiscalRecordOutboxFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastErrorMessage",
                table: "InvoiceFiscalRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextRetryAtUtc",
                table: "InvoiceFiscalRecords",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RetryCount",
                table: "InvoiceFiscalRecords",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceFiscalRecords_SubmissionStatus_NextRetryAtUtc",
                table: "InvoiceFiscalRecords",
                columns: new[] { "SubmissionStatus", "NextRetryAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InvoiceFiscalRecords_SubmissionStatus_NextRetryAtUtc",
                table: "InvoiceFiscalRecords");

            migrationBuilder.DropColumn(
                name: "LastErrorMessage",
                table: "InvoiceFiscalRecords");

            migrationBuilder.DropColumn(
                name: "NextRetryAtUtc",
                table: "InvoiceFiscalRecords");

            migrationBuilder.DropColumn(
                name: "RetryCount",
                table: "InvoiceFiscalRecords");
        }
    }
}
