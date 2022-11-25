using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopping_bag.Migrations
{
    public partial class RenameToDisabled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Removed",
                table: "Users",
                newName: "Disabled");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Disabled",
                table: "Users",
                newName: "Removed");
        }
    }
}
