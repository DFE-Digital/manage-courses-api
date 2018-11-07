using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenamedCourseFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine("ALTER TABLE course RENAME institution_id TO provider_id;");
            sqlBuilder.AppendLine("ALTER TABLE course RENAME accrediting_institution_id TO accrediting_provider_id;");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_course_accrediting_institution_id\" RENAME TO \"IX_course_accrediting_provider_id\";");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_course_institution_id_course_code\" RENAME TO \"IX_course_provider_id_course_code\";");
            sqlBuilder.AppendLine("ALTER TABLE course RENAME CONSTRAINT \"FK_course_institution_accrediting_institution_id\" TO \"FK_course_provider_accrediting_provider_id\";");
            sqlBuilder.AppendLine("ALTER TABLE course RENAME CONSTRAINT \"FK_course_institution_institution_id\" TO \"FK_course_provider_provider_id\";");
            sqlBuilder.AppendLine("ALTER TABLE pgde_course RENAME inst_code TO provider_code;");
            sqlBuilder.AppendLine("ALTER TABLE course_enrichment RENAME inst_code TO provider_code;");
            migrationBuilder.Sql(sqlBuilder.ToString());

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine("ALTER TABLE course RENAME provider_id TO institution_id;");
            sqlBuilder.AppendLine("ALTER TABLE course RENAME accrediting_provider_id TO accrediting_institution_id;");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_course_accrediting_provider_id\" RENAME TO \"IX_course_accrediting_institution_id\";");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_course_provider_id_course_code\" RENAME TO \"IX_course_institution_id_course_code\";");
            sqlBuilder.AppendLine("ALTER TABLE course RENAME CONSTRAINT \"FK_course_provider_accrediting_provider_id\" TO \"FK_course_institution_accrediting_institution_id\";");
            sqlBuilder.AppendLine("ALTER TABLE course RENAME CONSTRAINT \"FK_course_provider_provider_id\" TO \"FK_course_institution_institution_id\";");
            sqlBuilder.AppendLine("ALTER TABLE pgde_course RENAME provider_code TO inst_code;");
            sqlBuilder.AppendLine("ALTER TABLE course_enrichment RENAME provider_code TO inst_code;");

            migrationBuilder.Sql(sqlBuilder.ToString());

        }
    }
}
