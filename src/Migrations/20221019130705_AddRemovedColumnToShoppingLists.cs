using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopping_bag.Migrations
{
    public partial class AddRemovedColumnToShoppingLists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Removed",
                table: "ShoppingLists",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Removed",
                table: "ShoppingLists");
        }
    }
}
