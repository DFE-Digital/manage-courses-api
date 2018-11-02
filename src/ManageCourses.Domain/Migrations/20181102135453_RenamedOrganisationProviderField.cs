using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenamedOrganisationProviderField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_organisation_provider_institution_institution_id",
                table: "organisation_provider");

            migrationBuilder.RenameColumn(
                name: "institution_id",
                table: "organisation_provider",
                newName: "provider_id");

            migrationBuilder.RenameIndex(
                name: "IX_organisation_provider_institution_id",
                table: "organisation_provider",
                newName: "IX_organisation_provider_provider_id");

            migrationBuilder.AddForeignKey(
                name: "FK_organisation_provider_institution_provider_id",
                table: "organisation_provider",
                column: "provider_id",
                principalTable: "institution",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_organisation_provider_institution_provider_id",
                table: "organisation_provider");

            migrationBuilder.RenameColumn(
                name: "provider_id",
                table: "organisation_provider",
                newName: "institution_id");

            migrationBuilder.RenameIndex(
                name: "IX_organisation_provider_provider_id",
                table: "organisation_provider",
                newName: "IX_organisation_provider_institution_id");

            migrationBuilder.AddForeignKey(
                name: "FK_organisation_provider_institution_institution_id",
                table: "organisation_provider",
                column: "institution_id",
                principalTable: "institution",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
