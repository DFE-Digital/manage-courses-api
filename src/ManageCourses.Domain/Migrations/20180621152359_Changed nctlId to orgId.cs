using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class ChangednctlIdtoorgId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_institution_mc_organisation_nctl_id",
                table: "mc_organisation_institution");

            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_user_mc_organisation_nctl_id",
                table: "mc_organisation_user");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_mc_organisation_nctl_id",
                table: "mc_organisation");

            migrationBuilder.RenameColumn(
                name: "nctl_id",
                table: "provider_mapper",
                newName: "org_id");

            migrationBuilder.RenameColumn(
                name: "nctl_id",
                table: "mc_organisation_user",
                newName: "org_id");

            migrationBuilder.RenameIndex(
                name: "IX_mc_organisation_user_email_nctl_id",
                table: "mc_organisation_user",
                newName: "IX_mc_organisation_user_email_org_id");

            migrationBuilder.RenameIndex(
                name: "IX_mc_organisation_user_nctl_id",
                table: "mc_organisation_user",
                newName: "IX_mc_organisation_user_org_id");

            migrationBuilder.RenameColumn(
                name: "nctl_id",
                table: "mc_organisation_institution",
                newName: "org_id");

            migrationBuilder.RenameIndex(
                name: "IX_mc_organisation_institution_nctl_id_institution_code",
                table: "mc_organisation_institution",
                newName: "IX_mc_organisation_institution_org_id_institution_code");

            migrationBuilder.RenameColumn(
                name: "nctl_id",
                table: "mc_organisation",
                newName: "org_id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_mc_organisation_org_id",
                table: "mc_organisation",
                column: "org_id");

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_institution_mc_organisation_org_id",
                table: "mc_organisation_institution",
                column: "org_id",
                principalTable: "mc_organisation",
                principalColumn: "org_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_user_mc_organisation_org_id",
                table: "mc_organisation_user",
                column: "org_id",
                principalTable: "mc_organisation",
                principalColumn: "org_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_institution_mc_organisation_org_id",
                table: "mc_organisation_institution");

            migrationBuilder.DropForeignKey(
                name: "FK_mc_organisation_user_mc_organisation_org_id",
                table: "mc_organisation_user");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_mc_organisation_org_id",
                table: "mc_organisation");

            migrationBuilder.RenameColumn(
                name: "org_id",
                table: "provider_mapper",
                newName: "nctl_id");

            migrationBuilder.RenameColumn(
                name: "org_id",
                table: "mc_organisation_user",
                newName: "nctl_id");

            migrationBuilder.RenameIndex(
                name: "IX_mc_organisation_user_email_org_id",
                table: "mc_organisation_user",
                newName: "IX_mc_organisation_user_email_nctl_id");

            migrationBuilder.RenameIndex(
                name: "IX_mc_organisation_user_org_id",
                table: "mc_organisation_user",
                newName: "IX_mc_organisation_user_nctl_id");

            migrationBuilder.RenameColumn(
                name: "org_id",
                table: "mc_organisation_institution",
                newName: "nctl_id");

            migrationBuilder.RenameIndex(
                name: "IX_mc_organisation_institution_org_id_institution_code",
                table: "mc_organisation_institution",
                newName: "IX_mc_organisation_institution_nctl_id_institution_code");

            migrationBuilder.RenameColumn(
                name: "org_id",
                table: "mc_organisation",
                newName: "nctl_id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_mc_organisation_nctl_id",
                table: "mc_organisation",
                column: "nctl_id");

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_institution_mc_organisation_nctl_id",
                table: "mc_organisation_institution",
                column: "nctl_id",
                principalTable: "mc_organisation",
                principalColumn: "nctl_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_mc_organisation_user_mc_organisation_nctl_id",
                table: "mc_organisation_user",
                column: "nctl_id",
                principalTable: "mc_organisation",
                principalColumn: "nctl_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
