using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class ChangedCourseSiteFieldDataType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = $"ALTER TABLE \"course_site\" " +
                      "ALTER COLUMN \"applications_accepted_from\" " +
                      "TYPE date USING \"applications_accepted_from\"::date;";

            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //There no point of down as when you do go down you need code change therefore another migration
        }
    }
}
