using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCP_Facial.Migrations
{
    public partial class update3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateModified",
                table: "RecognizerTasks",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateModified",
                table: "RecognizerTasks");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4FBD4989-DF6E-479A-AE7D-641700E09A84",
                column: "ConcurrencyStamp",
                value: "42838265-6e6e-4e4a-a101-de7d870241e8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "EF7A8FDE-0005-4085-B26F-37D7278BE768",
                column: "ConcurrencyStamp",
                value: "41b19cce-001d-4642-89ed-910945a5864b");
        }
    }
}
