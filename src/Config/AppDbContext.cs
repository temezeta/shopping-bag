using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using shopping_bag.Models;
using shopping_bag.Models.User;
using System.Text.Json;

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
            var intListValueComparer = new ValueComparer<List<int>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            );
            builder.Entity<ReminderSettings>(e => {
                e.Property(r => r.ReminderDaysBeforeDueDate).HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null),
                    intListValueComparer);
                e.Property(r => r.ReminderDaysBeforeExpectedDate).HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null),
                    intListValueComparer);
            });
            builder.Entity<Reminder>(e => {
                e.Property(r => r.DueDaysBefore).HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null),
                    intListValueComparer);
                e.Property(r => r.ExpectedDaysBefore).HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null),
                    intListValueComparer);
                e.HasOne(r => r.User).WithMany(u => u.Reminders).OnDelete(DeleteBehavior.ClientCascade);
            });
            builder.Seed();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ReminderSettings> ReminderSettings { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
    }
}
