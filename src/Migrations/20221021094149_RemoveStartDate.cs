using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopping_bag.Migrations
{
    public partial class RemoveStartDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "ShoppingLists");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "ShoppingLists",
                type: "datetime2",
                nullable: true);
        }
    }
}
