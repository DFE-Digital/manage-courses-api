using Microsoft.EntityFrameworkCore.Migrations;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RailsyCourseTimestamps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "updated_timestamp_utc",
                table: "course_enrichment",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "created_timestamp_utc",
                table: "course_enrichment",
                newName: "created_at");

            migrationBuilder.Sql(@"
                alter table course_enrichment alter column created_at set default(now() at time zone 'utc');
                alter table course_enrichment alter column updated_at set default(now() at time zone 'utc');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                alter table course_enrichment alter column created_at set default('0001-01-01 00:00:00'::timestamp without time zone);
                alter table course_enrichment alter column updated_at set default('0001-01-01 00:00:00'::timestamp without time zone);
            ");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "course_enrichment",
                newName: "updated_timestamp_utc");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "course_enrichment",
                newName: "created_timestamp_utc");
        }
    }
}
