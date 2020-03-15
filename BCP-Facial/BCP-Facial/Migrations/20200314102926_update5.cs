using Microsoft.EntityFrameworkCore.Migrations;

namespace BCP_Facial.Migrations
{
    public partial class update5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassCode",
                table: "Classes",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4FBD4989-DF6E-479A-AE7D-641700E09A84",
                column: "ConcurrencyStamp",
                value: "41a47e16-27c6-4534-8261-ca932a68978e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "EF7A8FDE-0005-4085-B26F-37D7278BE768",
                column: "ConcurrencyStamp",
                value: "2e8cf3ae-ab7a-413d-a6e9-5fed9a38d808");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ClassCode",
                table: "Classes",
                column: "ClassCode",
                unique: true,
                filter: "[ClassCode] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Classes_ClassCode",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassCode",
                table: "Classes");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4FBD4989-DF6E-479A-AE7D-641700E09A84",
                column: "ConcurrencyStamp",
                value: "6dbaa718-be81-4dde-9616-786a5c710cb2");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "EF7A8FDE-0005-4085-B26F-37D7278BE768",
                column: "ConcurrencyStamp",
                value: "a24cbd5e-efc8-4359-a482-51818543bca1");
        }
    }
}
