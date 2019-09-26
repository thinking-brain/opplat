using Microsoft.EntityFrameworkCore.Migrations;

namespace Account.WebApi.Migrations
{
    public partial class AgregandoUsuarioInicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1", "4c2826e7-6c9a-4c22-a542-3d04c4676b8d", "administrador", "ADMINISTRADOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "Activo", "Apellidos", "Nombres" },
                values: new object[] { "f42559a2-2776-4e9b-9ba1-268597eff72b", 0, "36fd2616-8e8a-4cc6-8a5a-52d963207836", "Usuario", "admin@opplat.cu", false, false, null, "ADMIN@OPPLAT.CU", "ADMIN", "AQAAAAEAACcQAAAAEP4OedI6m26WUn/2C4AcBkzdT6SnL/6E+xakQ/9mGAkqqp3t9PwyIR6l9obLouKIVg==", null, false, "43VMKYQKNTENYZVJNU2TII26X23H5PGV", false, "admin", true, "General", "Administrador" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "f42559a2-2776-4e9b-9ba1-268597eff72b", "1" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "f42559a2-2776-4e9b-9ba1-268597eff72b", "1" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "f42559a2-2776-4e9b-9ba1-268597eff72b");
        }
    }
}
