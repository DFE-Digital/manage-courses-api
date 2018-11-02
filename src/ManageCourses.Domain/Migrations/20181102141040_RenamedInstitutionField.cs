using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenamedInstitutionField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "inst_code",
                table: "institution_enrichment",
                newName: "provider_code");

            migrationBuilder.RenameIndex(
                name: "IX_institution_enrichment_inst_code",
                table: "institution_enrichment",
                newName: "IX_institution_enrichment_provider_code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "provider_code",
                table: "institution_enrichment",
                newName: "inst_code");

            migrationBuilder.RenameIndex(
                name: "IX_institution_enrichment_provider_code",
                table: "institution_enrichment",
                newName: "IX_institution_enrichment_inst_code");
        }
    }
}
