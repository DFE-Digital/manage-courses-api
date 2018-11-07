using Microsoft.EntityFrameworkCore.Migrations;
using System.Text;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenamedInstitutionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine("ALTER TABLE institution RENAME inst_name TO provider_name;");
            sqlBuilder.AppendLine("ALTER TABLE institution RENAME inst_code TO provider_code;");
            sqlBuilder.AppendLine("ALTER TABLE institution RENAME inst_type TO provider_type;");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_institution_inst_code\" RENAME TO \"IX_provider_provider_code\";");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_ucas_institution_inst_code\" RENAME TO \"IX_ucas_provider_provider_code\";");
            sqlBuilder.AppendLine("ALTER TABLE institution RENAME CONSTRAINT \"PK_ucas_institution\" TO \"PK_ucas_provider\";");
            sqlBuilder.AppendLine("ALTER TABLE institution RENAME TO provider;");

            migrationBuilder.Sql(sqlBuilder.ToString());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine("ALTER TABLE provider RENAME provider_name TO inst_name;");
            sqlBuilder.AppendLine("ALTER TABLE provider RENAME provider_code TO inst_code;");
            sqlBuilder.AppendLine("ALTER TABLE provider RENAME provider_type TO inst_type;");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_provider_provider_code\" RENAME TO \"IX_institution_inst_code\";");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_ucas_provider_provider_code\" RENAME TO \"IX_ucas_institution_inst_code\";");
            sqlBuilder.AppendLine("ALTER TABLE provider RENAME CONSTRAINT \"PK_ucas_provider\" TO \"PK_ucas_institution\";");
            sqlBuilder.AppendLine("ALTER TABLE provider RENAME TO institution;");

            migrationBuilder.Sql(sqlBuilder.ToString());
        }
    }
}
