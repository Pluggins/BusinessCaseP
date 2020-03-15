using Microsoft.EntityFrameworkCore.Migrations;

namespace BCP_Facial.Migrations
{
    public partial class update6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceItems_ClassAllocations_StudentId",
                table: "AttendanceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_AttendanceItems_AttendanceItemId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_AttendanceItemId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "AttendanceItemId",
                table: "Attendances");

            migrationBuilder.AddColumn<string>(
                name: "AttendanceId",
                table: "AttendanceItems",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4FBD4989-DF6E-479A-AE7D-641700E09A84",
                column: "ConcurrencyStamp",
                value: "0aaa74a0-0f7f-4974-b8af-de367cf6bc4e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "EF7A8FDE-0005-4085-B26F-37D7278BE768",
                column: "ConcurrencyStamp",
                value: "d0b7ac2d-2104-4821-968d-2df57838951f");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceItems_AttendanceId",
                table: "AttendanceItems",
                column: "AttendanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceItems_Attendances_AttendanceId",
                table: "AttendanceItems",
                column: "AttendanceId",
                principalTable: "Attendances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceItems_BCPUsers_StudentId",
                table: "AttendanceItems",
                column: "StudentId",
                principalTable: "BCPUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceItems_Attendances_AttendanceId",
                table: "AttendanceItems");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceItems_BCPUsers_StudentId",
                table: "AttendanceItems");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceItems_AttendanceId",
                table: "AttendanceItems");

            migrationBuilder.DropColumn(
                name: "AttendanceId",
                table: "AttendanceItems");

            migrationBuilder.AddColumn<string>(
                name: "AttendanceItemId",
                table: "Attendances",
                type: "nvarchar(450)",
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
                name: "IX_Attendances_AttendanceItemId",
                table: "Attendances",
                column: "AttendanceItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceItems_ClassAllocations_StudentId",
                table: "AttendanceItems",
                column: "StudentId",
                principalTable: "ClassAllocations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_AttendanceItems_AttendanceItemId",
                table: "Attendances",
                column: "AttendanceItemId",
                principalTable: "AttendanceItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
