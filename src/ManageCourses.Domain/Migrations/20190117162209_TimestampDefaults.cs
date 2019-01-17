using Microsoft.EntityFrameworkCore.Migrations;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class TimestampDefaults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                alter table provider_enrichment alter column created_at set default (now() at time zone 'utc');
                alter table provider_enrichment alter column updated_at set default (now() at time zone 'utc');
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                alter table provider_enrichment alter column created_at set default ('0001-01-01 00:00:00'::timestamp without time zone);
                alter table provider_enrichment alter column updated_at set default ('0001-01-01 00:00:00'::timestamp without time zone);
            ");
        }
    }
}
