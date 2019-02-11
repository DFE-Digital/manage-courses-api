using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class AddProviderLastPublished : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_published_at",
                table: "provider",
                nullable: true);
            migrationBuilder.Sql(@"
                update provider set last_published_at = now() at time zone 'utc';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_published_at",
                table: "provider");
        }
    }
}
