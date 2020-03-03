using Microsoft.EntityFrameworkCore.Migrations;

namespace BCP_Facial.Migrations
{
    public partial class update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Classes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4FBD4989-DF6E-479A-AE7D-641700E09A84",
                column: "ConcurrencyStamp",
                value: "69739d9b-0cdd-40a8-a5e2-201d6b9bfe63");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "EF7A8FDE-0005-4085-B26F-37D7278BE768",
                column: "ConcurrencyStamp",
                value: "cfa3f274-abe5-43c4-a24c-0776c9175390");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Classes");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4FBD4989-DF6E-479A-AE7D-641700E09A84",
                column: "ConcurrencyStamp",
                value: "70afc8b1-1efc-41e3-9689-82262b6b4ed4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "EF7A8FDE-0005-4085-B26F-37D7278BE768",
                column: "ConcurrencyStamp",
                value: "6c6c6654-137b-4f78-b26b-abe45f0126ec");
        }
    }
}
