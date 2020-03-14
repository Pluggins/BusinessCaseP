using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCP_Facial.Migrations
{
    public partial class update4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                table: "ClassAllocations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ClassAllocations",
                nullable: false,
                defaultValue: false);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateCreated",
                table: "ClassAllocations");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ClassAllocations");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4FBD4989-DF6E-479A-AE7D-641700E09A84",
                column: "ConcurrencyStamp",
                value: "a7880fbe-881e-4893-a8e5-a6d76962e867");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "EF7A8FDE-0005-4085-B26F-37D7278BE768",
                column: "ConcurrencyStamp",
                value: "7c14bf5f-3a96-44b5-8d74-3b21dca08783");
        }
    }
}
