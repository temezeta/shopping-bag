using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace shopping_bag.Migrations
{
    public partial class SeedActualOfficeNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Name",
                value: "Tampere HVT11");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Name",
                value: "Tampere HVT34");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Name",
                value: "Espoo");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Name",
                value: "Hyvinkää");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 5L,
                column: "Name",
                value: "Oulu");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 6L,
                column: "Name",
                value: "Vaasa Wasa Innovation Center");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 7L,
                column: "Name",
                value: "Vaasa Wulffintie");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 8L,
                column: "Name",
                value: "Seinäjoki");

            migrationBuilder.InsertData(
                table: "Offices",
                columns: new[] { "Id", "Name", "Removed" },
                values: new object[,]
                {
                    { 9L, "Jyväskylä", false },
                    { 10L, "Kotka", false },
                    { 11L, "Ylivieska", false },
                    { 12L, "Kokkola", false },
                    { 13L, "Turku", false },
                    { 14L, "Kuopio", false },
                    { 15L, "Prague", false }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 15L);

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Name",
                value: "Espoo");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Name",
                value: "Hyvinkää");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Name",
                value: "Oulu");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Name",
                value: "Vaasa");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 5L,
                column: "Name",
                value: "Tampere");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 6L,
                column: "Name",
                value: "Seinäjoki");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 7L,
                column: "Name",
                value: "Jyväskylä");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 8L,
                column: "Name",
                value: "Kotka");
        }
    }
}
