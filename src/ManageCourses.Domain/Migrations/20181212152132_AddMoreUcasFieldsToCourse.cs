using Microsoft.EntityFrameworkCore.Migrations;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class AddMoreUcasFieldsToCourse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "english",
                table: "course",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "has_been_published",
                table: "course",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "maths",
                table: "course",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "science",
                table: "course",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "english",
                table: "course");

            migrationBuilder.DropColumn(
                name: "has_been_published",
                table: "course");

            migrationBuilder.DropColumn(
                name: "maths",
                table: "course");

            migrationBuilder.DropColumn(
                name: "science",
                table: "course");
        }
    }
}
