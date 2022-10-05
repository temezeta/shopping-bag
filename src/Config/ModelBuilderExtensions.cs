using Microsoft.EntityFrameworkCore;
using shopping_bag.Models;
using shopping_bag.Models.User;

namespace shopping_bag.Config
{
    public static class ModelBuilderExtensions
    {
        private static readonly Office[] Offices = new Office[] {
            new Office()
            {
                Id = 1,
                Name = "Espoo"
            },
            new Office()
            {
                Id = 2,
                Name = "Hyvinkää"
            },
            new Office()
            {
                Id = 3,
                Name = "Oulu"
            },
            new Office()
            {
                Id = 4,
                Name = "Vaasa"
            },
            new Office()
            {
                Id = 5,
                Name = "Tampere"
            },
            new Office()
            {
                Id = 6,
                Name = "Seinäjoki"
            },
            new Office()
            {
                Id = 7,
                Name = "Jyväskylä"
            },
            new Office()
            {
                Id = 8,
                Name = "Kotka"
            }};

        private static readonly UserRole[] UserRoles = new UserRole[] {
            new UserRole() {
                RoleId = 1,
                RoleName = "Admin"
            },
            new UserRole() {
                RoleId = 2,
                RoleName = "User"
            }
        };

        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Office>().HasData(Offices);
            modelBuilder.Entity<UserRole>().HasData(UserRoles);
        }
    }
}