using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BCP_Facial.Migrations
{
    public partial class update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceItems",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    StudentId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceItems_ClassAllocations_StudentId",
                        column: x => x.StudentId,
                        principalTable: "ClassAllocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Recognizers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Key = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recognizers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    AttendanceItemId = table.Column<string>(nullable: true),
                    ClassId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_AttendanceItems_AttendanceItemId",
                        column: x => x.AttendanceItemId,
                        principalTable: "AttendanceItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attendances_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecognizerTasks",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Command = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    RecognizerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecognizerTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecognizerTasks_Recognizers_RecognizerId",
                        column: x => x.RecognizerId,
                        principalTable: "Recognizers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceItems_StudentId",
                table: "AttendanceItems",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_AttendanceItemId",
                table: "Attendances",
                column: "AttendanceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ClassId",
                table: "Attendances",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_RecognizerTasks_RecognizerId",
                table: "RecognizerTasks",
                column: "RecognizerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "RecognizerTasks");

            migrationBuilder.DropTable(
                name: "AttendanceItems");

            migrationBuilder.DropTable(
                name: "Recognizers");
        }
    }
}
