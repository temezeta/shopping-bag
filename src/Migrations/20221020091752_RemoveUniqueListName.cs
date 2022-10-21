using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopping_bag.Migrations
{
    public partial class RemoveUniqueListName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShoppingLists_Name",
                table: "ShoppingLists");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ShoppingLists",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ShoppingLists",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingLists_Name",
                table: "ShoppingLists",
                column: "Name",
                unique: true);
        }
    }
}
