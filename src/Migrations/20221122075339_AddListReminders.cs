using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopping_bag.Migrations
{
    public partial class AddListReminders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListReminderSettings",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ShoppingListId = table.Column<long>(type: "bigint", nullable: false),
                    DueDateRemindersDisabled = table.Column<bool>(type: "bit", nullable: false),
                    ExpectedRemindersDisabled = table.Column<bool>(type: "bit", nullable: false),
                    ReminderDaysBeforeDueDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReminderDaysBeforeExpectedDate = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListReminderSettings", x => new { x.UserId, x.ShoppingListId });
                    table.ForeignKey(
                        name: "FK_ListReminderSettings_ShoppingLists_ShoppingListId",
                        column: x => x.ShoppingListId,
                        principalTable: "ShoppingLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListReminderSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListReminderSettings_ShoppingListId",
                table: "ListReminderSettings",
                column: "ShoppingListId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListReminderSettings");
        }
    }
}
