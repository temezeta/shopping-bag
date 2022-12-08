using Microsoft.EntityFrameworkCore;
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
                Name = "Tampere HVT11"
            },
            new Office()
            {
                Id = 2,
                Name = "Tampere HVT34"
            },
            new Office()
            {
                Id = 3,
                Name = "Espoo"
            },
            new Office()
            {
                Id = 4,
                Name = "Hyvinkää"
            },
            new Office()
            {
                Id = 5,
                Name = "Oulu"
            },
            new Office()
            {
                Id = 6,
                Name = "Vaasa Wasa Innovation Center"
            },
            new Office()
            {
                Id = 7,
                Name = "Vaasa Wulffintie"
            },
            new Office()
            {
                Id = 8,
                Name = "Seinäjoki"
            },
            new Office()
            {
                Id = 9,
                Name = "Jyväskylä"
            },
            new Office()
            {
                Id = 10,
                Name = "Kotka"
            },
            new Office()
            {
                Id = 11,
                Name = "Ylivieska"
            },
            new Office()
            {
                Id = 12,
                Name = "Kokkola"
            },
            new Office()
            {
                Id = 13,
                Name = "Turku"
            },
            new Office()
            {
                Id = 14,
                Name = "Kuopio"
            },
            new Office()
            {
                Id = 15,
                Name = "Prague"
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

        public static void SeedDefaultAdmin(this AppDbContext context)
        {
            // If Admin user exists, don't seed
            if (context.Users.Include(u => u.UserRoles).Any(it => it.UserRoles.Any(r => r.RoleName == Roles.AdminRole)))
            {
                Console.WriteLine("User with Admin role already exists");
                return;
            }

            var roles = context.UserRoles.Where(it => StaticConfig.DefaultAdminRoles.Contains(it.RoleName)).ToList();

            if (roles == null || roles.Count == 0)
            {
                Console.WriteLine("No valid roles");
                return;
            }

            var office = context.Offices.FirstOrDefault(it => it.Id == StaticConfig.DefaultAdminOfficeId);

            if (office == null)
            {
                Console.WriteLine($"Office with id {StaticConfig.DefaultAdminOfficeId} not found");
                return;
            }

            AuthHelper.CreatePasswordHash(StaticConfig.DefaultAdminPassword, out byte[] passwordHash, out byte[] passwordSalt);
            var verificationToken = AuthHelper.CreateHexToken();

            var user = new User()
            {
                FirstName = "Admin",
                LastName = "User",
                Email = StaticConfig.DefaultAdminEmail,
                OfficeId = office.Id,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerificationToken = verificationToken,
                VerifiedAt = DateTime.Now, // Don't require verification for default user
            };
            var reminderSettings = new ReminderSettings()
            {
                ReminderDaysBeforeDueDate = new List<int>() { 2 },
                ReminderDaysBeforeExpectedDate = new List<int>(),
                DueDateRemindersDisabled = false,
                ExpectedRemindersDisabled = true,
                UserId = user.Id,
                AllRemindersDisabled = false
            };
            user.ReminderSettings = reminderSettings;

            user.UserRoles.AddRange(roles);
            context.Users.Add(user);
            context.SaveChanges();
            Console.WriteLine("Default Admin user added");
        }
    }
}