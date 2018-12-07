using Microsoft.EntityFrameworkCore.Migrations;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class AddModularFieldToCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "modular",
                table: "course",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "modular",
                table: "course");
        }
    }
}
