using Microsoft.EntityFrameworkCore.Migrations;
using System.Text;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenamedInstitutionEnrichment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine("ALTER TABLE institution_enrichment RENAME inst_code TO provider_code;");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_institution_enrichment_created_by_user_id\" RENAME TO \"IX_provider_enrichment_created_by_user_id\";");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_institution_enrichment_inst_code\" RENAME TO \"IX_provider_enrichment_provider_code\";");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_institution_enrichment_updated_by_user_id\" RENAME TO \"IX_provider_enrichment_updated_by_user_id\";");
            sqlBuilder.AppendLine("ALTER TABLE institution_enrichment");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"FK_institution_enrichment_user_created_by_user_id\" TO \"FK_provider_enrichment_user_created_by_user_id\";");
            sqlBuilder.AppendLine("ALTER TABLE institution_enrichment");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"FK_institution_enrichment_user_updated_by_user_id\" TO \"FK_provider_enrichment_user_updated_by_user_id\";");
            sqlBuilder.AppendLine("ALTER TABLE institution_enrichment");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"PK_institution_enrichment\" TO \"PK_provider_enrichment\";");
            sqlBuilder.AppendLine("ALTER TABLE institution_enrichment RENAME TO provider_enrichment;");

            migrationBuilder.Sql(sqlBuilder.ToString());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendLine("ALTER TABLE provider_enrichment RENAME provider_code TO inst_code;");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_provider_enrichment_created_by_user_id\" RENAME TO \"IX_institution_enrichment_created_by_user_id\";");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_provider_enrichment_provider_code\" RENAME TO \"IX_institution_enrichment_inst_code\";");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_provider_enrichment_updated_by_user_id\" RENAME TO \"IX_institution_enrichment_updated_by_user_id\";");
            sqlBuilder.AppendLine("ALTER TABLE provider_enrichment");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"FK_provider_enrichment_user_created_by_user_id\" TO \"FK_institution_enrichment_user_created_by_user_id\";");
            sqlBuilder.AppendLine("ALTER TABLE provider_enrichment");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"FK_provider_enrichment_user_updated_by_user_id\" TO \"FK_institution_enrichment_user_updated_by_user_id\";");
            sqlBuilder.AppendLine("ALTER TABLE provider_enrichment");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"PK_provider_enrichment\" TO \"PK_institution_enrichment\";");
            sqlBuilder.AppendLine("ALTER TABLE provider_enrichment RENAME TO institution_enrichment;");

            migrationBuilder.Sql(sqlBuilder.ToString());
        }
    }
}
