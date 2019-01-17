using Microsoft.EntityFrameworkCore.Migrations;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenamePublishedAt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "last_published_timestamp_utc",
                table: "provider_enrichment",
                newName: "last_published_at");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "last_published_at",
                table: "provider_enrichment",
                newName: "last_published_timestamp_utc");
        }
    }
}
