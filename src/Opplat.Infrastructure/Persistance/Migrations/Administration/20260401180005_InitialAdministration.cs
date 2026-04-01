using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Opplat.Infrastructure.Persistance.Migrations.Administration
{
    /// <inheritdoc />
    public partial class InitialAdministration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminTenantInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2026, 4, 1, 18, 0, 5, 50, DateTimeKind.Utc).AddTicks(974)),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Identifier = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DatabaseName = table.Column<string>(type: "text", nullable: false),
                    DatabaseSchema = table.Column<string>(type: "text", nullable: false),
                    UserCount = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminTenantInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "database_instances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2026, 4, 1, 18, 0, 5, 50, DateTimeKind.Utc).AddTicks(974)),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Identifier = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ConnectionStringReference = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    CurrentTenantSchemaCount = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ArchivedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_database_instances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "subscription_plans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2026, 4, 1, 18, 0, 5, 50, DateTimeKind.Utc).AddTicks(974)),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    MaxActiveUsers = table.Column<int>(type: "integer", nullable: false),
                    MaxApiCallsPerMonth = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxStorageGb = table.Column<decimal>(type: "numeric", nullable: false),
                    PricingMonthly = table.Column<decimal>(type: "numeric", nullable: false),
                    ResourceLimits = table.Column<string>(type: "jsonb", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subscription_plans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Identifier = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SubscriptionPlanId = table.Column<Guid>(type: "uuid", nullable: false),
                    InactivatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DatabaseInstanceId = table.Column<Guid>(type: "uuid", nullable: false),
                    DatabaseName = table.Column<string>(type: "text", nullable: false),
                    DatabaseSchema = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.Id);
                    table.UniqueConstraint("AK_tenants_Identifier", x => x.Identifier);
                    table.ForeignKey(
                        name: "FK_tenants_database_instances_DatabaseInstanceId",
                        column: x => x.DatabaseInstanceId,
                        principalTable: "database_instances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tenants_subscription_plans_SubscriptionPlanId",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "subscription_plans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntraOid = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", maxLength: 128, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    IsPrimaryAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    DeactivatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2026, 4, 1, 18, 0, 5, 60, DateTimeKind.Utc).AddTicks(6726)),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_users", x => x.Id);
                    table.UniqueConstraint("AK_tenant_users_EntraOid", x => x.EntraOid);
                    table.ForeignKey(
                        name: "FK_tenant_users_tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2026, 4, 1, 18, 0, 5, 50, DateTimeKind.Utc).AddTicks(974)),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ActorOid = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    TargetTenantId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    TargetTenantIdFk = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    TargetUserId = table.Column<string>(type: "character varying(128)", nullable: true),
                    ActionType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    BeforeState = table.Column<string>(type: "jsonb", nullable: true),
                    AfterState = table.Column<string>(type: "jsonb", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2026, 4, 1, 18, 0, 5, 23, DateTimeKind.Utc).AddTicks(4911))
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_audit_logs_tenant_users_ActorOid",
                        column: x => x.ActorOid,
                        principalTable: "tenant_users",
                        principalColumn: "EntraOid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_audit_logs_tenant_users_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "tenant_users",
                        principalColumn: "EntraOid",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_audit_logs_tenants_TargetTenantIdFk",
                        column: x => x.TargetTenantIdFk,
                        principalTable: "tenants",
                        principalColumn: "Identifier",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_ActorOid",
                table: "audit_logs",
                column: "ActorOid");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_TargetTenantIdFk",
                table: "audit_logs",
                column: "TargetTenantIdFk");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_TargetUserId",
                table: "audit_logs",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_tenant_users_TenantId_EntraOid",
                table: "tenant_users",
                columns: new[] { "TenantId", "EntraOid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_DatabaseInstanceId",
                table: "tenants",
                column: "DatabaseInstanceId");

            migrationBuilder.CreateIndex(
                name: "IX_tenants_Identifier",
                table: "tenants",
                column: "Identifier",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_SubscriptionPlanId",
                table: "tenants",
                column: "SubscriptionPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminTenantInfo");

            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "tenant_users");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropTable(
                name: "database_instances");

            migrationBuilder.DropTable(
                name: "subscription_plans");
        }
    }
}
