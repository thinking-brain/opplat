using Account.WebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Account.WebApi.Data
{
    public class AccountDbContext : IdentityDbContext
    {
        public AccountDbContext()
        {

        }
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole[] {
                    new IdentityRole {Id = "1", Name = "administrador", NormalizedName = "ADMINISTRADOR" },
             });
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = "f42559a2-2776-4e9b-9ba1-268597eff72b",
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@opplat.cu",
                    NormalizedEmail = "ADMIN@OPPLAT.CU",
                    PasswordHash = "AQAAAAEAACcQAAAAEP4OedI6m26WUn/2C4AcBkzdT6SnL/6E+xakQ/9mGAkqqp3t9PwyIR6l9obLouKIVg==",
                    SecurityStamp = "43VMKYQKNTENYZVJNU2TII26X23H5PGV",
                    ConcurrencyStamp = "36fd2616-8e8a-4cc6-8a5a-52d963207836",
                    Activo = true,
                    Nombres = "Administrador",
                    Apellidos = "General",
                }
                );
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    UserId = "f42559a2-2776-4e9b-9ba1-268597eff72b",
                    RoleId = "1"
                }
                );
        }

        public DbSet<Usuario> Usuarios { get; set; }

    }
}