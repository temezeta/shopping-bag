using Microsoft.EntityFrameworkCore;
using shopping_bag.Models;
using shopping_bag.Models.User;

namespace shopping_bag.Config
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            builder.Entity<Office>().HasIndex(o => o.Name).IsUnique();
            builder.Seed();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Office> Offices { get; set; }
    }
}
