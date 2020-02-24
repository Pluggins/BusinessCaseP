using Microsoft.EntityFrameworkCore.Migrations;

namespace BCP_Facial.Migrations
{
    public partial class update3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonGroup",
                table: "Classes");

            migrationBuilder.AddColumn<string>(
                name: "PersonGroupId",
                table: "Classes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonGroupId",
                table: "Classes");

            migrationBuilder.AddColumn<string>(
                name: "PersonGroup",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
