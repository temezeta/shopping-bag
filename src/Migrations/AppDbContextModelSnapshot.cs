﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using shopping_bag.Config;

#nullable disable

namespace shopping_bag.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Likes", b =>
                {
                    b.Property<int>("LikedItemsId")
                        .HasColumnType("int");

                    b.Property<long>("UsersWhoLikedId")
                        .HasColumnType("bigint");

                    b.HasKey("LikedItemsId", "UsersWhoLikedId");

                    b.HasIndex("UsersWhoLikedId");

                    b.ToTable("Likes");
                });

            modelBuilder.Entity("shopping_bag.Models.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AmountOrdered")
                        .HasColumnType("int");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsChecked")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShopName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ShoppingListId")
                        .HasColumnType("bigint");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ShoppingListId");

                    b.HasIndex("UserId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("shopping_bag.Models.ListReminderSettings", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("ShoppingListId")
                        .HasColumnType("bigint");

                    b.Property<bool>("DueDateRemindersDisabled")
                        .HasColumnType("bit");

                    b.Property<bool>("ExpectedRemindersDisabled")
                        .HasColumnType("bit");

                    b.Property<string>("ReminderDaysBeforeDueDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReminderDaysBeforeExpectedDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "ShoppingListId");

                    b.HasIndex("ShoppingListId");

                    b.ToTable("ListReminderSettings");
                });

            modelBuilder.Entity("shopping_bag.Models.Office", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("Removed")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Offices");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            Name = "Espoo",
                            Removed = false
                        },
                        new
                        {
                            Id = 2L,
                            Name = "Hyvinkää",
                            Removed = false
                        },
                        new
                        {
                            Id = 3L,
                            Name = "Oulu",
                            Removed = false
                        },
                        new
                        {
                            Id = 4L,
                            Name = "Vaasa",
                            Removed = false
                        },
                        new
                        {
                            Id = 5L,
                            Name = "Tampere",
                            Removed = false
                        },
                        new
                        {
                            Id = 6L,
                            Name = "Seinäjoki",
                            Removed = false
                        },
                        new
                        {
                            Id = 7L,
                            Name = "Jyväskylä",
                            Removed = false
                        },
                        new
                        {
                            Id = 8L,
                            Name = "Kotka",
                            Removed = false
                        });
                });

            modelBuilder.Entity("shopping_bag.Models.Reminder", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("DueDaysBefore")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExpectedDaysBefore")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ShoppingListId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ShoppingListId");

                    b.HasIndex("UserId");

                    b.ToTable("Reminders");
                });

            modelBuilder.Entity("shopping_bag.Models.ReminderSettings", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<bool>("DueDateRemindersDisabled")
                        .HasColumnType("bit");

                    b.Property<bool>("ExpectedRemindersDisabled")
                        .HasColumnType("bit");

                    b.Property<string>("ReminderDaysBeforeDueDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReminderDaysBeforeExpectedDate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("ReminderSettings");
                });

            modelBuilder.Entity("shopping_bag.Models.ShoppingList", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ExpectedDeliveryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("OfficeId")
                        .HasColumnType("bigint");

                    b.Property<bool>("Ordered")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("OrderedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Removed")
                        .HasColumnType("bit");

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("OfficeId");

                    b.HasIndex("UserId");

                    b.ToTable("ShoppingLists");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            Comment = "Weekly order",
                            CreatedDate = new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified),
                            DueDate = new DateTime(2022, 12, 18, 17, 0, 0, 0, DateTimeKind.Unspecified),
                            ExpectedDeliveryDate = new DateTime(2022, 12, 20, 17, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Week 50 list",
                            OfficeId = 1L,
                            Ordered = false,
                            Removed = false
                        },
                        new
                        {
                            Id = 2L,
                            Comment = "List for office supplies",
                            CreatedDate = new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified),
                            DueDate = new DateTime(2023, 1, 15, 22, 0, 0, 0, DateTimeKind.Unspecified),
                            ExpectedDeliveryDate = new DateTime(2023, 2, 15, 12, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Office supplies",
                            OfficeId = 1L,
                            Ordered = false,
                            Removed = false
                        },
                        new
                        {
                            Id = 3L,
                            Comment = "No due or delivery dates set",
                            CreatedDate = new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Tampere office list",
                            OfficeId = 5L,
                            Ordered = false,
                            Removed = false
                        },
                        new
                        {
                            Id = 4L,
                            Comment = "Order that is overdue but not delivered",
                            CreatedDate = new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified),
                            DueDate = new DateTime(2022, 10, 9, 17, 0, 0, 0, DateTimeKind.Unspecified),
                            ExpectedDeliveryDate = new DateTime(2023, 1, 15, 12, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Week 40 list",
                            OfficeId = 1L,
                            Ordered = true,
                            OrderedDate = new DateTime(2022, 10, 10, 16, 0, 0, 0, DateTimeKind.Unspecified),
                            Removed = false
                        },
                        new
                        {
                            Id = 5L,
                            Comment = "Order that is overdue and delivered",
                            CreatedDate = new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified),
                            DueDate = new DateTime(2022, 9, 30, 17, 0, 0, 0, DateTimeKind.Unspecified),
                            ExpectedDeliveryDate = new DateTime(2022, 10, 3, 12, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "Week 39 list",
                            OfficeId = 1L,
                            Ordered = true,
                            OrderedDate = new DateTime(2022, 10, 10, 16, 0, 0, 0, DateTimeKind.Unspecified),
                            Removed = false
                        },
                        new
                        {
                            Id = 6L,
                            CreatedDate = new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified),
                            Name = "List with only a name",
                            OfficeId = 1L,
                            Ordered = false,
                            Removed = false
                        });
                });

            modelBuilder.Entity("shopping_bag.Models.User.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("OfficeId")
                        .HasColumnType("bigint");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("PasswordResetToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Removed")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ResetTokenExpires")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("TokenCreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("TokenExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("VerificationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("VerifiedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("OfficeId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("shopping_bag.Models.User.UserRole", b =>
                {
                    b.Property<long>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("RoleId"), 1L, 1);

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("RoleId");

                    b.ToTable("UserRoles");

                    b.HasData(
                        new
                        {
                            RoleId = 1L,
                            RoleName = "Admin"
                        },
                        new
                        {
                            RoleId = 2L,
                            RoleName = "User"
                        });
                });

            modelBuilder.Entity("UserUserRole", b =>
                {
                    b.Property<long>("UserRolesRoleId")
                        .HasColumnType("bigint");

                    b.Property<long>("UsersId")
                        .HasColumnType("bigint");

                    b.HasKey("UserRolesRoleId", "UsersId");

                    b.HasIndex("UsersId");

                    b.ToTable("UserUserRole");
                });

            modelBuilder.Entity("Likes", b =>
                {
                    b.HasOne("shopping_bag.Models.Item", null)
                        .WithMany()
                        .HasForeignKey("LikedItemsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("shopping_bag.Models.User.User", null)
                        .WithMany()
                        .HasForeignKey("UsersWhoLikedId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("shopping_bag.Models.Item", b =>
                {
                    b.HasOne("shopping_bag.Models.ShoppingList", "ShoppingList")
                        .WithMany("Items")
                        .HasForeignKey("ShoppingListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("shopping_bag.Models.User.User", "ItemAdder")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("ItemAdder");

                    b.Navigation("ShoppingList");
                });

            modelBuilder.Entity("shopping_bag.Models.ListReminderSettings", b =>
                {
                    b.HasOne("shopping_bag.Models.ShoppingList", "ShoppingList")
                        .WithMany()
                        .HasForeignKey("ShoppingListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("shopping_bag.Models.User.User", "User")
                        .WithMany("ListReminderSettings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("ShoppingList");

                    b.Navigation("User");
                });

            modelBuilder.Entity("shopping_bag.Models.Reminder", b =>
                {
                    b.HasOne("shopping_bag.Models.ShoppingList", "ShoppingList")
                        .WithMany()
                        .HasForeignKey("ShoppingListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("shopping_bag.Models.User.User", "User")
                        .WithMany("Reminders")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.Navigation("ShoppingList");

                    b.Navigation("User");
                });

            modelBuilder.Entity("shopping_bag.Models.ReminderSettings", b =>
                {
                    b.HasOne("shopping_bag.Models.User.User", "User")
                        .WithOne("ReminderSettings")
                        .HasForeignKey("shopping_bag.Models.ReminderSettings", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("shopping_bag.Models.ShoppingList", b =>
                {
                    b.HasOne("shopping_bag.Models.Office", "ListDeliveryOffice")
                        .WithMany()
                        .HasForeignKey("OfficeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("shopping_bag.Models.User.User", "ListCreatorUser")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("ListCreatorUser");

                    b.Navigation("ListDeliveryOffice");
                });

            modelBuilder.Entity("shopping_bag.Models.User.User", b =>
                {
                    b.HasOne("shopping_bag.Models.Office", "HomeOffice")
                        .WithMany()
                        .HasForeignKey("OfficeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("HomeOffice");
                });

            modelBuilder.Entity("UserUserRole", b =>
                {
                    b.HasOne("shopping_bag.Models.User.UserRole", null)
                        .WithMany()
                        .HasForeignKey("UserRolesRoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("shopping_bag.Models.User.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("shopping_bag.Models.ShoppingList", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("shopping_bag.Models.User.User", b =>
                {
                    b.Navigation("ListReminderSettings");

                    b.Navigation("ReminderSettings")
                        .IsRequired();

                    b.Navigation("Reminders");
                });
#pragma warning restore 612, 618
        }
    }
}
