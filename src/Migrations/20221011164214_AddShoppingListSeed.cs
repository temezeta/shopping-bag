using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopping_bag.Migrations
{
    public partial class AddShoppingListSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ShoppingLists",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "ShoppingLists",
                columns: new[] { "Id", "Comment", "DeliveryDate", "DueDate", "Name" },
                values: new object[,]
                {
                    { 1L, "Weekly order", new DateTime(2022, 12, 20, 17, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 12, 18, 17, 0, 0, 0, DateTimeKind.Unspecified), "Week 50 list" },
                    { 2L, "List for office supplies", new DateTime(2023, 2, 15, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 15, 22, 0, 0, 0, DateTimeKind.Unspecified), "Office supplies" },
                    { 3L, "No due or delivery dates set", null, null, "Tampere office list" },
                    { 4L, "Order that is overdue but not delivered", new DateTime(2023, 1, 15, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 10, 9, 17, 0, 0, 0, DateTimeKind.Unspecified), "Week 40 list" },
                    { 5L, "Order that is overdue and delivered", new DateTime(2022, 10, 3, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 9, 30, 17, 0, 0, 0, DateTimeKind.Unspecified), "Week 39 list" },
                    { 6L, null, null, null, "List with only a name" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingLists_Name",
                table: "ShoppingLists",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShoppingLists_Name",
                table: "ShoppingLists");

            migrationBuilder.DeleteData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ShoppingLists",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
