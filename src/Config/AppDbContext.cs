using Microsoft.EntityFrameworkCore;
using shopping_bag.Models;
using shopping_bag.Models.User;

namespace shopping_bag.Config
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(e => {
                e.HasIndex(u => u.Email).IsUnique();

                // Likes: many-to-many relationship
                e.HasMany(u => u.LikedItems).WithMany(i => i.UsersWhoLiked).UsingEntity<Dictionary<string, object>>(
                    "Likes",
                    x => x.HasOne<Item>().WithMany().OnDelete(DeleteBehavior.Cascade),
                    x => x.HasOne<User>().WithMany().OnDelete(DeleteBehavior.ClientCascade)
                );
            });
            builder.Entity<Office>().HasIndex(o => o.Name).IsUnique();
            builder.Entity<Item>().HasOne(i => i.ItemAdder).WithMany();
            builder.Seed();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<Item> Items { get; set; }
    }
}
