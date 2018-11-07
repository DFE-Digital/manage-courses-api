using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenamedOrganisationInstitution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine("ALTER TABLE organisation_institution RENAME institution_id TO provider_id;");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_mc_organisation_institution_institution_id\" RENAME TO \"IX_mc_organisation_provider_provider_id\";");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_mc_organisation_institution_mc_organisation_id\" RENAME TO \"IX_mc_organisation_provider_mc_organisation_id\";");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_organisation_institution_institution_id\" RENAME TO \"IX_organisation_provider_provider_id\";");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_organisation_institution_organisation_id\" RENAME TO \"IX_organisation_provider_organisation_id\";");
            sqlBuilder.AppendLine("ALTER TABLE organisation_institution");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"FK_mc_organisation_institution_institution_institution_id\" TO \"FK_mc_organisation_provider_provider_provider_id\";");
            sqlBuilder.AppendLine("ALTER TABLE organisation_institution");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"FK_mc_organisation_institution_mc_organisation_mc_organisation_\" TO \"FK_mc_organisation_provider_mc_organisation_mc_organisation_\";");
            sqlBuilder.AppendLine("ALTER TABLE organisation_institution");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"FK_organisation_institution_institution_institution_id\" TO \"FK_organisation_provider_provider_provider_id\";");
            sqlBuilder.AppendLine("ALTER TABLE organisation_institution");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"FK_organisation_institution_organisation_organisation_id\" TO \"FK_organisation_provider_organisation_organisation_id\";");
            sqlBuilder.AppendLine("ALTER TABLE organisation_institution");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"PK_organisation_institution\" TO \"PK_organisation_provider\";");
            sqlBuilder.AppendLine("ALTER TABLE organisation_institution RENAME TO organisation_provider;");

            migrationBuilder.Sql(sqlBuilder.ToString());

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine("ALTER TABLE organisation_provider RENAME provider_id TO institution_id;");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_mc_organisation_provider_provider_id\" RENAME TO \"IX_mc_organisation_institution_institution_id\";");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_mc_organisation_provider_mc_organisation_id\" RENAME TO \"IX_mc_organisation_institution_mc_organisation_id\";");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_organisation_provider_provider_id\" RENAME TO \"IX_organisation_institution_institution_id\";");
            sqlBuilder.AppendLine("ALTER INDEX \"IX_organisation_provider_organisation_id\" RENAME TO \"IX_organisation_institution_organisation_id\";");
            sqlBuilder.AppendLine("ALTER TABLE organisation_provider");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"FK_mc_organisation_provider_provider_provider_id\" TO \"FK_mc_organisation_institution_institution_institution_id\";");
            sqlBuilder.AppendLine("ALTER TABLE organisation_provider");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"FK_mc_organisation_provider_mc_organisation_mc_organisation_\" TO \"FK_mc_organisation_institution_mc_organisation_mc_organisation_\";");
            sqlBuilder.AppendLine("ALTER TABLE organisation_provider");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"FK_organisation_provider_provider_provider_id\" TO \"FK_organisation_institution_institution_institution_id\";");
            sqlBuilder.AppendLine("ALTER TABLE organisation_provider");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"FK_organisation_provider_organisation_organisation_id\" TO \"FK_organisation_institution_organisation_organisation_id\";");
            sqlBuilder.AppendLine("ALTER TABLE organisation_provider");
            sqlBuilder.AppendLine("RENAME CONSTRAINT \"PK_organisation_provider\" TO \"PK_organisation_institution\";");
            sqlBuilder.AppendLine("ALTER TABLE organisation_provider RENAME TO organisation_institution;");

            migrationBuilder.Sql(sqlBuilder.ToString());

        }
    }
}
