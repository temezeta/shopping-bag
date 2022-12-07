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
                value: "Tampere HVT11 (61.49142122101034, 23.770750652503075)");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Name",
                value: "Tampere HVT34 (61.4872349013685, 23.771011831141426)");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 3L,
                column: "Name",
                value: "Espoo (60.17320724919939, 24.82951312687209)");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 4L,
                column: "Name",
                value: "Hyvinkää (60.61885645330777, 24.81364812340716)");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 5L,
                column: "Name",
                value: "Oulu (65.0573847614918, 25.443263561914264)");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 6L,
                column: "Name",
                value: "Vaasa Wasa Innovation Center (63.116523411260665, 21.62020520510232)");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 7L,
                column: "Name",
                value: "Vaasa Wulffintie (63.09815319119998, 21.601082366419462)");

            migrationBuilder.UpdateData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 8L,
                column: "Name",
                value: "Seinäjoki (62.80052477778116, 22.822913653422315)");

            migrationBuilder.InsertData(
                table: "Offices",
                columns: new[] { "Id", "Name", "Removed" },
                values: new object[,]
                {
                    { 9L, "Jyväskylä (62.244890931070074, 25.750669670647447)", false },
                    { 10L, "Kotka (60.51600193933175, 26.928281488329468)", false },
                    { 11L, "Ylivieska (64.07478730741482, 24.51536955120399)", false },
                    { 12L, "Kokkola (63.83473200917329, 23.123709317260648)", false },
                    { 13L, "Turku (60.44991173801938, 22.293984601059858)", false },
                    { 14L, "Kuopio (62.890139950100824, 27.63171036451606)", false },
                    { 15L, "Prague (50.08481700492511, 14.44251624731215)", false }
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
