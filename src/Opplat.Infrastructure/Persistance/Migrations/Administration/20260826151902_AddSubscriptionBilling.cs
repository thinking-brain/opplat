using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Opplat.Infrastructure.Persistance.Migrations.Administration
{
    /// <inheritdoc />
    public partial class AddSubscriptionBilling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingInterval",
                table: "tenants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BillingStatus",
                table: "tenants",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "CancelAtPeriodEnd",
                table: "tenants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextBillingDate",
                table: "tenants",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeCustomerId",
                table: "tenants",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripeSubscriptionId",
                table: "tenants",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "subscription_plans",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PricingAnnual",
                table: "subscription_plans",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "StripePriceIdAnnual",
                table: "subscription_plans",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StripePriceIdMonthly",
                table: "subscription_plans",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "subscription_invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    StripeInvoiceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    AmountDue = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    PeriodStart = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PeriodEnd = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    HostedInvoiceUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subscription_invoices_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tenant_payment_methods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    StripePaymentMethodId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Brand = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Last4 = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    ExpMonth = table.Column<long>(type: "bigint", nullable: true),
                    ExpYear = table.Column<long>(type: "bigint", nullable: true),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_payment_methods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tenant_payment_methods_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_subscription_invoices_StripeInvoiceId",
                table: "subscription_invoices",
                column: "StripeInvoiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_subscription_invoices_TenantId_PeriodEnd",
                table: "subscription_invoices",
                columns: new[] { "TenantId", "PeriodEnd" });

            migrationBuilder.CreateIndex(
                name: "IX_tenant_payment_methods_StripePaymentMethodId",
                table: "tenant_payment_methods",
                column: "StripePaymentMethodId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenant_payment_methods_TenantId_IsDefault",
                table: "tenant_payment_methods",
                columns: new[] { "TenantId", "IsDefault" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscription_invoices");

            migrationBuilder.DropTable(
                name: "tenant_payment_methods");

            migrationBuilder.DropColumn(
                name: "BillingInterval",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "BillingStatus",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "CancelAtPeriodEnd",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "NextBillingDate",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "StripeCustomerId",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "StripeSubscriptionId",
                table: "tenants");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "PricingAnnual",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "StripePriceIdAnnual",
                table: "subscription_plans");

            migrationBuilder.DropColumn(
                name: "StripePriceIdMonthly",
                table: "subscription_plans");
        }
    }
}
