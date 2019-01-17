using Microsoft.EntityFrameworkCore.Migrations;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenameTimestamps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_timestamp_utc",
                table: "provider_enrichment",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "created_timestamp_utc",
                table: "provider_enrichment",
                newName: "created_at");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "provider_enrichment",
                newName: "updated_timestamp_utc");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "provider_enrichment",
                newName: "created_timestamp_utc");
        }
    }
}
