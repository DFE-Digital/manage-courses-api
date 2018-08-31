using Microsoft.EntityFrameworkCore.Migrations;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class UpperCaseKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                update course_enrichment
                set inst_code = upper(inst_code),
                  ucas_course_code = upper(ucas_course_code);
            ");
            migrationBuilder.Sql(@"
                update institution_enrichment
                set inst_code = upper(inst_code);
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                update course_enrichment
                set inst_code = lower(inst_code),
                  ucas_course_code = lower(ucas_course_code);
            ");
            migrationBuilder.Sql(@"
                update institution_enrichment
                set inst_code = lower(inst_code);
            ");
        }
    }
}
