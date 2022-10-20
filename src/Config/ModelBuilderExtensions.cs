﻿using Microsoft.EntityFrameworkCore;
using shopping_bag.Models;
using shopping_bag.Models.User;
using shopping_bag.Utility;

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

        private static readonly ShoppingList[] ShoppingLists = new ShoppingList[]
        {
            new ShoppingList()
            {
                Id = 1,
                Name = "Week 50 list",
                Comment = "Weekly order",
                Ordered = false,
                CreatedDate = new DateTime(2022, 10, 12, 12, 00, 00),
                DueDate = new DateTime(2022, 12, 18, 17, 00, 00),
                ExpectedDeliveryDate = new DateTime(2022, 12, 20, 17, 00, 00),
                OfficeId = 1,
                UserId = null
            },
            new ShoppingList()
            {
                Id = 2,
                Name = "Office supplies",
                Comment = "List for office supplies",
                Ordered = false,
                CreatedDate = new DateTime(2022, 10, 12, 12, 00, 00),
                DueDate = new DateTime(2023, 1, 15, 22, 00, 00),
                ExpectedDeliveryDate = new DateTime(2023, 2, 15, 12, 00, 00),
                OfficeId = 1,
                UserId = null
    },
            new ShoppingList()
            {
                Id = 3,
                Name = "Tampere office list",
                Comment = "No due or delivery dates set",
                Ordered = false,
                CreatedDate = new DateTime(2022, 10, 12, 12, 00, 00),
                OfficeId = 5,
                UserId = null
            },
            new ShoppingList()
            {
                Id = 4,
                Name = "Week 40 list",
                Comment = "Order that is overdue but not delivered",
                Ordered = true,
                CreatedDate = new DateTime(2022, 10, 12, 12, 00, 00),
                DueDate = new DateTime(2022, 10, 9, 17, 00, 00),
                ExpectedDeliveryDate = new DateTime(2023, 1, 15, 12, 00, 00),
                OfficeId = 1,
                UserId = null
            },
            new ShoppingList()
            {
                Id = 5,
                Name = "Week 39 list",
                Comment = "Order that is overdue and delivered",
                Ordered = true,
                CreatedDate = new DateTime(2022, 10, 12, 12, 00, 00),
                DueDate = new DateTime(2022, 9, 30, 17, 00, 00),
                ExpectedDeliveryDate = new DateTime(2022, 10, 3, 12, 00, 00),
                OfficeId = 1,
                UserId = null
            },
            new ShoppingList()
            {
                Id = 6,
                Name = "List with only a name",
                Ordered = false,
                CreatedDate = new DateTime(2022, 10, 12, 12, 00, 00),
                OfficeId = 1,
                UserId = null
            }
        };

        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Office>().HasData(Offices);
            modelBuilder.Entity<UserRole>().HasData(UserRoles);
            modelBuilder.Entity<ShoppingList>().HasData(ShoppingLists);
        }

        public static void SeedDefaultAdmin(this AppDbContext context)
        {
            // If Admin user exists, don't seed
            if (context.Users.Include(u => u.UserRoles).Any(it => it.UserRoles.Any(r => r.RoleName == Roles.AdminRole)))
            {
                Console.WriteLine("User with Admin role already exists");
                return;
            }

            var adminRole = context.UserRoles.FirstOrDefault(it => it.RoleName == Roles.AdminRole);

            if (adminRole == null)
            {
                Console.WriteLine("Admin role not found");
                return;
            }

            AuthHelper.CreatePasswordHash(StaticConfig.DefaultAdminPassword, out byte[] passwordHash, out byte[] passwordSalt);
            var verificationToken = AuthHelper.CreateHexToken();

            var user = new User()
            {
                FirstName = "Admin",
                LastName = "User",
                Email = StaticConfig.DefaultAdminEmail,
                OfficeId = Offices.First().Id,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken = verificationToken,
                VerifiedAt = DateTime.Now, // Don't require verification for default user
            };
            user.UserRoles.Add(adminRole);
            context.Users.Add(user);
            context.SaveChanges();
            Console.WriteLine("Default Admin user added");
        }
    }
}