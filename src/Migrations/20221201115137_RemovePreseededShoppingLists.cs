using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopping_bag.Migrations
{
    public partial class RemovePreseededShoppingLists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ShoppingLists",
                columns: new[] { "Id", "Comment", "CreatedDate", "DueDate", "ExpectedDeliveryDate", "Name", "OfficeId", "Ordered", "OrderedDate", "Removed", "UserId" },
                values: new object[,]
                {
                    { 1L, "Weekly order", new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 12, 18, 17, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 12, 20, 17, 0, 0, 0, DateTimeKind.Unspecified), "Week 50 list", 1L, false, null, false, null },
                    { 2L, "List for office supplies", new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 15, 22, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 15, 12, 0, 0, 0, DateTimeKind.Unspecified), "Office supplies", 1L, false, null, false, null },
                    { 3L, "No due or delivery dates set", new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tampere office list", 5L, false, null, false, null },
                    { 4L, "Order that is overdue but not delivered", new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 10, 9, 17, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 15, 12, 0, 0, 0, DateTimeKind.Unspecified), "Week 40 list", 1L, true, new DateTime(2022, 10, 10, 16, 0, 0, 0, DateTimeKind.Unspecified), false, null },
                    { 5L, "Order that is overdue and delivered", new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 9, 30, 17, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 10, 3, 12, 0, 0, 0, DateTimeKind.Unspecified), "Week 39 list", 1L, true, new DateTime(2022, 10, 10, 16, 0, 0, 0, DateTimeKind.Unspecified), false, null },
                    { 6L, null, new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), null, null, "List with only a name", 1L, false, null, false, null }
                });
        }
    }
}
