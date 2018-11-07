using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenamedSiteFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine("ALTER TABLE site RENAME institution_id TO provider_id;");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_site_institution_id_code\" RENAME TO \"IX_site_provider_id_code\";");
            sqlBuilder.AppendLine("ALTER TABLE site RENAME CONSTRAINT \"FK_site_institution_institution_id\" TO \"FK_site_provider_provider_id\";");

            migrationBuilder.Sql(sqlBuilder.ToString());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine("ALTER TABLE site RENAME provider_id TO institution_id;");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_site_provider_id_code\" RENAME TO \"IX_site_institution_id_code\";");
            sqlBuilder.AppendLine("ALTER TABLE site RENAME CONSTRAINT \"FK_site_provider_provider_id\" TO \"FK_site_institution_institution_id\";");

            migrationBuilder.Sql(sqlBuilder.ToString());

        }
    }
}
