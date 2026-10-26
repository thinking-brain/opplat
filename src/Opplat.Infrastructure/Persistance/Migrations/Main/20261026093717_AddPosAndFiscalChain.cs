using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Opplat.Infrastructure.Persistance.Migrations.Main
{
    /// <inheritdoc />
    public partial class AddPosAndFiscalChain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Invoices_Series_Number",
                table: "Invoices");

            migrationBuilder.AddColumn<string>(
                name: "CashierUserName",
                table: "Sales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAtUtc",
                table: "Sales",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Sales",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "RegisterId",
                table: "Sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftId",
                table: "Sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Sales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "SaleDetail",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "SaleDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "SaleDetail",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "SaleDetail",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "ProductsForSale",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryProductId",
                table: "ProductsForSale",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "ProductsForSale",
                type: "numeric",
                nullable: false,
                defaultValue: 21m);

            migrationBuilder.AddColumn<int>(
                name: "FiscalYear",
                table: "Invoices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "GrossTotal",
                table: "InvoiceLines",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "ChainSequence",
                table: "InvoiceFiscalRecords",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "IssuerTaxId",
                table: "InvoiceFiscalRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "PosRegisters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    InvoiceSeries = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    StorageId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosRegisters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalePayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SaleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    TenderedAmount = table.Column<decimal>(type: "numeric", nullable: true),
                    ChangeGiven = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalePayments_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTerminals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    RegisterId = table.Column<Guid>(type: "uuid", nullable: true),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ProviderTerminalId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTerminals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTerminals_PosRegisters_RegisterId",
                        column: x => x.RegisterId,
                        principalTable: "PosRegisters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PosShifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    RegisterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    OpenedBy = table.Column<string>(type: "text", nullable: false),
                    OpenedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OpeningFloat = table.Column<decimal>(type: "numeric", nullable: false),
                    ClosedBy = table.Column<string>(type: "text", nullable: true),
                    ClosedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExpectedCash = table.Column<decimal>(type: "numeric", nullable: true),
                    CountedCash = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosShifts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosShifts_PosRegisters_RegisterId",
                        column: x => x.RegisterId,
                        principalTable: "PosRegisters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    SalePaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TerminalId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IdempotencyKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ProviderReference = table.Column<string>(type: "text", nullable: true),
                    AuthorizationCode = table.Column<string>(type: "text", nullable: true),
                    CardBrand = table.Column<string>(type: "text", nullable: true),
                    CardLast4 = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    FailureReason = table.Column<string>(type: "text", nullable: true),
                    CustomerReceiptText = table.Column<string>(type: "text", nullable: true),
                    MerchantReceiptText = table.Column<string>(type: "text", nullable: true),
                    RequestedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_PaymentTerminals_TerminalId",
                        column: x => x.TerminalId,
                        principalTable: "PaymentTerminals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentAttempts_SalePayments_SalePaymentId",
                        column: x => x.SalePaymentId,
                        principalTable: "SalePayments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PosCashMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ShiftId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false),
                    User = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PosCashMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PosCashMovements_PosShifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "PosShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductsForSale_Barcode",
                table: "ProductsForSale",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Series_FiscalYear_Number",
                table: "Invoices",
                columns: new[] { "Series", "FiscalYear", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceFiscalRecords_ChainSequence",
                table: "InvoiceFiscalRecords",
                column: "ChainSequence",
                unique: true,
                filter: "\"ChainSequence\" > 0");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_IdempotencyKey",
                table: "PaymentAttempts",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_Provider_ProviderReference",
                table: "PaymentAttempts",
                columns: new[] { "Provider", "ProviderReference" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_SalePaymentId",
                table: "PaymentAttempts",
                column: "SalePaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAttempts_TerminalId",
                table: "PaymentAttempts",
                column: "TerminalId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTerminals_Provider_ProviderTerminalId",
                table: "PaymentTerminals",
                columns: new[] { "Provider", "ProviderTerminalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTerminals_RegisterId",
                table: "PaymentTerminals",
                column: "RegisterId");

            migrationBuilder.CreateIndex(
                name: "IX_PosCashMovements_ShiftId",
                table: "PosCashMovements",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_PosRegisters_Code",
                table: "PosRegisters",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PosRegisters_InvoiceSeries",
                table: "PosRegisters",
                column: "InvoiceSeries",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PosShifts_RegisterId",
                table: "PosShifts",
                column: "RegisterId",
                unique: true,
                filter: "\"Status\" = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SalePayments_SaleId",
                table: "SalePayments",
                column: "SaleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentAttempts");

            migrationBuilder.DropTable(
                name: "PosCashMovements");

            migrationBuilder.DropTable(
                name: "PaymentTerminals");

            migrationBuilder.DropTable(
                name: "SalePayments");

            migrationBuilder.DropTable(
                name: "PosShifts");

            migrationBuilder.DropTable(
                name: "PosRegisters");

            migrationBuilder.DropIndex(
                name: "IX_ProductsForSale_Barcode",
                table: "ProductsForSale");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_Series_FiscalYear_Number",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceFiscalRecords_ChainSequence",
                table: "InvoiceFiscalRecords");

            migrationBuilder.DropColumn(
                name: "CashierUserName",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CompletedAtUtc",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "RegisterId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "SaleDetail");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "SaleDetail");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "SaleDetail");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "SaleDetail");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "ProductsForSale");

            migrationBuilder.DropColumn(
                name: "InventoryProductId",
                table: "ProductsForSale");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "ProductsForSale");

            migrationBuilder.DropColumn(
                name: "FiscalYear",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "GrossTotal",
                table: "InvoiceLines");

            migrationBuilder.DropColumn(
                name: "ChainSequence",
                table: "InvoiceFiscalRecords");

            migrationBuilder.DropColumn(
                name: "IssuerTaxId",
                table: "InvoiceFiscalRecords");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Series_Number",
                table: "Invoices",
                columns: new[] { "Series", "Number" },
                unique: true);
        }
    }
}
