using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Opplat.MainApp.Migrations
{
    public partial class AddCostTabs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductForSaleProductTag_ProductTag_TagsId",
                table: "ProductForSaleProductTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductTag",
                table: "ProductTag");

            migrationBuilder.RenameTable(
                name: "ProductTag",
                newName: "ProductTags");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductTags",
                table: "ProductTags",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CostTabs",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Preparation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Presentation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlannedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpectedRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FixedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostTabs", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_CostTabs_ProductsForSale_ProductId1",
                        column: x => x.ProductId1,
                        principalTable: "ProductsForSale",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CostTabDetail",
                columns: table => new
                {
                    ProductForSaleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FixedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VariableCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostTabProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostTabDetail", x => new { x.ProductForSaleId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_CostTabDetail_CostTabs_CostTabProductId",
                        column: x => x.CostTabProductId,
                        principalTable: "CostTabs",
                        principalColumn: "ProductId");
                    table.ForeignKey(
                        name: "FK_CostTabDetail_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostTabDetail_ProductsForSale_ProductForSaleId",
                        column: x => x.ProductForSaleId,
                        principalTable: "ProductsForSale",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "8c9485e9-da6c-42a9-9b96-fc21b4ed3cf1");

            migrationBuilder.CreateIndex(
                name: "IX_CostTabDetail_CostTabProductId",
                table: "CostTabDetail",
                column: "CostTabProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CostTabDetail_ProductId",
                table: "CostTabDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CostTabs_ProductId1",
                table: "CostTabs",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductForSaleProductTag_ProductTags_TagsId",
                table: "ProductForSaleProductTag",
                column: "TagsId",
                principalTable: "ProductTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductForSaleProductTag_ProductTags_TagsId",
                table: "ProductForSaleProductTag");

            migrationBuilder.DropTable(
                name: "CostTabDetail");

            migrationBuilder.DropTable(
                name: "CostTabs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductTags",
                table: "ProductTags");

            migrationBuilder.RenameTable(
                name: "ProductTags",
                newName: "ProductTag");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductTag",
                table: "ProductTag",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "cafff030-f3be-4aaf-b12d-cd694674bf30");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductForSaleProductTag_ProductTag_TagsId",
                table: "ProductForSaleProductTag",
                column: "TagsId",
                principalTable: "ProductTag",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
