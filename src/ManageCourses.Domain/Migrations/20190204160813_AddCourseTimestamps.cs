using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class AddCourseTimestamps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "course",
                nullable: false,
                defaultValueSql: "now() at time zone 'utc'"); // https://stackoverflow.com/questions/16609724/using-current-time-in-utc-as-default-value-in-postgresql/16610360#16610360

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "course",
                nullable: false,
                defaultValueSql: "now() at time zone 'utc'"); // https://stackoverflow.com/questions/16609724/using-current-time-in-utc-as-default-value-in-postgresql/16610360#16610360
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "course");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "course");
        }
    }
}
