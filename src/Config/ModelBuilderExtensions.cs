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
                Name = "Tampere HVT11 (61.49142122101034, 23.770750652503075)"
            },
            new Office()
            {
                Id = 2,
                Name = "Tampere HVT34 (61.4872349013685, 23.771011831141426)"
            },
            new Office()
            {
                Id = 3,
                Name = "Espoo (60.17320724919939, 24.82951312687209)"
            },
            new Office()
            {
                Id = 4,
                Name = "Hyvinkää (60.61885645330777, 24.81364812340716)"
            },
            new Office()
            {
                Id = 5,
                Name = "Oulu (65.0573847614918, 25.443263561914264)"
            },
            new Office()
            {
                Id = 6,
                Name = "Vaasa Wasa Innovation Center (63.116523411260665, 21.62020520510232)"
            },
            new Office()
            {
                Id = 7,
                Name = "Vaasa Wulffintie (63.09815319119998, 21.601082366419462)"
            },
            new Office()
            {
                Id = 8,
                Name = "Seinäjoki (62.80052477778116, 22.822913653422315)"
            },
            new Office()
            {
                Id = 9,
                Name = "Jyväskylä (62.244890931070074, 25.750669670647447)"
            },
            new Office()
            {
                Id = 10,
                Name = "Kotka (60.51600193933175, 26.928281488329468)"
            },
            new Office()
            {
                Id = 11,
                Name = "Ylivieska (64.07478730741482, 24.51536955120399)"
            },
            new Office()
            {
                Id = 12,
                Name = "Kokkola (63.83473200917329, 23.123709317260648)"
            },
            new Office()
            {
                Id = 13,
                Name = "Turku (60.44991173801938, 22.293984601059858)"
            },
            new Office()
            {
                Id = 14,
                Name = "Kuopio (62.890139950100824, 27.63171036451606)"
            },
            new Office()
            {
                Id = 15,
                Name = "Prague (50.08481700492511, 14.44251624731215)"
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