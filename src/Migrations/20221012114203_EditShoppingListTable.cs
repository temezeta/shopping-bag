using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopping_bag.Migrations
{
    public partial class EditShoppingListTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeliveryDate",
                table: "ShoppingLists",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ShoppingLists",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpectedDeliveryDate",
                table: "ShoppingLists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OfficeId",
                table: "ShoppingLists",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "Ordered",
                table: "ShoppingLists",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "ShoppingLists",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedDate", "ExpectedDeliveryDate", "OfficeId", "StartDate" },
                values: new object[] { new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 12, 20, 17, 0, 0, 0, DateTimeKind.Unspecified), 1L, null });

            migrationBuilder.UpdateData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedDate", "ExpectedDeliveryDate", "OfficeId", "StartDate" },
                values: new object[] { new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 2, 15, 12, 0, 0, 0, DateTimeKind.Unspecified), 1L, null });

            migrationBuilder.UpdateData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedDate", "OfficeId" },
                values: new object[] { new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), 5L });

            migrationBuilder.UpdateData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedDate", "ExpectedDeliveryDate", "OfficeId", "Ordered", "StartDate" },
                values: new object[] { new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 1, 15, 12, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, null });

            migrationBuilder.UpdateData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedDate", "ExpectedDeliveryDate", "OfficeId", "Ordered", "StartDate" },
                values: new object[] { new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 10, 3, 12, 0, 0, 0, DateTimeKind.Unspecified), 1L, true, null });

            migrationBuilder.UpdateData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedDate", "OfficeId" },
                values: new object[] { new DateTime(2022, 10, 12, 12, 0, 0, 0, DateTimeKind.Unspecified), 1L });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ShoppingLists");

            migrationBuilder.DropColumn(
                name: "ExpectedDeliveryDate",
                table: "ShoppingLists");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "ShoppingLists");

            migrationBuilder.DropColumn(
                name: "Ordered",
                table: "ShoppingLists");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ShoppingLists");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "ShoppingLists",
                newName: "DeliveryDate");

            migrationBuilder.UpdateData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DeliveryDate",
                value: new DateTime(2022, 12, 20, 17, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 2L,
                column: "DeliveryDate",
                value: new DateTime(2023, 2, 15, 12, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 4L,
                column: "DeliveryDate",
                value: new DateTime(2023, 1, 15, 12, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "ShoppingLists",
                keyColumn: "Id",
                keyValue: 5L,
                column: "DeliveryDate",
                value: new DateTime(2022, 10, 3, 12, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
