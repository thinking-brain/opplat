using Account.WebApi.Models;
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

        public DbSet<Usuario> Usuarios { get; set; }

    }
}