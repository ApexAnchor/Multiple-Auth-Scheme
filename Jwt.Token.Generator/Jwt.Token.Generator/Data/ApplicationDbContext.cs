using Jwt.Token.Generator.Models;
using Microsoft.EntityFrameworkCore;

namespace Jwt.Token.Generator.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }

    }
}
