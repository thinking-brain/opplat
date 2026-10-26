using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Opplat.Infrastructure.Persistance.Migrations.Administration
{
    /// <inheritdoc />
    public partial class AddRedsysTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RenewalRetryCount",
                table: "tenants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "redsys_transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    Kind = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ResponseCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    CofTxnId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IdentifierToken = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    AmountCents = table.Column<long>(type: "bigint", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    RawResponsePayload = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_redsys_transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_redsys_transactions_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_redsys_transactions_Order",
                table: "redsys_transactions",
                column: "Order",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_redsys_transactions_TenantId_Kind_Status",
                table: "redsys_transactions",
                columns: new[] { "TenantId", "Kind", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "redsys_transactions");

            migrationBuilder.DropColumn(
                name: "RenewalRetryCount",
                table: "tenants");
        }
    }
}
