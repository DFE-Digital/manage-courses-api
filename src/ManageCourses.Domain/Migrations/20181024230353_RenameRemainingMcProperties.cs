using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace GovUk.Education.ManageCourses.Domain.Migrations
{
    public partial class RenameRemainingMcProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nctl_organisation_organisation_mc_organisation_id",
                table: "nctl_organisation");

            migrationBuilder.RenameColumn(
                name: "mc_organisation_id",
                table: "nctl_organisation",
                newName: "organisation_id");

            migrationBuilder.RenameIndex(
                name: "IX_nctl_organisation_mc_organisation_id",
                table: "nctl_organisation",
                newName: "IX_nctl_organisation_organisation_id");

            migrationBuilder.AddForeignKey(
                name: "FK_nctl_organisation_organisation_organisation_id",
                table: "nctl_organisation",
                column: "organisation_id",
                principalTable: "organisation",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nctl_organisation_organisation_organisation_id",
                table: "nctl_organisation");

            migrationBuilder.RenameColumn(
                name: "organisation_id",
                table: "nctl_organisation",
                newName: "mc_organisation_id");

            migrationBuilder.RenameIndex(
                name: "IX_nctl_organisation_organisation_id",
                table: "nctl_organisation",
                newName: "IX_nctl_organisation_mc_organisation_id");

            migrationBuilder.AddForeignKey(
                name: "FK_nctl_organisation_organisation_mc_organisation_id",
                table: "nctl_organisation",
                column: "mc_organisation_id",
                principalTable: "organisation",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
